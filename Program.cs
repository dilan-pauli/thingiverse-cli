﻿using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Typin;

namespace thingiverseCLI
{
    internal class Program
    {
        private static async Task Main(string[] args) =>
            await new CliApplicationBuilder()
            .ConfigureLogging(logger => logger.ClearProviders().AddConsole())
            .ConfigureServices(services =>
            {
                services.AddHttpClient<Services.ThingiverseAPI>();
                services.AddSingleton(provider => new ConfigurationBuilder().AddJsonFile("appsettings.json").Build());
            })
            .AddCommandsFromThisAssembly()
            .Build()
            .RunAsync();
    }
}