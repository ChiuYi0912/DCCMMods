using Hashlink.Proxy.Objects;
using HaxeProxy.Runtime;
using ModCore.Utilities;

namespace DebugMod.Core.Extensions
{

    public static class StringExtensions
    {
        public static dc.String ToHaxeString(this string str)
        {
            var haxeString = new HashlinkString(str).AsHaxe<dc.String>();
            return haxeString;
        }


        public static dc.String ToHaxeStringSafe(this string? str)
        {
            return (str ?? string.Empty).ToHaxeString();
        }


        public static dc.String Add_TwoHaxeStrings(this string str ,string str2)
        {
            return dc.String.Class.__add__( str.ToHaxeString(), str2.ToHaxeString());
        }

        

        public static bool IsNullOrWhiteSpace(this string? str)
        {
            return string.IsNullOrWhiteSpace(str);
        }

        public static bool IsNullOrEmpty(this string? str)
        {
            return string.IsNullOrEmpty(str);
        }


        public static string DefaultIfNullOrWhiteSpace(this string? str, string defaultValue)
        {
            return string.IsNullOrWhiteSpace(str) ? defaultValue : str;
        }


        public static string DefaultIfNullOrEmpty(this string? str, string defaultValue)
        {
            return string.IsNullOrEmpty(str) ? defaultValue : str;
        }


        public static string WithColor(this string str, string colorCode)
        {
            return $"{colorCode}{str}\x1b[0m";
        }

        public static string AsBlue(this string str)
        {
            return str.WithColor("\x1b[34m");
        }
        public static string AsGreen(this string str)
        {
            return str.WithColor("\x1b[32m");
        }

        public static string AsYellow(this string str)
        {
            return str.WithColor("\x1b[33m");
        }

        public static string AsRed(this string str)
        {
            return str.WithColor("\x1b[31m");
        }

        public static bool EqualsIgnoreCase(this string? str1, string? str2)
        {
            return string.Equals(str1, str2, StringComparison.OrdinalIgnoreCase);
        }

        public static bool ContainsIgnoreCase(this string str, string value)
        {
            return str.IndexOf(value, StringComparison.OrdinalIgnoreCase) >= 0;
        }

        public static IEnumerable<string> SplitLines(this string str, bool removeEmptyLines = true)
        {
            var lines = str.Split(new[] { "\r\n", "\r", "\n" },
                removeEmptyLines ? StringSplitOptions.RemoveEmptyEntries : StringSplitOptions.None);
            return lines.Select(line => line.Trim());
        }

        public static string Repeat(this string str, int count)
        {
            if (count <= 0) return string.Empty;
            return string.Concat(Enumerable.Repeat(str, count));
        }

    }
}