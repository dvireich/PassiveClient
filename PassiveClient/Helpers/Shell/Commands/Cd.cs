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

    public class Cd : IShellCommand
    {
        private string _command;

        private IDirectoryManager _directoryManager;

        public Cd(IDirectoryManager directoryManager)
        {
            _directoryManager = directoryManager;
        }

        [Log(AttributeExclude = true)]
        public bool IsMatch(string command)
        {
            _command = command.ToLower(); ;

            return command.ToLower().StartsWith("cd") && (!command.Equals("cd ..") && !command.Equals("cd.."));
        }

        public string PerformCommand()
        {
            var arg = string.Join(" ", _command.Split(' ').Skip(1)).Replace("\"","");
            var path = _directoryManager.Exists(arg) ? arg : Path.Combine(_directoryManager.GetCurrentDirectory(), arg);
            _directoryManager.SetCurrentDirectory(path);

            return _directoryManager.GetCurrentDirectory();
        }
    }
}
