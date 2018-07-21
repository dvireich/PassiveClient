using PassiveClient.Helpers.Shell.Interfaces;
using PostSharp.Patterns.Diagnostics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PassiveClient.Helpers.Shell.Commands
{
    class CdToParentFolder : IShellCommand
    {
        private string _command;

        private IDirectoryManager _directoryManager;

        public CdToParentFolder(IDirectoryManager directoryManager)
        {
            _directoryManager = directoryManager;
        }

        [Log(AttributeExclude = true)]
        public bool IsMatch(string command)
        {
            _command = command.ToLower(); ;

            return command.ToLower().Equals("cd..") ||
                   command.ToLower().Equals("cd ..");
        }

        public string PerformCommand()
        {
            _directoryManager.SetCurrentDirectory(Directory.GetParent(_directoryManager.GetCurrentDirectory()).FullName);
            return _directoryManager.GetCurrentDirectory();
        }
    }
}
