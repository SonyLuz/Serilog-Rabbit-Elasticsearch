using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.IO;
using Serilog.Sinks.RabbitMQ;
using Serilog.Sinks.RabbitMQ.Sinks.RabbitMQ;
using Serilog.Formatting.Json;

namespace Console.SeriLog
{
    class Program
    {
        static void Main(string[] args)
        {
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            var configurationBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("Serilog.json", optional: false, reloadOnChange: true);

            var configuration = configurationBuilder.Build();

            var configClient = new RabbitMQClientConfiguration
            {
                Username = "rabbitmq",
                Password = "rabbitmq",
                Exchange = "LogEvent",
                ExchangeType = "fanout",
                DeliveryMode = RabbitMQDeliveryMode.NonDurable,
                RouteKey = "Logs",
                Port = 5672,
                VHost = "/"
            };

            configClient.Hostnames.Add("localhost");

            RabbitMQSinkConfiguration sinkConfiguration = new RabbitMQSinkConfiguration
            {
                BatchPostingLimit = 0,
                Period = default,
                TextFormatter = new JsonFormatter()
            };


            Log.Logger = new LoggerConfiguration()
                .WriteTo.RabbitMQ(configClient, sinkConfiguration)
                .ReadFrom.Configuration(configuration)
                .Enrich.WithProperty("ApplicationName", "Console.SeriLog")
                .Enrich.WithProperty("Topic", $"Console.SeriLog-{environment}")
                .CreateLogger();

            try
            {
                Log.Information("Info - Log info");
                Log.Debug("Debug - Log debug");
                Log.Warning("Warning - Log warning");
                Log.Error("Error - Log error");
                Log.Fatal("Fatal - Log fatal");
                Log.Fatal(new Exception("teste"), "Fatal - Log fatal");
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Fatal Exception- Log fatal");
                throw;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}
