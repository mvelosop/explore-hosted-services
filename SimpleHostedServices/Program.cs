using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleHostedServices
{
    public class Program
    {
        private static async Task Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var builder = new HostBuilder()
                .ConfigureServices(services =>
                {
                    services.AddSingleton<IHostedService, ServiceA>();
                    services.AddSingleton<IHostedService, ServiceB>();
                });

            await builder.RunConsoleAsync();
        }
    }

    internal class ServiceA : IHostedService
    {
        private readonly IApplicationLifetime _lifetime;
        private Task _runningTask;

        public ServiceA(
            IApplicationLifetime lifetime)
        {
            _lifetime = lifetime;
        }

        public async Task DoWorkAsync(CancellationToken token)
        {
            token.Register(() => Console.WriteLine($"{DateTime.Now:HH:mm:ss.fff} - Stopping Service A..."));

            while (!token.IsCancellationRequested)
            {
                Console.WriteLine($"{DateTime.Now:HH:mm:ss.fff} - Service A running (thread {Thread.CurrentThread.ManagedThreadId})...");
                await Task.Delay(2200, token);
            }
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine($"{DateTime.Now:HH:mm:ss.fff} - Starting Service A...");
            _runningTask = Task.Run(() => DoWorkAsync(_lifetime.ApplicationStopping));

            return Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await Task.WhenAll(_runningTask);
        }
    }

    internal class ServiceB : IHostedService
    {
        private readonly IApplicationLifetime _lifetime;
        private readonly ILogger<ServiceB> _logger;
        private Task _runningTask;

        public ServiceB(
            IApplicationLifetime lifetime,
            ILogger<ServiceB> logger)
        {
            _lifetime = lifetime;
            _logger = logger;
        }

        public async Task DoWorkAsync(CancellationToken token)
        {
            token.Register(() => Console.WriteLine($"{DateTime.Now:HH:mm:ss.fff} - Stopping Service B..."));

            while (!token.IsCancellationRequested)
            {
                Console.WriteLine($"{DateTime.Now:HH:mm:ss.fff} - Service B running (thread {Thread.CurrentThread.ManagedThreadId})...");
                await Task.Delay(2200, token);
            }
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine($"{DateTime.Now:HH:mm:ss.fff} - Starting Service A...");
            _runningTask = Task.Run(() => DoWorkAsync(_lifetime.ApplicationStopping));
            //_runningTask = DoWorkAsync(_lifetime.ApplicationStopping);

            return Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await Task.WhenAll(_runningTask);
        }
    }
}