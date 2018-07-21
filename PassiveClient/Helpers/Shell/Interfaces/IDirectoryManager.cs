using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PassiveClient.Helpers.Shell.Interfaces
{
    public interface IDirectoryManager
    {
        string GetCurrentDirectory();

        string[] GetDirectories(string path);

        string[] GetFiles(string path);

        void SetCurrentDirectory(string path);

        void CreateDirectory(string path);

        bool Exists(string path);
    }
}
