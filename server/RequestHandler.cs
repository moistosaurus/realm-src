using Anna.Request;
using Anna.Responses;
using common;
using common.resources;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace server
{
    abstract class RequestHandler
    {
        public abstract void HandleRequest(RequestContext context, NameValueCollection query);

        public virtual void InitHandler(Resources resources) { }

        protected Database Database => Program.Database;

        internal void Write(RequestContext req, string val)
        {
            Write(req.Response(val), "text/plain");
        }

        internal void Write(RequestContext req, byte[] val)
        {
            Write(req.Response(val), "text/plain");
        }

        internal void WriteXml(RequestContext req, string val)
        {
            Write(req.Response(val), "application/xml");
        }

        internal void WriteXml(RequestContext req, byte[] val)
        {
            Write(req.Response(val), "application/xml");
        }

        internal void WriteImg(RequestContext req, byte[] val)
        {
            Write(req.Response(val), "image/png");
        }

        internal void WriteSnd(RequestContext req, byte[] val)
        {
            Write(req.Response(val), "*/*");
        }

        internal void Write(Response r, string type)
        {
            r.Headers["Content-Type"] = type;
            r.Send();
        }
    }

    internal static class RequestHandlers
    {
        public static void Initialize(Resources resources)
        {
            foreach (var h in Get)
                h.Value.InitHandler(resources);
            foreach (var h in Post)
                h.Value.InitHandler(resources);

            InitWebFiles(resources);
        }

        private static void InitWebFiles(Resources resources)
        {
            foreach (var f in resources.WebFiles)
                Get[f.Key] = new StaticFile(f.Value, MimeMapping.GetMimeMapping(f.Key));
        }

        public static readonly Dictionary<string, RequestHandler> Get = new Dictionary<string, RequestHandler>
        {
        };

        public static readonly Dictionary<string, RequestHandler> Post = new Dictionary<string, RequestHandler>
        {
            { "/app/init", new app.init() },
            { "/app/globalNews", new app.globalNews() },
            { "/account/verify", new account.verify() },
            { "/char/list", new @char.list() },
        };
    }
}
