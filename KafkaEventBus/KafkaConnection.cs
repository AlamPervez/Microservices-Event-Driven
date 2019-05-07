using EventBus;
using Microsoft.Extensions.Logging;
using System;
using Confluent.Kafka;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using Confluent.Kafka.SyncOverAsync;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace KafkaEventBus
{

    public class KafkaConnection
    {
        private readonly ProducerConfig _producerConfiguration;
        private readonly SchemaRegistryConfig _schemaRegistryConfiguration;
        private readonly ConsumerConfig _consumerConfiguration;
        private readonly AvroSerializerConfig _avroSerializerConfiguration;
        private  object _producerBuilder;

        public KafkaConnection( ProducerConfig producerConfig, ConsumerConfig consumerConfig,
            SchemaRegistryConfig schemaRegistryConfig,AvroSerializerConfig avroSerializerConfig)
        {

     
            this._producerConfiguration = producerConfig ?? throw new ArgumentNullException(nameof(producerConfig));
            this._consumerConfiguration=consumerConfig??throw new ArgumentNullException(nameof(consumerConfig));
            this._schemaRegistryConfiguration = schemaRegistryConfig ?? throw new ArgumentNullException(nameof(schemaRegistryConfig));
            this._avroSerializerConfiguration = avroSerializerConfig ?? throw new ArgumentNullException(nameof(avroSerializerConfig));

        }

        public IProducer<Null, T> ProducerBuilder<T>()
        {
            if (_producerBuilder == null)
            {
              var schemaRegistry = new CachedSchemaRegistryClient(_schemaRegistryConfiguration);
             _producerBuilder = new ProducerBuilder<Null, T>(_producerConfiguration)
                          //.SetKeySerializer(new AvroSerializer<string>(schemaRegistry))
                          .SetValueSerializer(new AvroSerializer<T>(schemaRegistry))
                         .Build();
            }
            return (IProducer<Null,T>)_producerBuilder;
        }

        public IConsumer<Null, T> ConsumerBuilder<T>()
        {
            var schemaRegistry = new CachedSchemaRegistryClient(_schemaRegistryConfiguration);
            var consumer = new ConsumerBuilder<Null, T>(_consumerConfiguration)
                //.SetKeyDeserializer(new AvroDeserializer<string>(schemaRegistry).AsSyncOverAsync())
                .SetValueDeserializer(new AvroDeserializer<T>(schemaRegistry).AsSyncOverAsync())
                .Build();
            return consumer;
        }



    }

}



