using System.Management;

namespace UsbSecureManager
{
    public class DeviceManager
    {

        public static string TemporaryPath { get; set; }
        public List<UsbDevice> DeviceList = new List<UsbDevice>();
        public UsbDevice? CurrentDevice { get; set; }

        public void UpdateDeviceList()
        {
            DeviceList.Clear();
            ManagementObjectSearcher collection = new ManagementObjectSearcher(
                @"SELECT * FROM Win32_DiskDrive WHERE InterfaceType='USB' AND MediaType='Removable Media'"
            );
            foreach (var device in collection.Get())
                DeviceList.Add(GetDeviceLetter(device["DeviceID"].ToString()));
        }

        private UsbDevice GetDeviceLetter(String deviceID)
        {
            ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_LogicalDisk");
            foreach (ManagementObject disk in searcher.Get())
            {
                ManagementObjectSearcher partitions = new ManagementObjectSearcher(
                    @"ASSOCIATORS OF {Win32_LogicalDisk.DeviceID='" + disk["DeviceID"].ToString() + "'} WHERE AssocClass = Win32_LogicalDiskToPartition"
                );

                foreach (var partition in partitions.Get())
                {
                    partitions = new ManagementObjectSearcher(
                        @"ASSOCIATORS OF {Win32_DiskPartition.DeviceID='" + partition["DeviceID"] + "'} WHERE AssocClass = Win32_DiskDriveToDiskPartition"
                    );

                    string diskDeviceID = String.Empty;
                    foreach (var drive in partitions.Get())
                        diskDeviceID = drive["DeviceID"].ToString();

                    if (diskDeviceID.Equals(deviceID))
                        return new UsbDevice(disk["Name"].ToString(), disk["VolumeName"].ToString());
                }
            }
            return null;
        }

        public List<DirectoryInfo> GetDirectories(string dirPath)
        {
            DirectoryInfo[] dirs = new DirectoryInfo(dirPath).GetDirectories();
            List<DirectoryInfo> processedDirs = new List<DirectoryInfo>();

            foreach (var dir in dirs)
                if ((dir.Attributes & FileAttributes.Hidden) == 0 && !dir.Name.Equals("System Volume Information"))
                    processedDirs.Add(dir);

            return processedDirs;
        }

        public List<FileInfo> GetFiles(string dirPath)
        {
            FileInfo[] files = new DirectoryInfo(dirPath).GetFiles();
            List<FileInfo> processedFiles = new List<FileInfo>();

            foreach (var file in files)
                if ((file.Attributes & FileAttributes.Hidden) == 0)
                    processedFiles.Add(file);

            return processedFiles;
        }

        public static bool IsEncrypted(UsbDevice device)
        {
            return File.Exists($"{device.LogicalDiskLetter}\\config");
        }

        public static bool ConfirmPassword(UsbDevice device, string password)
        {
            string fullPath = $"{device.LogicalDiskLetter}\\config";
            using (StreamReader sr = File.OpenText(fullPath))
            {
                string s = "";
                while ((s = sr.ReadLine()) != null)
                {
                    if (s.StartsWith("Pas"))
                    {
                        if (s.Substring(s.IndexOf(":") + 2).Equals(CipherUtils.GetHash(password)))
                            return true;
                        return false;
                    }   
                }
            }
            return false;
        }

        public static void UnlockDevice(string deviceLetter, string password)
        {
            string docPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            string tmpPath = docPath + "\\Usb Secure Manager\\tmp";
            Directory.CreateDirectory(docPath + "\\Usb Secure Manager");
            Directory.CreateDirectory(tmpPath);

            foreach (string dir in Directory.GetDirectories(tmpPath))
                Directory.Delete(dir, true);
            foreach (string file in Directory.GetFiles(tmpPath))
                File.Delete(file);

            DirectoryInfo sourcePath = new DirectoryInfo(deviceLetter + "\\");
            DirectoryInfo targetPath = new DirectoryInfo(tmpPath);
            DeviceManager dm = new DeviceManager();
            CopyFiles(sourcePath, targetPath, dm);
            new FileInfo($"{sourcePath.FullName}\\config").CopyTo($"{targetPath.FullName}\\config");
            CipherUtils.DecryptDevice(targetPath.FullName, password);
        }

        public static void CopyFiles(DirectoryInfo sourcePath, DirectoryInfo targetPath, DeviceManager dm)
        {
            foreach (DirectoryInfo dir in dm.GetDirectories(sourcePath.FullName))
                CopyFiles(dir, targetPath.CreateSubdirectory(dir.Name), dm);
            foreach (FileInfo file in dm.GetFiles(sourcePath.FullName))
                file.CopyTo(Path.Combine(targetPath.FullName, file.Name));
        }

        public static void AddEncFile(string[] param, string password)
        {
            string fileName = new FileInfo(param[0]).Name;
            new FileInfo(param[0]).CopyTo(Path.Combine(param[1], fileName));
            new FileInfo(param[0]).CopyTo(Path.Combine(param[2], fileName));


            DeviceManager dm = new DeviceManager();
            string dirPath = param[2] + "\\";
            bool encFileName = CipherUtils.ReadConfigFile(dirPath).Split(",")[1].Equals("True") ? true : false;
            CipherUtils.EncryptFile(Path.Combine(param[2], fileName), password, encFileName);
        }

        public static void AddEncDirectory(string[] param, string password)
        {
            DeviceManager dm = new DeviceManager();
            string dirName = new DirectoryInfo(param[0]).Name;
            
            Directory.CreateDirectory(param[1]+dirName);
            CopyFiles(new DirectoryInfo(param[0]), new DirectoryInfo(param[1] + dirName), dm);
            Directory.CreateDirectory(param[2] + dirName);
            CopyFiles(new DirectoryInfo(param[0]), new DirectoryInfo(param[2] + dirName), dm);

            string dirPath = param[2] + "\\";
            bool encFileName = CipherUtils.ReadConfigFile(dirPath).Split(",")[1].Equals("True") ? true : false;
            CipherUtils.EncryptDirectory(dm, Path.Combine(param[2], dirName), password, encFileName);
            CipherUtils.EncryptDirectoryHeader(Path.Combine(param[2], dirName), password);
        }
    }

    public class UsbDevice
    {
        public UsbDevice(string logicalDiskLetter, string logicalDiskName)
        {
            this.LogicalDiskLetter = logicalDiskLetter;
            this.LogicalDiskName = logicalDiskName;
        }
        public string LogicalDiskLetter { get; private set; }
        public string LogicalDiskName { get; private set; }
    }
}
