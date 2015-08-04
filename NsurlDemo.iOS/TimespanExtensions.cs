using System;
using System.Threading;
using System.Threading.Tasks;

public static class TimespanExtensions
{
    public static Task ExecuteWithDelay(this TimeSpan delay, Action methodToExecute, CancellationToken cancellationToken)
    {
        return Task.Run(() =>
        {
            while (true)
            {
                var isCancelled = cancellationToken.WaitHandle.WaitOne(delay);
                if (isCancelled)
                    break;

                methodToExecute.Invoke();
            }
        }, cancellationToken);
    }

}