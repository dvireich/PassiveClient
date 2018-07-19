using PassiveClient.Helpers.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace PassiveClient.Helpers
{
    public static class DirHelper
    {
        private static List<HardDrive> _hardDrives;

        private static List<HardDrive> GetHradDisksData()
        {
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_DiskDrive");
            List<HardDrive> hdCollection = new List<HardDrive>();

            foreach (ManagementObject wmi_HD in searcher.Get())
            {
                HardDrive hd = new HardDrive();
                hd.Model = wmi_HD["Model"].ToString();
                hd.Type = wmi_HD["InterfaceType"].ToString();
                hdCollection.Add(hd);
            }

            int i = 0;
            foreach (ManagementObject wmi_HD in searcher.Get())
            {
                // get the hard drive from collection
                // using index
                HardDrive hd = (HardDrive)hdCollection[i];

                // get the hardware serial no.
                if (wmi_HD["SerialNumber"] == null)
                    hd.SerialNo = "None";
                else
                    hd.SerialNo = wmi_HD["SerialNumber"].ToString();

                ++i;
            }

            return hdCollection;
        }

        private static List<HardDrive> GetDriveLetterAndLabelFromID(List<HardDrive> drives)
        {
            foreach (var drive in drives)
            {
                ManagementClass devs = new ManagementClass(@"Win32_Diskdrive");
                {
                    ManagementObjectCollection moc = devs.GetInstances();
                    foreach (ManagementObject mo in moc)
                    {
                        string serialNo = (string)mo["SerialNumber"];
                        if (serialNo == drive.SerialNo)
                        {
                            foreach (ManagementObject b in mo.GetRelated("Win32_DiskPartition"))
                            {
                                foreach (ManagementBaseObject c in b.GetRelated("Win32_LogicalDisk"))
                                {
                                    drive.Name = c["VolumeName"].ToString();
                                    drive.Letter = c["DeviceID"].ToString();
                                }
                            }
                        }
                    }
                }
            }

            return drives;
        }

        private static List<HardDrive> GetHardDrives()
        {
            return GetDriveLetterAndLabelFromID(GetHradDisksData());
        }

        private static string GetDriveLetter()
        {
            var currentDriveLetter = Directory.GetCurrentDirectory().Split(':').First() + ":";
            _hardDrives = _hardDrives ?? GetHardDrives();
            var drive = _hardDrives.FirstOrDefault(d => d.Letter == currentDriveLetter);
            if (drive == null) return string.Empty;

            return drive.Letter;
        }

        private static string GetDriveName()
        {
            var currentDriveLetter = Directory.GetCurrentDirectory().Split(':').First() + ":";
            _hardDrives = _hardDrives ?? GetHardDrives();
            var drive = _hardDrives.FirstOrDefault(d => d.Letter == currentDriveLetter);
            if (drive == null) return string.Empty;

            return drive.Name;
        }

        private static string GetDrivSerialNumber()
        {
            var currentDriveLetter = Directory.GetCurrentDirectory().Split(':').First() + ":";
            _hardDrives = _hardDrives ?? GetHardDrives();
            var drive = _hardDrives.FirstOrDefault(d => d.Letter == currentDriveLetter);
            if (drive == null) return string.Empty;

            return drive.SerialNo.Trim();
        }

        public static string GenerateFilesAndDirString()
        {
            var sb = new StringBuilder();

            sb.AppendLine(Directory.GetCurrentDirectory());
            sb.AppendLine();
            var volLetterAndName = $" Volume in drive {GetDriveLetter()} is {GetDriveName()}";
            var volSerialNum = $" Volume Serial Number is {GetDrivSerialNumber()}";

            sb.AppendLine(volLetterAndName);
            sb.AppendLine(volSerialNum);
            sb.AppendLine();

            var path = $" Directory of {Directory.GetCurrentDirectory()}";
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
            sb.AppendLine(Directory.GetCurrentDirectory());
            return sb.ToString();
        }

        public static string GenerateBareFormatAllFileAndFolderString()
        {
            return string.Concat(Environment.NewLine,
                                 Directory.GetCurrentDirectory(),
                                 Environment.NewLine,
                                 string.Join(Environment.NewLine, GetDirectories().Select(dir => dir.Split('\\').Last()).ToList()),
                                 string.Join(Environment.NewLine, GetFiles().Select(file => file.Split('\\').Last()).ToList()),
                                 Environment.NewLine,
                                 Environment.NewLine,
                                 Directory.GetCurrentDirectory());
        }

        public static string GenerateBareFormatFolderString()
        {
            return string.Concat(Directory.GetCurrentDirectory(),
                                 Environment.NewLine,
                                 string.Join(Environment.NewLine, GetDirectories().Select(dir => dir.Split('\\').Last()).ToList()),
                                 Environment.NewLine,
                                 Environment.NewLine,
                                 Directory.GetCurrentDirectory());
        }

        private static string[] GetDirectories()
        {
            return Directory.GetDirectories(Directory.GetCurrentDirectory());
        }

        private static string[] GetFiles()
        {
            return Directory.GetFiles(Directory.GetCurrentDirectory());
        }

        private static string GetFileSize(string path)
        {
            return new FileInfo(path).Length.ToString();
        }

        private static string GetModificationDate(string path)
        {
            return File.GetLastWriteTime(path).Date.ToString("dd/MM/yyyy");
        }

        private static string GetModificationTime(string path)
        {
            return File.GetLastWriteTime(path).ToString("HH:mm");
        }

        private static string CreateDirStringLine(string modificationDate, string modificationTime, bool directory, string size, string name)
        {
            return directory ? $"{modificationDate}  {modificationTime}    <DIR>            {name}" :
                               $"{modificationDate}  {modificationTime}              {size} {name}";
        }


    }
}
