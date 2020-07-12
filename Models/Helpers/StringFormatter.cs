using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace WebTelegramBotsBuilder.Models.Helpers
{
    public static class StringFormatter
    {
        public static string FormatString(string pattern, string source, string value)
        {
            return source.Replace(pattern, value);
        }

        public static string FormatStringWithOr(string pattern, string source)
        {
            Match match = Regex.Match(source, pattern);
            if (!match.Success)
            {
                return source;
            }
            string[] values = match.Groups[1].Value.Split("|");
            Random rnd = new Random();
            string result = values[rnd.Next(0, values.Length)];
            return Regex.Replace(source, pattern, result);
        }

        public static string FormatStringWithRandom(string pattern, string source)
        {
            Match match = Regex.Match(source, pattern, RegexOptions.Multiline);
            if(!match.Success)
            {
                return source;
            }
            int firstValue = Convert.ToInt32(match.Groups[1].Value);
            int lastValue = Convert.ToInt32(match.Groups[2].Value);
            Random random = new Random();
            int resultValue = random.Next(Math.Min(firstValue, lastValue), Math.Max(firstValue, lastValue));
            return Regex.Replace(source, pattern, resultValue.ToString());
        }

        public static string FormatStringWithParams(string pattern, string source, Dictionary<string,string> values)
        {
            Match match = Regex.Match(source, pattern, RegexOptions.Multiline);
            if (!match.Success)
            {
                return source;
            }
            string result = match.Groups[3].Value;
            foreach (var i in values)
            {
                result = result.Replace(i.Key, i.Value);
            }
            return Regex.Replace(source, pattern, result);
        }
    }
}
