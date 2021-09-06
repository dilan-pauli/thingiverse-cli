using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Typin;
using Typin.Attributes;
using Typin.Console;

namespace thingiverseCLI.Commands
{
    [Command("temp", Description = "Open the temp folder")]
    public class TempCommand : ICommand
    {
        public async ValueTask ExecuteAsync(IConsole console)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                System.Diagnostics.Process.Start("explorer.exe", Config.TempPath);
            }

            return;
        }
    }
}
