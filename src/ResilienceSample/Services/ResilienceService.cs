namespace ResilienceSample.Services
{
    using System;
    using System.Threading.Tasks;
    using Polly;
    using Polly.CircuitBreaker;
    using Polly.Fallback;

    public class ResilienceService : IResilienceService
    {
        private readonly IAsyncPolicy wrapStrategyAsync;

        public ResilienceService()
        {
            this.wrapStrategyAsync = Policy.WrapAsync(this.GetRetryAsync(), this.GetCircuitBreakerAsync());
        }

        public async Task<T> ExecuteAsync<T>(
            Func<Task<T>> baseAction,
            Func<Task<T>> fallbackAction = null)
        {
            PolicyResult<T> policyResult;

            if (fallbackAction != null)
            {
                policyResult = await this.GetFallbackAsync(fallbackAction)
                    .WrapAsync(this.wrapStrategyAsync)
                    .ExecuteAndCaptureAsync(baseAction);
            }
            else
            {
                policyResult = await this.wrapStrategyAsync
                    .ExecuteAndCaptureAsync(baseAction);
            }

            if (policyResult.Outcome == OutcomeType.Successful)
            {
                return policyResult.Result;
            }

            return default;
        }

        private IAsyncPolicy GetRetryAsync()
        {
            return Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(
                    sleepDurations: new[]
                    {
                        TimeSpan.FromSeconds(2),
                        TimeSpan.FromSeconds(2),
                        TimeSpan.FromSeconds(2),
                    },
                    onRetry: (execption, timeSpan, retryCount, context) =>
                    {
                        Console.BackgroundColor = ConsoleColor.DarkGray;
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine($"{DateTime.Now.ToLongTimeString()} - Request failed. Waiting {timeSpan} before next retry. Retry attempt {retryCount}");
                        Console.ResetColor();
                    });
        }

        private IAsyncPolicy GetCircuitBreakerAsync()
        {
            return Policy
                .Handle<Exception>()
                .CircuitBreakerAsync(
                    exceptionsAllowedBeforeBreaking: 3,
                    durationOfBreak: TimeSpan.FromSeconds(30),
                    onBreak: (exception, timeSpan) =>
                    {
                        Console.BackgroundColor = ConsoleColor.Yellow;
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine($"{DateTime.Now.ToLongTimeString()} - Failed! Circuit open, waiting: {timeSpan}");
                        Console.ResetColor();
                    },
                    onReset: () =>
                    {
                        Console.BackgroundColor = ConsoleColor.DarkGreen;
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine($"{DateTime.Now.ToLongTimeString()} - Circuit is reset.");
                        Console.ResetColor();
                    },
                    onHalfOpen: () =>
                    {
                        Console.BackgroundColor = ConsoleColor.DarkBlue;
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine($"{DateTime.Now.ToLongTimeString()} - Circuit is half open.");
                        Console.ResetColor();
                    });
        }

        private AsyncFallbackPolicy<T> GetFallbackAsync<T>(Func<Task<T>> action)
        {
            return Policy<T>
                .Handle<Exception>()
                .Or<BrokenCircuitException>()
                .FallbackAsync(
                    fallbackAction: cancellationToken => action(),
                    onFallbackAsync: ex =>
                    {
                        Console.BackgroundColor = ConsoleColor.Magenta;
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.WriteLine($"{DateTime.Now.ToLongTimeString()} - Fallback method used due to: {ex.Exception.Message}");
                        Console.ResetColor();

                        return Task.CompletedTask;
                    });
        }
    }
}
