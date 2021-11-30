using Microsoft.Extensions.Hosting;
using System;
using System.Diagnostics;
using System.Threading;
using System.Windows.Forms;

namespace MyCustomDiscordBot.MyCustomDiscordBot
{
    public partial class DiscordBOTGaming : Form
    {
        public static Microsoft.Win32.RegistryKey XXXXX = Microsoft.Win32.Registry.CurrentUser.CreateSubKey("DiscordBOT");
        public static string ToolVersion = "1.0.0.0";
        //public static char ConfigPerfix = (char)XXXXX.GetValue(@"perfix");
        //public static string ConfigToken = (string)XXXXX.GetValue(@"Token2");
        //public static ulong Configserverid = (ulong)XXXXX.GetValue(@"serverid");

        public DiscordBOTGaming()
        {
            InitializeComponent();
            version.Text = ToolVersion;
            INetWorkComunucator nc = new INetWorkComunucator();
            string versioncheck = nc.Version();
            Console.WriteLine("Crossfire BOT");
            //if (versioncheck == "CONECTION_ERROR")
            //{
            //    MessageBox.Show("There is no internet Connections!", "Error!", MessageBoxButtons.OK,
            //        MessageBoxIcon.Error);
            //    Environment.Exit(0);
            //    return;
            //}

            //if (ToolVersion != versioncheck)
            //{
            //    MessageBox.Show("A new version [" + versioncheck + "] was realeased with some improves!. Please download it and try again the bypass process. This version was deprecated.", "ERROR: This version is deprecated!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            //    //Process.Start("https://www.itools-removal.com/");
            //    // Process.Start("https://mega.nz/folder/yfZUkbgC#mBZCQwIyrlEQl2xYmiBtUA");
            //    Environment.Exit(0);
            //    return;
            //}
            if ( XXXXX.GetValue(@"Token2") != null || XXXXX.GetValue(@"serverid") != null)
            {




                //perfix.Text = XXXXX.GetValue(@"perfix").ToString();
                Token2.Text = XXXXX.GetValue(@"Token2").ToString();
                serverid.Text = XXXXX.GetValue(@"serverid").ToString();

                // XXXXX.Close();

            }

            //if (XXXXX.GetValue(@"startfull") != null || XXXXX.GetValue(@"centerfull") != null || XXXXX.GetValue(@"Endfull") != null || XXXXX.GetValue(@"startnull") != null || XXXXX.GetValue(@"centernull") != null || XXXXX.GetValue(@"endnull") != null)
            //{
            //    //progress bar
            //    Startfull.Text = XXXXX.GetValue(@"startfull").ToString();
            //    Centerfull.Text = XXXXX.GetValue(@"centerfull").ToString();
            //    Endfull.Text = XXXXX.GetValue(@"Endfull").ToString();
            //    Startnull.Text = XXXXX.GetValue(@"startnull").ToString();
            //    Centernull.Text = XXXXX.GetValue(@"centernull").ToString();
            //    Endnull.Text = XXXXX.GetValue(@"endnull").ToString();

            //}
            //if (XXXXX.GetValue(@"norank") != null || XXXXX.GetValue(@"Bronze") != null || XXXXX.GetValue(@"Silver") != null || XXXXX.GetValue(@"Gold") != null || XXXXX.GetValue(@"Platinum") != null || XXXXX.GetValue(@"Diamond") != null || XXXXX.GetValue(@"Master") != null || XXXXX.GetValue(@"legend") != null || XXXXX.GetValue(@"mythical") != null || XXXXX.GetValue(@"GrandMaster") != null)
            //{

            //    norank.Text =  XXXXX.GetValue(@"norank").ToString();
            //    Bronze.Text = XXXXX.GetValue(@"Bronze").ToString();
            //    Silver.Text = XXXXX.GetValue(@"Silver").ToString();
            //    Gold.Text = XXXXX.GetValue(@"Gold").ToString();
            //    Platinum.Text = XXXXX.GetValue(@"Platinum").ToString();
            //    Diamond.Text = XXXXX.GetValue(@"Diamond").ToString();
            //    Master.Text = XXXXX.GetValue(@"Master").ToString();
            //    legend.Text = XXXXX.GetValue(@"legend").ToString();
            //    mythical.Text = XXXXX.GetValue(@"mythical").ToString();
            //    GrandMaster.Text = XXXXX.GetValue(@"GrandMaster").ToString();

            //}
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


                if ( serverid.Text == "" || Token2.Text == "")
                {
                    MessageBox.Show("Please Write (Prefix, Token, Server ID) !");


                    return;

                }


             //   XXXXX.SetValue(@"perfix", perfix.Text);
                XXXXX.SetValue(@"Token2", Token2.Text);
                XXXXX.SetValue(@"serverid", serverid.Text);





                Debug.Visible = true;
                Conncet.Enabled = false;
                button2.Enabled = false;
                button3.Enabled = false;
                button1.Enabled = false;
                // Conncet.Text = "Connecting";
                Thread.Sleep(1000);
                Conncet.Text = "Connected";
                //   Debug.Text = "Connecting..";
                Debug.Text = @"Press Ctrl+C to shut down at Console";
                await BOT.RunAsync();
                Conncet.Enabled = true;
                button1.Enabled = true;
                button2.Enabled = true;
                button3.Enabled = true;
                await BOT.StopAsync();
                Console.Clear();
                Conncet.Text = "Connect";
                Debug.Visible = false;






            }

        }



        private void perfix_TextChanged(object sender, EventArgs e)
        {

        }

        private void serverid_TextChanged(object sender, EventArgs e)
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

        private async void button1_Click(object sender, EventArgs e)
        {
            string[] arguments = Environment.GetCommandLineArgs();
            var BOT = Program.CreateHostBuilder(arguments).Build();
            await BOT.StopAsync();
            Application.Exit();
            Environment.Exit(-1);

        }

        private void startfull_TextChanged(object sender, EventArgs e)
        {

        }

        private void Centerfull_TextChanged(object sender, EventArgs e)
        {

        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (Startfull.Text == "" || Centerfull.Text == "" || Endfull.Text == "" || Startnull.Text == "" || Centernull.Text == "" || Endnull.Text == "")
            {
                MessageBox.Show("Please Write (ALL ID's Emoji) !");


                return;

            }
            XXXXX.SetValue(@"startfull", Startfull.Text);
            XXXXX.SetValue(@"centerfull", Centerfull.Text);
            XXXXX.SetValue(@"Endfull", Endfull.Text);
            XXXXX.SetValue(@"startnull", Startnull.Text);
            XXXXX.SetValue(@"centernull", Centernull.Text);
            XXXXX.SetValue(@"endnull", Endnull.Text);
            MessageBox.Show("Done");

        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (norank.Text == "" || Bronze.Text == "" || Silver.Text == "" || Gold.Text == "" || Platinum.Text == "" || Diamond.Text == "" || Master.Text == "" || legend.Text == "" || mythical.Text == "" || GrandMaster.Text == "")
            {
                MessageBox.Show("Please Write (ALL ID's Emoji) !");


                return;

            }
            XXXXX.SetValue(@"norank", norank.Text);
            XXXXX.SetValue(@"Bronze", Bronze.Text);
            XXXXX.SetValue(@"Silver", Silver.Text);
            XXXXX.SetValue(@"Gold", Gold.Text);
            XXXXX.SetValue(@"Platinum", Platinum.Text);
            XXXXX.SetValue(@"Diamond", Diamond.Text);
            XXXXX.SetValue(@"Master", Master.Text);
            XXXXX.SetValue(@"legend", legend.Text);
            XXXXX.SetValue(@"mythical", mythical.Text);
            XXXXX.SetValue(@"GrandMaster", GrandMaster.Text);
            MessageBox.Show("Done");
        }

        private void version_Click(object sender, EventArgs e)
        {

        }
    }
    }

