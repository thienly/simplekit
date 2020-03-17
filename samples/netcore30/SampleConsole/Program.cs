using System;
using System.Threading;
using System.Threading.Tasks;

namespace SampleConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Person");
        }
        public static async Task<T> WithCancellation<T>(Task<T> task, CancellationToken cancellationToken)
        {
           var tcs = new TaskCompletionSource<object>(TaskCreationOptions.RunContinuationsAsynchronously);
           using (cancellationToken.Register(state =>
               {
                   ((TaskCompletionSource<object>)state).TrySetResult(null);
               },
               tcs))
           {
               var resultTask = await Task.WhenAny(task, tcs.Task);
               if (resultTask == tcs.Task)
               {
                   // Operation cancelled
                   throw new OperationCanceledException(cancellationToken);
               }

               return await task;
           }
        }
    }
    
}