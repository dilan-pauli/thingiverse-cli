using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using thingiverseCLI.Services;
using Typin;
using Typin.Attributes;
using Typin.Console;

namespace thingiverseCLI.Commands
{
    [Command()]

    public class Default : ICommand
    {
        private readonly ThingiverseAPI api;

        [CommandParameter(0, Description = "The thing id from to download and open in cura")]
        public int ThingId { get; set; }

        public Default(ThingiverseAPI api)
        {
            this.api = api;
        }

        public async ValueTask ExecuteAsync(IConsole console)
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
                if (indexChoosen == 0)
                {
                    console.Output.WriteLine($"Opening all...");

                }
                else
                {
                    console.Output.WriteLine($"Opening {files[indexChoosen].name}");
                }
            }
        }
    }
}
