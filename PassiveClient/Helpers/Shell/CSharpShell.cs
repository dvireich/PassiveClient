using PassiveClient.Helpers.Interfaces;
using PassiveClient.Helpers.Shell.Commands;
using PassiveClient.Helpers.Shell.Helpers;
using PassiveClient.Helpers.Shell.Interfaces;
using PostSharp.Patterns.Diagnostics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PassiveClient.Helpers
{
    [Log(AttributeExclude = true)]
    class CSharpShell : IShell
    {

        private List<IShellCommand> _commands;
        private readonly IDirHelper _dirHelper;
        private readonly IDirectoryManager _directoryManager;

        public CSharpShell(IDirectoryManager directoryManager)
        {
            _directoryManager = directoryManager;
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
            return _directoryManager.GetCurrentDirectory();
        }

        private void InitializeShellCommands()
        {
            _commands = new List<IShellCommand>
            {
                new Dir(new DirHelper(new DirectoryManager(), new FileManager(), new FileInfoHelper(), new HardDriveHelper())),
                new DirBareFolder(new DirHelper(new DirectoryManager(), new FileManager(), new FileInfoHelper(), new HardDriveHelper())),
                new DirBareFormat(new DirHelper(new DirectoryManager(), new FileManager(), new FileInfoHelper(), new HardDriveHelper())),
                new Cd(new DirectoryManager()),
                new CdToParentFolder(new DirectoryManager()),
                new Copy(new DirectoryManager() ,new FileManager()),
                new Delete(new DirectoryManager() ,new FileManager()),
                new Rename(new DirectoryManager() ,new FileManager())
            };
        }
    }
}
