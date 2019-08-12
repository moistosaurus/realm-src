using Anna;
using Anna.Request;
using common;
using common.resources;
using NLog;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace server
{
    class Program
    {
        static readonly Logger Log = LogManager.GetCurrentClassLogger();
        static readonly ManualResetEvent Shutdown = new ManualResetEvent(false);

        internal static ServerConfig Config;
        internal static Resources Resources;
        internal static Database Database;

        static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += LogUnhandledException;
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            Thread.CurrentThread.Name = "Entry";

            Console.CancelKeyPress += delegate
            {
                Shutdown.Set();
            };

            Config = new ServerConfig();
            Resources = new Resources(Config.ServerSettings.ResourcePath, true);
            using (Database = new Database(Resources, Config))
            {
                RequestHandlers.Initialize(Resources);
                var port = Config.ServerSettings.Port;
                var bindAddress = Config.ServerSettings.BindAddress;
                using (var server = new HttpServer($"http://{bindAddress}:{port}/"))
                {
                    if (Config.ServerSettings.DebugRequests)
                    {
                        server.GET("*").Subscribe(Response);
                        server.POST("*").Subscribe(Response);
                    }
                    else
                    {
                        foreach (var uri in RequestHandlers.Get.Keys)
                            server.GET(uri).Subscribe(Response);
                        foreach (var uri in RequestHandlers.Post.Keys)
                            server.POST(uri).Subscribe(Response);
                    }

                    Log.Info("Listening at address {0}:{1}...", bindAddress, port);
                    Shutdown.WaitOne();
                }

                Log.Info("Terminating...");
            }
        }

        public static void Stop()
        {
            Shutdown.Set();
        }

        private static void Response(RequestContext rContext)
        {
            try
            {
                Log.Info("Dispatching '{0}'@{1}",
                    rContext.Request.Url.LocalPath,
                    rContext.Request.ClientIP());

                bool getRequest = rContext.Request.HttpMethod.Equals("GET");
                if (Config.ServerSettings.DebugRequests)
                {
                    var dict = getRequest ? RequestHandlers.Get : RequestHandlers.Post;
                    if (!dict.ContainsKey(rContext.Request.Url.LocalPath))
                    {
                        Log.Warn("Unknown {0} request '{1}'@{2}",
                            getRequest ? "GET" : "POST",
                            rContext.Request.Url.LocalPath,
                            rContext.Request.ClientIP());
                        rContext.Respond("<Error>Internal server error</Error>", 500);
                        return;
                    }
                }

                if (getRequest)
                {
                    var query = HttpUtility.ParseQueryString(rContext.Request.Url.Query);
                    RequestHandlers.Get[rContext.Request.Url.LocalPath].HandleRequest(rContext, query);
                    return;
                }

                GetBody(rContext.Request, 4096).Subscribe(body =>
                {
                    try
                    {
                        var query = HttpUtility.ParseQueryString(body);
                        RequestHandlers.Post[rContext.Request.Url.LocalPath]
                            .HandleRequest(rContext, query);
                    }
                    catch (Exception e)
                    {
                        OnError(e, rContext);
                    }
                });
            }
            catch (Exception e)
            {
                OnError(e, rContext);
            }
        }

        private static void OnError(Exception e, RequestContext rContext)
        {
            Log.Error($"{e.Message}\r\n{e.StackTrace}");

            try
            {
                rContext.Respond("<Error>Internal server error</Error>", 500);
            }
            catch
            {
            }
        }

        private static IObservable<string> GetBody(Request r, int maxContentLength = 50000)
        {
            int bufferSize = maxContentLength;
            if (r.Headers.ContainsKey("Content-Length"))
                bufferSize = Math.Min(maxContentLength, int.Parse(r.Headers["Content-Length"].First()));

            var buffer = new byte[bufferSize];
            return Observable.FromAsyncPattern<byte[], int, int, int>(r.InputStream.BeginRead, r.InputStream.EndRead)(buffer, 0, bufferSize)
                .Select(bytesRead => r.ContentEncoding.GetString(buffer, 0, bytesRead));
        }

        private static void LogUnhandledException(object sender, UnhandledExceptionEventArgs args)
        {
            Log.Fatal((Exception)args.ExceptionObject);
        }
    }
}
