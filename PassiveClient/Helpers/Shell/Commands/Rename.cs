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
    public class Rename : IShellCommand
    {
        private string _command;
        private IDirectoryManager _directoryManager;
        private IFileManager _fileManager;

        public Rename(IDirectoryManager directoryManager, IFileManager fileManager)
        {
            _directoryManager = directoryManager;
            _fileManager = fileManager;
        }

        [Log(AttributeExclude = true)]
        public bool IsMatch(string command)
        {
            _command = command;
            return command.ToLower().StartsWith("rename");
        }

        public string PerformCommand()
        {
            var args = _command.Split(' ').Skip(1).ToArray();
            if (args.Length < 2) throw new ArgumentException($"The rename command need at least 2 argumenrs");

            var oldFileName = args[0].Replace("\"","");
            var newFileName = args[1].Replace("\"", "");

            _fileManager.Move(oldFileName, newFileName);

            return _directoryManager.GetCurrentDirectory();
        }
    }
}
