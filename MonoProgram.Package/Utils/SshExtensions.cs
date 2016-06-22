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
        public static SshCommand BeginCommand(this SshClient ssh, string commandText, IVsOutputWindowPane outputPane, string project)
        {
            IAsyncResult asyncResult;
            return ssh.BeginCommand(commandText, outputPane, project, out asyncResult);
        }

        public static SshCommand BeginCommand(this SshClient ssh, string commandText, IVsOutputWindowPane outputPane, string project, AsyncCallback callback)
        {
            IAsyncResult asyncResult;
            return ssh.BeginCommand(commandText, outputPane, project, out asyncResult);
        }

        public static SshCommand BeginCommand(this SshClient ssh, string commandText, IVsOutputWindowPane outputPane, string project, out IAsyncResult asyncResult)
        {
            return ssh.BeginCommand(commandText, outputPane, project, null, out asyncResult);
        }

        private static readonly Regex errorRegex = new Regex(@"(.*\..*)\((.*)\,(.*)\)\: (.*)");
        private static readonly Regex locationlessErrorRegex = new Regex(@"(.*\..*?)\:(.*)");

        private static SshCommand BeginCommand(this SshClient ssh, string commandText, IVsOutputWindowPane outputPane, string project, AsyncCallback callback, out IAsyncResult asyncResult)
        {
            var command = ssh.CreateCommand(commandText);
            asyncResult = command.BeginExecute(callback);
            using (var reader = new StreamReader(command.OutputStream))
            {
                var s = reader.ReadToEnd();
                bool atWarnings = false;
                bool atErrors = false;
                foreach (var line in s.Split('\n'))
                {
                    if (line == "Warnings:")
                        atWarnings = true;
                    if (line == "Errors:")
                        atErrors = true;

                    var errorMatch = errorRegex.Match(line);
                    var locationlessErrorMatch = locationlessErrorRegex.Match(line);
                    var match = errorMatch.Success ? errorMatch : locationlessErrorMatch;
                    if ((atWarnings || atErrors) && match.Success)
                    {
                        var file = match.Groups[1].Value.Trim();
                        var lineNumber = 1;
                        var column = 0;
                        string message;
                        if (errorMatch.Success)
                        {
                            lineNumber = int.Parse(errorMatch.Groups[2].Value);
                            column = int.Parse(errorMatch.Groups[3].Value);
                            message = errorMatch.Groups[4].Value.Trim();
                        }
                        else
                        {
                            message = locationlessErrorMatch.Groups[2].Value.Trim();
                        }
                        VsLogSeverity severity = message.StartsWith("error") ? VsLogSeverity.Error : message.StartsWith("warning") ? VsLogSeverity.Warning : VsLogSeverity.Message;
                        int firstSpace = message.IndexOf(' ');
                        message = message.Substring(firstSpace + 1).TrimStart(' ', ':');
                        outputPane.Log(severity, project, file, line, message, lineNumber - 1, column);
                    }
                    else
                    {
                        outputPane.Log(line);
                    }
                }
            }
            return command;
        }

        public static int RunCommand(this SshClient ssh, string commandText, IVsOutputWindowPane outputPane, string project)
        {
            IAsyncResult asyncResult;
            var command = ssh.BeginCommand(commandText, outputPane, project, out asyncResult);
            command.EndExecute(asyncResult);
            return command.ExitStatus;
        }
    }
}