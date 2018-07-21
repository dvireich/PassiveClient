using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PassiveClient.Helpers.Shell.Interfaces
{
    public interface IFileManager
    {
        DateTime GetLastWriteTime(string path);

        Stream OpenRead(string path);

        bool Exists(string path);

        Stream Create(string path);

        void Delete(string path);

        void Move(string moveFrom, string moveTo);

        void Copy(string copyFrom, string copyTo);
    }
}
