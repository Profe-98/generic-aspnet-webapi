using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows;
using WebApiFunction.Threading.Task;

namespace WebApiFunction.Application
{

    public class ThreadManager
    {
        #region Private
        private int _threadPoolSize = 10;
        private Dictionary<string, Thread> _threadPool = new Dictionary<string, Thread>();
        private Dictionary<string, TaskObject> _taskDict = new Dictionary<string, TaskObject>();
        #endregion
        #region Public
        public Dictionary<string, Thread> ThreadPool
        {
            get
            {
                return _threadPool;
            }
        }
        public Dictionary<string, TaskObject> Tasks
        {
            get
            {
                return _taskDict;
            }
        }
        #endregion
        #region Ctor
        public ThreadManager()
        {
        }
        #endregion
        #region Methods
        public TaskObject NewTask(TaskObject thread, string name)
        {
            TaskObject threadHandle = thread;
            string tName = null;
            if (!string.IsNullOrEmpty(name))
            {
                tName = name;
            }
            if (_taskDict.ContainsKey(tName))
            {
                _taskDict.Add(tName, threadHandle);
            }
            else
            {
                _taskDict[tName] = thread;
            }
            return threadHandle;
        }
        public void SetThreadPoolSize(int threadSize)
        {
            _threadPoolSize = threadSize;
        }
        public void AddToThreadPool(string key, Thread thread)
        {
            if (_threadPool.Count < _threadPoolSize)
            {
                _threadPool.Add(key, thread);
                thread.Start();
            }
        }
        public void RemoveThreadFromPool(string key, Thread thread)
        {
            if (_threadPool.ContainsKey(key))
            {
                _threadPool.Remove(key);
            }
        }
        #endregion
    }
}
