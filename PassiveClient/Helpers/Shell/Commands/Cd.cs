using PassiveClient.Helpers.Shell.Interfaces;
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

        public bool IsMatch(string command)
        {
            _command = command.ToLower(); ;

            return command.ToLower().StartsWith("cd") && (!command.Equals("cd ..") && !command.Equals("cd.."));
        }

        public string PerformCommand()
        {
            var arg = string.Join(" ", _command.Split(' ').Skip(1)).Replace("\"","");
            var path = Directory.Exists(arg) ? arg : Path.Combine(Directory.GetCurrentDirectory(), arg);
            Directory.SetCurrentDirectory(path);

            return Directory.GetCurrentDirectory();
        }
    }
}
