using Microsoft.VisualStudio.Shell.Interop;

namespace MonoProgram.Package.Utils
{
    public static class VsOutputWindowPaneExtensions
    {
        public static void Log(this IVsOutputWindowPane pane, string message)
        {
            pane.OutputString(message + "\r\n");
        }
    }
}