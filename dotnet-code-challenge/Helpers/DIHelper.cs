using System;
using System.Collections.Generic;
using dotnet_code_challenge.Interfaces;
using dotnet_code_challenge.Services;
using dotnet_code_challenge.Services.FileProcessors;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;

namespace dotnet_code_challenge.Helpers
{
    public static class DIHelper
    {
        public static IServiceProvider BuildDi(IConfiguration config)
        {
            if (config == null) throw new ArgumentNullException(nameof(config));
            
            //setup our DI
            var serviceProvider = new ServiceCollection()
                .AddLogging(loggingBuilder =>
                {
                    // configure Logging with NLog
                    loggingBuilder.ClearProviders();
                    loggingBuilder.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
                    loggingBuilder.AddNLog(config);
                })
                .AddTransient<XMLFileFeedParser>()
                .AddTransient<JsonFileFeedParser>()
                .AddTransient(factory =>
                {
                    Func<string, IFileFeedParser> accessor = key =>
                    {
                        switch (key)
                        {
                            case FileProcessManager.XmlFileType:
                                return factory.GetService<XMLFileFeedParser>();
                            case FileProcessManager.JSONFileType:
                                return factory.GetService<JsonFileFeedParser>();
                            default:
                                throw new KeyNotFoundException(); // or maybe return null, up to you
                        }
                    };
                    return accessor;
                })
                .AddTransient<FileProcessManager>();
                
            return serviceProvider.BuildServiceProvider();
            
        }
    }
}