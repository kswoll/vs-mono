using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Shell.Interop;
using Renci.SshNet;

namespace MonoProgram.Package.Utils
{
    public static class SshExtensions
    {
        public static SshCommand BeginCommand(this SshClient ssh, string commandText, IVsOutputWindowPane outputPane)
        {
            IAsyncResult asyncResult;
            return ssh.BeginCommand(commandText, outputPane, out asyncResult);
        }

        public static SshCommand BeginCommand(this SshClient ssh, string commandText, IVsOutputWindowPane outputPane, AsyncCallback callback)
        {
            IAsyncResult asyncResult;
            return ssh.BeginCommand(commandText, outputPane, out asyncResult);
        }

        public static SshCommand BeginCommand(this SshClient ssh, string commandText, IVsOutputWindowPane outputPane, out IAsyncResult asyncResult)
        {
            return ssh.BeginCommand(commandText, outputPane, null, out asyncResult);
        }

        private static SshCommand BeginCommand(this SshClient ssh, string commandText, IVsOutputWindowPane outputPane, AsyncCallback callback, out IAsyncResult asyncResult)
        {
            var command = ssh.CreateCommand(commandText);
            asyncResult = command.BeginExecute(callback);
            Task.Run(() =>
            {
                using (var reader = new StreamReader(command.OutputStream))
                {
                    for (var line = reader.ReadLine(); line != null; line = reader.ReadLine())
                    {
                        outputPane.Log(line);
                    }
                }
            });
            return command;
        }

        public static void RunCommand(this SshClient ssh, string commandText, IVsOutputWindowPane outputPane)
        {
            IAsyncResult asyncResult;
            var command = ssh.BeginCommand(commandText, outputPane, out asyncResult);
            command.EndExecute(asyncResult);
        }
    }
}