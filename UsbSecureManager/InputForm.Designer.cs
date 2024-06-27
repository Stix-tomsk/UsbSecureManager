namespace UsbSecureManager
{
    partial class InputForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(InputForm));
            passwordTextBox = new TextBox();
            sendButton = new Button();
            cancelButton = new Button();
            label1 = new Label();
            checkBox = new CheckBox();
            passConfirmTextBox = new TextBox();
            label2 = new Label();
            deviceNameLabel = new Label();
            SuspendLayout();
            // 
            // passwordTextBox
            // 
            passwordTextBox.Location = new Point(30, 51);
            passwordTextBox.MaxLength = 32;
            passwordTextBox.Name = "passwordTextBox";
            passwordTextBox.PasswordChar = '*';
            passwordTextBox.Size = new Size(208, 23);
            passwordTextBox.TabIndex = 0;
            // 
            // sendButton
            // 
            sendButton.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 204);
            sendButton.Location = new Point(30, 161);
            sendButton.Name = "sendButton";
            sendButton.Size = new Size(100, 25);
            sendButton.TabIndex = 6;
            sendButton.Text = "Encrypt";
            sendButton.UseVisualStyleBackColor = true;
            sendButton.Click += sendButton_Click;
            // 
            // cancelButton
            // 
            cancelButton.Font = new Font("Segoe UI", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 204);
            cancelButton.Location = new Point(138, 161);
            cancelButton.Name = "cancelButton";
            cancelButton.Size = new Size(100, 25);
            cancelButton.TabIndex = 6;
            cancelButton.Text = "Cancel";
            cancelButton.UseVisualStyleBackColor = true;
            cancelButton.Click += cancelButton_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(30, 33);
            label1.Name = "label1";
            label1.Size = new Size(90, 15);
            label1.TabIndex = 7;
            label1.Text = "Enter password:";
            // 
            // checkBox
            // 
            checkBox.AutoSize = true;
            checkBox.Location = new Point(30, 136);
            checkBox.Name = "checkBox";
            checkBox.Size = new Size(123, 19);
            checkBox.TabIndex = 8;
            checkBox.Text = "Encrypt file names";
            checkBox.UseVisualStyleBackColor = true;
            // 
            // passConfirmTextBox
            // 
            passConfirmTextBox.Location = new Point(30, 95);
            passConfirmTextBox.MaxLength = 32;
            passConfirmTextBox.Name = "passConfirmTextBox";
            passConfirmTextBox.PasswordChar = '*';
            passConfirmTextBox.Size = new Size(208, 23);
            passConfirmTextBox.TabIndex = 1;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(30, 77);
            label2.Name = "label2";
            label2.Size = new Size(107, 15);
            label2.TabIndex = 7;
            label2.Text = "Confirm password:";
            // 
            // deviceNameLabel
            // 
            deviceNameLabel.AutoSize = true;
            deviceNameLabel.Location = new Point(30, 9);
            deviceNameLabel.Name = "deviceNameLabel";
            deviceNameLabel.Size = new Size(80, 15);
            deviceNameLabel.TabIndex = 7;
            deviceNameLabel.Text = "Device Name:";
            // 
            // InputForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(273, 198);
            Controls.Add(checkBox);
            Controls.Add(label2);
            Controls.Add(deviceNameLabel);
            Controls.Add(label1);
            Controls.Add(cancelButton);
            Controls.Add(sendButton);
            Controls.Add(passConfirmTextBox);
            Controls.Add(passwordTextBox);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "InputForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Encrypt device";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox passwordTextBox;
        private Button sendButton;
        private Button cancelButton;
        private Label label1;
        private CheckBox checkBox;
        private TextBox passConfirmTextBox;
        private Label label2;
        private Label deviceNameLabel;
    }
}