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
    public class Delete : IShellCommand
    {
        private string _command;
        private IDirectoryManager _directoryManager;
        private IFileManager _fileManager;

        public Delete(IDirectoryManager directoryManager, IFileManager fileManager)
        {
            _directoryManager = directoryManager;
            _fileManager = fileManager;
        }

        [Log(AttributeExclude = true)]
        public bool IsMatch(string command)
        {
            _command = command;
            return command.StartsWith("del");
        }

        public string PerformCommand()
        {
            var pathToDel = _command.Split(' ').Skip(1).FirstOrDefault();
            if (pathToDel == null) throw new ArgumentNullException($"The command should not get null argument: {_command} ");

            pathToDel = pathToDel.Replace("\"", "");

            var path = string.Empty;                                              
            if(_fileManager.Exists(pathToDel) || _directoryManager.Exists(pathToDel))
            {
                path = pathToDel;
            }
            else if(_fileManager.Exists(Path.Combine(_directoryManager.GetCurrentDirectory(), pathToDel)) ||
                    _directoryManager.Exists(Path.Combine(_directoryManager.GetCurrentDirectory(), pathToDel)))
            {
                path = Path.Combine(_directoryManager.GetCurrentDirectory(), pathToDel);
            }
            _fileManager.Delete(path);

            return _directoryManager.GetCurrentDirectory();
        }
    }
}
