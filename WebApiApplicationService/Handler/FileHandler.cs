using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Compression;
using WebApiApplicationService.InternalModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;


namespace WebApiApplicationService.Handler
{
    public class FileHandler : IFileHandler
    {
        private readonly IAppconfig _appConfig;
        public virtual void Dispose()
        {
            GC.SuppressFinalize(this);
        }
        private Encoding _currentEncoding = Encoding.UTF8;
        public FileHandler(IAppconfig appconfig)
        {
            _appConfig = appconfig;
            CreateAppFolders();

        }

        #region FolderCreation & Deletion
        private List<FileSystemResponseObject> CreateAppFolders()
        {
            List<FileSystemResponseObject> responseObjects = new List<FileSystemResponseObject>();

            if (!Directory.Exists(AppPaths.LogFolderPath))
            {
                FileSystemResponseObject tmp = CreateFolder(AppPaths.LogFolderPath);
                responseObjects.Add(tmp);
            }
            if (!Directory.Exists(AppPaths.UserProfilePicturesPath))
            {
                FileSystemResponseObject tmp = CreateFolder(AppPaths.UserProfilePicturesPath);
                responseObjects.Add(tmp);
            }
            if (!Directory.Exists(_appConfig.AppServiceConfiguration.MailConfigurationModel.EmailAttachmentPath))
            {
                FileSystemResponseObject tmp = CreateFolder(_appConfig.AppServiceConfiguration.MailConfigurationModel.EmailAttachmentPath);
                responseObjects.Add(tmp);
            }
            if (!Directory.Exists(AppPaths.AvQuarantinePath))
            {
                FileSystemResponseObject tmp = CreateFolder(AppPaths.AvQuarantinePath);
                responseObjects.Add(tmp);
            }
            return responseObjects;
        }

        public FileSystemResponseObject CreateFolder(string path)
        {
            IExceptionList<Exception> exception = new IExceptionList<Exception>();
            string pathString = Path.Combine(path);

            try
            {
                if (!Directory.Exists(pathString))
                {
                    Directory.CreateDirectory(pathString);
                }
            }
            catch (ArgumentException ex)
            {
                exception.Add(ex, AppManager.MESSAGE_LEVEL.LEVEL_WARNING);
            }
            catch (UnauthorizedAccessException ex)
            {
                exception.Add(ex, AppManager.MESSAGE_LEVEL.LEVEL_WARNING);
            }
            catch (PathTooLongException ex)
            {
                exception.Add(ex, AppManager.MESSAGE_LEVEL.LEVEL_WARNING);
            }
            catch (NotSupportedException ex)
            {
                exception.Add(ex, AppManager.MESSAGE_LEVEL.LEVEL_WARNING);
            }
            catch (DirectoryNotFoundException ex)
            {
                exception.Add(ex, AppManager.MESSAGE_LEVEL.LEVEL_WARNING);
            }
            catch (FileNotFoundException ex)
            {
                exception.Add(ex, AppManager.MESSAGE_LEVEL.LEVEL_WARNING);
            }
            finally
            {

            }

            FileSystemResponseObject responseObject = new FileSystemResponseObject(exception, path, null);
            return responseObject;
        }
        public FileSystemResponseObject CreateFolder(string path, string folderName)
        {
            IExceptionList<Exception> exception = new IExceptionList<Exception>();
            string pathString = Path.Combine(path, folderName);

            try
            {
                if (!Directory.Exists(pathString))
                {
                    Directory.CreateDirectory(pathString);
                }
            }
            catch (ArgumentException ex)
            {
                exception.Add(ex, AppManager.MESSAGE_LEVEL.LEVEL_WARNING);
            }
            catch (UnauthorizedAccessException ex)
            {
                exception.Add(ex, AppManager.MESSAGE_LEVEL.LEVEL_WARNING);
            }
            catch (PathTooLongException ex)
            {
                exception.Add(ex, AppManager.MESSAGE_LEVEL.LEVEL_WARNING);
            }
            catch (NotSupportedException ex)
            {
                exception.Add(ex, AppManager.MESSAGE_LEVEL.LEVEL_WARNING);
            }
            catch (DirectoryNotFoundException ex)
            {
                exception.Add(ex, AppManager.MESSAGE_LEVEL.LEVEL_WARNING);
            }
            catch (FileNotFoundException ex)
            {
                exception.Add(ex, AppManager.MESSAGE_LEVEL.LEVEL_WARNING);
            }
            finally
            {
            }

            FileSystemResponseObject responseObject = new FileSystemResponseObject(exception, pathString, null);
            return responseObject;
        }
        #endregion
        #region CreateFile & Deletion

        public FileSystemResponseObject CreateFile(string path, string fileName)
        {
            IExceptionList<Exception> exception = new IExceptionList<Exception>();
            string pathString = Path.Combine(path, fileName);
            try
            {
                using (FileStream fileStream = File.Create(pathString))
                {
                    fileStream.Close();
                }
            }
            catch (ArgumentException ex)
            {
                exception.Add(ex, AppManager.MESSAGE_LEVEL.LEVEL_WARNING);
            }
            catch (UnauthorizedAccessException ex)
            {
                exception.Add(ex, AppManager.MESSAGE_LEVEL.LEVEL_WARNING);
            }
            catch (PathTooLongException ex)
            {
                exception.Add(ex, AppManager.MESSAGE_LEVEL.LEVEL_WARNING);
            }
            catch (NotSupportedException ex)
            {
                exception.Add(ex, AppManager.MESSAGE_LEVEL.LEVEL_WARNING);
            }
            catch (DirectoryNotFoundException ex)
            {
                exception.Add(ex, AppManager.MESSAGE_LEVEL.LEVEL_WARNING);
            }
            catch (FileLoadException ex)
            {
                exception.Add(ex, AppManager.MESSAGE_LEVEL.LEVEL_WARNING);
            }
            catch (FileNotFoundException ex)
            {
                exception.Add(ex, AppManager.MESSAGE_LEVEL.LEVEL_WARNING);
            }
            finally
            {
            }
            FileSystemResponseObject responseObject = new FileSystemResponseObject(exception, pathString, null);
            return responseObject;
        }
        public FileSystemResponseObject CreateFile(string path, string fileName, byte[] content)
        {
            IExceptionList<Exception> exception = new IExceptionList<Exception>();
            string pathString = Path.Combine(path, fileName);
            try
            {
                using (FileStream fileStream = File.Create(pathString))
                {
                    fileStream.Write(content, 0, content.Length);
                    fileStream.Close();
                }
            }
            catch (ArgumentException ex)
            {
                exception.Add(ex, AppManager.MESSAGE_LEVEL.LEVEL_WARNING);
            }
            catch (UnauthorizedAccessException ex)
            {
                exception.Add(ex, AppManager.MESSAGE_LEVEL.LEVEL_WARNING);
            }
            catch (PathTooLongException ex)
            {
                exception.Add(ex, AppManager.MESSAGE_LEVEL.LEVEL_WARNING);
            }
            catch (NotSupportedException ex)
            {
                exception.Add(ex, AppManager.MESSAGE_LEVEL.LEVEL_WARNING);
            }
            catch (DirectoryNotFoundException ex)
            {
                exception.Add(ex, AppManager.MESSAGE_LEVEL.LEVEL_WARNING);
            }
            catch (FileLoadException ex)
            {
                exception.Add(ex, AppManager.MESSAGE_LEVEL.LEVEL_WARNING);
            }
            catch (FileNotFoundException ex)
            {
                exception.Add(ex, AppManager.MESSAGE_LEVEL.LEVEL_WARNING);
            }
            finally
            {
            }
            FileSystemResponseObject responseObject = new FileSystemResponseObject(exception, pathString, content);
            return responseObject;
        }
        public FileSystemResponseObject CreateFile(string path, byte[] content)
        {
            IExceptionList<Exception> exception = new IExceptionList<Exception>();
            string pathString = Path.Combine(path);
            try
            {
                using (FileStream fileStream = File.Create(pathString))
                {
                    fileStream.Write(content, 0, content.Length);
                    fileStream.Close();
                }
            }
            catch (ArgumentException ex)
            {
                exception.Add(ex, AppManager.MESSAGE_LEVEL.LEVEL_WARNING);
            }
            catch (UnauthorizedAccessException ex)
            {
                exception.Add(ex, AppManager.MESSAGE_LEVEL.LEVEL_WARNING);
            }
            catch (PathTooLongException ex)
            {
                exception.Add(ex, AppManager.MESSAGE_LEVEL.LEVEL_WARNING);
            }
            catch (NotSupportedException ex)
            {
                exception.Add(ex, AppManager.MESSAGE_LEVEL.LEVEL_WARNING);
            }
            catch (DirectoryNotFoundException ex)
            {
                exception.Add(ex, AppManager.MESSAGE_LEVEL.LEVEL_WARNING);
            }
            catch (FileLoadException ex)
            {
                exception.Add(ex, AppManager.MESSAGE_LEVEL.LEVEL_WARNING);
            }
            catch (FileNotFoundException ex)
            {
                exception.Add(ex, AppManager.MESSAGE_LEVEL.LEVEL_WARNING);
            }
            finally
            {
            }
            FileSystemResponseObject responseObject = new FileSystemResponseObject(exception, pathString, content);
            return responseObject;
        }
        #endregion
        #region FileRead
        public async Task<FileSystemResponseObject> ReadAllText(string path)
        {
            FileSystemResponseObject responseObject = null;
            IExceptionList<Exception> exception = new IExceptionList<Exception>();
            string fileContent = null;
            if (File.Exists(path))
            {
                try
                {
                    using (FileStream fileStream = File.OpenRead(path))
                    {
                        using (StreamReader streamReader = new StreamReader(fileStream))
                        {
                            fileContent = await streamReader.ReadToEndAsync();
                        }
                    }

                }
                catch (ArgumentException ex)
                {
                    exception.Add(ex, AppManager.MESSAGE_LEVEL.LEVEL_WARNING);
                }
                catch (UnauthorizedAccessException ex)
                {
                    exception.Add(ex, AppManager.MESSAGE_LEVEL.LEVEL_WARNING);
                }
                catch (PathTooLongException ex)
                {
                    exception.Add(ex, AppManager.MESSAGE_LEVEL.LEVEL_WARNING);
                }
                catch (NotSupportedException ex)
                {
                    exception.Add(ex, AppManager.MESSAGE_LEVEL.LEVEL_WARNING);
                }
                catch (DirectoryNotFoundException ex)
                {
                    exception.Add(ex, AppManager.MESSAGE_LEVEL.LEVEL_WARNING);
                }
                catch (FileLoadException ex)
                {
                    exception.Add(ex, AppManager.MESSAGE_LEVEL.LEVEL_WARNING);
                }
                catch (FileNotFoundException ex)
                {
                    exception.Add(ex, AppManager.MESSAGE_LEVEL.LEVEL_WARNING);
                }
                finally
                {
                    responseObject = new FileSystemResponseObject(exception, path, fileContent);
                }
            }

            return responseObject;
        }
        public async Task<FileSystemResponseObject> ReadAllBytes(string path)
        {
            FileSystemResponseObject responseObject = null;
            IExceptionList<Exception> exception = new IExceptionList<Exception>();
            byte[] fileContent = null;
            if (File.Exists(path))
            {
                try
                {
                    using (FileStream fileStream = File.OpenRead(path))
                    {
                        long i = 0;
                        fileContent = new byte[fileStream.Length];
                        int numBytesToRead = (int)fileStream.Length;
                        int numBytesRead = 0;
                        while (numBytesToRead > 0)
                        {
                            int n = fileStream.Read(fileContent, numBytesRead, numBytesToRead);

                            if (n == 0)
                                break;

                            numBytesRead += n;
                            numBytesToRead -= n;
                        }
                    }

                }
                catch (ArgumentException ex)
                {
                    exception.Add(ex, AppManager.MESSAGE_LEVEL.LEVEL_WARNING);
                }
                catch (UnauthorizedAccessException ex)
                {
                    exception.Add(ex, AppManager.MESSAGE_LEVEL.LEVEL_WARNING);
                }
                catch (PathTooLongException ex)
                {
                    exception.Add(ex, AppManager.MESSAGE_LEVEL.LEVEL_WARNING);
                }
                catch (NotSupportedException ex)
                {
                    exception.Add(ex, AppManager.MESSAGE_LEVEL.LEVEL_WARNING);
                }
                catch (DirectoryNotFoundException ex)
                {
                    exception.Add(ex, AppManager.MESSAGE_LEVEL.LEVEL_WARNING);
                }
                catch (FileLoadException ex)
                {
                    exception.Add(ex, AppManager.MESSAGE_LEVEL.LEVEL_WARNING);
                }
                catch (FileNotFoundException ex)
                {
                    exception.Add(ex, AppManager.MESSAGE_LEVEL.LEVEL_WARNING);
                }
                finally
                {
                    responseObject = new FileSystemResponseObject(exception, path, fileContent);
                }
            }

            return responseObject;
        }
        #endregion
        #region FileWrite
        public async Task<FileSystemResponseObject> WriteAllText(string path, string content, Encoding encoding = null)
        {
            FileSystemResponseObject responseObject = null;
            byte[] contentBinary = null;

            if (content == null)
                return responseObject;

            contentBinary = _currentEncoding.GetBytes(content);
            FileSystemResponseObject response = await WriteAllBytes(path, contentBinary);

            responseObject = new FileSystemResponseObject(response.WriteExceptions, path, content);
            return responseObject;
        }
        public async Task<FileSystemResponseObject> WriteAllBytes(string path, byte[] content)
        {
            FileSystemResponseObject responseObject = null;
            IExceptionList<Exception> exception = new IExceptionList<Exception>();
            byte[] contentBinary = null;

            if (content == null)
                return responseObject;

            contentBinary = content;
            if (File.Exists(path))
            {
                try
                {
                    using (FileStream fileStream = File.OpenWrite(path))
                    {
                        await fileStream.WriteAsync(contentBinary, 0, contentBinary.Length);
                        fileStream.Close();
                    }
                }
                catch (ArgumentException ex)
                {
                    exception.Add(ex, AppManager.MESSAGE_LEVEL.LEVEL_WARNING);
                }
                catch (UnauthorizedAccessException ex)
                {
                    exception.Add(ex, AppManager.MESSAGE_LEVEL.LEVEL_WARNING);
                }
                catch (PathTooLongException ex)
                {
                    exception.Add(ex, AppManager.MESSAGE_LEVEL.LEVEL_WARNING);
                }
                catch (NotSupportedException ex)
                {
                    exception.Add(ex, AppManager.MESSAGE_LEVEL.LEVEL_WARNING);
                }
                catch (DirectoryNotFoundException ex)
                {
                    exception.Add(ex, AppManager.MESSAGE_LEVEL.LEVEL_WARNING);
                }
                catch (FileLoadException ex)
                {
                    exception.Add(ex, AppManager.MESSAGE_LEVEL.LEVEL_WARNING);
                }
                catch (FileNotFoundException ex)
                {
                    exception.Add(ex, AppManager.MESSAGE_LEVEL.LEVEL_WARNING);
                }
                finally
                {
                    responseObject = new FileSystemResponseObject(exception, path, contentBinary);
                }
            }
            else
            {
                responseObject = CreateFile(path, contentBinary);
            }

            return responseObject;
        }
        #endregion
        #region ZipArchiv
        public FileSystemResponseObject Zip(string sourceDir, string dstDir)
        {
            FileSystemResponseObject responseObject = null;
            IExceptionList<Exception> exception = new IExceptionList<Exception>();
            if (!Directory.Exists(sourceDir))
                return responseObject;

            if (!Directory.Exists(dstDir))
            {
                responseObject = CreateFolder(dstDir);
            }
            else
            {
                Directory.Delete(dstDir, true);
                responseObject = CreateFolder(dstDir);
            }
            try
            {
                ZipFile.CreateFromDirectory(sourceDir, dstDir);
            }
            catch (ArgumentException ex)
            {
                exception.Add(ex, AppManager.MESSAGE_LEVEL.LEVEL_WARNING);
            }
            catch (PathTooLongException ex)
            {
                exception.Add(ex, AppManager.MESSAGE_LEVEL.LEVEL_WARNING);
            }
            catch (DirectoryNotFoundException ex)
            {
                exception.Add(ex, AppManager.MESSAGE_LEVEL.LEVEL_WARNING);

            }
            catch (UnauthorizedAccessException ex)
            {

                exception.Add(ex, AppManager.MESSAGE_LEVEL.LEVEL_WARNING);
            }
            catch (NotSupportedException ex)
            {

                exception.Add(ex, AppManager.MESSAGE_LEVEL.LEVEL_WARNING);
            }
            finally
            {
            }


            if (Directory.Exists(dstDir))
                return responseObject;

            return responseObject;
        }
        public FileSystemResponseObject Unzip(string filePath, string dstDir)
        {
            FileSystemResponseObject responseObject = null;
            IExceptionList<Exception> exception = new IExceptionList<Exception>();
            if (!File.Exists(filePath))
                return responseObject;

            if (!Directory.Exists(dstDir))
            {
                responseObject = CreateFolder(dstDir);
            }
            else
            {
                Directory.Delete(dstDir, true);
                responseObject = CreateFolder(dstDir);
            }
            try
            {
                ZipFile.ExtractToDirectory(filePath, dstDir);
            }
            catch (ArgumentException ex)
            {
                exception.Add(ex, AppManager.MESSAGE_LEVEL.LEVEL_WARNING);
            }
            catch (PathTooLongException ex)
            {
                exception.Add(ex, AppManager.MESSAGE_LEVEL.LEVEL_WARNING);
            }
            catch (DirectoryNotFoundException ex)
            {
                exception.Add(ex, AppManager.MESSAGE_LEVEL.LEVEL_WARNING);

            }
            catch (UnauthorizedAccessException ex)
            {

                exception.Add(ex, AppManager.MESSAGE_LEVEL.LEVEL_WARNING);
            }
            catch (NotSupportedException ex)
            {

                exception.Add(ex, AppManager.MESSAGE_LEVEL.LEVEL_WARNING);
            }
            finally
            {
            }




            if (Directory.Exists(dstDir))
                return responseObject;
            return responseObject;
        }
        #endregion
    }
}
