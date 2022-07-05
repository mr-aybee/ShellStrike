using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace ShellStrike
{
    public class ScalarFunctions
    {

        /// <summary>
        /// Gets First Regex Match from string set.
        /// </summary>
        /// <param name="pattern"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string GetFirstRegexMatch(string pattern, string text)
        {
            if (isMatchRegex(text, pattern))
                return Regex.Matches(text, pattern, RegexOptions.IgnoreCase)[0].Value ?? "";
            else return "";
        }

        /// <summary>
        /// Check if a string character matches a regex pattern
        /// </summary>
        /// <param name="pattern"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        public static bool isMatchRegex(string text, string pattern)
        {
            if (string.IsNullOrEmpty(pattern)) return true;
            return Regex.IsMatch(text, pattern);
        }

        private static readonly Random random = new Random();
        /// <summary>
        /// Get a Random String Character w.r.t Length Provided
        /// </summary>
        /// <param name="Length"></param>
        /// <returns></returns>
        public static string GetRandomString(int Length = 10)
        {
            return new string(Enumerable.Repeat("ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789", Length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

      

    }
}
