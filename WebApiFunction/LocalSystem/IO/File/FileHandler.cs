using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.Compression;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using WebApiFunction.Application.Model.Database.MySql;
using WebApiFunction.Application.Model.Database.MySql.Entity;
using WebApiFunction.Cache.Distributed.RedisCache;
using WebApiFunction.Ampq.Rabbitmq.Data;
using WebApiFunction.Ampq.Rabbitmq;
using WebApiFunction.Antivirus;
using WebApiFunction.Antivirus.nClam;
using WebApiFunction.Application.Model.DataTransferObject.Helix.Frontend.Transfer;
using WebApiFunction.Application.Model.DataTransferObject;
using WebApiFunction.Application.Model;
using WebApiFunction.Configuration;
using WebApiFunction.Collections;
using WebApiFunction.Controller;
using WebApiFunction.Data;
using WebApiFunction.Data.Web;
using WebApiFunction.Data.Format.Json;
using WebApiFunction.Data.Web.Api.Abstractions.JsonApiV1;
using WebApiFunction.Database;
using WebApiFunction.Database.MySQL;
using WebApiFunction.Database.MySQL.Data;
using WebApiFunction.Filter;
using WebApiFunction.Formatter;
using WebApiFunction.Healthcheck;
using WebApiFunction.LocalSystem.IO.File;
using WebApiFunction.Log;
using WebApiFunction.Metric;
using WebApiFunction.Metric.Influxdb;
using WebApiFunction.MicroService;
using WebApiFunction.Network;
using WebApiFunction.Security;
using WebApiFunction.Security.Encryption;
using WebApiFunction.Threading;
using WebApiFunction.Threading.Service;
using WebApiFunction.Threading.Task;
using WebApiFunction.Utility;
using WebApiFunction.Web;
using WebApiFunction.Web.AspNet;
using WebApiFunction.Web.Authentification;
using WebApiFunction.Web.Http.Api.Abstractions.JsonApiV1;
using WebApiFunction.Web.Http;

namespace WebApiFunction.LocalSystem.IO.File
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
            if (appconfig != null && appconfig.AppServiceConfiguration != null && appconfig.AppServiceConfiguration.AppPaths.Keys.Count != 0)
                CreateAppFolders(appconfig.AppServiceConfiguration.AppPaths.Values.ToArray());

        }

        #region FolderCreation & Deletion
        private List<FileSystemResponseObject> CreateAppFolders(string[] pathsToCreate)
        {
            List<FileSystemResponseObject> responseObjects = new List<FileSystemResponseObject>();

            foreach (string path in pathsToCreate)
            {

                if (!Directory.Exists(path))
                {
                    FileSystemResponseObject tmp = CreateFolder(path);
                    responseObjects.Add(tmp);
                }
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
                exception.Add(ex, Log.General.MESSAGE_LEVEL.LEVEL_WARNING);
            }
            catch (UnauthorizedAccessException ex)
            {
                exception.Add(ex, Log.General.MESSAGE_LEVEL.LEVEL_WARNING);
            }
            catch (PathTooLongException ex)
            {
                exception.Add(ex, Log.General.MESSAGE_LEVEL.LEVEL_WARNING);
            }
            catch (NotSupportedException ex)
            {
                exception.Add(ex, Log.General.MESSAGE_LEVEL.LEVEL_WARNING);
            }
            catch (DirectoryNotFoundException ex)
            {
                exception.Add(ex, Log.General.MESSAGE_LEVEL.LEVEL_WARNING);
            }
            catch (FileNotFoundException ex)
            {
                exception.Add(ex, Log.General.MESSAGE_LEVEL.LEVEL_WARNING);
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
                exception.Add(ex, Log.General.MESSAGE_LEVEL.LEVEL_WARNING);
            }
            catch (UnauthorizedAccessException ex)
            {
                exception.Add(ex, Log.General.MESSAGE_LEVEL.LEVEL_WARNING);
            }
            catch (PathTooLongException ex)
            {
                exception.Add(ex, Log.General.MESSAGE_LEVEL.LEVEL_WARNING);
            }
            catch (NotSupportedException ex)
            {
                exception.Add(ex, Log.General.MESSAGE_LEVEL.LEVEL_WARNING);
            }
            catch (DirectoryNotFoundException ex)
            {
                exception.Add(ex, Log.General.MESSAGE_LEVEL.LEVEL_WARNING);
            }
            catch (FileNotFoundException ex)
            {
                exception.Add(ex, Log.General.MESSAGE_LEVEL.LEVEL_WARNING);
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
                using (FileStream fileStream = System.IO.File.Create(pathString))
                {
                    fileStream.Close();
                }
            }
            catch (ArgumentException ex)
            {
                exception.Add(ex, Log.General.MESSAGE_LEVEL.LEVEL_WARNING);
            }
            catch (UnauthorizedAccessException ex)
            {
                exception.Add(ex, Log.General.MESSAGE_LEVEL.LEVEL_WARNING);
            }
            catch (PathTooLongException ex)
            {
                exception.Add(ex, Log.General.MESSAGE_LEVEL.LEVEL_WARNING);
            }
            catch (NotSupportedException ex)
            {
                exception.Add(ex, Log.General.MESSAGE_LEVEL.LEVEL_WARNING);
            }
            catch (DirectoryNotFoundException ex)
            {
                exception.Add(ex, Log.General.MESSAGE_LEVEL.LEVEL_WARNING);
            }
            catch (FileLoadException ex)
            {
                exception.Add(ex, Log.General.MESSAGE_LEVEL.LEVEL_WARNING);
            }
            catch (FileNotFoundException ex)
            {
                exception.Add(ex, Log.General.MESSAGE_LEVEL.LEVEL_WARNING);
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
                using (FileStream fileStream = System.IO.File.Create(pathString))
                {
                    fileStream.Write(content, 0, content.Length);
                    fileStream.Close();
                }
            }
            catch (ArgumentException ex)
            {
                exception.Add(ex, Log.General.MESSAGE_LEVEL.LEVEL_WARNING);
            }
            catch (UnauthorizedAccessException ex)
            {
                exception.Add(ex, Log.General.MESSAGE_LEVEL.LEVEL_WARNING);
            }
            catch (PathTooLongException ex)
            {
                exception.Add(ex, Log.General.MESSAGE_LEVEL.LEVEL_WARNING);
            }
            catch (NotSupportedException ex)
            {
                exception.Add(ex, Log.General.MESSAGE_LEVEL.LEVEL_WARNING);
            }
            catch (DirectoryNotFoundException ex)
            {
                exception.Add(ex, Log.General.MESSAGE_LEVEL.LEVEL_WARNING);
            }
            catch (FileLoadException ex)
            {
                exception.Add(ex, Log.General.MESSAGE_LEVEL.LEVEL_WARNING);
            }
            catch (FileNotFoundException ex)
            {
                exception.Add(ex, Log.General.MESSAGE_LEVEL.LEVEL_WARNING);
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
                using (FileStream fileStream = System.IO.File.Create(pathString))
                {
                    fileStream.Write(content, 0, content.Length);
                    fileStream.Close();
                }
            }
            catch (ArgumentException ex)
            {
                exception.Add(ex, Log.General.MESSAGE_LEVEL.LEVEL_WARNING);
            }
            catch (UnauthorizedAccessException ex)
            {
                exception.Add(ex, Log.General.MESSAGE_LEVEL.LEVEL_WARNING);
            }
            catch (PathTooLongException ex)
            {
                exception.Add(ex, Log.General.MESSAGE_LEVEL.LEVEL_WARNING);
            }
            catch (NotSupportedException ex)
            {
                exception.Add(ex, Log.General.MESSAGE_LEVEL.LEVEL_WARNING);
            }
            catch (DirectoryNotFoundException ex)
            {
                exception.Add(ex, Log.General.MESSAGE_LEVEL.LEVEL_WARNING);
            }
            catch (FileLoadException ex)
            {
                exception.Add(ex, Log.General.MESSAGE_LEVEL.LEVEL_WARNING);
            }
            catch (FileNotFoundException ex)
            {
                exception.Add(ex, Log.General.MESSAGE_LEVEL.LEVEL_WARNING);
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
            if (System.IO.File.Exists(path))
            {
                try
                {
                    using (FileStream fileStream = System.IO.File.OpenRead(path))
                    {
                        using (StreamReader streamReader = new StreamReader(fileStream))
                        {
                            fileContent = await streamReader.ReadToEndAsync();
                        }
                    }

                }
                catch (ArgumentException ex)
                {
                    exception.Add(ex, Log.General.MESSAGE_LEVEL.LEVEL_WARNING);
                }
                catch (UnauthorizedAccessException ex)
                {
                    exception.Add(ex, Log.General.MESSAGE_LEVEL.LEVEL_WARNING);
                }
                catch (PathTooLongException ex)
                {
                    exception.Add(ex, Log.General.MESSAGE_LEVEL.LEVEL_WARNING);
                }
                catch (NotSupportedException ex)
                {
                    exception.Add(ex, Log.General.MESSAGE_LEVEL.LEVEL_WARNING);
                }
                catch (DirectoryNotFoundException ex)
                {
                    exception.Add(ex, Log.General.MESSAGE_LEVEL.LEVEL_WARNING);
                }
                catch (FileLoadException ex)
                {
                    exception.Add(ex, Log.General.MESSAGE_LEVEL.LEVEL_WARNING);
                }
                catch (FileNotFoundException ex)
                {
                    exception.Add(ex, Log.General.MESSAGE_LEVEL.LEVEL_WARNING);
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
            if (System.IO.File.Exists(path))
            {
                try
                {
                    using (FileStream fileStream = System.IO.File.OpenRead(path))
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
                    exception.Add(ex, Log.General.MESSAGE_LEVEL.LEVEL_WARNING);
                }
                catch (UnauthorizedAccessException ex)
                {
                    exception.Add(ex, Log.General.MESSAGE_LEVEL.LEVEL_WARNING);
                }
                catch (PathTooLongException ex)
                {
                    exception.Add(ex, Log.General.MESSAGE_LEVEL.LEVEL_WARNING);
                }
                catch (NotSupportedException ex)
                {
                    exception.Add(ex, Log.General.MESSAGE_LEVEL.LEVEL_WARNING);
                }
                catch (DirectoryNotFoundException ex)
                {
                    exception.Add(ex, Log.General.MESSAGE_LEVEL.LEVEL_WARNING);
                }
                catch (FileLoadException ex)
                {
                    exception.Add(ex, Log.General.MESSAGE_LEVEL.LEVEL_WARNING);
                }
                catch (FileNotFoundException ex)
                {
                    exception.Add(ex, Log.General.MESSAGE_LEVEL.LEVEL_WARNING);
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
            if (System.IO.File.Exists(path))
            {
                try
                {
                    using (FileStream fileStream = System.IO.File.OpenWrite(path))
                    {
                        await fileStream.WriteAsync(contentBinary, 0, contentBinary.Length);
                        fileStream.Close();
                    }
                }
                catch (ArgumentException ex)
                {
                    exception.Add(ex, Log.General.MESSAGE_LEVEL.LEVEL_WARNING);
                }
                catch (UnauthorizedAccessException ex)
                {
                    exception.Add(ex, Log.General.MESSAGE_LEVEL.LEVEL_WARNING);
                }
                catch (PathTooLongException ex)
                {
                    exception.Add(ex, Log.General.MESSAGE_LEVEL.LEVEL_WARNING);
                }
                catch (NotSupportedException ex)
                {
                    exception.Add(ex, Log.General.MESSAGE_LEVEL.LEVEL_WARNING);
                }
                catch (DirectoryNotFoundException ex)
                {
                    exception.Add(ex, Log.General.MESSAGE_LEVEL.LEVEL_WARNING);
                }
                catch (FileLoadException ex)
                {
                    exception.Add(ex, Log.General.MESSAGE_LEVEL.LEVEL_WARNING);
                }
                catch (FileNotFoundException ex)
                {
                    exception.Add(ex, Log.General.MESSAGE_LEVEL.LEVEL_WARNING);
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
                exception.Add(ex, Log.General.MESSAGE_LEVEL.LEVEL_WARNING);
            }
            catch (PathTooLongException ex)
            {
                exception.Add(ex, Log.General.MESSAGE_LEVEL.LEVEL_WARNING);
            }
            catch (DirectoryNotFoundException ex)
            {
                exception.Add(ex, Log.General.MESSAGE_LEVEL.LEVEL_WARNING);

            }
            catch (UnauthorizedAccessException ex)
            {

                exception.Add(ex, Log.General.MESSAGE_LEVEL.LEVEL_WARNING);
            }
            catch (NotSupportedException ex)
            {

                exception.Add(ex, Log.General.MESSAGE_LEVEL.LEVEL_WARNING);
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
            if (!System.IO.File.Exists(filePath))
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
                exception.Add(ex, Log.General.MESSAGE_LEVEL.LEVEL_WARNING);
            }
            catch (PathTooLongException ex)
            {
                exception.Add(ex, Log.General.MESSAGE_LEVEL.LEVEL_WARNING);
            }
            catch (DirectoryNotFoundException ex)
            {
                exception.Add(ex, Log.General.MESSAGE_LEVEL.LEVEL_WARNING);

            }
            catch (UnauthorizedAccessException ex)
            {

                exception.Add(ex, Log.General.MESSAGE_LEVEL.LEVEL_WARNING);
            }
            catch (NotSupportedException ex)
            {

                exception.Add(ex, Log.General.MESSAGE_LEVEL.LEVEL_WARNING);
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
