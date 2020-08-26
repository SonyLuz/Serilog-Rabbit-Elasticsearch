using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using System;
using System.IO;

namespace Console.SeriLog
{
    class Program
    {
        static void Main(string[] args)
        {
            var configurationBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("Serilog.json", optional: false, reloadOnChange: true);

            var configuration = configurationBuilder.Build();

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();

            try
            {
                Log.Information("Info - Log info");
                Log.Debug("Debug - Log debug");
                Log.Warning("Warning - Log warning");
                Log.Error("Error - Log error");
                Log.Fatal("Fatal - Log fatal");
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
