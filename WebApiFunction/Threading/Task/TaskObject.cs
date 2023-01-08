﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;
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
using System.Web.Http.ExceptionHandling;
using WebApiFunction.Application;

namespace WebApiFunction.Threading.Task
{

    public class TaskObject : IDisposable
    {
        private CancellationTokenSource _taskCancelTokenInstance = new CancellationTokenSource();
        private CancellationToken _taskCancelToken;
        private Action _action;
        private Func<System.Threading.Tasks.Task> _faction;
        private object[] _args;
        private System.Threading.Tasks.Task _task = null;
        private TimeSpan _repeatTime = TimeSpan.Zero;
        private DateTime _nextExecTime = DateTime.MinValue;
        private DateTime _createTime = DateTime.MinValue;
        private delegate void CancelTokenRequestDefaultMethod();
        private CancelTokenRequestDefaultMethod DefaultMethod;
        private event EventHandler<TaskCompletionEventArgs> _completionEvent;
        private event EventHandler<TaskCancelRequestEventArgs> _cancelTaskRequestEvent;
        private event EventHandler<Exception> _exceptionEvent;

        public event EventHandler<Exception> ExceptionEvent
        {
            add => _exceptionEvent += value;
            remove => _exceptionEvent += value;
        }
        public event EventHandler<TaskCompletionEventArgs> CompletionEvent
        {
            add => _completionEvent += value;
            remove => _completionEvent += value;
        }
        public event EventHandler<TaskCancelRequestEventArgs> CancelRequestEvent
        {
            add => _cancelTaskRequestEvent += value;
            remove => _cancelTaskRequestEvent += value;
        }
        public CancellationTokenSource CancellationTokenInstance
        {
            get
            {
                return _taskCancelTokenInstance;
            }
        }

        public TaskObject(Action action, Action cancelTokenRequestAction = null)
        {
            Init(action, TimeSpan.Zero, cancelTokenRequestAction);
        }
        public TaskObject(Action action, TimeSpan repeationTime, Action cancelTokenRequestAction = null)
        {
            Init(action, repeationTime, cancelTokenRequestAction);
        }
        public TaskObject(Func<System.Threading.Tasks.Task> action, TimeSpan repeationTime, Action cancelTokenRequestAction = null)
        {
            Init(action, repeationTime, cancelTokenRequestAction);
        }

        private void Init(Action action, TimeSpan repeationTime, Action cancelTokenRequestAction = null, object[] args = null)
        {
            _createTime = DateTime.Now;
            DefaultMethod = DefaultCancelTaskRequestMethod;
            _args = args;
            if (action == null)
                Dispose();
            _action = action;
            _repeatTime = repeationTime;

            _taskCancelToken = _taskCancelTokenInstance.Token;
            _taskCancelToken.Register(cancelTokenRequestAction == null ?
                new Action(DefaultMethod) : cancelTokenRequestAction);

        }

        private void Init(Func<System.Threading.Tasks.Task> action, TimeSpan repeationTime, Action cancelTokenRequestAction = null, object[] args = null)
        {
            _createTime = DateTime.Now;
            DefaultMethod = DefaultCancelTaskRequestMethod;
            _args = args;
            if (action == null)
                Dispose();
            _faction = action;
            _repeatTime = repeationTime;

            _taskCancelToken = _taskCancelTokenInstance.Token;
            _taskCancelToken.Register(cancelTokenRequestAction == null ?
                new Action(DefaultMethod) : cancelTokenRequestAction);

        }

        ~TaskObject()
        {
            Dispose();
        }
        public string MonitorLockName
        {
            get
            {
                return "taskobject_locker_" + _createTime.Ticks.ToString();
            }
        }
        public Delegate[] AllCompletedEventSubscribers
        {
            get
            {
                var alternative = new Delegate[0];
                return _completionEvent == null ?
                    alternative : _completionEvent.GetInvocationList() ?? alternative;
            }
        }
        public bool IsNextRepeationAvaible
        {
            get
            {
                return (this.NextExecTime <= DateTime.Now);
            }
        }
        public DateTime NextExecTime
        {
            get
            {
                return _nextExecTime;
            }
        }
        public bool IsRunning
        {
            get
            {
                return (_task.Status == TaskStatus.Running ?
                    (true) : (false));
            }
        }
        public bool IsRepeatTask
        {
            get
            {
                return (_repeatTime != TimeSpan.Zero ?
                    (true) : (false));
            }
        }
        public bool IsCanceled
        {
            get
            {
                return (_task.Status == TaskStatus.Canceled ?
                    (true) : (false));
            }
        }
        public bool IsRanToCompletion
        {
            get
            {
                return (_task.Status == TaskStatus.RanToCompletion ?
                    (true) : (false));
            }
        }
        public TaskStatus TaskStatus
        {
            get
            {
                return (_task.Status);
            }
        }
        public System.Threading.Tasks.Task Task
        {
            get
            {
                return _task;
            }
        }
        public void Run()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            if (this.IsRepeatTask)
            {
                _nextExecTime = DateTime.Now + _repeatTime;
            }
            bool canRun = _task == null ? true : _task.Status != TaskStatus.Running;
            if (canRun)
            {
                if (_faction != null)
                {

                    _task = System.Threading.Tasks.Task.Run(_faction, _taskCancelToken).ContinueWith(delegate {
                        this.TaskCompletionEvent(this, new TaskCompletionEventArgs(_task, this, stopwatch));

                    });
                }
                else
                {

                    _task = System.Threading.Tasks.Task.Run(_action, _taskCancelToken).ContinueWith(delegate {
                        this.TaskCompletionEvent(this, new TaskCompletionEventArgs(_task, this, stopwatch));

                    });
                }
            }
        }
        public void StopTask()
        {
            if (_task != null)
            {
                if (_task.Status == TaskStatus.Running)
                {
                    try
                    {
                        _taskCancelTokenInstance.Cancel();
                    }
                    catch (Exception ex)
                    {
                        EventHandler<Exception> handler = _exceptionEvent;
                        handler?.Invoke(this, ex);
                    }
                    finally
                    {
                    }
                }
            }
        }
        public void Restart()
        {
            if (!this.IsRunning && this.IsRanToCompletion)
                Run();
        }

        private void DefaultCancelTaskRequestMethod()
        {

        }

        protected virtual void TaskCompletionEvent(object sender, TaskCompletionEventArgs args)
        {
            args.Stopwatch.Stop();
            EventHandler<TaskCompletionEventArgs> handler = _completionEvent;
            handler?.Invoke(this, args);
        }
        protected virtual void CancelTaskRequestEvent(object sender, TaskCancelRequestEventArgs args)
        {
            EventHandler<TaskCancelRequestEventArgs> handler = _cancelTaskRequestEvent;
            handler?.Invoke(this, args);
        }

        public virtual void Dispose()
        {
            if (_task != null)
            {
                try
                {
                    _taskCancelTokenInstance.Cancel();
                }
                catch (Exception ex)
                {
                    EventHandler<Exception> handler = _exceptionEvent;
                    handler?.Invoke(this, ex);
                }
                finally
                {
                    _taskCancelTokenInstance.Dispose();
                }
            }
            GC.SuppressFinalize(this);
        }
        public void ResetAllEvents()
        {
            _completionEvent = null;
            _cancelTaskRequestEvent = null;
        }
    }

    public class TaskCompletionEventArgs : EventArgs
    {
        public TaskObject TaskObject { get; }
        public readonly System.Threading.Tasks.Task Task;
        public readonly Stopwatch Stopwatch;
        public TaskCompletionEventArgs(System.Threading.Tasks.Task t, TaskObject taskObject, Stopwatch stopwatch)
        {
            Task = t;
            Stopwatch = stopwatch;
            TaskObject = taskObject;
        }
    }
    public class TaskCancelRequestEventArgs : EventArgs
    {
        public TaskObject TaskObject { get; }
        public readonly System.Threading.Tasks.Task Task;
        public readonly CancellationToken Token;
        public TaskCancelRequestEventArgs(System.Threading.Tasks.Task t, TaskObject taskObject, CancellationToken tk)
        {
            Task = t;
            Token = tk;
            TaskObject = taskObject;
        }
    }
}
