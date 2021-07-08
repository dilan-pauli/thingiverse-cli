using System;
using System.Threading.Tasks;
using Typin;
using Typin.Attributes;
using Typin.Console;

namespace thingiverseCLI.Commands
{
    [Command]
    public class DefaultCommand : ICommand
    {

        public ValueTask ExecuteAsync(IConsole console)
        {
            throw new NotImplementedException();
        }
    }
}