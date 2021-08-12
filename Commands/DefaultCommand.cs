using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using thingiverseCLI.Services;
using Typin;
using Typin.Attributes;
using Typin.Console;
using static System.Net.WebRequestMethods;

namespace thingiverseCLI.Commands
{
    [Command()]
    public class DefaultCommand : ICommand
    {
        private readonly ThingiverseAPI api;
        private readonly ILogger<DefaultCommand> logger;
        private readonly IConfiguration config;

        public static string TempPath { get { return Path.Combine(Path.GetTempPath(), "thingiverse"); } }

        [CommandParameter(0, Description = "The thing id from to download and open in cura")]
        public int ThingId { get; set; }

        public DefaultCommand(ThingiverseAPI api, ILogger<DefaultCommand> logger, IConfiguration config)
        {
            this.api = api;
            this.logger = logger;
            this.config = config;
        }

        public async ValueTask ExecuteAsync(IConsole console)
        {
            try
            {
                var files = (await api.GetFiles(ThingId)).ToList();

                console.Output.WriteLine($"Got {files.Count()} files, choose file to open - 0 for all");

                int i = 1;

                files = files.OrderByDescending(x => x.download_count).ToList();

                foreach (var file in files)
                {
                    console.Output.WriteLine($"{i} - '{file.name}' Downloaded: {file.download_count} times");
                    i++;
                }

                var choice = console.Input.ReadLine();

                int indexChoosen;
                if (int.TryParse(choice, out indexChoosen))
                {
                    var filesToOpen = new List<string>();

                    if (indexChoosen == 0)
                    {
                        console.Output.WriteLine($"Opening all...");
                        foreach (var file in files)
                        {
                            await console.Output.WriteLineAsync($"Downloading {file.name}");
                            var filePath = Download(files[indexChoosen].name, file.download_url);
                            await console.Output.WriteLineAsync($"Downloading {file.name} complete @ {filePath}");
                            filesToOpen.Add(filePath);
                        }
                    }
                    else
                    {
                        indexChoosen--;
                        console.Output.WriteLine($"Downloading and opening {files[indexChoosen].name}");
                        var url = files[indexChoosen].download_url;
                        var filePath = Download(files[indexChoosen].name, url);
                        await console.Output.WriteLineAsync($"Downloading {files[indexChoosen].name} complete @ {filePath}");
                        filesToOpen.Add(filePath);

                    }

                    Process.Start(config.GetValue<string>(Config.CuraEXELocation), string.Join(' ', filesToOpen));
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Unable to open thing {this.ThingId}");
            }
        }

        /// <summary>
        /// TO DO Add Prgress bar
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        private string Download(string fileName, string url)
        {
            string filePath = Path.Combine(TempPath, fileName);
            using (System.Net.WebClient wc = new System.Net.WebClient())
            {
                try
                {
                    wc.DownloadFile(url + this.api.AccessParameter, Path.Combine(TempPath, filePath));
                    return filePath;
                }
                catch (System.Exception ex)
                {
                    logger.LogError(ex, $"Error downloading file {filePath}");
                    return null;
                }
            }
        }
    }
}
