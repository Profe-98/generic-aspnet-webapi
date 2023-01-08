using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace WebApiApplicationService.InternalModels
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
                return File.Exists(ObjectPath);
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
                if (this.PathData != null)
                {
                    Type t = this.PathData.GetType();
                    return t == typeof(string) ? true : false;
                }
                throw new NotSupportedException("");
            }
        }
        public bool IsPathDataBinary
        {
            get
            {
                if (this.PathData != null)
                {
                    Type t = this.PathData.GetType();
                    return t == typeof(byte[]) ? true : false;
                }
                throw new NotSupportedException("");
            }
        }
        public bool HasPathData
        {
            get
            {
                return this.PathData == null ? false : true;
            }
        }
        public byte[] PathDataBinary
        {
            get
            {
                if (this.IsPathDataBinary)
                {
                    return (byte[])this.PathData;
                }
                throw new NotSupportedException("");
            }
        }
        public string PathDataString
        {
            get
            {
                if (this.IsPathDataString)
                {
                    return (string)this.PathData;
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
        public IExceptionList<Exception> WriteExceptions { get; private set; } = new IExceptionList<Exception>();
        public bool WouldFileBeWritten
        {
            get
            {
                return !this.ErrorWhileWriting;
            }
        }
        #endregion
        #region Ctor & Dtor
        public FileSystemResponseObject(IExceptionList<Exception> ex, string path, object data)
        {
            this.WriteExceptions = ex;
            this.PathData = data;
            this.ObjectPath = path;
        }
        ~FileSystemResponseObject()
        {
            this.Dispose();
        }
        #endregion
        #region Methods
        public virtual void Dispose()
        {
            GC.SuppressFinalize(this);
        }
        public bool ExceptionTypeIsOccured<T>() where T : Exception
        {
            if (this.WriteExceptions.FindLikeType(typeof(T)))
            {
                return true;
            }
            return false;
        }
        #endregion
    }
}
