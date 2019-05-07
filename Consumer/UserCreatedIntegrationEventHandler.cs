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

    public class UserCreatedIntegrationEventHandler : IIntegrationEventHandler<UserCreatedIntegrationEvent>
    {
        private static int count = 0;
        public async Task Handle(UserCreatedIntegrationEvent @event)
        {
            Console.WriteLine($"Consumed user created event with ID {@event.UserId} by Process{@event.Age}");
            Console.WriteLine($"Total Process {Interlocked.Increment(ref count)}");
            // await Task.Delay(5000);
            await Task.FromResult(false);
        }
    }

}


