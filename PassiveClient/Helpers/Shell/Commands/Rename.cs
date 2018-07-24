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

            GetArgs(_command, out var oldFileName, out var newFileName);

            _fileManager.Move(oldFileName, newFileName);

            return _directoryManager.GetCurrentDirectory();
        }

        private void GetArgs(string command, out string renameFrom, out string renameTo)
        {
            int renameFromStart = -1, renameFromEnd = -1, renameToStart = -1, renameToEnd = -1;
            int count = 0;

            for (int i = 0; i < command.Length; i++)
            {
                if (command[i] != '"') continue;

                switch (count)
                {
                    case 0:
                        renameFromStart = i;
                        count++;
                        break;
                    case 1:
                        renameFromEnd = i;
                        count++;
                        break;
                    case 2:
                        renameToStart = i;
                        count++;
                        break;
                    case 3:
                        renameToEnd = i;
                        count++;
                        break;
                }
            }

            renameFrom = command.Substring(renameFromStart, renameFromEnd - renameFromStart).Replace("\"", "");
            renameTo = command.Substring(renameToStart, renameToEnd - renameToStart).Replace("\"", "");
        }
    }
}
