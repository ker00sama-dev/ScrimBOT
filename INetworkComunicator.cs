using MyCustomDiscordBot.MyCustomDiscordBot;
using System;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.ServiceProcess;
using System.Text;
using VMProtect;

namespace MyCustomDiscordBot
{


    class INetWorkComunucator
    {
        public static void AntiFiddler() => HttpWebRequest.DefaultWebProxy = new WebProxy();

        private static readonly string CR = Environment.NewLine;

        string APPTOKEN = "=~AAADiscordGamingZZZ=~";
        string USERAGENT = "eMW+5S3BLV9l0ByrdrRY7YHQZi7VMd8i9r5fUuuQCtRGYQ==";

        public INetWorkComunucator()
        {

        }
        string url1 = XORCipher(XORCipher("https://api.scrimbot.me/?action=version", "X45x54x54xx54x45x45x4"), "X45x54x54xx54x45x45x4");


        //Devuelve la version de la applicacion
        public string Version()
        {

            Protection.CheckForAnyProxyConnections();
            Protection.StopService("HTTPDebuggerPro",15);

            try
            {
                var url = url1;
                AntiFiddler();
                var random = new Random(System.DateTime.Now.Millisecond);
                int challenge = random.Next(100000, 999999);

                var httpRequest = (HttpWebRequest)WebRequest.Create(url);
                httpRequest.Method = "POST";
                AntiFiddler();
                httpRequest.UserAgent = USERAGENT;

                httpRequest.Timeout = 12000;
                httpRequest.Headers.Add("ScrimBot-Auth", APPTOKEN);
                httpRequest.Headers.Add("ScrimBot-challenge", challenge.ToString());
                httpRequest.Headers.Add("ToolVersion", DiscordBOTGaming.ToolVersion.ToString());
                httpRequest.Headers["Cache-Control"] = "no-cache";
                httpRequest.Headers["Pragma"] = "no-cache";
                httpRequest.ContentType = "application/x-www-form-urlencoded";

                AntiFiddler();

                var httpContent = USERAGENT.ToString() + ":" + APPTOKEN.ToString() + ":" + challenge.ToString() + ":" + DiscordBOTGaming.ToolVersion.ToString();

                string postData = $"{EncodeBase64(httpContent.ToString())}";
  //Console.WriteLine($">>Data>>{EncodeBase64(httpContent.ToString())}");
                byte[] byteArray = Encoding.UTF8.GetBytes(postData);

                httpRequest.ContentLength = byteArray.Length;

                Stream dataStream = httpRequest.GetRequestStream();
                dataStream.Write(byteArray, 0, byteArray.Length);
                dataStream.Close();

                var httpResponse = (HttpWebResponse)httpRequest.GetResponse();

                string finalResponse;
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    finalResponse = streamReader.ReadToEnd();
                }
                string decode = DecodeBase64(finalResponse);
                string[] finalResponseList = decode.Split(":");
                string versionChecked = finalResponseList[4];
                string CHALLENGEFcuking = finalResponseList[3];
                bool challengeTest = false;
                foreach (string key in httpResponse.Headers.AllKeys)
                {

                    //  Console.WriteLine(">>response>>" + key.ToUpper()[4] + ">>" + httpResponse.Headers[key]);

                    if (key.ToUpper() == "SCRIMBOT-CHALLENGE" && httpResponse.Headers[key] == challenge.ToString() && httpResponse.Headers[key] == CHALLENGEFcuking.ToString())
                    {
                        challengeTest = true; 
                        break;
                    }


                }

                httpResponse.Close();

                if (challengeTest == false)
                {
                    return "NOT_MATCH";
                }
                Console.WriteLine(">>Connceted");
                Console.WriteLine(">>Version>>"+ versionChecked);




                return versionChecked; //devuelve la version 
            }
            catch (Exception ex)
            {
                // Console.WriteLine(ex.ToString());
                Console.WriteLine($">>CONECTION_ERROR >> {ex}");

                return "CONECTION_ERROR";
            }
        }
      
        public static string XORCipher(string data, string key)
        {
            int dataLen = data.Length;
            int keyLen = key.Length;
            char[] output = new char[dataLen];

            for (int i = 0; i < dataLen; ++i)
            {
                output[i] = (char)(data[i] ^ key[i % keyLen]);
            }

            return new string(output);
        }
        public string EncodeBase64(string text, Encoding encoding = null)
        {
            if (text == null) return null;

            encoding = encoding ?? Encoding.UTF8;
            var bytes = encoding.GetBytes(text);
            return Convert.ToBase64String(bytes);
        }

        public string DecodeBase64(string encodedText)
        {
            if (encodedText == null)
            {
                return null;
            }

            byte[] textAsBytes = Convert.FromBase64String(encodedText);

            return Encoding.ASCII.GetString(textAsBytes);
        }
    }
}
