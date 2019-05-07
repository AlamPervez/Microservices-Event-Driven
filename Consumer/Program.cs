using Confluent.Kafka;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using EventBus;
using KafkaEventBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Producer.AvroSchema;
using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Consumer
{
    class Program
    {
        static async Task Main(string[] args)
        {

            IServiceCollection services = new ServiceCollection();

            Startup startup = new Startup();
            startup.ConfigureServices(services);
            IServiceProvider serviceProvider = services.BuildServiceProvider();

            //configure console logging
            serviceProvider
                .GetService<ILoggerFactory>()
                .AddConsole(LogLevel.Debug);

            var logger = serviceProvider.GetService<ILoggerFactory>()
                .CreateLogger<Program>();

            logger.LogDebug("Logger is working!");

            // Get Service and call method


            var eventBus = serviceProvider.GetRequiredService<IEventBus>();
            eventBus.SubscribeAsync<UserCreatedIntegrationEvent, UserCreatedIntegrationEventHandler>();
            await eventBus.SubscribeAsync<UserCreatedIntegrationEvent, UserCreatedIntegrationEventHandler1>();


            Console.ReadKey();

        }
    }

}


