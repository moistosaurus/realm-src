using common.resources;
using NLog;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace common
{
    public static class Utils
    {
        static readonly Logger Log = LogManager.GetCurrentClassLogger();

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
                    Log.Error(ex.ToString());
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
                return (T)Convert.ChangeType(GetInt(val), t);
            else if (t == typeof(uint))
                return (T)Convert.ChangeType(Convert.ToUInt32(val, 16), t);
            else if (t == typeof(double))
                return (T)Convert.ChangeType(double.Parse(val, CultureInfo.InvariantCulture), t);
            else if (t == typeof(float))
                return (T)Convert.ChangeType(float.Parse(val, CultureInfo.InvariantCulture), t);
            else if (t == typeof(bool))
                return (T)Convert.ChangeType(string.IsNullOrWhiteSpace(val) || bool.Parse(val), t);

            Log.Error(string.Format("Type of {0} is not supported by this method, returning default value: {1}...", t, def));
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
                return (T)Convert.ChangeType(GetInt(val), t);
            else if (t == typeof(uint))
                return (T)Convert.ChangeType(Convert.ToUInt32(val, 16), t);
            else if (t == typeof(double))
                return (T)Convert.ChangeType(double.Parse(val, CultureInfo.InvariantCulture), t);
            else if (t == typeof(float))
                return (T)Convert.ChangeType(float.Parse(val, CultureInfo.InvariantCulture), t);
            else if (t == typeof(bool))
                return (T)Convert.ChangeType(string.IsNullOrWhiteSpace(val) || bool.Parse(val), t);

            Log.Error(string.Format("Type of {0} is not supported by this method, returning default value: {1}...", t, def));
            return def;
        }

        public static bool HasElement(this XElement e, string name)
        {
            return e.Element(name) != null;
        }

        public static bool HasAttribute(this XElement e, string name)
        {
            return e.Attribute(name) != null;
        }

        public static int GetInt(string x)
        {
            return x.Contains("x") ? Convert.ToInt32(x, 16) : int.Parse(x);
        }

        public static ConditionEffectIndex GetEffect(string val)
        {
            ConditionEffectIndex ret = (ConditionEffectIndex)Enum.Parse(typeof(ConditionEffectIndex), val.Replace(" ", ""));
            return ret;
        }

        public static T[] CommaToArray<T>(this string x)
        {
            if (typeof(T) == typeof(ushort))
                return x.Split(',').Select(_ => (T)(object)(ushort)GetInt(_.Trim())).ToArray();
            if (typeof(T) == typeof(string))
                return x.Split(',').Select(_ => (T)(object)_.Trim()).ToArray();
            else //assume int
                return x.Split(',').Select(_ => (T)(object)GetInt(_.Trim())).ToArray();
        }

        public static string ToCommaSepString<T>(this T[] arr)
        {
            StringBuilder ret = new StringBuilder();
            for (var i = 0; i < arr.Length; i++)
            {
                if (i != 0) ret.Append(", ");
                ret.Append(arr[i].ToString());
            }
            return ret.ToString();
        }

        public static int ToUnixTimestamp(this DateTime dateTime)
        {
            return (int)(dateTime - new DateTime(1970, 1, 1)).TotalSeconds;
        }

        public static bool IsValidEmail(string strIn)
        {
            var invalid = false;
            if (String.IsNullOrEmpty(strIn))
                return false;

            MatchEvaluator domainMapper = match =>
            {
                // IdnMapping class with default property values.
                var idn = new IdnMapping();

                var domainName = match.Groups[2].Value;
                try
                {
                    domainName = idn.GetAscii(domainName);
                }
                catch (ArgumentException)
                {
                    invalid = true;
                }
                return match.Groups[1].Value + domainName;
            };

            // Use IdnMapping class to convert Unicode domain names. 
            strIn = Regex.Replace(strIn, @"(@)(.+)$", domainMapper);
            if (invalid)
                return false;

            // Return true if strIn is in valid e-mail format. 
            return Regex.IsMatch(strIn,
                      @"^(?("")(""[^""]+?""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                      @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9]{2,17}))$",
                      RegexOptions.IgnoreCase);
        }

        public static byte[] SHA1(string val)
        {
            SHA1Managed sha1 = new SHA1Managed();
            return sha1.ComputeHash(Encoding.UTF8.GetBytes(val));
        }

        public static void Shuffle<T>(this IList<T> list)
        {
            var provider = new RNGCryptoServiceProvider();
            int n = list.Count;
            while (n > 1)
            {
                var box = new byte[1];
                do provider.GetBytes(box); while (!(box[0] < n * (Byte.MaxValue / n)));
                int k = (box[0] % n);
                n--;
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public static string GetDescription(this Enum value)
        {
            Type type = value.GetType();
            string name = Enum.GetName(type, value);
            if (name != null)
            {
                FieldInfo field = type.GetField(name);
                if (field != null)
                {
                    DescriptionAttribute attr =
                           Attribute.GetCustomAttribute(field,
                           typeof(DescriptionAttribute)) as DescriptionAttribute;
                    return attr?.Description;
                }
            }
            return null;
        }

        public static T[] ResizeArray<T>(T[] array, int newSize)
        {
            var inventory = new T[newSize];
            for (int i = 0; i < array.Length; i++)
                inventory[i] = array[i];

            return inventory;
        }

        public static string To4Hex(this ushort x)
        {
            return "0x" + x.ToString("x4");
        }

        public static DateTime FromUnixTimestamp(int time)
        {
            var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddSeconds(time).ToLocalTime();
            return dateTime;
        }

        public static bool ContainsIgnoreCase(this string self, string val)
        {
            return self.IndexOf(val, StringComparison.InvariantCultureIgnoreCase) != -1;
        }

        public static bool EqualsIgnoreCase(this string self, string val)
        {
            return self.Equals(val, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
