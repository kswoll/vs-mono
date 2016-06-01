namespace MonoProgram.Package.Debuggers
{
    public class MonoDebuggerSettings
    {
        public string Host { get; set; } 
        public string Username { get; set; }
        public string Password { get; set; }

        /// <summary>
        /// The path to the directory that contains the .csproj file.
        /// </summary>
        public string SourceRoot { get; set; }

        /// <summary>
        /// The path to the directory on the build server that contains the .csproj file.
        /// </summary>
        public string BuildRoot { get; set; }

        public MonoDebuggerSettings(string host, string username, string password, string sourceRoot, string buildRoot)
        {
            Host = host;
            Username = username;
            Password = password;
            SourceRoot = sourceRoot;
            BuildRoot = buildRoot;
        }

        public static MonoDebuggerSettings Parse(string options)
        {
            var parts = options.Split(';');

            var sourceRoot = parts[0];
            var buildRoot = parts[1];

            var hostInformation = parts[2];
            var credentialsAndHost = hostInformation.Split('@');
            var host = credentialsAndHost[1];
            var usernameAndPassword = credentialsAndHost[0].Split(':');
            var username = usernameAndPassword[0];
            var password = usernameAndPassword[1];

            return new MonoDebuggerSettings(host, username, password, sourceRoot, buildRoot);
        }

        public override string ToString()
        {
            return $"{SourceRoot};{BuildRoot};{Username}:{Password}@{Host}";
        }
    }
}