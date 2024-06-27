namespace UsbSecureManager
{
    public partial class InputForm : Form
    {
        public static InputForm Instance;
        private UsbDevice Device;
        private bool IsEncryption;
        private bool IsTmpDecryption;
        private string[] FileEncryptionParam;

        public InputForm(UsbDevice device, bool isEncryption, bool isTmpDec = false, string[] fileEnc = null)
        {
            InitializeComponent();
            Device = device;
            IsEncryption = isEncryption;
            IsTmpDecryption = isTmpDec;
            FileEncryptionParam = fileEnc;
            deviceNameLabel.Text += $" {Device.LogicalDiskName}({Device.LogicalDiskLetter})";

            if (!IsEncryption || IsTmpDecryption || fileEnc != null)
            {
                this.Text = "Decrypt device";
                sendButton.Text = "Decrypt";
                this.Size = new Size(289, 167);
                sendButton.Location = new Point(30, 90);
                cancelButton.Location = new Point(138, 90);
                passConfirmTextBox.Visible = false;
                checkBox.Visible = false;
                label2.Visible = false;
            }
        }

        public static InputForm GetInstance(UsbDevice device, bool isEncryption, bool isTmpDec = false, string[] fileEnc = null)
        {
            if (Instance == null || Instance.IsDisposed)
                Instance = new InputForm(device, isEncryption, isTmpDec, fileEnc);

            return Instance;
        }

        public void ShowForm()
        {
            Show();
            Activate();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            Instance.Close();
        }

        private void sendButton_Click(object sender, EventArgs e)
        {
            string userPassword = passwordTextBox.Text;

            if (passwordTextBox.Text.Length < 8)
                ShowWarning("Minimal password length is 8 characters");
            else if (!IsEncryption && !DeviceManager.ConfirmPassword(Device, userPassword))
                ShowWarning("Wrong password");
            else if (IsEncryption && !passwordTextBox.Text.Equals(passConfirmTextBox.Text))
                ShowWarning("Password mismatch");
            else
            {
                string waitText = IsEncryption ? "Encrypt..." : "Decrypt...";
                if (FileEncryptionParam != null) waitText = "Encrypt...";
                bool isDecryption = !(IsTmpDecryption || IsEncryption || FileEncryptionParam!= null);
                ManualResetEvent dialogLoadedFlag = new ManualResetEvent(false);
                (new Thread(() =>
                {
                    Form waitDialog = new Form()
                    {
                        Name = "WaitForm",
                        Text = waitText,
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
                            if (isDecryption)
                                CipherUtils.DecryptDevice(Device.LogicalDiskLetter, userPassword);
                            else if (FileEncryptionParam != null)
                            {
                                if (FileEncryptionParam[3].Equals("file"))
                                    DeviceManager.AddEncFile(FileEncryptionParam, userPassword);
                                else
                                    DeviceManager.AddEncDirectory(FileEncryptionParam, userPassword);
                            }
                            else
                            {
                                if (IsEncryption)
                                    CipherUtils.EncryptDevice(Device.LogicalDiskLetter, userPassword, checkBox.Checked);
                                DeviceManager.UnlockDevice(Device.LogicalDiskLetter, userPassword);
                            }
                           
                            this.Invoke((MethodInvoker)(() => waitDialog.Close()));
                        })).Start();
                    });
                    this.Invoke((MethodInvoker)(() => waitDialog.ShowDialog(this)));
                })).Start();

                while (dialogLoadedFlag.WaitOne(100, true) == false)
                    Application.DoEvents();

                string deviceName = $"{Device.LogicalDiskName}({Device.LogicalDiskLetter})";
                MainForm.Instance.ChangeDeviceButtonIcon(deviceName, !isDecryption);
                if (isDecryption) MainForm.Instance.OpenDevice(Device);
                else
                {
                    string docPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                    DeviceManager.TemporaryPath = docPath + "\\Usb Secure Manager\\tmp\\";
                    MainForm.Instance.OpenDevice(Device, DeviceManager.TemporaryPath);
                }

                Instance.Close();
            }
        }

        private void ShowWarning(string message)
        {
            MessageBox.Show(message, "Warning",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }
}
