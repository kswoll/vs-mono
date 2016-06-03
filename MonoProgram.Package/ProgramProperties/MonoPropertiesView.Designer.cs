using System.Windows.Forms;

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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.GroupBox groupBox1;
            System.Windows.Forms.Label label4;
            System.Windows.Forms.Label label3;
            System.Windows.Forms.Label label2;
            System.Windows.Forms.Label label1;
            System.Windows.Forms.GroupBox groupBox3;
            System.Windows.Forms.Label label14;
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MonoPropertiesView));
            System.Windows.Forms.Label label11;
            System.Windows.Forms.Label label10;
            System.Windows.Forms.Label label9;
            System.Windows.Forms.Label label8;
            System.Windows.Forms.Label label7;
            System.Windows.Forms.Label label6;
            System.Windows.Forms.Label buildServerLabel;
            this.destinationTextBox = new System.Windows.Forms.TextBox();
            this.destinationLabel = new System.Windows.Forms.Label();
            this.passwordTextBox = new System.Windows.Forms.TextBox();
            this.passwordLabel = new System.Windows.Forms.Label();
            this.usernameTextBox = new System.Windows.Forms.TextBox();
            this.usernameLabel = new System.Windows.Forms.Label();
            this.hostTextBox = new System.Windows.Forms.TextBox();
            this.hostLabel = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.buildPassword = new System.Windows.Forms.TextBox();
            this.buildUsername = new System.Windows.Forms.TextBox();
            this.buildRootLabel = new System.Windows.Forms.Label();
            this.buildFolderTextBox = new System.Windows.Forms.TextBox();
            this.buildServerTextBox = new System.Windows.Forms.TextBox();
            this.buildRootTextBox = new System.Windows.Forms.TextBox();
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            groupBox1 = new System.Windows.Forms.GroupBox();
            label4 = new System.Windows.Forms.Label();
            label3 = new System.Windows.Forms.Label();
            label2 = new System.Windows.Forms.Label();
            label1 = new System.Windows.Forms.Label();
            groupBox3 = new System.Windows.Forms.GroupBox();
            label14 = new System.Windows.Forms.Label();
            label11 = new System.Windows.Forms.Label();
            label10 = new System.Windows.Forms.Label();
            label9 = new System.Windows.Forms.Label();
            label8 = new System.Windows.Forms.Label();
            label7 = new System.Windows.Forms.Label();
            label6 = new System.Windows.Forms.Label();
            buildServerLabel = new System.Windows.Forms.Label();
            groupBox1.SuspendLayout();
            groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(label4);
            groupBox1.Controls.Add(this.destinationTextBox);
            groupBox1.Controls.Add(this.destinationLabel);
            groupBox1.Controls.Add(label3);
            groupBox1.Controls.Add(label2);
            groupBox1.Controls.Add(label1);
            groupBox1.Controls.Add(this.passwordTextBox);
            groupBox1.Controls.Add(this.passwordLabel);
            groupBox1.Controls.Add(this.usernameTextBox);
            groupBox1.Controls.Add(this.usernameLabel);
            groupBox1.Controls.Add(this.hostTextBox);
            groupBox1.Controls.Add(this.hostLabel);
            groupBox1.Location = new System.Drawing.Point(9, 16);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new System.Drawing.Size(1260, 346);
            groupBox1.TabIndex = 24;
            groupBox1.TabStop = false;
            groupBox1.Text = "Host";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new System.Drawing.Point(541, 290);
            label4.Name = "label4";
            label4.Size = new System.Drawing.Size(615, 25);
            label4.TabIndex = 28;
            label4.Text = "The folder on the host to which the output files will be uploaded";
            // 
            // destinationTextBox
            // 
            this.destinationTextBox.Location = new System.Drawing.Point(23, 287);
            this.destinationTextBox.Name = "destinationTextBox";
            this.destinationTextBox.Size = new System.Drawing.Size(482, 31);
            this.destinationTextBox.TabIndex = 27;
            // 
            // destinationLabel
            // 
            this.destinationLabel.AutoSize = true;
            this.destinationLabel.Location = new System.Drawing.Point(18, 259);
            this.destinationLabel.Name = "destinationLabel";
            this.destinationLabel.Size = new System.Drawing.Size(267, 25);
            this.destinationLabel.TabIndex = 26;
            this.destinationLabel.Text = "Destination Folder on Host";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new System.Drawing.Point(541, 219);
            label3.Name = "label3";
            label3.Size = new System.Drawing.Size(422, 25);
            label3.TabIndex = 25;
            label3.Text = "The password to authenticate with the host";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new System.Drawing.Point(541, 145);
            label2.Name = "label2";
            label2.Size = new System.Drawing.Size(444, 25);
            label2.TabIndex = 24;
            label2.Text = "The username with which to log in to the host";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new System.Drawing.Point(541, 69);
            label1.Name = "label1";
            label1.Size = new System.Drawing.Size(512, 25);
            label1.TabIndex = 23;
            label1.Text = "The hostname of the machine that will run your code";
            // 
            // passwordTextBox
            // 
            this.passwordTextBox.Location = new System.Drawing.Point(23, 216);
            this.passwordTextBox.Name = "passwordTextBox";
            this.passwordTextBox.PasswordChar = '#';
            this.passwordTextBox.Size = new System.Drawing.Size(482, 31);
            this.passwordTextBox.TabIndex = 22;
            // 
            // passwordLabel
            // 
            this.passwordLabel.AutoSize = true;
            this.passwordLabel.Location = new System.Drawing.Point(18, 188);
            this.passwordLabel.Name = "passwordLabel";
            this.passwordLabel.Size = new System.Drawing.Size(106, 25);
            this.passwordLabel.TabIndex = 21;
            this.passwordLabel.Text = "Password";
            // 
            // usernameTextBox
            // 
            this.usernameTextBox.Location = new System.Drawing.Point(23, 142);
            this.usernameTextBox.Name = "usernameTextBox";
            this.usernameTextBox.Size = new System.Drawing.Size(482, 31);
            this.usernameTextBox.TabIndex = 20;
            // 
            // usernameLabel
            // 
            this.usernameLabel.AutoSize = true;
            this.usernameLabel.Location = new System.Drawing.Point(18, 114);
            this.usernameLabel.Name = "usernameLabel";
            this.usernameLabel.Size = new System.Drawing.Size(110, 25);
            this.usernameLabel.TabIndex = 19;
            this.usernameLabel.Text = "Username";
            // 
            // hostTextBox
            // 
            this.hostTextBox.Location = new System.Drawing.Point(23, 66);
            this.hostTextBox.Name = "hostTextBox";
            this.hostTextBox.Size = new System.Drawing.Size(482, 31);
            this.hostTextBox.TabIndex = 18;
            // 
            // hostLabel
            // 
            this.hostLabel.AutoSize = true;
            this.hostLabel.Location = new System.Drawing.Point(18, 37);
            this.hostLabel.Name = "hostLabel";
            this.hostLabel.Size = new System.Drawing.Size(56, 25);
            this.hostLabel.TabIndex = 17;
            this.hostLabel.Text = "Host";
            // 
            // groupBox3
            // 
            groupBox3.Controls.Add(label14);
            groupBox3.Controls.Add(this.label13);
            groupBox3.Controls.Add(this.label12);
            groupBox3.Controls.Add(this.buildPassword);
            groupBox3.Controls.Add(label11);
            groupBox3.Controls.Add(this.buildUsername);
            groupBox3.Controls.Add(label10);
            groupBox3.Controls.Add(this.buildRootLabel);
            groupBox3.Controls.Add(label9);
            groupBox3.Controls.Add(this.buildFolderTextBox);
            groupBox3.Controls.Add(label8);
            groupBox3.Controls.Add(label7);
            groupBox3.Controls.Add(label6);
            groupBox3.Controls.Add(this.buildServerTextBox);
            groupBox3.Controls.Add(buildServerLabel);
            groupBox3.Controls.Add(this.buildRootTextBox);
            groupBox3.Location = new System.Drawing.Point(9, 380);
            groupBox3.Name = "groupBox3";
            groupBox3.Size = new System.Drawing.Size(1260, 520);
            groupBox3.TabIndex = 26;
            groupBox3.TabStop = false;
            groupBox3.Text = "Build Host";
            // 
            // label14
            // 
            label14.AutoSize = true;
            label14.Location = new System.Drawing.Point(18, 41);
            label14.Name = "label14";
            label14.Size = new System.Drawing.Size(1211, 75);
            label14.TabIndex = 41;
            label14.Text = resources.GetString("label14.Text");
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(541, 469);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(312, 25);
            this.label13.TabIndex = 40;
            this.label13.Text = "The password on the build host";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(541, 397);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(315, 25);
            this.label12.TabIndex = 39;
            this.label12.Text = "The username on the build host";
            // 
            // buildPassword
            // 
            this.buildPassword.Location = new System.Drawing.Point(23, 466);
            this.buildPassword.Name = "buildPassword";
            this.buildPassword.PasswordChar = '#';
            this.buildPassword.Size = new System.Drawing.Size(482, 31);
            this.buildPassword.TabIndex = 38;
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.Location = new System.Drawing.Point(18, 438);
            label11.Name = "label11";
            label11.Size = new System.Drawing.Size(106, 25);
            label11.TabIndex = 37;
            label11.Text = "Password";
            // 
            // buildUsername
            // 
            this.buildUsername.Location = new System.Drawing.Point(24, 394);
            this.buildUsername.Name = "buildUsername";
            this.buildUsername.Size = new System.Drawing.Size(481, 31);
            this.buildUsername.TabIndex = 36;
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Location = new System.Drawing.Point(18, 366);
            label10.Name = "label10";
            label10.Size = new System.Drawing.Size(110, 25);
            label10.TabIndex = 35;
            label10.Text = "Username";
            // 
            // buildRootLabel
            // 
            this.buildRootLabel.AutoSize = true;
            this.buildRootLabel.Location = new System.Drawing.Point(18, 148);
            this.buildRootLabel.Name = "buildRootLabel";
            this.buildRootLabel.Size = new System.Drawing.Size(111, 25);
            this.buildRootLabel.TabIndex = 34;
            this.buildRootLabel.Text = "Build Root";
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Location = new System.Drawing.Point(541, 325);
            label9.Name = "label9";
            label9.Size = new System.Drawing.Size(668, 25);
            label9.TabIndex = 33;
            label9.Text = "The folder on the build server in which the build should be performed";
            // 
            // buildFolderTextBox
            // 
            this.buildFolderTextBox.Location = new System.Drawing.Point(23, 322);
            this.buildFolderTextBox.Name = "buildFolderTextBox";
            this.buildFolderTextBox.Size = new System.Drawing.Size(482, 31);
            this.buildFolderTextBox.TabIndex = 32;
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new System.Drawing.Point(18, 294);
            label8.Name = "label8";
            label8.Size = new System.Drawing.Size(127, 25);
            label8.TabIndex = 31;
            label8.Text = "Build Folder";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new System.Drawing.Point(541, 251);
            label7.Name = "label7";
            label7.Size = new System.Drawing.Size(381, 25);
            label7.TabIndex = 30;
            label7.Text = "The server that is performing the build.";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new System.Drawing.Point(541, 179);
            label6.Name = "label6";
            label6.Size = new System.Drawing.Size(553, 25);
            label6.TabIndex = 29;
            label6.Text = "The folder on the build server that matches Source Root.";
            // 
            // buildServerTextBox
            // 
            this.buildServerTextBox.Location = new System.Drawing.Point(23, 248);
            this.buildServerTextBox.Name = "buildServerTextBox";
            this.buildServerTextBox.Size = new System.Drawing.Size(482, 31);
            this.buildServerTextBox.TabIndex = 28;
            // 
            // buildServerLabel
            // 
            buildServerLabel.AutoSize = true;
            buildServerLabel.Location = new System.Drawing.Point(18, 220);
            buildServerLabel.Name = "buildServerLabel";
            buildServerLabel.Size = new System.Drawing.Size(56, 25);
            buildServerLabel.TabIndex = 27;
            buildServerLabel.Text = "Host";
            // 
            // buildRootTextBox
            // 
            this.buildRootTextBox.Location = new System.Drawing.Point(23, 176);
            this.buildRootTextBox.Name = "buildRootTextBox";
            this.buildRootTextBox.Size = new System.Drawing.Size(482, 31);
            this.buildRootTextBox.TabIndex = 26;
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.Text = "notifyIcon1";
            this.notifyIcon1.Visible = true;
            // 
            // MonoPropertiesView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.Controls.Add(groupBox3);
            this.Controls.Add(groupBox1);
            this.Name = "MonoPropertiesView";
            this.Size = new System.Drawing.Size(1380, 1300);
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            groupBox3.ResumeLayout(false);
            groupBox3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private TextBox passwordTextBox;
        private Label passwordLabel;
        private TextBox usernameTextBox;
        private Label usernameLabel;
        private TextBox hostTextBox;
        private Label hostLabel;
        private TextBox destinationTextBox;
        private Label destinationLabel;
        private Label buildRootLabel;
        private TextBox buildFolderTextBox;
        private TextBox buildServerTextBox;
        private TextBox buildRootTextBox;
        private TextBox buildPassword;
        private TextBox buildUsername;
        private Label label13;
        private Label label12;
    }
}
