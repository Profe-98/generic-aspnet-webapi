using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using WebApiFunction.Application.Model.Database.MySql;
using WebApiFunction.Application.Model.Database.MySql.Entity;
using WebApiFunction.Cache.Distributed.RedisCache;
using WebApiFunction.Ampq.Rabbitmq.Data;
using WebApiFunction.Ampq.Rabbitmq;
using WebApiFunction.Antivirus;
using WebApiFunction.Antivirus.nClam;
using WebApiFunction.Application.Model.DataTransferObject.Frontend.Transfer;
using WebApiFunction.Application.Model.DataTransferObject;
using WebApiFunction.Application.Model;
using WebApiFunction.Configuration;
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

    public class FileSystemResponseObject : IDisposable
    {
        #region Private

        #endregion
        #region Public
        public DirectoryInfo ObjectPathDirectoryInfo
        {
            get
            {
                if (IsObjectPathDirectory)
                {
                    DirectoryInfo dirInfo = new DirectoryInfo(ObjectPath);
                    return dirInfo;
                }
                else if (IsObjectPathFile)
                {
                    FileInfo fileInfo = new FileInfo(ObjectPath);
                    string folderPathFromFile = fileInfo.DirectoryName;
                    if (folderPathFromFile != null)
                    {
                        DirectoryInfo dirInfo = new DirectoryInfo(ObjectPath);
                        return dirInfo;
                    }
                }
                throw new NotSupportedException("");
            }
        }
        public FileInfo ObjectPathFileInfo
        {
            get
            {
                if (IsObjectPathFile)
                {
                    FileInfo fileInfo = new FileInfo(ObjectPath);
                    return fileInfo;
                }
                throw new NotSupportedException("");
            }
        }
        public bool IsObjectPathFile
        {
            get
            {
                return System.IO.File.Exists(ObjectPath);
            }
        }
        public bool IsObjectPathDirectory
        {
            get
            {
                return Directory.Exists(ObjectPath);
            }
        }
        public bool IsPathDataString
        {
            get
            {
                if (PathData != null)
                {
                    Type t = PathData.GetType();
                    return t == typeof(string) ? true : false;
                }
                throw new NotSupportedException("");
            }
        }
        public bool IsPathDataBinary
        {
            get
            {
                if (PathData != null)
                {
                    Type t = PathData.GetType();
                    return t == typeof(byte[]) ? true : false;
                }
                throw new NotSupportedException("");
            }
        }
        public bool HasPathData
        {
            get
            {
                return PathData == null ? false : true;
            }
        }
        public byte[] PathDataBinary
        {
            get
            {
                if (IsPathDataBinary)
                {
                    return (byte[])PathData;
                }
                throw new NotSupportedException("");
            }
        }
        public string PathDataString
        {
            get
            {
                if (IsPathDataString)
                {
                    return (string)PathData;
                }
                throw new NotSupportedException("");
            }
        }
        public object PathData { get; private set; }//content of ObjectPath / only avaible when ObjectPath is file
        public string ObjectPath { get; private set; }//path to file or directory
        public bool ErrorWhileWriting
        {
            get
            {
                return WriteExceptions.Count == 0 ? false : true;
            }
        }
        public WebApiFunction.Collections.IExceptionList<Exception> WriteExceptions { get; private set; } = new WebApiFunction.Collections.IExceptionList<Exception>();
        public bool WouldFileBeWritten
        {
            get
            {
                return !ErrorWhileWriting;
            }
        }
        #endregion
        #region Ctor & Dtor
        public FileSystemResponseObject(WebApiFunction.Collections.IExceptionList<Exception> ex, string path, object data)
        {
            WriteExceptions = ex;
            PathData = data;
            ObjectPath = path;
        }
        ~FileSystemResponseObject()
        {
            Dispose();
        }
        #endregion
        #region Methods
        public virtual void Dispose()
        {
            GC.SuppressFinalize(this);
        }
        public bool ExceptionTypeIsOccured<T>() where T : Exception
        {
            if (WriteExceptions.FindLikeType(typeof(T)))
            {
                return true;
            }
            return false;
        }
        #endregion
    }
}
