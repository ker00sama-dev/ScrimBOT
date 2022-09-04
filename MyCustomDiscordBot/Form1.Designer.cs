using System;

namespace MyCustomDiscordBot.MyCustomDiscordBot
{
    partial class DiscordBOTGaming
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DiscordBOTGaming));
            this.button1 = new System.Windows.Forms.Button();
            this.Debug = new System.Windows.Forms.Label();
            this.Token2 = new System.Windows.Forms.TextBox();
            this.Conncet = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.DBConnectionString = new System.Windows.Forms.TextBox();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(118, 120);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(172, 23);
            this.button1.TabIndex = 8;
            this.button1.Text = "Exit";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // Debug
            // 
            this.Debug.AutoSize = true;
            this.Debug.Location = new System.Drawing.Point(105, 146);
            this.Debug.Name = "Debug";
            this.Debug.Size = new System.Drawing.Size(204, 15);
            this.Debug.TabIndex = 6;
            this.Debug.Text = "Press Ctrl+C to shut down at Console";
            this.Debug.Visible = false;
            this.Debug.Click += new System.EventHandler(this.Debug_Click);
            // 
            // Token2
            // 
            this.Token2.Location = new System.Drawing.Point(7, 18);
            this.Token2.Name = "Token2";
            this.Token2.PlaceholderText = "Token";
            this.Token2.Size = new System.Drawing.Size(385, 23);
            this.Token2.TabIndex = 5;
            this.Token2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.Token2.TextChanged += new System.EventHandler(this.Token2_TextChanged);
            // 
            // Conncet
            // 
            this.Conncet.Location = new System.Drawing.Point(118, 90);
            this.Conncet.Name = "Conncet";
            this.Conncet.Size = new System.Drawing.Size(172, 25);
            this.Conncet.TabIndex = 4;
            this.Conncet.Text = "Connect";
            this.Conncet.UseVisualStyleBackColor = true;
            this.Conncet.Click += new System.EventHandler(this.Conncet_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.DBConnectionString);
            this.groupBox3.Controls.Add(this.button1);
            this.groupBox3.Controls.Add(this.Conncet);
            this.groupBox3.Controls.Add(this.Debug);
            this.groupBox3.Controls.Add(this.Token2);
            this.groupBox3.Location = new System.Drawing.Point(3, 3);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(405, 171);
            this.groupBox3.TabIndex = 11;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Configuration";
            this.groupBox3.Enter += new System.EventHandler(this.groupBox3_Enter);
            // 
            // DBConnectionString
            // 
            this.DBConnectionString.Location = new System.Drawing.Point(7, 47);
            this.DBConnectionString.Name = "DBConnectionString";
            this.DBConnectionString.PlaceholderText = "DBConnectionString";
            this.DBConnectionString.Size = new System.Drawing.Size(385, 23);
            this.DBConnectionString.TabIndex = 9;
            this.DBConnectionString.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.DBConnectionString.TextChanged += new System.EventHandler(this.DBConnectionString_TextChanged);
            // 
            // DiscordBOTGaming
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoValidate = System.Windows.Forms.AutoValidate.EnableAllowFocusChange;
            this.ClientSize = new System.Drawing.Size(412, 180);
            this.ControlBox = false;
            this.Controls.Add(this.groupBox3);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "DiscordBOTGaming";
            this.Text = "ScrimBOT ( PUG BOT )";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);

        }

        private void token_TextChanged(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        #endregion
        private System.Windows.Forms.Button Conncet;
        private System.Windows.Forms.TextBox Token2;
        private System.Windows.Forms.Label Debug;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox DBConnectionString;
    }
}