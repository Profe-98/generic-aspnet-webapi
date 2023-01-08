using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Concurrent;
using WebApiFunction.Threading.Task;

namespace WebApiFunction.Threading.Service
{
    public interface ITaskSchedulerBackgroundServiceQueuer
    {
        public ConcurrentQueue<TaskObject> Queue { get; }
        public void Enqueue(TaskObject taskObject);

        public TaskObject Dequeue();
        public TaskObject Peek();
    }
}
