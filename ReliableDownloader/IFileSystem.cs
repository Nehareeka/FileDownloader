using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ReliableDownloader
{
    public interface IFileSystem
    {
        void CreateDirectory(string path);
        bool DirectoryExists(string path);
        bool FileExists(string path);
        void DeleteFile(string path);
        void CreateFile(string path);
        FileInfo GetFileInfo(string path);
        Stream OpenFile(string path, FileMode mode = FileMode.Append, FileAccess access = FileAccess.Write);
        Stream OpenFileReadAccess(string path, FileMode mode = FileMode.Open, FileAccess access = FileAccess.Read);
        void VerifyIntegrity(string path, HttpResponseMessage response);
    }
}
