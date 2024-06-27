namespace UsbSecureManager
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            devicePanel = new FlowLayoutPanel();
            filesPanel = new FlowLayoutPanel();
            updateButton = new Button();
            encryptButton = new Button();
            decryptButton = new Button();
            fileBackButton = new Button();
            filePathTextBox = new TextBox();
            SuspendLayout();
            // 
            // devicePanel
            // 
            devicePanel.Location = new Point(12, 12);
            devicePanel.Name = "devicePanel";
            devicePanel.Size = new Size(776, 57);
            devicePanel.TabIndex = 1;
            // 
            // filesPanel
            // 
            filesPanel.AllowDrop = true;
            filesPanel.AutoScroll = true;
            filesPanel.BorderStyle = BorderStyle.FixedSingle;
            filesPanel.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point, 204);
            filesPanel.Location = new Point(12, 104);
            filesPanel.Name = "filesPanel";
            filesPanel.Size = new Size(776, 285);
            filesPanel.TabIndex = 2;
            filesPanel.DragDrop += filesPanel_DragDrop;
            filesPanel.DragEnter += filesPanel_DragEnter;
            // 
            // updateButton
            // 
            updateButton.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 204);
            updateButton.Location = new Point(688, 403);
            updateButton.Name = "updateButton";
            updateButton.Size = new Size(100, 35);
            updateButton.TabIndex = 3;
            updateButton.Text = "Update";
            updateButton.UseVisualStyleBackColor = true;
            updateButton.Click += updateButton_Click;
            // 
            // encryptButton
            // 
            encryptButton.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 204);
            encryptButton.Location = new Point(16, 403);
            encryptButton.Name = "encryptButton";
            encryptButton.Size = new Size(100, 35);
            encryptButton.TabIndex = 4;
            encryptButton.Text = "Encrypt";
            encryptButton.UseVisualStyleBackColor = true;
            encryptButton.Click += encryptButton_Click;
            // 
            // decryptButton
            // 
            decryptButton.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 204);
            decryptButton.Location = new Point(134, 403);
            decryptButton.Name = "decryptButton";
            decryptButton.Size = new Size(100, 35);
            decryptButton.TabIndex = 5;
            decryptButton.Text = "Decrypt";
            decryptButton.UseVisualStyleBackColor = true;
            decryptButton.Click += decryptButton_Click;
            // 
            // fileBackButton
            // 
            fileBackButton.BackColor = Color.Transparent;
            fileBackButton.FlatAppearance.BorderSize = 0;
            fileBackButton.FlatStyle = FlatStyle.Flat;
            fileBackButton.Image = Properties.Resources.arrow;
            fileBackButton.ImageAlign = ContentAlignment.MiddleLeft;
            fileBackButton.Location = new Point(12, 75);
            fileBackButton.Name = "fileBackButton";
            fileBackButton.Size = new Size(25, 23);
            fileBackButton.TabIndex = 6;
            fileBackButton.UseVisualStyleBackColor = false;
            fileBackButton.Click += fileBackButton_Click;
            // 
            // filePathTextBox
            // 
            filePathTextBox.Location = new Point(47, 75);
            filePathTextBox.Name = "filePathTextBox";
            filePathTextBox.ReadOnly = true;
            filePathTextBox.Size = new Size(741, 23);
            filePathTextBox.TabIndex = 7;
            // 
            // MainForm
            // 
            AutoScaleMode = AutoScaleMode.None;
            ClientSize = new Size(800, 450);
            Controls.Add(filePathTextBox);
            Controls.Add(fileBackButton);
            Controls.Add(decryptButton);
            Controls.Add(encryptButton);
            Controls.Add(updateButton);
            Controls.Add(filesPanel);
            Controls.Add(devicePanel);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MaximumSize = new Size(816, 489);
            MinimizeBox = false;
            MinimumSize = new Size(816, 489);
            Name = "MainForm";
            SizeGripStyle = SizeGripStyle.Hide;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "USB Secure Manager";
            FormClosing += MainForm_FormClosing;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private FlowLayoutPanel devicePanel;
        private FlowLayoutPanel filesPanel;
        private Button updateButton;
        private Button encryptButton;
        private Button decryptButton;
        private Button fileBackButton;
        private TextBox filePathTextBox;
    }
}
