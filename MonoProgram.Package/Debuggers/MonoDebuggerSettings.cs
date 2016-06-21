using System.Collections.Generic;
using System.Linq;

namespace MonoProgram.Package.Debuggers
{
    public class MonoDebuggerSettings
    {
        public string Host { get; }
        public string Username { get; }
        public string Password { get; }
        public IReadOnlyList<MonoSourceMapping> SourceMappings { get; }

        public MonoDebuggerSettings(string host, string username, string password, params MonoSourceMapping[] sourceMappings)
        {
            Host = host;
            Username = username;
            Password = password;
            SourceMappings = sourceMappings;
        }

        public static MonoDebuggerSettings Parse(string options)
        {
            var parts = options.Split(';');

            var sourceMappingsPart = parts[0];
            var sourceMappingsChunks = sourceMappingsPart.Split('&');
            var sourceMappings = sourceMappingsChunks.Select(x => x.Split('|')).Select(x => new MonoSourceMapping(x[0], x[1])).ToArray();

            var hostInformation = parts[1];
            var credentialsAndHost = hostInformation.Split('@');
            var host = credentialsAndHost[1];
            var usernameAndPassword = credentialsAndHost[0].Split(':');
            var username = usernameAndPassword[0];
            var password = usernameAndPassword[1];

            return new MonoDebuggerSettings(host, username, password, sourceMappings);
        }

        public override string ToString()
        {
            return $"{string.Join("&", SourceMappings.Select(x => $"{x.SourceRoot}|{x.BuildRoot}"))};{Username}:{Password}@{Host}";
        }
    }
}