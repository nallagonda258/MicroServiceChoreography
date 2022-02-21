using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Formatting.Json;
using System;
using System.IO;

namespace KafkaService.Impl
{
    public static class CustomLogger
    {
        public static Action<HostBuilderContext, LoggerConfiguration> Configure =>
        (hostingContext, loggerConfiguration) =>
        {
            loggerConfiguration.MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)
                .MinimumLevel.Override("System", Serilog.Events.LogEventLevel.Warning)
                .WriteTo.File(new JsonFormatter(), Path.Combine(Directory.GetCurrentDirectory().Substring(0, Directory.GetCurrentDirectory().LastIndexOf("\\")) + "\\Logs\\log.txt"), shared: true);
        };
    }
}
