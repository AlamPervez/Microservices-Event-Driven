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

    public class Startup
    {
        IConfigurationRoot Configuration { get; }

        public Startup()
        {
            var builder = new ConfigurationBuilder();

            Configuration = builder.Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging();
            services.AddSingleton<IConfigurationRoot>(Configuration);

            var producerConfiguration = new ProducerConfig { BootstrapServers = "localhost:9092" };
            var schemaRegistryConfiguration = new SchemaRegistryConfig
            {
                SchemaRegistryUrl = "localhost:8081",
                SchemaRegistryRequestTimeoutMs = 5000,
                SchemaRegistryMaxCachedSchemas = 10
            };
            var avroSerializerConfiguration = new AvroSerializerConfig
            {
                // optional Avro serializer properties:
                // BufferBytes = 100,
                AutoRegisterSchemas = true,
            };

            var consumerConfiguration = new ConsumerConfig
            {
                BootstrapServers = "localhost:9092",
                GroupId = Assembly.GetExecutingAssembly().GetName().Name
            };

            //Set up the event bus
            services.AddSingleton<KafkaConnection>(new KafkaConnection(
          producerConfiguration
          , consumerConfiguration
          , schemaRegistryConfiguration
          , avroSerializerConfiguration));
            services.AddSingleton<IEventBusSubscriptionManager, EventBusSubscriptionManager>();

            services.AddSingleton<IEventBus, KafkaEventBus.KafkaEventBus>(sp =>
            {
                var kafkaConnection = sp.GetRequiredService<KafkaConnection>();
                var logger = sp.GetRequiredService<ILogger<KafkaEventBus.KafkaEventBus>>();
                var eventBusSubcriptionsManager = sp.GetRequiredService<IEventBusSubscriptionManager>();
                return new KafkaEventBus.KafkaEventBus(eventBusSubcriptionsManager, logger, kafkaConnection, sp);
            });

            services.AddTransient<UserCreatedIntegrationEventHandler>();
            services.AddTransient<UserCreatedIntegrationEventHandler1>();
        }
    }

}


