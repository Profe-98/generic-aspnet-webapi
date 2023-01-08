using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApiApplicationService.InternalModels;
using System.Text;

namespace WebApiApplicationService
{
    public interface IFileHandler : IDisposable
    {
        public FileSystemResponseObject CreateFolder(string path);
        public FileSystemResponseObject CreateFolder(string path, string folderName);

        public FileSystemResponseObject CreateFile(string path, string fileName);
        public FileSystemResponseObject CreateFile(string path, string fileName, byte[] content);
        public FileSystemResponseObject CreateFile(string path, byte[] content);

        public Task<FileSystemResponseObject> ReadAllText(string path);
        public Task<FileSystemResponseObject> ReadAllBytes(string path);

        public Task<FileSystemResponseObject> WriteAllText(string path, string content, Encoding encoding = null);
        public Task<FileSystemResponseObject> WriteAllBytes(string path, byte[] content);

        public FileSystemResponseObject Zip(string sourceDir, string dstDir);
        public FileSystemResponseObject Unzip(string filePath, string dstDir);
    }
}
