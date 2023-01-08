using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using WebApiApplicationService.InternalModels;
using System.Collections.Concurrent;

namespace WebApiApplicationService
{
    public interface ITaskSchedulerBackgroundServiceQueuer
    {
        public ConcurrentQueue<TaskObject> Queue { get; }
        public void Enqueue(TaskObject taskObject);

        public TaskObject Dequeue();
        public TaskObject Peek();
    }
}
