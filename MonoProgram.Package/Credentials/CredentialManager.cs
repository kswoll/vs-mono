using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Win32.SafeHandles;

namespace MonoProgram.Package.Credentials
{
    /// <summary>
    /// CredentialManager, wraps the Windows CredMan/CredVault APIs (e.g. CredReadW/CredWriteW)
    /// </summary>
    [SuppressMessage("ReSharper", "FieldCanBeMadeReadOnly.Local")]
    public static class CredentialManager
    {
        private static string DeriveApplicationName(string category, string host, string username)
        {
            return $"vs-mono-{category}://{username}@{host}";
        }

        public static Credential GetCredentials(string category, string host, string username)
        {
            var applicationName = DeriveApplicationName(category, host, username);
            return ReadCredential(applicationName) ?? new Credential(CredentialType.Generic, applicationName, null, null);
        }

        public static void SetCredentials(string category, string host, string username, string token)
        {
            WriteCredential(DeriveApplicationName(category, host, username), username, token);
        }

        private static Credential ReadCredential(string applicationName)
        {
            IntPtr nCredPtr;
            var read = CredRead(applicationName, CredentialType.Generic, 0, out nCredPtr);
            
            if (read)
            {
                using (CriticalCredentialHandle critCred = new CriticalCredentialHandle(nCredPtr))
                {
                    var cred = critCred.GetCredential();

                    return ReadCredential(cred);
                }
            }

            return null;
        }

        private static Credential ReadCredential(CREDENTIAL credential)
        {
            var applicationName = Marshal.PtrToStringUni(credential.TargetName);
            var userName = Marshal.PtrToStringUni(credential.UserName);
            string secret = null;

            if (credential.CredentialBlob != IntPtr.Zero)
            {
                secret = Marshal.PtrToStringUni(credential.CredentialBlob, (int)credential.CredentialBlobSize / 2);
            }

            return new Credential(credential.Type, applicationName, userName, secret);
        }

        private static void WriteCredential(string applicationName, string userName, string secret)
        {
            var byteArray = secret == null ? null : Encoding.Unicode.GetBytes(secret);

            if (Environment.OSVersion.Version < new Version(6, 1) /* Windows 7 */)
            {
                // XP and Vista: 512; 
                if (byteArray != null && byteArray.Length > 512)
                {
                    throw new ArgumentOutOfRangeException(nameof(secret), "The secret message has exceeded 512 bytes.");
                }
            }
            else
            {
                // 7 and above: 5*512
                if (byteArray != null && byteArray.Length > 512 * 5)
                {
                    throw new ArgumentOutOfRangeException(nameof(secret), "The secret message has exceeded 2560 bytes.");
                }
            }

            var credential = new CREDENTIAL();
            credential.AttributeCount = 0;
            credential.Attributes = IntPtr.Zero;
            credential.Comment = IntPtr.Zero;
            credential.TargetAlias = IntPtr.Zero;
            credential.Type = CredentialType.Generic;
            credential.Persist = (uint)CredentialPersistence.LocalMachine;
            credential.CredentialBlobSize = (uint)(byteArray?.Length ?? 0);
            credential.TargetName = Marshal.StringToCoTaskMemUni(applicationName);
            credential.CredentialBlob = Marshal.StringToCoTaskMemUni(secret);
            credential.UserName = Marshal.StringToCoTaskMemUni(userName ?? Environment.UserName);

            bool written = CredWrite(ref credential, 0);
            Marshal.FreeCoTaskMem(credential.TargetName);
            Marshal.FreeCoTaskMem(credential.CredentialBlob);
            Marshal.FreeCoTaskMem(credential.UserName);

            if (!written)
            {
                int lastError = Marshal.GetLastWin32Error();
                throw new Exception($"CredWrite failed with the error code {lastError}.");
            }
        }

        [DllImport("Advapi32.dll", EntryPoint = "CredReadW", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern bool CredRead(string target, CredentialType type, int reservedFlag, out IntPtr credentialPtr);

        [DllImport("Advapi32.dll", EntryPoint = "CredWriteW", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern bool CredWrite([In] ref CREDENTIAL userCredential, [In] uint flags);

        [DllImport("Advapi32.dll", EntryPoint = "CredFree", SetLastError = true)]
        private static extern bool CredFree([In] IntPtr cred);

        private enum CredentialPersistence : uint
        {
            Session = 1,
            LocalMachine,
            Enterprise
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        private struct CREDENTIAL
        {
            public uint Flags;
            public CredentialType Type;
            public IntPtr TargetName;
            public IntPtr Comment;
            public System.Runtime.InteropServices.ComTypes.FILETIME LastWritten;
            public uint CredentialBlobSize;
            public IntPtr CredentialBlob;
            public uint Persist;
            public uint AttributeCount;
            public IntPtr Attributes;
            public IntPtr TargetAlias;
            public IntPtr UserName;
        }

        private sealed class CriticalCredentialHandle : CriticalHandleZeroOrMinusOneIsInvalid
        {
            public CriticalCredentialHandle(IntPtr preexistingHandle)
            {
                SetHandle(preexistingHandle);
            }

            public CREDENTIAL GetCredential()
            {
                if (!IsInvalid)
                {
                    var credential = (CREDENTIAL)Marshal.PtrToStructure(handle, typeof(CREDENTIAL));
                    return credential;
                }

                throw new InvalidOperationException("Invalid CriticalHandle!");
            }

            protected override bool ReleaseHandle()
            {
                if (!IsInvalid)
                {
                    CredFree(handle);
                    SetHandleAsInvalid();
                    return true;
                }

                return false;
            }
        }
    }
}