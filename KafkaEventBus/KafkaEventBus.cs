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
    public class KafkaEventBus : IEventBus
    {
        private readonly IEventBusSubscriptionManager _subscriptionManager;
        private readonly ILogger<KafkaEventBus> _logger;
        private readonly KafkaConnection _kafkaConnection;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        public KafkaEventBus(IEventBusSubscriptionManager subscriptionManager, ILogger<KafkaEventBus> logger,
            KafkaConnection kafkaConnection, IServiceProvider serviceProvider)
        {
            this._subscriptionManager = subscriptionManager ?? throw new ArgumentNullException(nameof(subscriptionManager));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this._kafkaConnection = kafkaConnection ?? throw new ArgumentNullException(nameof(kafkaConnection));
            _serviceScopeFactory = serviceProvider?.GetRequiredService<IServiceScopeFactory>()
                ?? throw new ArgumentException($"Cannot resolve IServiceScopeFactory from {nameof(serviceProvider)}");
        }

        public async Task PublishAsync<T>(T @event) where T : IntegrationEvent
        {
            var eventType = typeof(T).Name;

            //using (var producer = _kafkaConnection.ProducerBuilder<T>())
            //{
                try
                {
                var producer = _kafkaConnection.ProducerBuilder<T>();
                    // _logger.LogInformation($"Publishing the event {eventType} to Kafka topic {eventType}");
                  var producerResult=  await producer.ProduceAsync(eventType, new Message<Null, T>() { Value = @event });
               // producer.Flush();
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error occured during publishing the event to topic {eventType}");
                    _logger.LogError(ex.Message + "\n" + ex.StackTrace);
                }
           // }
        }

        public async Task SubscribeAsync<T, TH>()
            where T : IntegrationEvent
            where TH : IIntegrationEventHandler<T>
        {
            var eventName = typeof(T).Name;
            using (var consumer = _kafkaConnection.ConsumerBuilder<T>())
            {
                //subscribe the handler to the event
                _subscriptionManager.AddSubscription<T, TH>();

                consumer.Subscribe(eventName);

                //create a task to listen to the topic
                await Task.Run(async () =>
                     {
                         while (true)
                         {
                             try
                             {
                                 //_logger.LogInformation($"Consuming from topic {eventName}");
                                 var consumerResult = consumer.Consume();
                                 await ProcessEvent(consumerResult.Message.Value);
                             }
                             catch (ConsumeException e)
                             {
                                 _logger.LogError($"Error `{e.Error.Reason}` occured during consuming the event from topic {eventName}");
                                 _logger.LogError(e.Message + "\n" + e.StackTrace);
                             }
                         }
                     }).ConfigureAwait(false);
            }

        }

        private async Task<bool> ProcessEvent<T>(T value) where T : IntegrationEvent
        {
            var processed = false;
            if (_subscriptionManager.HasEvent<T>())
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var subscriptions = _subscriptionManager.GetHandlersForEvent<T>();
                    foreach (var subscription in subscriptions)
                    {
                        var handler = scope.ServiceProvider.GetRequiredService(subscription);
                        if (handler == null) continue;
                        var concreteType = typeof(IIntegrationEventHandler<>).MakeGenericType(typeof(T));
                        await (Task)concreteType.GetMethod("Handle").Invoke(handler, new object[] { value });
                    }
                }
                processed = true;
            }
            return processed;
        }
    }

}



