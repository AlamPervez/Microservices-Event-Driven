# Microservices-Event-Driven
A sample POC of services using event based communication

## Projects
EventBus: An Abstraction of Event Bus to be used for integration events communication between the services.
KafkaEventBus: An implementation of event bus using Kafka.
Producer: A sample application to stream events to Kafka using KafkaEventBus.
Consumer: A sample application to read events from Kafka log using KafkaEventBus.

(Ordering, Invoicing, Dispatch): A sample POC for event communications between these services using KafkaEventBus.

Please refer to https://github.com/dotnet-architecture/eShopOnContainers project for learning more about microservices using .NET.
