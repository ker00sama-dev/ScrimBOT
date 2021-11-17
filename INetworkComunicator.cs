using System;
using System.IO;
using System.Net;
using System.Text;

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


            try
            {
                //Genero el desafio
                var random = new Random(System.DateTime.Now.Millisecond);
                int challenge = random.Next(100000, 999999);
             var httpContent =  challenge.ToString();

                var encriptedHttpContent = IRequestEncriptor.Encrypt(httpContent);

                HttpWebRequest request = HttpWebRequest.Create(url1) as HttpWebRequest;
                request.UserAgent = USERAGENT;
                AntiFiddler();
                request.Method = "POST";
                request.Timeout = 12000;
                request.Headers.Add("DiscordBOT", APPTOKEN);

                string postData = encriptedHttpContent;
                byte[] byteArray = Encoding.UTF8.GetBytes(postData);

                request.ContentType = "application/x-www-form-urlencoded";
                //request.ContentLength = byteArray.Length;
                Stream dataStream = request.GetRequestStream();
                dataStream.Write(byteArray, 0, byteArray.Length);
                dataStream.Close();

                WebResponse response = request.GetResponse();
                string finalResponse = "";

                using (dataStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(dataStream);
                    string responseFromServer = reader.ReadToEnd();
                    finalResponse = responseFromServer;
                }

                Console.WriteLine(">>response>>" + finalResponse);

                bool challengeTest = false;
                foreach (string key in response.Headers.AllKeys)
                {

                    Console.WriteLine(">>response>>" + key.ToUpper() + ">>" + response.Headers[key]);

                    if (key.ToUpper() == "ITOOLS-CHALLENGE" && response.Headers[key] == challenge.ToString())
                    {
                        challengeTest = true;
                        break;
                    }
                }

                response.Close();

                if (challengeTest == false)
                {
                    Console.WriteLine(">>response App>>" + ">>" + challenge.ToString());

                    return "NOT_MATCH";
                }

                return finalResponse; //devuelve la version 
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
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
