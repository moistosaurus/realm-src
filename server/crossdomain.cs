using Anna.Request;
using common.resources;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace server
{
    class crossdomain : RequestHandler
    {
        private static byte[] _data;

        public override void InitHandler(Resources resources)
        {
            string data = @"<cross-domain-policy>
                                <allow-access-from domain=""*""/>
                            </cross-domain-policy>";
            _data = Encoding.UTF8.GetBytes(data);
        }

        public override void HandleRequest(RequestContext context, NameValueCollection query)
        {
            Write(context, _data);
        }
    }
}
