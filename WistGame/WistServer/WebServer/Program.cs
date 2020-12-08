using System;
using System.Net;
using System.IO;
using System.Text;

namespace WebServer
{

    class Program
    {
        static void Main(string[] args)
        {
            HttpListener server = new HttpListener();
            server.Prefixes.Add("http://+:80/");
//            server.Prefixes.Add("http://localhost/");

            server.Start();

            Console.WriteLine("Listening...");

            while (true)
            {
                HttpListenerContext context = server.GetContext();
                HttpListenerResponse response = context.Response;

                string msg = "<html><body><br>HELLO</br></body></html>";

                byte[] buffer = Encoding.UTF8.GetBytes(msg);

                response.ContentLength64 = buffer.Length;
                Stream st = response.OutputStream;
                st.Write(buffer, 0, buffer.Length);

                context.Response.Close();
            }

        }
    }

}