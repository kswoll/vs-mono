namespace MonoProgram.Package.ProgramProperties
{
    partial class MonoPropertiesView
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.hostLabel = new System.Windows.Forms.Label();
            this.hostTextBox = new System.Windows.Forms.TextBox();
            this.usernameLabel = new System.Windows.Forms.Label();
            this.usernameTextBox = new System.Windows.Forms.TextBox();
            this.passwordLabel = new System.Windows.Forms.Label();
            this.passwordTextBox = new System.Windows.Forms.TextBox();
            this.destinationLabel = new System.Windows.Forms.Label();
            this.destinationTextBox = new System.Windows.Forms.TextBox();
            this.sourceRootLabel = new System.Windows.Forms.Label();
            this.sourceRootTextBox = new System.Windows.Forms.TextBox();
            this.buildRootLabel = new System.Windows.Forms.Label();
            this.buildRootTextBox = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // hostLabel
            // 
            this.hostLabel.AutoSize = true;
            this.hostLabel.Location = new System.Drawing.Point(4, 4);
            this.hostLabel.Name = "hostLabel";
            this.hostLabel.Size = new System.Drawing.Size(56, 25);
            this.hostLabel.TabIndex = 0;
            this.hostLabel.Text = "Host";
            // 
            // hostTextBox
            // 
            this.hostTextBox.Location = new System.Drawing.Point(9, 33);
            this.hostTextBox.Name = "hostTextBox";
            this.hostTextBox.Size = new System.Drawing.Size(482, 31);
            this.hostTextBox.TabIndex = 1;
            // 
            // usernameLabel
            // 
            this.usernameLabel.AutoSize = true;
            this.usernameLabel.Location = new System.Drawing.Point(4, 81);
            this.usernameLabel.Name = "usernameLabel";
            this.usernameLabel.Size = new System.Drawing.Size(110, 25);
            this.usernameLabel.TabIndex = 2;
            this.usernameLabel.Text = "Username";
            // 
            // usernameTextBox
            // 
            this.usernameTextBox.Location = new System.Drawing.Point(9, 109);
            this.usernameTextBox.Name = "usernameTextBox";
            this.usernameTextBox.Size = new System.Drawing.Size(482, 31);
            this.usernameTextBox.TabIndex = 3;
            // 
            // passwordLabel
            // 
            this.passwordLabel.AutoSize = true;
            this.passwordLabel.Location = new System.Drawing.Point(4, 155);
            this.passwordLabel.Name = "passwordLabel";
            this.passwordLabel.Size = new System.Drawing.Size(305, 25);
            this.passwordLabel.TabIndex = 4;
            this.passwordLabel.Text = "Password (Stored in plain text)";
            // 
            // passwordTextBox
            // 
            this.passwordTextBox.Location = new System.Drawing.Point(9, 183);
            this.passwordTextBox.Name = "passwordTextBox";
            this.passwordTextBox.PasswordChar = '#';
            this.passwordTextBox.Size = new System.Drawing.Size(482, 31);
            this.passwordTextBox.TabIndex = 5;
            // 
            // destinationLabel
            // 
            this.destinationLabel.AutoSize = true;
            this.destinationLabel.Location = new System.Drawing.Point(4, 231);
            this.destinationLabel.Name = "destinationLabel";
            this.destinationLabel.Size = new System.Drawing.Size(267, 25);
            this.destinationLabel.TabIndex = 6;
            this.destinationLabel.Text = "Destination Folder on Host";
            // 
            // destinationTextBox
            // 
            this.destinationTextBox.Location = new System.Drawing.Point(9, 259);
            this.destinationTextBox.Name = "destinationTextBox";
            this.destinationTextBox.Size = new System.Drawing.Size(482, 31);
            this.destinationTextBox.TabIndex = 7;
            // 
            // sourceRootLabel
            // 
            this.sourceRootLabel.AutoSize = true;
            this.sourceRootLabel.Location = new System.Drawing.Point(4, 305);
            this.sourceRootLabel.Name = "sourceRootLabel";
            this.sourceRootLabel.Size = new System.Drawing.Size(131, 25);
            this.sourceRootLabel.TabIndex = 8;
            this.sourceRootLabel.Text = "Source Root";
            // 
            // sourceRootTextBox
            // 
            this.sourceRootTextBox.Location = new System.Drawing.Point(9, 333);
            this.sourceRootTextBox.Name = "sourceRootTextBox";
            this.sourceRootTextBox.Size = new System.Drawing.Size(482, 31);
            this.sourceRootTextBox.TabIndex = 9;
            // 
            // buildRootLabel
            // 
            this.buildRootLabel.AutoSize = true;
            this.buildRootLabel.Location = new System.Drawing.Point(4, 377);
            this.buildRootLabel.Name = "buildRootLabel";
            this.buildRootLabel.Size = new System.Drawing.Size(111, 25);
            this.buildRootLabel.TabIndex = 10;
            this.buildRootLabel.Text = "Build Root";
            // 
            // buildRootTextBox
            // 
            this.buildRootTextBox.Location = new System.Drawing.Point(9, 405);
            this.buildRootTextBox.Name = "buildRootTextBox";
            this.buildRootTextBox.Size = new System.Drawing.Size(482, 31);
            this.buildRootTextBox.TabIndex = 11;
            // 
            // MonoPropertiesView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.buildRootTextBox);
            this.Controls.Add(this.buildRootLabel);
            this.Controls.Add(this.sourceRootTextBox);
            this.Controls.Add(this.sourceRootLabel);
            this.Controls.Add(this.destinationTextBox);
            this.Controls.Add(this.destinationLabel);
            this.Controls.Add(this.passwordTextBox);
            this.Controls.Add(this.passwordLabel);
            this.Controls.Add(this.usernameTextBox);
            this.Controls.Add(this.usernameLabel);
            this.Controls.Add(this.hostTextBox);
            this.Controls.Add(this.hostLabel);
            this.Name = "MonoPropertiesView";
            this.Size = new System.Drawing.Size(740, 624);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label hostLabel;
        private System.Windows.Forms.TextBox hostTextBox;
        private System.Windows.Forms.Label usernameLabel;
        private System.Windows.Forms.TextBox usernameTextBox;
        private System.Windows.Forms.Label passwordLabel;
        private System.Windows.Forms.TextBox passwordTextBox;
        private System.Windows.Forms.Label destinationLabel;
        private System.Windows.Forms.TextBox destinationTextBox;
        private System.Windows.Forms.Label sourceRootLabel;
        private System.Windows.Forms.TextBox sourceRootTextBox;
        private System.Windows.Forms.Label buildRootLabel;
        private System.Windows.Forms.TextBox buildRootTextBox;
    }
}
