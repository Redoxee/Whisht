namespace WebWist
{
    public class PlayerThread
    {
        private PlayerThread()
        {
            
        }

        public static void StartPlayerThread(Microsoft.AspNetCore.Http.HttpContext httpContext)
        {
            System.Threading.Thread thread = new System.Threading.Thread(PlayerThread.PlayerThreadWork);
            thread.Start(httpContext);
        }


        private static void PlayerThreadWork(object contextObject)
        {
            Microsoft.AspNetCore.Http.HttpContext httpContext = contextObject as Microsoft.AspNetCore.Http.HttpContext;
            System.Diagnostics.Debug.Assert(httpContext != null);
        }
    }
}
