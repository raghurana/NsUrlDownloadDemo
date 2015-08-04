using System;
using System.Threading;
using System.Threading.Tasks;

namespace NsurlDemo.iOS
{
    public class PollingBackgroundWorker
    {
        private CancellationTokenSource cancelTokenSource;

        public Action CompletionHandler { get; set; }

        public Action FailureHandler { get; set; }

        public void ExecuteInBackground(Action methodToExecute, TimeSpan delay)
        {
            cancelTokenSource?.Cancel();
            cancelTokenSource = new CancellationTokenSource();

            delay
                .ExecuteWithDelay(methodToExecute, cancelTokenSource.Token)
                .ContinueWith(t => CompletionHandler?.Invoke(), TaskContinuationOptions.OnlyOnRanToCompletion)
                .ContinueWith(t => FailureHandler?.Invoke(), TaskContinuationOptions.OnlyOnFaulted);
        }

        public void FinishExecution()
        {
            cancelTokenSource?.Cancel();
        }


    }
}
