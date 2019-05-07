using System;
using KafkaEventBus;
using EventBus;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Producer.AvroSchema;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using System.Reflection;
using Confluent.Kafka;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;

namespace Producer
{
    class Program
    {
        public static async Task Main(string[] args)
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
           
            try
            {
                var counter = 0;
                foreach (var process in Enumerable.Range(1, 10))
                {
                    Task.Run(async () =>
                    {
                        var range = Enumerable.Range(1, 10000);
                        foreach (var i in range)
                        {
                            var @event = new UserCreatedIntegrationEvent()
                            {
                                FirstName = "Jane",
                                LastName = "Doe",
                                FullName = "Jane Doe",
                                Age = process,
                                UserId = Interlocked.Increment(ref counter)
                            };
                            await eventBus.PublishAsync(@event);
                            Console.WriteLine($"Published user created event UserId: {@event.UserId} - {@event.Id}" +
                                $"by Process:{process}");

                            // Thread.Sleep(5000);
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + ex.StackTrace);
            }
            
            Console.ReadKey();
        }
    }

}