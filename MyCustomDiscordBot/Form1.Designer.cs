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
            this.panel1 = new System.Windows.Forms.Panel();
            this.serverid = new System.Windows.Forms.TextBox();
            this.Debug = new System.Windows.Forms.Label();
            this.Token2 = new System.Windows.Forms.TextBox();
            this.Conncet = new System.Windows.Forms.Button();
            this.perfix = new System.Windows.Forms.TextBox();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.serverid);
            this.panel1.Controls.Add(this.Debug);
            this.panel1.Controls.Add(this.Token2);
            this.panel1.Controls.Add(this.Conncet);
            this.panel1.Controls.Add(this.perfix);
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(405, 177);
            this.panel1.TabIndex = 0;
            this.panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
            // 
            // serverid
            // 
            this.serverid.Location = new System.Drawing.Point(12, 77);
            this.serverid.Name = "serverid";
            this.serverid.PlaceholderText = "Server ID";
            this.serverid.Size = new System.Drawing.Size(384, 23);
            this.serverid.TabIndex = 7;
            this.serverid.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.serverid.TextChanged += new System.EventHandler(this.serverid_TextChanged_1);

            this.serverid.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.serverid_KeyPress);
            // 
            // Debug
            // 
            this.Debug.AutoSize = true;
            this.Debug.Location = new System.Drawing.Point(108, 148);
            this.Debug.Name = "Debug";
            this.Debug.Size = new System.Drawing.Size(204, 15);
            this.Debug.TabIndex = 6;
            this.Debug.Text = "Press Ctrl+C to shut down at Console";
            this.Debug.Visible = false;
            this.Debug.Click += new System.EventHandler(this.Debug_Click);
            // 
            // Token2
            // 
            this.Token2.Location = new System.Drawing.Point(12, 19);
            this.Token2.Name = "Token2";
            this.Token2.PlaceholderText = "Token";
            this.Token2.Size = new System.Drawing.Size(385, 23);
            this.Token2.TabIndex = 5;
            this.Token2.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.Token2.TextChanged += new System.EventHandler(this.Token2_TextChanged);
            // 
            // Conncet
            // 
            this.Conncet.Location = new System.Drawing.Point(121, 119);
            this.Conncet.Name = "Conncet";
            this.Conncet.Size = new System.Drawing.Size(172, 25);
            this.Conncet.TabIndex = 4;
            this.Conncet.Text = "Connect";
            this.Conncet.UseVisualStyleBackColor = true;
            this.Conncet.Click += new System.EventHandler(this.Conncet_Click);
            // 
            // perfix
            // 
            this.perfix.Location = new System.Drawing.Point(11, 48);
            this.perfix.Name = "perfix";
            this.perfix.PlaceholderText = "Prefix";
            this.perfix.Size = new System.Drawing.Size(385, 23);
            this.perfix.TabIndex = 2;
            this.perfix.Tag = "";
            this.perfix.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.perfix.TextChanged += new System.EventHandler(this.perfix_TextChanged);
            // 
            // DiscordBOTGaming
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(409, 426);
            this.Controls.Add(this.panel1);
            this.Name = "DiscordBOTGaming";
            this.Text = "DiscordBOTGaming";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        private void token_TextChanged(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TextBox perfix;
        private System.Windows.Forms.Button Conncet;
        private System.Windows.Forms.TextBox Token2;
        private System.Windows.Forms.Label Debug;
        private System.Windows.Forms.TextBox serverid;
    }
}