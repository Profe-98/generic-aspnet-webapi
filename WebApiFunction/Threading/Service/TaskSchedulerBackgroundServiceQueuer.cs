using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Concurrent;
using WebApiFunction.Threading.Task;

namespace WebApiFunction.Threading.Service
{
    /// <summary>
    /// For Register task that should be executed in BackgroundService.cs in paralell Tasks
    /// </summary>
    public class TaskSchedulerBackgroundServiceQueuer : ITaskSchedulerBackgroundServiceQueuer
    {
        private ConcurrentQueue<TaskObject> _queue = new ConcurrentQueue<TaskObject>();

        public ConcurrentQueue<TaskObject> Queue
        {
            get
            {
                return _queue;
            }
        }

        public void Enqueue(TaskObject taskObject)
        {
            _queue.Enqueue(taskObject);
        }

        public TaskObject Dequeue()
        {
            _queue.TryDequeue(out var workItem);

            return workItem;
        }
        public TaskObject Peek()
        {
            _queue.TryPeek(out var workItem);

            return workItem;
        }
    }
}
