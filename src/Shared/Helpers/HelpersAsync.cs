using System;
using System.Threading;
using System.Threading.Tasks;

namespace Shared.Helpers
{
    public class HelpersAsync
    {
        /// <summary>
        /// асинхронная задача которая завершается только при срабатывани токена отмены
        /// </summary>
        public static async Task WaitCancellation(CancellationToken ct)
        {
            var tcs = new TaskCompletionSource<int>();
            using (ct.Register(() => tcs.SetResult(0)))
                await tcs.Task;
        }


        /// <summary>
        /// 
        /// </summary>
        public static async Task<T> WithTimeout<T>(Task<T> task, int time, CancellationToken ct)
        {
            Task delayTask = Task.Delay(time, ct);
            Task firstToFinish = await Task.WhenAny(task, delayTask);

            if (firstToFinish == delayTask)
            {
                task.ContinueWith(HandleException, ct);  //к основной задаче прикрепили обработку иключений
                throw new TimeoutException();
            }

            return await task;
        }


        private static void HandleException<T>(Task<T> task)
        {
            if (task.Exception != null)
            {
                ; //чтото делаем с исключеним возникшим в основной задаче.
            }
        }
    }
}