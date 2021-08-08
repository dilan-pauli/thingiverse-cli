using System;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Typin;
using Typin.Attributes;
using Typin.Console;

namespace thingiverseCLI.Commands
{
    [Command("config", Description = "Interact with the application configuration, e.g. set the api key.")]
    public class Config : ICommand
    {
        public const string APIKeyEnvironmentVarName = "THINGIVERSE_API_KEY";
        public const string CuraEXELocation = "THINGIVERSE_CURA_LOCATION";

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
                    await console.Output.WriteLineAsync($"Your THINGIVERSE_API_KEY is {config.GetValue<string>(APIKeyEnvironmentVarName)}");
                    await console.Output.WriteLineAsync($"Your THINGIVERSE_CURA_LOCATION is {config.GetValue<string>(CuraEXELocation)}");
                    break;
            }

            return;
        }

        /// <summary>
        /// Need to be administrator to set the enviroment variables.
        /// </summary>
        /// <param name="console"></param>
        /// <returns></returns>
        public async Task SetEnvironmentVariable(IConsole console)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && new WindowsPrincipal(WindowsIdentity.GetCurrent()).IsInRole(WindowsBuiltInRole.Administrator))
            {
                console.Output.WriteLine("Must be administrator to set the enviroment variable.");
                return;
            }

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