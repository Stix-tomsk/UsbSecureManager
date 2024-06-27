using System.Diagnostics;

namespace UsbSecureManager

{
    public partial class MainForm : Form
    {
        public static MainForm Instance;
        private DeviceManager dm;
        public MainForm()
        {
            InitializeComponent();
            Instance = this;
            this.BackColor = Color.FromArgb(246, 248, 250);
            filesPanel.BackColor = Color.White;

            dm = new DeviceManager();
            dm.UpdateDeviceList();
            FillDeviceList();
        }


        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            string docPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            if (Directory.Exists(docPath + "\\Usb Secure Manager\\tmp"))
                Directory.Delete(docPath + "\\Usb Secure Manager\\tmp", true);
        }

        private void FillDeviceList()
        {
            foreach (var device in dm.DeviceList)
            {
                string deviceName = $"{device.LogicalDiskName}({device.LogicalDiskLetter})";
                Button deviceButton = CreateDeviceButton(deviceName, device);
                devicePanel.Controls.Add(deviceButton);
            }
        }

        private Button CreateDeviceButton(string deviceName, UsbDevice device)
        {
            Button deviceButton = new Button();
            if (deviceName.Length > 13) deviceButton.Text = $"{deviceName.Substring(0, 11)}...";
            else deviceButton.Text = deviceName;

            deviceButton.Font = new Font(deviceButton.Font.FontFamily, 10);
            deviceButton.TextImageRelation = TextImageRelation.ImageBeforeText;
            deviceButton.Size = new Size(150, 50);
            new ToolTip().SetToolTip(deviceButton, deviceName);

            ImageList imageList = new ImageList();
            imageList.ImageSize = new Size(38, 38);
            if (DeviceManager.IsEncrypted(device))
                imageList.Images.Add(Properties.Resources.usbLockedImage);
            else
                imageList.Images.Add(Properties.Resources.usbImage);

            deviceButton.Image = imageList.Images[0];
            deviceButton.ImageAlign = ContentAlignment.MiddleLeft;

            deviceButton.Click += new EventHandler((object s, EventArgs e) =>
            {
                OpenDevice(device);
            });

            return deviceButton;
        }

        public void ChangeDeviceButtonIcon(string deviceName, bool encIcon)
        {
            foreach (Control button in devicePanel.Controls)
            {
                if (button.Text.Equals(deviceName))
                {
                    ImageList imageList = new ImageList();
                    imageList.ImageSize = new Size(38, 38);
                    if (encIcon)
                        imageList.Images.Add(Properties.Resources.usbLockedImage);
                    else
                        imageList.Images.Add(Properties.Resources.usbImage);
                    ((Button)button).Image = imageList.Images[0];
                    return;
                }
            }
        }

        public void OpenDevice(UsbDevice device, string dirPath = null)
        {
            filesPanel.Controls.Clear();
            filePathTextBox.Text = $"{device.LogicalDiskLetter}\\";
            string devicePath = $"{device.LogicalDiskLetter}\\";
            dm.CurrentDevice = device;

            if (dirPath != null)
            {
                OpenDirectory(dirPath);
                return;
            }

            if (DeviceManager.IsEncrypted(device))
            {
                DeviceManager.TemporaryPath = null;
                InputForm.GetInstance(dm.CurrentDevice, false, true).ShowForm();
                return;
            }

            OpenDirectory(devicePath);
        }

        private DoubleClickButton CreateFileButton(string fileName, bool dir)
        {
            DoubleClickButton fileButton = new DoubleClickButton();
            Image buttonImage = dir ? Properties.Resources.folder : Properties.Resources.document;

            fileButton.Font = new Font(fileButton.Font.FontFamily, 9);
            fileButton.Text = GetButtonText(fileName, fileButton.Font);
            fileButton.TextImageRelation = TextImageRelation.ImageBeforeText;
            fileButton.Size = new Size(120, 35);
            fileButton.Padding = new Padding(3, 0, 0, 0);
            new ToolTip().SetToolTip(fileButton, fileName);

            ImageList imageList = new ImageList();
            imageList.ImageSize = new Size(20, 20);
            imageList.Images.Add(buttonImage);
            fileButton.Image = imageList.Images[0];
            fileButton.ImageAlign = ContentAlignment.MiddleLeft;

            if (dir)
            {
                fileButton.DoubleClick += new EventHandler((object s, EventArgs e) =>
                {
                    filePathTextBox.Text += $"{fileName}\\";
                    if (DeviceManager.TemporaryPath != null)
                    {
                        DeviceManager.TemporaryPath += $"{fileName}\\";
                        OpenDirectory(DeviceManager.TemporaryPath);
                        return;
                    }
                    OpenDirectory(filePathTextBox.Text);
                });
            }
            else
            {
                fileButton.DoubleClick += new EventHandler((object s, EventArgs e) =>
                {
                    string fileDir = DeviceManager.TemporaryPath != null ?
                        DeviceManager.TemporaryPath : filePathTextBox.Text;
                    new Process
                    {
                        StartInfo = new ProcessStartInfo($"{fileDir}{fileName}")
                        { UseShellExecute = true }
                    }.Start();
                });
            }

            return fileButton;
        }

        private string GetButtonText(string fileName, Font font)
        {
            string buttonText = fileName;
            while (TextRenderer.MeasureText(buttonText, font).Width > 80)
            {
                buttonText = buttonText.Substring(0, buttonText.Length - 2);
            }

            if (!buttonText.Equals(fileName))
                buttonText += "...";
            return buttonText;
        }

        private void OpenDirectory(string dirPath)
        {
            filesPanel.Controls.Clear();
            foreach (var file in dm.GetDirectories(dirPath))
            {
                DoubleClickButton fileButton = CreateFileButton(file.Name, true);
                filesPanel.Controls.Add(fileButton);
            }

            foreach (var file in dm.GetFiles(dirPath))
            {
                DoubleClickButton fileButton = CreateFileButton(file.Name, false);
                filesPanel.Controls.Add(fileButton);
            }
        }

        private void updateButton_Click(object sender, EventArgs e)
        {
            ManualResetEvent dialogLoadedFlag = new ManualResetEvent(false);
            (new Thread(() =>
            {
                Form waitDialog = new Form()
                {
                    Name = "WaitForm",
                    Text = "Update...",
                    ControlBox = false,
                    FormBorderStyle = FormBorderStyle.FixedDialog,
                    StartPosition = FormStartPosition.CenterParent,
                    Width = 240,
                    Height = 50,
                    Enabled = true
                };
                ProgressBar ScrollingBar = new ProgressBar()
                {
                    Style = ProgressBarStyle.Marquee,
                    Parent = waitDialog,
                    Dock = DockStyle.Fill,
                    Enabled = true
                };
                waitDialog.Load += new EventHandler((x, y) =>
                {
                    dialogLoadedFlag.Set();
                });
                waitDialog.Shown += new EventHandler((x, y) =>
                {
                    (new Thread(() =>
                    {
                        dm.UpdateDeviceList();
                        this.Invoke((MethodInvoker)(() => waitDialog.Close()));
                    })).Start();
                });
                this.Invoke((MethodInvoker)(() => waitDialog.ShowDialog(this)));
            })).Start();

            while (dialogLoadedFlag.WaitOne(100, true) == false)
                Application.DoEvents();

            devicePanel.Controls.Clear();
            filesPanel.Controls.Clear();
            filePathTextBox.Text = String.Empty;
            FillDeviceList();
        }

        private void fileBackButton_Click(object sender, EventArgs e)
        {
            if (filePathTextBox.Text.Length < 4) return;
            int slashIndex = filePathTextBox.Text.Substring(0, filePathTextBox.Text.Length - 2).LastIndexOf("\\");
            filePathTextBox.Text = $"{filePathTextBox.Text.Substring(0, slashIndex)}\\";

            if (DeviceManager.TemporaryPath != null)
            {
                slashIndex = DeviceManager.TemporaryPath.Substring(0, DeviceManager.TemporaryPath.Length - 2).LastIndexOf("\\");
                DeviceManager.TemporaryPath = DeviceManager.TemporaryPath.Substring(0, slashIndex) + "\\";
                OpenDirectory(DeviceManager.TemporaryPath);
                return;
            }
            OpenDirectory(filePathTextBox.Text);
        }

        private void encryptButton_Click(object sender, EventArgs e)
        {
            if (dm.CurrentDevice == null)
            {
                ShowWarning("Please select a device");
                return;
            }
            else if (DeviceManager.IsEncrypted(dm.CurrentDevice))
            {
                ShowWarning("Device is already encrypted");
                return;
            }
            else
                InputForm.GetInstance(dm.CurrentDevice, true).ShowForm();
        }

        private void decryptButton_Click(object sender, EventArgs e)
        {
            if (dm.CurrentDevice == null)
            {
                ShowWarning("Please select a device");
                return;
            }
            else if (!DeviceManager.IsEncrypted(dm.CurrentDevice))
            {
                ShowWarning("Device is not encrypted yet");
                return;
            }
            InputForm.GetInstance(dm.CurrentDevice, false).ShowForm();
            string docPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            if (Directory.Exists(docPath + "\\Usb Secure Manager\\tmp"))
                Directory.Delete(docPath + "\\Usb Secure Manager\\tmp", true);
        }

        private void ShowWarning(string message)
        {
            MessageBox.Show(message, "Warning",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        private void filesPanel_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false) == true)
            {
                e.Effect = DragDropEffects.All;
            }
        }

        private void filesPanel_DragDrop(object sender, DragEventArgs e)
        {
            if (dm.CurrentDevice == null)
            {
                ShowWarning("Please select a device");
                return;
            }

            string curDirectory;
            bool deviceEncrypted = DeviceManager.IsEncrypted(dm.CurrentDevice);
            if (DeviceManager.TemporaryPath != null)
                curDirectory = DeviceManager.TemporaryPath;
            else curDirectory = filePathTextBox.Text;
            
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

            foreach (string file in files)
            {
                if (File.Exists(file))
                {
                    if (deviceEncrypted)
                    {
                        string tmpPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal) + 
                            "\\Usb Secure Manager\\tmp\\";
                        if (File.Exists(tmpPath + new FileInfo(file).Name))
                        {
                            ShowWarning("File is already exists");
                            continue;
                        }

                        string[] encryptionParam =
                        [
                            file,
                            tmpPath,
                            dm.CurrentDevice.LogicalDiskLetter,
                            "file",
                        ];

                        InputForm.GetInstance(dm.CurrentDevice, false, false, encryptionParam).ShowForm();
                        continue;
                    }
                    else
                    {
                        string fileName = new FileInfo(file).Name;
                        string fullPath = curDirectory + fileName;
                        if (File.Exists(fullPath))
                        {
                            ShowWarning("File is already exists");
                            continue;
                        }

                        new FileInfo(file).CopyTo(Path.Combine(curDirectory, fileName));
                        DoubleClickButton fileButton = CreateFileButton(fileName, false);
                        filesPanel.Controls.Add(fileButton);
                    }
                }
                else if (Directory.Exists(file))
                {
                    if (deviceEncrypted)
                    {
                        string tmpPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal) +
                            "\\Usb Secure Manager\\tmp\\";
                        if (Directory.Exists(tmpPath + new DirectoryInfo(file).Name))
                        {
                            ShowWarning("Directory is already exists");
                            continue;
                        }

                        string[] encryptionParam =
                        [
                            file,
                            tmpPath,
                            dm.CurrentDevice.LogicalDiskLetter,
                            "dir",
                        ];
                        InputForm.GetInstance(dm.CurrentDevice, false, false, encryptionParam).ShowForm();
                        continue;
                    }
                    else
                    {
                        string dirName = new DirectoryInfo(file).Name;
                        string fullPath = curDirectory + dirName;
                        if (Directory.Exists(fullPath))
                        {
                            ShowWarning("Directory is already exists");
                            continue;
                        }

                        Directory.CreateDirectory(fullPath);
                        DeviceManager.CopyFiles(new DirectoryInfo(file), new DirectoryInfo(fullPath), dm);
                        DoubleClickButton fileButton = CreateFileButton(dirName, true);
                        filesPanel.Controls.Add(fileButton);
                    }
                }
                else continue;
            }
        }
    }

    public class DoubleClickButton : Button
    {
        public DoubleClickButton()
        {
            SetStyle(ControlStyles.StandardClick | ControlStyles.StandardDoubleClick, true);
        }
    }
}
