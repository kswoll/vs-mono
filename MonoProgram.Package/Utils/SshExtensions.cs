using System;
using System.IO;
using System.Text.RegularExpressions;
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

        private static Regex errorRegex = new Regex(@"(.*\..*)\((.*)\,(.*)\)\: (.*)");

        private static SshCommand BeginCommand(this SshClient ssh, string commandText, IVsOutputWindowPane outputPane, AsyncCallback callback, out IAsyncResult asyncResult)
        {
            var command = ssh.CreateCommand(commandText);
            asyncResult = command.BeginExecute(callback);
            using (var reader = new StreamReader(command.OutputStream))
            {
                var s = reader.ReadToEnd();
                bool atErrors = false;
                foreach (var line in s.Split('\n'))
                {
                    if (line == "Errors:")
                        atErrors = true;

                    var match = errorRegex.Match(line);
                    if (atErrors && match.Success)
                    {
                        var file = match.Groups[1].Value.Trim();
                        var lineNumber = match.Groups[2].Value;
                        var message = match.Groups[4].Value;
                        outputPane.LogError(file, message, int.Parse(lineNumber) - 1);
                    }
                    else
                    {
                        outputPane.Log(line);
                    }
                }
            }
            return command;
        }

        public static int RunCommand(this SshClient ssh, string commandText, IVsOutputWindowPane outputPane)
        {
            IAsyncResult asyncResult;
            var command = ssh.BeginCommand(commandText, outputPane, out asyncResult);
            command.EndExecute(asyncResult);
            return command.ExitStatus;
        }
    }
}