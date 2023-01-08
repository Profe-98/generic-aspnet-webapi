﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApiApplicationService.Handler;

namespace WebApiApplicationService
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
        [Obsolete("Use Add(T exception, AppManager.MESSAGE_LEVEL exceptionLevel) instead of Add(T exception))]")]
        public new void Add(T exception)
        {
            throw new NotImplementedException("Use Add(T exception, AppManager.MESSAGE_LEVEL exceptionLevel) instead of Add(T exception))]");
        }
        [Obsolete("Use Insert(int index, T exception, AppManager.MESSAGE_LEVEL exceptionLevel) instead of Insert(int index, T exception)")]
        public new void Insert(int index, T exception)
        {
            throw new NotImplementedException("Use Insert(int index, T exception, AppManager.MESSAGE_LEVEL exceptionLevel) instead of Insert(int index, T exception)");
        }
        public void Add(T exception, AppManager.MESSAGE_LEVEL exceptionLevel)
        {
            internalList.Add(exception);
            ExceptionHandledEventArgs eventArgs = new ExceptionHandledEventArgs();
            eventArgs.Exception = exception;
            eventArgs.Index = this.Count - 1;
            OnExceptionAdded(this, eventArgs);
        }
        public void Insert(int index, T exception, AppManager.MESSAGE_LEVEL exceptionLevel)
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
            ExceptionHandler.ReportException(this.ToString(), e.Exception, e.ExceptionLevel);
        }
        #endregion
        #region EventArgs
        public class ExceptionHandledEventArgs : EventArgs
        {
            public AppManager.MESSAGE_LEVEL ExceptionLevel { get; set; } = AppManager.MESSAGE_LEVEL.LEVEL_INFO;
            public Exception Exception { get; set; }
            public int Index { get; set; }
        }
        #endregion
    }
}
