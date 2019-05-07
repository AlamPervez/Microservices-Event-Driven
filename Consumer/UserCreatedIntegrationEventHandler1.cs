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

    public class UserCreatedIntegrationEventHandler1 : IIntegrationEventHandler<UserCreatedIntegrationEvent>
    {
        public async Task Handle(UserCreatedIntegrationEvent @event)
        {
            Console.WriteLine($"Id:{@event.UserId} FullName:{@event.FullName} " +
                $"Age:{@event.Age} CreationDate:{@event.CreationDate} - {@event.Id}");
                        await Task.FromResult(false);
        }
    }

}


