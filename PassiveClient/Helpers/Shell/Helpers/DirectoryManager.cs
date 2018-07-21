using PassiveClient.Helpers.Shell.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PassiveClient.Helpers.Shell.Helpers
{
    public class DirectoryManager : IDirectoryManager
    {
        public void CreateDirectory(string path)
        {
            Directory.CreateDirectory(path);
        }

        public bool Exists(string path)
        {
            return Directory.Exists(path);
        }

        public string GetCurrentDirectory()
        {
            return Directory.GetCurrentDirectory();
        }

        public string[] GetDirectories(string path)
        {
            return Directory.GetDirectories(path);
        }

        public string[] GetFiles(string path)
        {
            return Directory.GetFiles(path);
        }

        public void SetCurrentDirectory(string path)
        {
            Directory.SetCurrentDirectory(path);
        }
    }
}
