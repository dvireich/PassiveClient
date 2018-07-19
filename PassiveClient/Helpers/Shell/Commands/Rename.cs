using PassiveClient.Helpers.Shell.Interfaces;
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

        public bool IsMatch(string command)
        {
            _command = command;
            return command.ToLower().StartsWith("rename");
        }

        public string PerformCommand()
        {
            var args = _command.Split(' ').Skip(1).ToArray();
            if (args.Length < 2) throw new ArgumentException($"The rename command need at least 2 argumenrs");

            var oldFileName = args[0];
            var newFileName = args[1];

            System.IO.File.Move(oldFileName, newFileName);

            return Directory.GetCurrentDirectory();
        }
    }
}
