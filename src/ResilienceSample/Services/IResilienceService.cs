namespace ResilienceSample.Services
{
    using System;
    using System.Threading.Tasks;

    public interface IResilienceService
    {
        Task<T> ExecuteAsync<T>(Func<Task<T>> baseAction, Func<Task<T>> fallbackAction = null);
    }
}
