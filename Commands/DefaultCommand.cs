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

        [CommandParameter(0, Description = "The thing id from to download and open in slicer")]
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
                var thing = await api.GetInfo(ThingId);

                var files = (await api.GetFiles(ThingId)).ToList();

                console.Output.WriteLine($"{thing.name} - Got {files.Count()} files, choose file to open - 0 for all");

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
                    var folder = thing.name.SanitizeFileName().Replace(' ','_');
                    var downloadPath = Directory.CreateDirectory(Path.Combine(Config.TempPath, folder)).FullName;

                    if (indexChoosen == 0)
                    {
                        console.Output.WriteLine($"{thing.name} - Opening all...");
                        foreach (var file in files)
                        {
                            await console.Output.WriteLineAsync($"Downloading {file.name}");
                            var filePath = Download(file.name, file.download_url, downloadPath);
                            await console.Output.WriteLineAsync($"Downloading {file.name} complete @ {filePath}");
                            filesToOpen.Add(filePath);
                        }
                    }
                    else
                    {
                        indexChoosen--;
                        console.Output.WriteLine($"{thing.name} - Downloading and opening {files[indexChoosen].name}");
                        var url = files[indexChoosen].download_url;
                        var filePath = Download(files[indexChoosen].name, url, downloadPath);
                        await console.Output.WriteLineAsync($"{thing.name} - Downloading {files[indexChoosen].name} complete @ {filePath}");
                        filesToOpen.Add(filePath);

                    }

                    Process.Start(config.GetValue<string>(Config.SlicerLocation), string.Join(' ', filesToOpen));
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
        private string Download(string fileName, string url, string downloadPath)
        {
            string filePath = Path.Combine(downloadPath, fileName);
            using (System.Net.WebClient wc = new System.Net.WebClient())
            {
                try
                {
                    wc.DownloadFile(url + this.api.AccessParameter, Path.Combine(Config.TempPath, filePath));
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
