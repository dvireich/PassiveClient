using PassiveClient.Helpers.Data;
using PassiveClient.Helpers.Interfaces;
using PassiveClient.Helpers.Shell.Commands;
using PassiveClient.Helpers.Shell.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace PassiveClient.Helpers
{
    class CSharpShell : IShell
    {
        private HashSet<string> _supportedCommands = new HashSet<string>()
        {
            //Must be in this order
            "rename",
            "del",
            "copy",
        };

        private List<IShellCommand> _commands;

        public void CloseShell()
        {
            throw new NotImplementedException();
        }

        public string NextCommand(string commandToPerform)
        {

            var command = _commands.FirstOrDefault(c => c.IsMatch(commandToPerform));
            if (command == null) throw new NotImplementedException($"The command {commandToPerform} is not supported");

            return command.PerformCommand();
        }

        public string Run()
        {
            throw new NotImplementedException();
        }

        private void InitializeShellCommands()
        {
            _commands = new List<IShellCommand>
            {
                new Dir(),
                new DirBareFolder(),
                new DirBareFormat(),
            };


        }
    }
}
