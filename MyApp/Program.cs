using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ServiceStack;
using ServiceStack.Messaging;
using ServiceStack.RabbitMq;
using MyApp.ServiceModel;

namespace MyApp
{
    public class Program
    {
        public static ServiceStackHost AppHost { get; set; }

        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    AppHost = new GenericAppHost(typeof(MyService).Assembly)
                    {
                        ConfigureAppHost = host =>
                        {
                            var mqServer = new RabbitMqServer(hostContext.Configuration.GetConnectionString("RabbitMq")) {
                                DisablePublishingToOutq = true,
                            };
                            mqServer.RegisterHandler<Hello>(host.ExecuteMessage);
                            host.Register<IMessageService>(mqServer);
                        }
                    }.Init();

                    services.AddSingleton(AppHost.Resolve<IMessageService>());
                    services.AddHostedService<MqWorker>();
                });
    }
}
