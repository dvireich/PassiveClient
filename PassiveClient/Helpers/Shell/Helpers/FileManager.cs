using PassiveClient.Helpers.Shell.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PassiveClient.Helpers.Shell.Helpers
{
    public class FileManager : IFileManager
    {
        public void Copy(string copyFrom, string copyTo)
        {
            File.Copy(copyFrom, copyTo);
        }

        public Stream Create(string path)
        {
            return File.Create(path);
        }

        public void Delete(string path)
        {
            if (!Exists(path)) return;

            File.Delete(path);
        }

        public bool Exists(string path)
        {
            return File.Exists(path);
        }

        public DateTime GetLastWriteTime(string path)
        {
            return File.GetLastWriteTime(path);
        }

        public void Move(string moveFrom, string moveTo)
        {
            File.Move(moveFrom, moveTo);
        }

        public Stream OpenRead(string path)
        {
            return File.OpenRead(path);
        }
    }
}
