using System;

namespace MonoProgram.Package.Utils
{
    public class FileUtils
    {
        public static string ToRelativePath(string basePath, string childPath)
        {
            if (!childPath.StartsWith(basePath, StringComparison.InvariantCultureIgnoreCase))
                throw new Exception($"{childPath} is not contained inside {basePath}");

            childPath = childPath.Substring(basePath.Length);
            childPath = childPath.Trim('\\');
            return childPath;
        } 
    }
}