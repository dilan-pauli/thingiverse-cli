using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Typin;
using Typin.Attributes;
using Typin.Console;

namespace thingiverseCLI.Commands
{
    [Command("config")]
    public class Config : ICommand
    {
        public const string APIKeyEnvironmentVarName = "THINGIVERSE_API_KEY";
        private readonly IConfiguration config;

        public Config(IConfiguration config)
        {
            this.config = config;
        }

        [CommandParameter(0)]
        public ConfigParameters parameters { get; set; }

        [CommandOption("apiKey", 'i')]
        public string APIKey { get; set; }

        public async ValueTask ExecuteAsync(IConsole console)
        {
            switch (parameters)
            {
                case ConfigParameters.set:
                    await this.SetEnvironmentVariable(console);
                    break;
                case ConfigParameters.get:
                    await console.Output.WriteLineAsync($"Your Thingiverse API key is {config.GetValue<string>(APIKeyEnvironmentVarName)}");
                    break;
            }

            return;
        }

        public async Task SetEnvironmentVariable(IConsole console)
        {
            // How does this work?
            if (string.IsNullOrEmpty(this.APIKey))
            {
                await console.Output.WriteLineAsync("Error: When setting the API Key a -i value is required.");
            }
            else
            {
                Environment.SetEnvironmentVariable(APIKeyEnvironmentVarName, this.APIKey, EnvironmentVariableTarget.User);
            }

            return;
        }
    }

    public enum ConfigParameters
    {
        set,
        get
    }
}