using PassiveClient.Helpers.Shell.Interfaces;
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

        public bool IsMatch(string command)
        {
            _command = command.ToLower(); ;

            return command.ToLower().Equals("cd..") ||
                   command.ToLower().Equals("cd ..");
        }

        public string PerformCommand()
        {
            Directory.SetCurrentDirectory(Directory.GetParent(Directory.GetCurrentDirectory()).FullName);
            return Directory.GetCurrentDirectory();
        }
    }
}
