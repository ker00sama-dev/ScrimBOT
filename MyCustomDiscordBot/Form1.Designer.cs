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
            this.button1 = new System.Windows.Forms.Button();
            this.serverid = new System.Windows.Forms.TextBox();
            this.Debug = new System.Windows.Forms.Label();
            this.Token2 = new System.Windows.Forms.TextBox();
            this.Conncet = new System.Windows.Forms.Button();
            this.perfix = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.Endnull = new System.Windows.Forms.TextBox();
            this.Centernull = new System.Windows.Forms.TextBox();
            this.Endfull = new System.Windows.Forms.TextBox();
            this.Startnull = new System.Windows.Forms.TextBox();
            this.Centerfull = new System.Windows.Forms.TextBox();
            this.Startfull = new System.Windows.Forms.TextBox();
            this.button2 = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.button1);
            this.panel1.Controls.Add(this.serverid);
            this.panel1.Controls.Add(this.Debug);
            this.panel1.Controls.Add(this.Token2);
            this.panel1.Controls.Add(this.Conncet);
            this.panel1.Controls.Add(this.perfix);
            this.panel1.Location = new System.Drawing.Point(2, 11);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(405, 200);
            this.panel1.TabIndex = 0;
            this.panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(121, 149);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(172, 23);
            this.button1.TabIndex = 8;
            this.button1.Text = "Exit";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
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
            this.Debug.Location = new System.Drawing.Point(108, 175);
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
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 5);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(81, 15);
            this.label1.TabIndex = 9;
            this.label1.Text = "Configuration";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.button2);
            this.panel2.Controls.Add(this.label3);
            this.panel2.Controls.Add(this.Endnull);
            this.panel2.Controls.Add(this.Centernull);
            this.panel2.Controls.Add(this.Endfull);
            this.panel2.Controls.Add(this.Startnull);
            this.panel2.Controls.Add(this.Centerfull);
            this.panel2.Controls.Add(this.Startfull);
            this.panel2.Location = new System.Drawing.Point(2, 204);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(405, 167);
            this.panel2.TabIndex = 10;
            this.panel2.Paint += new System.Windows.Forms.PaintEventHandler(this.panel2_Paint);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 4);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(69, 15);
            this.label3.TabIndex = 9;
            this.label3.Text = "ProgressBar";
            // 
            // Endnull
            // 
            this.Endnull.Location = new System.Drawing.Point(224, 88);
            this.Endnull.Name = "Endnull";
            this.Endnull.PlaceholderText = "End null      #5";
            this.Endnull.Size = new System.Drawing.Size(173, 23);
            this.Endnull.TabIndex = 5;
            // 
            // Centernull
            // 
            this.Centernull.Location = new System.Drawing.Point(225, 59);
            this.Centernull.Name = "Centernull";
            this.Centernull.PlaceholderText = "Center null #4";
            this.Centernull.Size = new System.Drawing.Size(172, 23);
            this.Centernull.TabIndex = 4;
            // 
            // Endfull
            // 
            this.Endfull.Location = new System.Drawing.Point(12, 88);
            this.Endfull.Name = "Endfull";
            this.Endfull.PlaceholderText = "End full       #2";
            this.Endfull.Size = new System.Drawing.Size(172, 23);
            this.Endfull.TabIndex = 3;
            // 
            // Startnull
            // 
            this.Startnull.Location = new System.Drawing.Point(225, 30);
            this.Startnull.Name = "Startnull";
            this.Startnull.PlaceholderText = "Start null     #3";
            this.Startnull.Size = new System.Drawing.Size(172, 23);
            this.Startnull.TabIndex = 2;
            // 
            // Centerfull
            // 
            this.Centerfull.Location = new System.Drawing.Point(12, 59);
            this.Centerfull.Name = "Centerfull";
            this.Centerfull.PlaceholderText = "Center full  #1";
            this.Centerfull.Size = new System.Drawing.Size(172, 23);
            this.Centerfull.TabIndex = 1;
            this.Centerfull.TextChanged += new System.EventHandler(this.Centerfull_TextChanged);
            // 
            // Startfull
            // 
            this.Startfull.Location = new System.Drawing.Point(12, 30);
            this.Startfull.Name = "Startfull";
            this.Startfull.PlaceholderText = "Start full      #0";
            this.Startfull.Size = new System.Drawing.Size(172, 23);
            this.Startfull.TabIndex = 0;
            this.Startfull.TextChanged += new System.EventHandler(this.startfull_TextChanged);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(121, 123);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(172, 23);
            this.button2.TabIndex = 10;
            this.button2.Text = "Save";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // DiscordBOTGaming
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(409, 426);
            this.ControlBox = false;
            this.Controls.Add(this.label1);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel2);
            this.Name = "DiscordBOTGaming";
            this.Text = "DiscordBOTGaming";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

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
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.TextBox Endnull;
        private System.Windows.Forms.TextBox Centernull;
        private System.Windows.Forms.TextBox Endfull;
        private System.Windows.Forms.TextBox Startnull;
        private System.Windows.Forms.TextBox Centerfull;
        private System.Windows.Forms.TextBox Startfull;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button button2;
    }
}