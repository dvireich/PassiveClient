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
        private readonly IDirHelper dirHelper;
        private readonly IDirectoryManager directoryManager;
        private readonly IFileManager fileManager;

        public CSharpShell(IDirectoryManager directoryManager,
                           IFileManager fileManager,
                           IDirHelper dirHelper)
        {
            this.fileManager = fileManager;
            this.dirHelper = dirHelper;
            this.directoryManager = directoryManager;
            InitializeShellCommands();
        }

        public CSharpShell() : this(new DirectoryManager(), new FileManager(),
                                    new DirHelper(new DirectoryManager(), new FileManager(), new FileInfoHelper(), new HardDriveHelper()))
        {
            
        }

        public void CloseShell()
        {
            return;
        }

        public string NextCommand(string commandToPerform)
        {
            var command = _commands.FirstOrDefault(c => c.IsMatch(commandToPerform));
            if (command == null) throw new NotImplementedException($"The command {commandToPerform} is not supported");
            return command.PerformCommand();
        }

        public string Run()
        {
            return directoryManager.GetCurrentDirectory();
        }

        private void InitializeShellCommands()
        {
            _commands = new List<IShellCommand>
            {
                new Dir(dirHelper),
                new DirBareFolder(dirHelper),
                new DirBareFormat(dirHelper),
                new Cd(directoryManager),
                new CdToParentFolder(directoryManager),
                new Copy(directoryManager ,fileManager),
                new Delete(directoryManager ,fileManager),
                new Rename(directoryManager ,fileManager)
            };
        }
    }
}
