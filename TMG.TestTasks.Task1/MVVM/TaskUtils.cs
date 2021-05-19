using System;
using System.Threading.Tasks;

namespace TMG.TestTasks.Task1.MVVM
{
    //https://johnthiriet.com/removing-async-void/

    public static class TaskUtilities
    {
        public static async void FireAndForgetSafeAsync(this Task task, IErrorHandler handler = null)
        {
            try
            {
                await task;
            }
            catch (Exception ex)
            {
                handler?.HandleError(ex);
            }
        }
    }

    public interface IErrorHandler
    {
        void HandleError(Exception ex);
    }
}
