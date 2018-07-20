using PassiveClient.Helpers.Data;
using PassiveClient.Helpers.Interfaces;
using PassiveClient.Helpers.Shell.Commands;
using PassiveClient.Helpers.Shell.Interfaces;
using PostSharp.Patterns.Diagnostics;
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
    [Log(AttributeExclude = true)]
    class CSharpShell : IShell
    {

        private List<IShellCommand> _commands;
        private IDirHelper _dirHelper;

        public CSharpShell(IDirHelper dirHelper)
        {
            _dirHelper = dirHelper;
        }

        public void CloseShell()
        {
            return;
        }

        public string NextCommand(string commandToPerform)
        {
            InitializeShellCommands();
            var command = _commands.FirstOrDefault(c => c.IsMatch(commandToPerform));
            if (command == null) throw new NotImplementedException($"The command {commandToPerform} is not supported");
            return command.PerformCommand();
        }

        public string Run()
        {
            return Directory.GetCurrentDirectory();
        }

        private void InitializeShellCommands()
        {
            _commands = new List<IShellCommand>
            {
                new Dir(_dirHelper),
                new DirBareFolder(_dirHelper),
                new DirBareFormat(_dirHelper),
                new Cd(),
                new CdToParentFolder(),
                new Copy(),
                new Delete(),
                new Rename()
            };
        }
    }
}
