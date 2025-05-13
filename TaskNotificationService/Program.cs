using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TasksETM.Interfaces.ITasks;

namespace TaskNotificationService
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHostedService<Worker>();
                    services.AddSingleton<ITaskService, TaskService>(); // Регистрация твоего сервиса
                })
                .Build()
                .RunAsync();
        }
    }
}