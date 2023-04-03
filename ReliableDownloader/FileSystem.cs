using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ReliableDownloader
{
    public class FileSystem : IFileSystem
    {
        public void CreateDirectory(string path)
        {
            Directory.CreateDirectory(path);
        }

        public bool DirectoryExists(string path)
        {
            return Directory.Exists(path);
        }

        public void CreateFile(string path)
        {
            File.Create(path);
        }

        public void DeleteFile(string path)
        {
            File.Delete(path);
        }

        public bool FileExists(string path)
        {
            return File.Exists(path);
        }

        public Stream OpenFile(string path, FileMode mode = FileMode.Append, FileAccess access = FileAccess.Write)
        {
            return File.Open(path, mode, access);
        }

        public Stream OpenFileReadAccess(string path, FileMode mode = FileMode.Open, FileAccess access = FileAccess.Read)
        {
            return File.Open(path, mode, access);
        }

        public FileInfo GetFileInfo(string path)
        {
            return new FileInfo(path);
        }

        public void VerifyIntegrity(string path, HttpResponseMessage response)
        {
            using var stream = response.Content.ReadAsStreamAsync().Result;
                using var file = OpenFileReadAccess(path);
                var hash = Convert.ToBase64String(MD5.Create().ComputeHash(file));
                if (!hash.Equals(Convert.ToBase64String(response.Content.Headers.ContentMD5)))
                {
                    Console.WriteLine("File corrupted. Deleting...");
                    file.Close();
                    DeleteFile(path);
                    throw new IOException("The file is corrupted.");
                }
            
            
        }
    }
}
