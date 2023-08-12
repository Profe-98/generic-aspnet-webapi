using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Application.Shared.Kernel.Threading.Task
{

    public class TaskObject : IDisposable
    {
        private CancellationTokenSource _taskCancelTokenInstance = new CancellationTokenSource();
        private CancellationToken _taskCancelToken;
        private Action _action;
        private Func<System.Threading.Tasks.Task> _faction;
        private System.Threading.Tasks.Task _taction;
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
        public TaskObject(System.Threading.Tasks.Task action, TimeSpan repeationTime, Action cancelTokenRequestAction = null)
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
        private void Init(System.Threading.Tasks.Task action, TimeSpan repeationTime, Action cancelTokenRequestAction = null, object[] args = null)
        {
            _createTime = DateTime.Now;
            DefaultMethod = DefaultCancelTaskRequestMethod;
            _args = args;
            if (action == null)
                Dispose();
            _taction = action;
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
        public async void Run()
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
                if(_taction != null)
                {
                    _task = _taction;
                    await _task.WaitAsync(_taskCancelToken);
                }
                else if (_faction != null)
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
