using System.IO;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Shell.Interop;
using Renci.SshNet;

namespace MonoProgram.Package.Utils
{
    public static class SshExtensions
    {
        public static void RunCommand(this SshClient ssh, string commandText, IVsOutputWindowPane outputPane)
        {
            var command = ssh.CreateCommand(commandText);
            var asyncResult = command.BeginExecute();
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
            command.EndExecute(asyncResult);
        }
    }
}