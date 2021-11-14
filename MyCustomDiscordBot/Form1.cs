using System;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Extensions.Hosting;
using MyCustomDiscordBot;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;


namespace MyCustomDiscordBot.MyCustomDiscordBot
{
    public partial class DiscordBOTGaming : Form
    {
        public static Microsoft.Win32.RegistryKey XXXXX = Microsoft.Win32.Registry.CurrentUser.CreateSubKey("DiscordBOT");

        public static char ConfigPerfix = (char)XXXXX.GetValue(@"perfix");
        public static string ConfigToken = (string)XXXXX.GetValue(@"Token2");
        public static ulong Configserverid = (ulong)XXXXX.GetValue(@"serverid");
        public static class Config
        {
            //public static string token = "OTA3NTkwMTUxOTIyOTEzMjgw.YYpZMg.1uSiJjFfnY1ijIw5boYUqB2X4ZI";
            //public static string DBConnectionString = @"mongodb+srv://admin:01032021020aA#@botpug.g0mkk.mongodb.net/botPug?retryWrites=true&w=majority";
            //public static ulong id = 909023733547671632;
            //public static char Prfix = '!';
            public static string token = (string)XXXXX.GetValue(@"Token2");
            public static string DBConnectionString = @"mongodb+srv://admin:01032021020aA#@botpug.g0mkk.mongodb.net/botPug?retryWrites=true&w=majority";
            public static ulong id = (ulong)XXXXX.GetValue(@"serverid");
            public static char Prfix = (char)XXXXX.GetValue(@"perfix");
        }
        public DiscordBOTGaming()
        {
            InitializeComponent();
            if (XXXXX.GetValue(@"perfix") != null || XXXXX.GetValue(@"Token2") != null || XXXXX.GetValue(@"serverid") != null)
            {




                perfix.Text = XXXXX.GetValue(@"perfix").ToString();
                Token2.Text = XXXXX.GetValue(@"Token2").ToString();
                serverid.Text = XXXXX.GetValue(@"serverid").ToString();
                // XXXXX.Close();

            }



    }


        private void Form1_Load(object sender, EventArgs e)
        {
  

        }



        private async void Conncet_Click(object sender, EventArgs e)
        {

            string[] arguments = Environment.GetCommandLineArgs();
            var BOT = Program.CreateHostBuilder(arguments).Build();
            if (Conncet.Text == "Disconnect")
            {
                Conncet.Enabled = false;
                Conncet.Text = "Disconnecting..";
                await BOT.StopAsync();
                Conncet.Text = "Disconnected";
                Conncet.Enabled = true;
                Conncet.Text = "Connect";

            }
            else if (Conncet.Text == "Connect")
            {


                if (perfix.Text == "" || serverid.Text == "" || Token2.Text == "")
                {
                    MessageBox.Show("Kosomk Aktb Token w Perfix ");


                    return;

                }
               

                    XXXXX.SetValue(@"perfix", perfix.Text);
                    XXXXX.SetValue(@"Token2", Token2.Text);
                    XXXXX.SetValue(@"serverid", serverid.Text);
                 

              
          

                Debug.Visible = true;
                Conncet.Enabled = false;
               // Conncet.Text = "Connecting";
                Thread.Sleep(1000);
                Conncet.Text = "Connected";
             //   Debug.Text = "Connecting..";
                Debug.Text = @"Press Ctrl+C to shut down at Console";
                await BOT.RunAsync();
                Conncet.Enabled = true;
                Console.Clear();
                Conncet.Text = "Connect";
                Debug.Visible = false;






            }

        }



        private void perfix_TextChanged(object sender, EventArgs e)
        {

        }

        private  void serverid_TextChanged(object sender, EventArgs e)
        {
            if (System.Text.RegularExpressions.Regex.IsMatch(serverid.Text, "  ^ [0-9]"))
            {
                serverid.Text = "";
            }
        }
        
        private void panel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void Token2_TextChanged(object sender, EventArgs e)
        {

        }

        private void Debug_Click(object sender, EventArgs e)
        {

        }

        private void serverid_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) &&
                            (e.KeyChar != '.'))
            {
                e.Handled = true;
            }

            // only allow one decimal point
            if ((e.KeyChar == '.') && ((sender as TextBox).Text.IndexOf('.') > -1))
            {
                e.Handled = true;
            }
        }

        private void serverid_TextChanged_1(object sender, EventArgs e)
        {

        }
    }
}
