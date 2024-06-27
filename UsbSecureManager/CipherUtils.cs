using System.Security.Cryptography;
using System.Text;

namespace UsbSecureManager
{
    public class CipherUtils
    {
        static public void EncryptDevice(string logicalLetter, string password, bool encryptFileName)
        {
            DeviceManager dm = new DeviceManager();
            string dirPath = logicalLetter + "\\";
            EncryptDirectory(dm, dirPath, password, encryptFileName);
            CreateConfigFile(dirPath, GetHash(password), encryptFileName);
        }

        static public void DecryptDevice(string logicalLetter, string password)
        {
            DeviceManager dm = new DeviceManager();
            string dirPath = logicalLetter + "\\";
            string[] cipherConfig = ReadConfigFile(dirPath).Split(",");

            string configPassHash = cipherConfig[0];
            bool encryptFileName = cipherConfig[1].Equals("True") ? true : false;
            
            if (!configPassHash.Equals(GetHash(password))) return;
            DecryptDirectory(dm, dirPath, password, encryptFileName);

            File.Delete($"{dirPath}\\config");
        }

        static public void EncryptDirectory(DeviceManager dm, string folderPath, string password, bool encryptFileName)
        {
            foreach (var file in dm.GetFiles(folderPath))
                EncryptFile(file.FullName, password, encryptFileName);

            foreach (var dir in dm.GetDirectories(folderPath))
            {
                EncryptDirectory(dm, dir.FullName, password, encryptFileName);
                if (encryptFileName) EncryptDirectoryHeader(dir.FullName, password);
            }
        }

        static private void DecryptDirectory(DeviceManager dm, string folderPath, string password, bool encryptFileName)
        {
            foreach (var file in dm.GetFiles(folderPath))
                DecryptFile(file.FullName, password, encryptFileName);

            foreach (var dir in dm.GetDirectories(folderPath))
            {
                DecryptDirectory(dm, dir.FullName, password, encryptFileName);
                if (encryptFileName) DecryptDirectoryHeader(dir.FullName, password);
            }
        }

        static public void EncryptFile(string filePath, string password, bool encryptFileName)
        {
            try
            {
                string fullPath = filePath;
                if (encryptFileName)
                    fullPath = GetEncryptedFilePath(filePath, password).Replace("/", "!sl");

                byte[] encryptedData = GetEncryptedData(File.ReadAllBytes(filePath), password);
                File.WriteAllBytes(fullPath, encryptedData);

                if (encryptFileName)
                    File.Delete(filePath);
            }
            catch (Exception) { return; }
        }

        static public void DecryptFile(string filePath, string password, bool encryptFileName)
        {
            try
            {
                string fullPath = filePath;
                if (encryptFileName)
                    fullPath = GetDecryptedFilePath(filePath, password);

                byte[] decryptedData = GetDecryptedData(File.ReadAllBytes(filePath), password);
                File.WriteAllBytes(fullPath, decryptedData);

                if (encryptFileName)
                    File.Delete(filePath);
            }
            catch (Exception) { return; }
}

        static private string GetEncryptedFilePath(string filePath, string password)
        {
            try
            {
                FileInfo fileInfo = new FileInfo(filePath);
                string encryptedFileName = Convert.ToBase64String(
                    GetEncryptedData(Encoding.UTF8.GetBytes(fileInfo.Name), password)
                    ) + ".enc";
                return filePath.Replace(fileInfo.Name, encryptedFileName);
            }
            catch (Exception) { return filePath; }
        }

        static private string GetDecryptedFilePath(string filePath, string password)
        {
            try
            {
                FileInfo fileInfo = new FileInfo(filePath);
                byte[] srcFileName = Convert.FromBase64String(
                    fileInfo.Name.Substring(0, fileInfo.Name.Length - 4).Replace("!sl", "/")
                    );
                string decryptedFileName = Encoding.UTF8.GetString(GetDecryptedData(srcFileName, password));
                return filePath.Replace(fileInfo.Name, decryptedFileName);
            }
            catch (Exception) { return filePath; }
        }

        static public void EncryptDirectoryHeader(string dirPath, string password)
        {
            try
            {
                DirectoryInfo dirInfo = new DirectoryInfo(dirPath);
                byte[] encryptedDirName = GetEncryptedData(Encoding.UTF8.GetBytes(dirInfo.Name), password);
                string newDirName = Convert.ToBase64String(encryptedDirName).Replace("/", "!sl");
                dirInfo.MoveTo(Path.Combine(dirInfo.Parent.FullName, newDirName));
            }
            catch (Exception) { return; }
        }

        static private void DecryptDirectoryHeader(string dirPath, string password)
        {
            try
            {
                DirectoryInfo dirInfo = new DirectoryInfo(dirPath);
                string scrDirName = dirInfo.Name.Replace("!sl", "/");
                byte[] decryptedDirName = GetDecryptedData(Convert.FromBase64String(scrDirName), password);
                dirInfo.MoveTo(Path.Combine(dirInfo.Parent.FullName, Encoding.UTF8.GetString(decryptedDirName)));
            }
            catch (Exception) { return; }
        }

        static private byte[] GetEncryptedData(byte[] plainBytes, string password)
        {
            byte[][] cipherParams = GetCipherParameters(password);
            byte[] key = cipherParams[0];
            byte[] iv = cipherParams[1];
            return Encrypt(plainBytes, key, iv);
        }

        static private byte[] GetDecryptedData(byte[] cipherBytes, string password)
        {
            byte[][] cipherParams = GetCipherParameters(password);
            byte[] key = cipherParams[0];
            byte[] iv = cipherParams[1];
            return Decrypt(cipherBytes, key, iv);
        }

        static private byte[][] GetCipherParameters(string password)
        {
            byte[] key = new byte[32];

            byte[] passBytes = Encoding.ASCII.GetBytes(password);
            for (int i = 0; i < passBytes.Length; i++)
                key[i] = passBytes[i];

            byte[] iv = Encoding.ASCII.GetBytes(GetHash(password).Substring(0, 16));

            return [key, iv];
        }

        static public byte[] Encrypt(byte[] plainBytes, byte[] key, byte[] iv)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = key;
                aesAlg.IV = iv;
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
                byte[] encryptedBytes;
                using (var msEncrypt = new MemoryStream())
                {
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        csEncrypt.Write(plainBytes, 0, plainBytes.Length);
                    }
                    encryptedBytes = msEncrypt.ToArray();
                }
                return encryptedBytes;
            }
        }

        static public byte[] Decrypt(byte[] cipherBytes, byte[] key, byte[] iv)
        {
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = key;
                aesAlg.IV = iv;
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
                byte[] decryptedBytes;
                using (var msDecrypt = new MemoryStream(cipherBytes))
                {
                    using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (var msPlain = new MemoryStream())
                        {
                            csDecrypt.CopyTo(msPlain);
                            decryptedBytes = msPlain.ToArray();
                        }
                    }
                }
                return decryptedBytes;
            }
        }

        static public string GetHash(string text)
        {
            var textBytes = Encoding.UTF8.GetBytes(text);
            var textHash = SHA256.HashData(textBytes);
            return Convert.ToHexString(textHash);
        }

        static private void CreateConfigFile(string filePath, string passHash, bool encFileName)
        {
            string fullPath = $"{filePath}\\config";

            try
            {
                using (FileStream fs = File.Create(fullPath))
                {
                    string data = $"Encrypted by Usb Secure Manager\n" +
                        $"Password hash: {passHash}\n" +
                        $"Encrypted file names: {encFileName}";
                    byte[] info = new UTF8Encoding(true).GetBytes(data);
                    fs.Write(info, 0, info.Length);
                }

                File.SetAttributes(fullPath, FileAttributes.ReadOnly);
                File.SetAttributes(fullPath, FileAttributes.Hidden);
            }

            catch (Exception) { }
        }

        static public string ReadConfigFile(string filePath)
        {
            string fullPath = $"{filePath}\\config";
            string configData = String.Empty;
            using (StreamReader sr = File.OpenText(fullPath))
            {
                string s = "";
                while ((s = sr.ReadLine()) != null)
                {
                    if (s.StartsWith("Pas")) configData = s.Substring(s.IndexOf(":") + 2);
                    if (s.StartsWith("Enc")) configData += "," + s.Substring(s.IndexOf(":") + 2);
                }
            }

            return configData;
        }
    }
}
