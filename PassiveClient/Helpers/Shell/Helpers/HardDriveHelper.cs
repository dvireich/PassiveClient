using PassiveClient.Helpers.Data;
using PassiveClient.Helpers.Shell.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace PassiveClient.Helpers.Shell.Helpers
{
    class HardDriveHelper : IHardDriveHelper
    {
        private List<HardDrive> GetHradDisksData()
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

        private List<HardDrive> GetDriveLetterAndLabelFromID(List<HardDrive> drives)
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

        public List<HardDrive> GetHardDrives()
        {
            return GetDriveLetterAndLabelFromID(GetHradDisksData());
        }
    }
}
