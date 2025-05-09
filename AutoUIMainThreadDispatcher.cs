using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Assets.Scripts.Tools.Editor.AutoUI
{
    public class AutoUIMainThreadDispatcher
    {
        private static AutoUIMainThreadDispatcher instance;
        public static AutoUIMainThreadDispatcher getInstace()
        {
            if (instance == null){
                instance = new AutoUIMainThreadDispatcher();
            }
            return instance;
        }
        public ConcurrentQueue<Action> actions = new ConcurrentQueue<Action>();
        public void Init()
        {
            if (instance != null)
            {
                return;
            }
            else
            {
                instance = new AutoUIMainThreadDispatcher();
            }
        }
        public void Run(Action action)
        {
            instance.actions.Enqueue(action);
        }
        public Task<T> AsyncRun<T>(Func<T> func)
        {
            var tcs = new TaskCompletionSource<T>();
            Run(() =>
            {
                try
                {
                    T result = func();
                    tcs.SetResult(result);
                }
                catch (Exception e)
                {
                    tcs.SetException(e);
                }
            });
            return tcs.Task;
        }
    }
}