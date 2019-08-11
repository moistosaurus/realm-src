using NLog;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace common
{
    public static partial class Utils
    {
        static readonly Logger log = LogManager.GetCurrentClassLogger();

        public static async Task<string> ReadAsync(string p)
        {
            var t1 = Task.Run(() => {
                string ret = "";
                Invoke(true, () => {
                    ret = File.ReadAllText(p);
                });
                return ret;
            });
            await t1;
            return t1.Result;
        }

        public static void ReadAfter(string p, Action<string> c)
        {
            string ret = "";
            Invoke(true, () => {
                ret = File.ReadAllText(p);
            });
            c(ret);
        }

        public static string Read(string p)
        {
            string ret = "";
            Invoke(true, () => {
                ret = File.ReadAllText(p);
            });
            return ret;
        }

        public static byte[] ReadBytes(string p)
        {
            byte[] ret = new byte[0];
            Invoke(true, () => {
                ret = File.ReadAllBytes(p);
            });
            return ret;
        }

        public static bool Invoke(bool l, Action a)
        {
            try
            {
                a();
                return true;
            }
            catch (Exception ex)
            {
                if (l)
                    log.Error(ex.ToString());
                return false;
            }
        }

        public static T GetValue<T>(this XElement e, string n, T def = default(T))
        {
            if (e.Element(n) == null)
                return def;

            string val = e.Element(n).Value;
            var t = typeof(T);
            if (t == typeof(string))
                return (T)Convert.ChangeType(val, t);
            else if (t == typeof(ushort))
                return (T)Convert.ChangeType(Convert.ToUInt16(val, 16), t);
            else if (t == typeof(int))
                return (T)Convert.ChangeType(int.Parse(val), t);
            else if (t == typeof(uint))
                return (T)Convert.ChangeType(Convert.ToUInt32(val, 16), t);
            else if (t == typeof(double))
                return (T)Convert.ChangeType(double.Parse(val, CultureInfo.InvariantCulture), t);
            else if (t == typeof(float))
                return (T)Convert.ChangeType(float.Parse(val, CultureInfo.InvariantCulture), t);
            else if (t == typeof(bool))
                return (T)Convert.ChangeType(string.IsNullOrWhiteSpace(val) || bool.Parse(val), t);

            log.Error(string.Format("Type of {0} is not supported by this method, returning default value: {1}...", t, def));
            return def;
        }

        public static T GetAttribute<T>(this XElement e, string n, T def = default(T))
        {
            if (e.Attribute(n) == null)
                return def;

            string val = e.Attribute(n).Value;
            var t = typeof(T);
            if (t == typeof(string))
                return (T)Convert.ChangeType(val, t);
            else if (t == typeof(ushort))
                return (T)Convert.ChangeType(Convert.ToUInt16(val, 16), t);
            else if (t == typeof(int))
                return (T)Convert.ChangeType(int.Parse(val), t);
            else if (t == typeof(uint))
                return (T)Convert.ChangeType(Convert.ToUInt32(val, 16), t);
            else if (t == typeof(double))
                return (T)Convert.ChangeType(double.Parse(val, CultureInfo.InvariantCulture), t);
            else if (t == typeof(float))
                return (T)Convert.ChangeType(float.Parse(val, CultureInfo.InvariantCulture), t);
            else if (t == typeof(bool))
                return (T)Convert.ChangeType(string.IsNullOrWhiteSpace(val) || bool.Parse(val), t);

            log.Error(string.Format("Type of {0} is not supported by this method, returning default value: {1}...", t, def));
            return def;
        }

        public static int[] ToIntArray(this string s, string p)
        {
            string[] sep = new string[] { p };
            var split = s.Split(sep, StringSplitOptions.None);
            return split.Select(o => o.Contains("-") ? int.Parse(o) : (int)Convert.ToUInt32(o, 16)).ToArray();
        }
    }
}
