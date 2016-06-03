using System.IO;
using Renci.SshNet;

namespace MonoProgram.Package.Utils
{
    public static class SftpExtensions
    {
        public static void Clear(this SftpClient client)
        {
            Clear(client, ".");
        }

        public static void Clear(this SftpClient client, string path)
        {
            foreach (var file in client.ListDirectory(path))
            {
                if (file.IsDirectory)
                {
                    client.Clear(file.Name);
                }
                file.Delete();
            }
        }

        public static void Upload(this SftpClient client, string localFolder)
        {
            var directory = new DirectoryInfo(localFolder);
            foreach (var file in directory.EnumerateFiles())
            {
                using (var fileStream = file.OpenRead())
                {
                    client.UploadFile(fileStream, file.Name);
                }
            }
        }

        public static void CreateFullDirectory(this SftpClient client, string path)
        {
            var parts = path.Split('/');
            var currentPath = ".";
            foreach (var part in parts)
            {
                var newPath = Path.Combine(currentPath, part);
                if (!client.Exists(newPath)) 
                    client.CreateDirectory(newPath);
                currentPath = newPath;
            }
        }
    }
}