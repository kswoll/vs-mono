namespace MonoProgram.Package.Debuggers
{
    public class MonoSourceMapping
    {
        /// <summary>
        /// The path to the directory that contains the .csproj file.
        /// </summary>
        public string SourceRoot { get; }

        /// <summary>
        /// The path to the directory on the build server that contains the .csproj file.
        /// </summary>
        public string BuildRoot { get; }

        public MonoSourceMapping(string sourceRoot, string buildRoot)
        {
            SourceRoot = sourceRoot;
            BuildRoot = buildRoot;
        }
    }
}