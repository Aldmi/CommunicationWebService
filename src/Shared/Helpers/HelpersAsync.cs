using System;
using System.Threading;
using System.Threading.Tasks;

namespace Shared.Helpers
{
    public static class HelpersAsync
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
        public static async Task<T> WithTimeout2HandleException<T>(this Task<T> task, int time, CancellationToken ct)
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


        /// <summary>
        /// Остановить задачу по истечении времени и выкинуть TimeoutException()
        /// Если задача выполнилась быстрее, вернуть результат 
        /// </summary>
        /// <param name="ctsTimeout">источник токена для отмены основной задачи task</param>
        /// <param name="ct">Отмена ожидания</param>
        /// <returns></returns>
        public static async Task<T> WithTimeout2CanceledTask<T>(this Task<T> task, int time, CancellationTokenSource ctsTimeout)
        {
            Task delayTask = Task.Delay(time);
            Task firstToFinish = await Task.WhenAny(task, delayTask);
            if (firstToFinish == delayTask)
            {
                ctsTimeout.Cancel();
                ctsTimeout.Dispose();
                throw new TimeoutException();
            }
            return await task;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="task"></param>
        /// <param name="time"></param>
        /// <param name="ctsTask"></param>
        /// <returns></returns>
        public static async Task<T> WithTimeout<T>(this Task<T> task, int time, CancellationTokenSource ctsTask)
        {
            ctsTask.CancelAfter(time);
            try
            {
                return await task;
            }
            catch (OperationCanceledException)
            {
                throw new TimeoutException("The function has taken longer than the maximum time allowed.");
            }
        }


        public static async Task WithTimeout(this Task task, int time, CancellationTokenSource ctsTask)
        {
            ctsTask.CancelAfter(time);
            try
            {
                await task;
            }
            catch (OperationCanceledException ex)
            {
                throw new TimeoutException("The function has taken longer than the maximum time allowed.");
            }
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