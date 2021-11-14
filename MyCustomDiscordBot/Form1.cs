using Microsoft.Extensions.Hosting;
using System;
using System.Threading;
using System.Windows.Forms;

namespace MyCustomDiscordBot.MyCustomDiscordBot
{
    public partial class DiscordBOTGaming : Form
    {
        public static Microsoft.Win32.RegistryKey XXXXX = Microsoft.Win32.Registry.CurrentUser.CreateSubKey("DiscordBOT");

        //public static char ConfigPerfix = (char)XXXXX.GetValue(@"perfix");
        //public static string ConfigToken = (string)XXXXX.GetValue(@"Token2");
        //public static ulong Configserverid = (ulong)XXXXX.GetValue(@"serverid");

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

            if (XXXXX.GetValue(@"startfull") != null || XXXXX.GetValue(@"centerfull") != null || XXXXX.GetValue(@"Endfull") != null || XXXXX.GetValue(@"startnull") != null || XXXXX.GetValue(@"centernull") != null || XXXXX.GetValue(@"endnull") != null)
            {
                //progress bar
                Startfull.Text = XXXXX.GetValue(@"startfull").ToString();
                Centerfull.Text = XXXXX.GetValue(@"centerfull").ToString();
                Endfull.Text = XXXXX.GetValue(@"Endfull").ToString();
                Startnull.Text = XXXXX.GetValue(@"startnull").ToString();
                Centernull.Text = XXXXX.GetValue(@"centernull").ToString();
                Endnull.Text = XXXXX.GetValue(@"endnull").ToString();

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
                    MessageBox.Show("Please Write (Prefix, Token, Server ID) !");


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
    }
}
