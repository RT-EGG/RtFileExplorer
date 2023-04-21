using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace RtFileExplorer.ViewModel.Wpf
{
    internal delegate void BackgroundTaskCallback();
    internal class BackgroundTask
    {
        public BackgroundTaskCallback Method { get; set; } = () => { };
        public bool Async { get; set; } = false;
    }

    internal class BackgroundTaskQueue
    {
        private BackgroundTaskQueue()
        {
            Timer = new DispatcherTimer();
            Timer.Interval = TimeSpan.FromMilliseconds(1.0);
            Timer.Tick += (_, _) => Execute();

            AsyncTimer = new DispatcherTimer();
            AsyncTimer.Interval = TimeSpan.FromMilliseconds(100.0);
            AsyncTimer.Tick += async (_, _) => await ExecuteAsync();
        }

        public static BackgroundTaskQueue Instance
        { get; } = new BackgroundTaskQueue();

        public void Start()
        {
            Timer.Start();
            AsyncTimer.Start();
        }

        public void Stop()
        {
            Timer.Stop();
            AsyncTimer.Stop();
        }

        public void AddTask(BackgroundTask inTask)
        {
            if (inTask.Async)
            {
                lock (AsyncMutex)
                {
                    AsyncTaskQueue.Enqueue(inTask);
                }
            }
            else
            {
                lock (Mutex)
                {
                    TaskQueue.Enqueue(inTask);
                }
            }
        }

        private void Execute()
        {
            var sw = new Stopwatch();
            sw.Start();
            while (true)
            {
                BackgroundTask task;
                lock (Mutex)
                {
                    if (!TaskQueue.Any())
                        break;

                    task = TaskQueue.Dequeue();
                }

                task.Method.Invoke();

                if (sw.ElapsedMilliseconds > 1.0)
                    break;
            }
        }

        private async Task ExecuteAsync()
        {
            while (true)
            {
                BackgroundTask task;
                lock (AsyncMutex)
                {
                    if (!AsyncTaskQueue.Any())
                        break;

                    task = AsyncTaskQueue.Dequeue();
                }

                await Task.Run(() => task.Method.Invoke());
            }
        }

        private readonly Queue<BackgroundTask> TaskQueue = new Queue<BackgroundTask>();
        private readonly Queue<BackgroundTask> AsyncTaskQueue = new Queue<BackgroundTask>();
        private readonly object Mutex = new object();
        private readonly object AsyncMutex = new object();

        private readonly DispatcherTimer Timer;
        private readonly DispatcherTimer AsyncTimer;
    }
}
