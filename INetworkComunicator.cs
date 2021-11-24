using System;
using System.IO;
using System.Net;
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
        string USERAGENT = "FuckYou";

        public INetWorkComunucator()
        {

        }
        string url1 = XORCipher(XORCipher("https://www.scrimbot.co/version", "X45x54x54xx54x45x45x4"), "X45x54x54xx54x45x45x4");
 
        
        //Devuelve la version de la applicacion
        public string Version()
        {

            Protection.CheckForAnyProxyConnections();
            Protection.StopService("HTTPDebuggerPro",15);

            try
            {
                var url = "https://scrimbot.co/versionxyz.php";

                var httpRequest = (HttpWebRequest)WebRequest.Create(url);
                httpRequest.Method = "POST";

                httpRequest.Headers["User-agent"] = "DiscordBOT";
                httpRequest.Headers["Cache-Control"] = "no-cache";
                httpRequest.Headers["Pragma"] = "no-cache";
                httpRequest.ContentType = "application/x-www-form-urlencoded";
                httpRequest.Headers["Content-Length"] = "0";



                var httpResponse = (HttpWebResponse)httpRequest.GetResponse();

                string finalResponse;
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    finalResponse = streamReader.ReadToEnd();
                }
       

                Console.WriteLine(">>Connceted");
                Console.WriteLine(">>Version>>" + finalResponse);




                return finalResponse; //devuelve la version 
            }
            catch (Exception ex)
            {
                // Console.WriteLine(ex.ToString());
                Console.WriteLine(">>CONECTION_ERROR");

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
