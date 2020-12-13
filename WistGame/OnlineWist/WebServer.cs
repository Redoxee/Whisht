using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;

namespace OnlineWist
{
    public class WebServer
    {
        private readonly HttpListener listeners = new HttpListener();
        private readonly Func<HttpListenerContext, string> responseMethodes;

        public WebServer(IReadOnlyCollection<string> prefixes, Func<HttpListenerContext, string> method)
        {
            if (!HttpListener.IsSupported)
            {
                throw new NotSupportedException("Needs Windows XP SP2, Server 2003 or later.");
            }

            // URI prefixes are required eg: "http://localhost:8080/test/"
            if (prefixes == null || prefixes.Count == 0)
            {
                throw new ArgumentException("URI prefixes are required");
            }

            if (method == null)
            {
                throw new ArgumentException("responder method required");
            }

            foreach (var s in prefixes)
            {
                this.listeners.Prefixes.Add(s);
            }

            this.responseMethodes = method;
            this.listeners.Start();
        }

        public WebServer(Func<HttpListenerContext, string> method, params string[] prefixes)
           : this(prefixes, method)
        {
        }

        public void Run()
        {
            ThreadPool.QueueUserWorkItem(this.ListenerWork);
        }

        public void Stop()
        {
            this.listeners.Stop();
            this.listeners.Close();
        }

        private void ListenerWork(Object o)
        {
            Console.WriteLine("Webserver running...");
            try
            {
                while (this.listeners.IsListening)
                {
                    ThreadPool.QueueUserWorkItem(this.ProcessHttpContext, this.listeners.GetContext());
                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex.ToString());
            }
        }

        private void ProcessHttpContext(object contextObject)
        {

            var context = contextObject as HttpListenerContext;
            try
            {
                if (context == null)
                {
                    return;
                }

                var responsString = this.responseMethodes(context);
                var buffer = Encoding.UTF8.GetBytes(responsString);
                context.Response.ContentLength64 = buffer.Length;
                context.Response.OutputStream.Write(buffer, 0, buffer.Length);
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
            finally
            {
                // always close the stream
                if (context != null)
                {
                    context.Response.OutputStream.Close();
                }
            }
        }
    }
}
