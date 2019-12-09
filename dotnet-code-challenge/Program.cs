using System;
using System.Collections.Generic;
using dotnet_code_challenge.Helpers;
using dotnet_code_challenge.Models;
using dotnet_code_challenge.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NLog;

namespace dotnet_code_challenge
{
    class Program
    {
        static void Main(string[] args)
        {
            var logger = LogManager.GetCurrentClassLogger();
            try
            {
                logger.Info("Starting run sequence");
                var config = new ConfigurationBuilder()
                    .SetBasePath(System.IO.Directory.GetCurrentDirectory()) //From NuGet Package Microsoft.Extensions.Configuration.Json
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                    .Build();
                
                logger.Info("Build dependencies");
                var servicesProvider = DIHelper.BuildDi(config);
                
                logger.Info("Access FileProcessManager");
                var fileProcessManager = servicesProvider.GetRequiredService<FileProcessManager>();
                
                //TODO: Logic to get horses

                Console.ReadLine();
                logger.Info("Finished run sequence");
            }
            catch (Exception ex)
            {
                // NLog: catch any exception and log it.
                logger.Error(ex, "Stopped program because of exception");
                throw;
            }
            finally
            {
                // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
                LogManager.Shutdown();
            }
            Console.WriteLine("Hello World!");
        }
    }
}
