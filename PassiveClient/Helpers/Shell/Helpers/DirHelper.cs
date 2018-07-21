using PassiveClient.Helpers.Data;
using PassiveClient.Helpers.Interfaces;
using PassiveClient.Helpers.Shell.Interfaces;
using PostSharp.Patterns.Diagnostics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace PassiveClient.Helpers
{
    [Log(AttributeExclude = true)]
    public class DirHelper : IDirHelper
    {
        public DirHelper(IDirectoryManager directoryManager, 
                         IFileManager fIleManager,
                         IFileInfoHelper fileHelper,
                         IHardDriveHelper hardDriveHelper)
        {
            _hardDriveHelper = hardDriveHelper;
            _hardDrives = _hardDriveHelper.GetHardDrives();
            _directoryManager = directoryManager;
            _fIleManager = fIleManager;
            _fileHelper = fileHelper;
        }

        private static List<HardDrive> _hardDrives;
        private IDirectoryManager _directoryManager;
        private IFileManager _fIleManager;
        private IFileInfoHelper _fileHelper;
        private IHardDriveHelper _hardDriveHelper;

        private string GetDriveLetter()
        {
            var currentDriveLetter = _directoryManager.GetCurrentDirectory().Split(':').First() + ":";
            _hardDrives = _hardDrives ?? _hardDriveHelper. GetHardDrives();
            var drive = _hardDrives.FirstOrDefault(d => d.Letter == currentDriveLetter);
            if (drive == null) return string.Empty;

            return drive.Letter;
        }

        private string GetDriveName()
        {
            var currentDriveLetter = _directoryManager.GetCurrentDirectory().Split(':').First() + ":";
            _hardDrives = _hardDrives ?? _hardDriveHelper.GetHardDrives();
            var drive = _hardDrives.FirstOrDefault(d => d.Letter == currentDriveLetter);
            if (drive == null) return string.Empty;

            return drive.Name;
        }

        private string GetDrivSerialNumber()
        {
            var currentDriveLetter = _directoryManager.GetCurrentDirectory().Split(':').First() + ":";
            _hardDrives = _hardDrives ?? _hardDriveHelper.GetHardDrives();
            var drive = _hardDrives.FirstOrDefault(d => d.Letter == currentDriveLetter);
            if (drive == null) return string.Empty;

            return drive.SerialNo.Trim();
        }

        public string GenerateFilesAndDirString()
        {
            var sb = new StringBuilder();

            sb.AppendLine(_directoryManager.GetCurrentDirectory());
            sb.AppendLine();
            var volLetterAndName = $" Volume in drive {GetDriveLetter()} is {GetDriveName()}";
            var volSerialNum = $" Volume Serial Number is {GetDrivSerialNumber()}";

            sb.AppendLine(volLetterAndName);
            sb.AppendLine(volSerialNum);
            sb.AppendLine();

            var path = $" Directory of {_directoryManager.GetCurrentDirectory()}";
            sb.AppendLine(path);
            sb.AppendLine();

            var numberOfDir = 2;
            sb.AppendLine(CreateDirStringLine(string.Empty, string.Empty, true , string.Empty, "." ));
            sb.AppendLine(CreateDirStringLine(string.Empty, string.Empty, true, string.Empty, ".."));
            foreach (var dir in GetDirectories())
            {
                var dirName = dir.Split('\\').Last();
                var modificationDate = GetModificationDate(dir);
                var modificationTime = GetModificationTime(dir);
                sb.AppendLine(CreateDirStringLine(modificationDate, modificationTime, true, null, dirName));
                numberOfDir++;
            }

            var numberOfFiles = 0;
            foreach (var file in GetFiles())
            {
                var fileName = file.Split('\\').Last(); ;
                var modificationDate = GetModificationDate(file);
                var modificationTime = GetModificationTime(file);
                var fileSize = GetFileSize(file);
                sb.AppendLine(CreateDirStringLine(modificationDate, modificationTime, false, fileSize, fileName));
                numberOfFiles++;
            }

            sb.AppendLine($"{numberOfFiles} File(s)");
            sb.AppendLine($"{numberOfDir} Dir(s)");
            sb.AppendLine();
            sb.AppendLine(_directoryManager.GetCurrentDirectory());
            return sb.ToString();
        }

        public string GenerateBareFormatAllFileAndFolderString()
        {
            return string.Concat(Environment.NewLine,
                                 _directoryManager.GetCurrentDirectory(),
                                 Environment.NewLine,
                                 string.Join(Environment.NewLine, GetDirectories().Select(dir => dir.Split('\\').Last()).ToList()),
                                 string.Join(Environment.NewLine, GetFiles().Select(file => file.Split('\\').Last()).ToList()),
                                 Environment.NewLine,
                                 Environment.NewLine,
                                 _directoryManager.GetCurrentDirectory());
        }

        public string GenerateBareFormatFolderString()
        {
            return string.Concat(_directoryManager.GetCurrentDirectory(),
                                 Environment.NewLine,
                                 string.Join(Environment.NewLine, GetDirectories().Select(dir => dir.Split('\\').Last()).ToList()),
                                 Environment.NewLine,
                                 Environment.NewLine,
                                 _directoryManager.GetCurrentDirectory());
        }

        private string[] GetDirectories()
        {
            return _directoryManager.GetDirectories(_directoryManager.GetCurrentDirectory());
        }

        private string[] GetFiles()
        {
            return _directoryManager.GetFiles(_directoryManager.GetCurrentDirectory());
        }

        private string GetFileSize(string path)
        {
            return _fileHelper.GetFileLength(path).ToString();
        }

        private string GetModificationDate(string path)
        {
            return _fIleManager.GetLastWriteTime(path).Date.ToString("dd/MM/yyyy");
        }

        private string GetModificationTime(string path)
        {
            return _fIleManager.GetLastWriteTime(path).ToString("HH:mm");
        }

        private string CreateDirStringLine(string modificationDate, string modificationTime, bool directory, string size, string name)
        {
            return directory ? $"{modificationDate}  {modificationTime}    <DIR>            {name}" :
                               $"{modificationDate}  {modificationTime}              {size} {name}";
        }
    }
}
