using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiFunction.Application.Model.Internal;


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
using WebApiFunction.Web.AspNet.Controller;
using WebApiFunction.Data;
using WebApiFunction.Data.Web;
using WebApiFunction.Data.Format.Json;
using WebApiFunction.Data.Web.Api.Abstractions.JsonApiV1;
using WebApiFunction.Database;
using WebApiFunction.Application.Model.Database.MySQL;
using WebApiFunction.Application.Model.Database.MySQL.Data;
using WebApiFunction.Web.AspNet.Filter;
using WebApiFunction.Formatter;
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

namespace WebApiFunction.Collections
{

    public class IExceptionList<T> : IBaseList<T> where T : Exception
    {
        #region Private

        private IList<T> internalList = new List<T>();

        #endregion
        #region Public

        public new T this[int i]
        {
            get
            {
                return internalList[i];
            }
        }
        #endregion
        #region Ctor & Dtor
        public IExceptionList()
        {

        }
        ~IExceptionList()
        {

        }
        #endregion
        #region Methods
        [Obsolete("Use Add(T exception, WebApiFunction.Log.General.MESSAGE_LEVEL exceptionLevel) instead of Add(T exception))]")]
        public new void Add(T exception)
        {
            throw new NotImplementedException("Use Add(T exception, WebApiFunction.Log.General.MESSAGE_LEVEL exceptionLevel) instead of Add(T exception))]");
        }
        [Obsolete("Use Insert(int index, T exception, WebApiFunction.Log.General.MESSAGE_LEVEL exceptionLevel) instead of Insert(int index, T exception)")]
        public new void Insert(int index, T exception)
        {
            throw new NotImplementedException("Use Insert(int index, T exception, WebApiFunction.Log.General.MESSAGE_LEVEL exceptionLevel) instead of Insert(int index, T exception)");
        }
        public void Add(T exception, Log.General.MESSAGE_LEVEL exceptionLevel)
        {
            internalList.Add(exception);
            ExceptionHandledEventArgs eventArgs = new ExceptionHandledEventArgs();
            eventArgs.Exception = exception;
            eventArgs.Index = Count - 1;
            OnExceptionAdded(this, eventArgs);
        }
        public void Insert(int index, T exception, Log.General.MESSAGE_LEVEL exceptionLevel)
        {
            internalList.Insert(index, exception);
            ExceptionHandledEventArgs eventArgs = new ExceptionHandledEventArgs();
            eventArgs.Exception = exception;
            eventArgs.Index = index;
            OnExceptionAdded(this, eventArgs);
        }
        public bool FindLikeType(Type excObjType)
        {
            foreach (T item in internalList)
            {
                Type t = item.GetType();
                if (excObjType == t)
                    return true;
            }
            return false;
        }
        #endregion
        #region Events

        public delegate void ExceptionAddEventHandler<T, U>(T value, U eventArgs);
        public event ExceptionAddEventHandler<object, ExceptionHandledEventArgs> AddExceptionEvent;
        protected virtual void OnExceptionAdded(object sender, ExceptionHandledEventArgs e)
        {
            AddExceptionEvent(sender, e);
        }
        #endregion
        #region EventArgs
        public class ExceptionHandledEventArgs : EventArgs
        {
            public Log.General.MESSAGE_LEVEL ExceptionLevel { get; set; } = Log.General.MESSAGE_LEVEL.LEVEL_INFO;
            public Exception Exception { get; set; }
            public int Index { get; set; }
        }
        #endregion
    }
}
