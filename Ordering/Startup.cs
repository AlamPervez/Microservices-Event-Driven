using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Confluent.Kafka;
using Confluent.SchemaRegistry;
using EventBus;
using KafkaEventBus;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Confluent.SchemaRegistry.Serdes;
using Microsoft.EntityFrameworkCore;
using Ordering.Infrastructure.Data;
using Ordering.Repository;
using Ordering.IntegrationEvents.Handlers;
using Dispatch.AvroSchema;

namespace Ordering
{
    public class Startup
    {
        private readonly IHostingEnvironment _env = null;
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
           .SetBasePath(env.ContentRootPath)
           .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
           .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
           .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var databaseConnectionString = Configuration.GetConnectionString("Application");
            services.RegisterEntities(databaseConnectionString)
                .AddCustomSwagger(Configuration)
                .AddCustomDbContext(Configuration)
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
                       "Ordering API");
               });

            var eventBus = app.ApplicationServices.GetRequiredService<IEventBus>();
            eventBus.SubscribeAsync<OrderDispatchedIntegrationEvent, OrderDispatchIntegrationEventHandler>();
         }
    }

    public static class ServiceCollectionExtension
    {
        public static IServiceCollection RegisterEntities(this IServiceCollection services, string databaseConnectionString)
        {
            services.AddScoped<OrderRepository>();
            services.AddTransient<OrderDispatchIntegrationEventHandler>();
         
            return services;
        }

        public static IServiceCollection AddEventBus(this IServiceCollection services)
        {
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
                    Title = "Ordering API",
                    Version = "v1",
                    Description = "Ordering API Documentation",
                    TermsOfService = "Terms Of Service",

                });
            });

            return services;
        }
    }
}
