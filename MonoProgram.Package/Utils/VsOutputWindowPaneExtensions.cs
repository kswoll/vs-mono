using Microsoft.VisualStudio.Shell.Interop;

namespace MonoProgram.Package.Utils
{
    public static class VsOutputWindowPaneExtensions
    {
        public static void Log(this IVsOutputWindowPane pane, string message)
        {
            pane.OutputString(message + "\r\n");
        }

        public static void LogError(this IVsOutputWindowPane pane, string file, string message)
        {
            pane.OutputTaskItemString(message + "\r\n", VSTASKPRIORITY.TP_HIGH, VSTASKCATEGORY.CAT_BUILDCOMPILE, "Test Subcategory", 
                0, file, 0, message);
        }
    }
}