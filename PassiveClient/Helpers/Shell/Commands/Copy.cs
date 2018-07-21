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
    public class Copy : IShellCommand
    {
        private string _command;

        private IDirectoryManager _directoryManager;
        private IFileManager _fileManager;

        public Copy(IDirectoryManager directoryManager , IFileManager fileManager)
        {
            _directoryManager = directoryManager;
            _fileManager = fileManager;
        }

        [Log(AttributeExclude = true)]
        public bool IsMatch(string command)
        {
            _command = command;
            return command.ToLower().StartsWith("copy");
        }

        public string PerformCommand()
        {
            var args = _command.Split(' ').Skip(1).ToArray();
            if (args.Length < 2) throw new ArgumentException($"The copy command need at least 2 argumenrs");

            GetArgs(_command, out string copyFrom, out string copyTo);
            _fileManager.Copy(copyFrom, copyTo);

            return _directoryManager.GetCurrentDirectory();
        }

        private void GetArgs(string command, out string copyFrom, out string copyTo)
        {
            int copyfromStart = -1, copyformEnd = -1, copyToStart = -1, copyToEnd = -1;
            int count = 0;

            for (int i = 0; i < command.Length; i++)
            {
                if (command[i] != '"') continue;

                switch (count)
                {
                    case 0:
                        copyfromStart = i;
                        count++;
                        break;
                    case 1:
                        copyformEnd = i;
                        count++;
                        break;
                    case 2:
                        copyToStart = i;
                        count++;
                        break;
                    case 3:
                        copyToEnd = i;
                        count++;
                        break;
                }
            }

            copyFrom = command.Substring(copyfromStart, copyformEnd - copyfromStart).Replace("\"","");
            copyTo = command.Substring(copyToStart, copyToEnd - copyToStart).Replace("\"", "");
            var filename = Path.GetFileName(copyFrom);

            if (!copyTo.Contains(filename))
            {
                copyTo = Path.Combine(copyTo, filename);
            }
        }
    }
}
