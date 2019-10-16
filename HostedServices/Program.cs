using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace HostedServices
{
    public class Program
    {
        private static async Task Main(string[] args)
        {
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine("Hosted services shutdown demo");
            Console.WriteLine("-----------------------------");
            Console.WriteLine();

            var builder = new HostBuilder()
                .ConfigureLogging((hostContext, loggingBuilder) =>
                {
                    Console.WriteLine("Setting up logging...");

                    Log.Logger = new LoggerConfiguration()
                        .MinimumLevel.Verbose()
                        .WriteTo.Console()
                        .CreateLogger();

                    loggingBuilder.Services
                        .AddLogging(configure => configure
                            .ClearProviders()
                            .AddSerilog());
                })
                .ConfigureServices(services =>
                {
                    services.AddSingleton<IHostedService, ServiceA>();
                    services.AddSingleton<IHostedService, ServiceB>();
                });

            try
            {
                await builder.RunConsoleAsync();
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }

    public class ServiceA : BackgroundService
    {
        private readonly ILogger<ServiceA> _logger;

        public ServiceA(ILogger<ServiceA> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.Register(() => _logger.LogInformation("Service A - Stopping (thread {ThreadId})...", Thread.CurrentThread.ManagedThreadId));

            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    _logger.LogInformation("Service A - Running (thread {ThreadId})...", Thread.CurrentThread.ManagedThreadId);
                    await Task.Delay(2500, stoppingToken);
                }
            }
            catch (TaskCanceledException)
            {
                _logger.LogInformation("Service A - Task.Delay was cancelled (thread {ThreadId}).", Thread.CurrentThread.ManagedThreadId);
            }

            _logger.LogInformation("Service A - Has stopped (thread {ThreadId}).", Thread.CurrentThread.ManagedThreadId);
        }
    }

    public class ServiceB : BackgroundService
    {
        private readonly ILogger<ServiceA> _logger;

        public ServiceB(ILogger<ServiceA> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.Register(() => _logger.LogInformation("Service B - Stopping (thread {ThreadId})...", Thread.CurrentThread.ManagedThreadId));

            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    _logger.LogInformation("Service B - Running (thread {ThreadId})...", Thread.CurrentThread.ManagedThreadId);
                    await Task.Delay(2500, stoppingToken);
                }
            }
            catch (TaskCanceledException)
            {
                _logger.LogInformation("Service B - Task.Delay was cancelled (thread {ThreadId}).", Thread.CurrentThread.ManagedThreadId);
            }

            _logger.LogInformation("Service B - Has stopped (thread {ThreadId}).", Thread.CurrentThread.ManagedThreadId);
        }
    }
}