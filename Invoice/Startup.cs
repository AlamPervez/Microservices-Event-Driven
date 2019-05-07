using Confluent.Kafka;
using Confluent.SchemaRegistry;
using Confluent.SchemaRegistry.Serdes;
using EventBus;
using Invoicing.Infrastructure.Data;
using Invoicing.IntegrationEvents.Handlers;
using Invoicing.Repository;
using KafkaEventBus;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Ordering.AvroSchema;
using System.Reflection;

namespace Invoicing
{
    public class Startup
    {
        private readonly IHostingEnvironment _env = null;
        public Startup(IHostingEnvironment env)
        {
            IConfigurationBuilder builder = new ConfigurationBuilder()
           .SetBasePath(env.ContentRootPath)
           .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
           .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
           .AddEnvironmentVariables();
            this.Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var databaseConnectionString = this.Configuration.GetConnectionString("Application");
            services.RegisterEntities(databaseConnectionString)
                .AddCustomSwagger(this.Configuration)
                .AddCustomDbContext(this.Configuration)
                .AddEventBus();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();

            app.UseSwagger(o =>
            {
                o.RouteTemplate = "api/swagger/{documentName}/swagger.json";
            }).UseSwaggerUI(c =>
            {
                c.RoutePrefix = "api";
                c.SwaggerEndpoint($"swagger/v1/swagger.json",
                    "Invoicing API");
            });

            var eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();

            eventBus.SubscribeAsync<OrderCreatedIntegrationEvent, OrderCreatedIntegrationEventHandler>();
        }
    }

    public static class ServiceCollectionExtension
    {
        public static IServiceCollection RegisterEntities(this IServiceCollection services, string databaseConnectionString)
        {
            services.AddScoped<InvoiceRepository>();
            services.AddTransient<OrderCreatedIntegrationEventHandler>();
            return services;
        }

        public static IServiceCollection AddEventBus(this IServiceCollection services)
        {
            ProducerConfig producerConfiguration = new ProducerConfig { BootstrapServers = "localhost:9092" };
            SchemaRegistryConfig schemaRegistryConfiguration = new SchemaRegistryConfig
            {
                SchemaRegistryUrl = "localhost:8081",
                SchemaRegistryRequestTimeoutMs = 5000,
                SchemaRegistryMaxCachedSchemas = 10
            };
            AvroSerializerConfig avroSerializerConfiguration = new AvroSerializerConfig
            {
                // optional Avro serializer properties:
                // BufferBytes = 100,
                AutoRegisterSchemas = true,
            };

            ConsumerConfig consumerConfiguration = new ConsumerConfig
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
                KafkaConnection kafkaConnection = sp.GetRequiredService<KafkaConnection>();
                ILogger<KafkaEventBus.KafkaEventBus> logger = sp.GetRequiredService<ILogger<KafkaEventBus.KafkaEventBus>>();
                IEventBusSubscriptionManager eventBusSubcriptionsManager = sp.GetRequiredService<IEventBusSubscriptionManager>();
                return new KafkaEventBus.KafkaEventBus(eventBusSubcriptionsManager, logger, kafkaConnection, sp);
            });

            return services;
        }
        public static IServiceCollection AddCustomDbContext(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddDbContext<AppDbContext>(o =>
            {
                o.UseNpgsql(configuration.GetConnectionString("Application"),
                   npgsqlOptionsAction: sqlOptions =>
                   {
                       sqlOptions.EnableRetryOnFailure();
                   });
            });

            return services;
        }

        public static IServiceCollection AddCustomSwagger(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSwaggerGen(options =>
            {
                options.DescribeAllEnumsAsStrings();
                options.SwaggerDoc("v1", new Swashbuckle.AspNetCore.Swagger.Info
                {
                    Title = "Invoicing API",
                    Version = "v1",
                    Description = "Invoicing API Documentation",
                    TermsOfService = "Terms Of Service",

                });
            });

            return services;
        }
    }
}
