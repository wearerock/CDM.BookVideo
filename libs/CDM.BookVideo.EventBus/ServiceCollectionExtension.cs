using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace CDM.BookVideo.EventBus {
  public static class ServiceCollectionExtension {
    public static IServiceCollection AddEventBus(this IServiceCollection services, IConfiguration config) {
      services.AddSingleton<IEventBusConnection>(sp => {
        var logger = sp.GetRequiredService<ILogger<EventBusConnection>>();
        var retryCount = int.TryParse(config["EventBusRetryCount"], out int rc) ? rc : 2;
        var factory = new ConnectionFactory() {
          HostName = config["EventBusConnection"],
          DispatchConsumersAsync = true
        };

        return new EventBusConnection(logger, factory, retryCount);
      });

      services.AddSingleton<IEventBus>(sp => {
        var queueName = config["ShippingQueueName"] ?? "Shipping";
        var connection = sp.GetRequiredService<IEventBusConnection>();
        var scopeFactory = sp.GetRequiredService<IServiceScopeFactory>();
        var logger = sp.GetRequiredService<ILogger<EventBus>>();

        var retryCount = int.TryParse(config["EventBusRetryCount"], out int rc) ? rc : 2;

        return new EventBus(scopeFactory, connection, logger, queueName, retryCount);
      });

      return services;
    }
  }
}
