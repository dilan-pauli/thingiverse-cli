using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace thingiverseCLI.Services
{
    public static class StringPathExtensions
    {
        private static Regex _invalidPathPartsRegex;

        static StringPathExtensions()
        {
            var invalidReg = System.Text.RegularExpressions.Regex.Escape(new string(Path.GetInvalidFileNameChars()));
            _invalidPathPartsRegex = new Regex($"(?<reserved>^(CON|PRN|AUX|CLOCK\\$|NUL|COM0|COM1|COM2|COM3|COM4|COM5|COM6|COM7|COM8|COM9|LPT0|LPT1|LPT2|LPT3|LPT4|LPT5|LPT6|LPT7|LPT8|LPT9))|(?<invalid>[{invalidReg}:]+|\\.$)", RegexOptions.Compiled);
        }

        public static string SanitizeFileName(this string path)
        {
            return _invalidPathPartsRegex.Replace(path, m =>
            {
                if (!string.IsNullOrWhiteSpace(m.Groups["reserved"].Value))
                    return string.Concat("_", m.Groups["reserved"].Value);
                return "_";
            });
        }
    }
}
