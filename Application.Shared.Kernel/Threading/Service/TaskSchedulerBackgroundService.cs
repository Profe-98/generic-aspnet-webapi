using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Extensions.DependencyInjection;
using Application.Shared.Kernel.Threading.Task;

namespace Application.Shared.Kernel.Threading.Service
{
    public class TaskSchedulerBackgroundService : BackgroundService
    {
        private Hashtable taskList = new Hashtable();
        private System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
        private readonly ILogger<TaskSchedulerBackgroundService> _logger;
        private readonly ITaskSchedulerBackgroundServiceQueuer _taskSchedulerBackgroundServiceQueuer;
        #region Ctor
        public TaskSchedulerBackgroundService(ILogger<TaskSchedulerBackgroundService> logger, ITaskSchedulerBackgroundServiceQueuer taskSchedulerBackgroundServiceQueuer)
        {
            _logger = logger;
            _taskSchedulerBackgroundServiceQueuer = taskSchedulerBackgroundServiceQueuer;
        }
        #endregion Ctor
        /// <summary>
        /// Execute the Service Code / Background code, [...]teAsync(CancellationToken stoppingToken) => Task.Run(() => [...] is important for dont block the IServiceCollection Flow in StartUp.cs
        /// </summary>
        /// <param name="stoppingToken"></param>
        /// <returns></returns>
        protected override System.Threading.Tasks.Task ExecuteAsync(CancellationToken stoppingToken) => System.Threading.Tasks.Task.Run(() =>
        {
            while (!stoppingToken.IsCancellationRequested)
            {

                if (_taskSchedulerBackgroundServiceQueuer.Queue.Count != 0)
                {
                    if(!stopwatch.IsRunning)
                        stopwatch.Start();

                    TaskObject taskObject = _taskSchedulerBackgroundServiceQueuer.Peek();
                    if(taskObject != null)
                    {
                        bool shouldRunNow = (taskObject.IsRepeatTask?
                        taskObject.IsNextRepeationAvaible:true);

                        if(shouldRunNow)
                        {
                            taskObject = _taskSchedulerBackgroundServiceQueuer.Dequeue();
                            taskList.Add(taskObject, taskObject);
                            if(taskObject.AllCompletedEventSubscribers.Length ==0)
                            {
                                taskObject.CompletionEvent += TaskObject_CompletionEvent;
                            }
                            taskObject.Run();
                            //taskList.Add(taskObject,taskObject.Task);
                            _logger.LogInformation($"peek: #{taskObject.Task.Id} task in queue is starting #{Thread.CurrentThread.ManagedThreadId} thread");
                            
                        }

                    }
                    Thread.Sleep(1000);
                }
                else
                {
                    if (stopwatch.IsRunning)
                    {
                        System.Threading.Tasks.Task[] tasks = GetTasks();
                        if (tasks != null)
                            System.Threading.Tasks.Task.WaitAll(tasks);

                        stopwatch.Stop();
                        Console.WriteLine($"{stopwatch.ElapsedMilliseconds} ms vergangen");
                    }
                    _logger.LogInformation($"ITaskSchedulerBackgroundServiceQueuer goint to sleep for 10sec");

                    Thread.Sleep(10000);
                }
            }
        });
        private System.Threading.Tasks.Task[] GetTasks()
        {
            List<System.Threading.Tasks.Task> t = new List<System.Threading.Tasks.Task>();
            foreach (DictionaryEntry entry in taskList)
            {
                TaskObject taskObject = (TaskObject)entry.Value;
                if (taskObject != null)
                {

                    t.Add(taskObject.Task);
                }
            }
            return t.Count != 0 ?
                t.ToArray() : null;
        }

        private void TaskObject_CompletionEvent(object sender, TaskCompletionEventArgs e)
        {
            taskList.Remove(e.TaskObject);
            _logger.LogInformation($"dequeue: #{e.Task.Id} task in queue is completed, running-time: {e.Stopwatch.ElapsedMilliseconds}, completed-event-subscribers: {e.TaskObject.AllCompletedEventSubscribers.Length}");
            if (e.TaskObject.IsRepeatTask)
            {
                _taskSchedulerBackgroundServiceQueuer.Enqueue(e.TaskObject);
            }
            else
            {
                e.TaskObject.ResetAllEvents();
            }
        }

        public override void Dispose()
        {
            base.Dispose();
        }
    }
    public static class TaskSchedulerBackgroundServiceExtension
    {
        public static IServiceCollection UseAsyncTaskScheduler(this IServiceCollection builder)
        {
            return builder.AddHostedService<TaskSchedulerBackgroundService>();
        }
    }
}
