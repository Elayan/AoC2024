using AoCTools.Loggers;
using AoCTools.Workers;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace AoC2024.Workers.Day03
{
    public class Multiplicator : WorkerBase
    {
        private static string _mulRegexPattern = @"mul\((?<val1>[0-9]{1,3}),(?<val2>[0-9]{1,3})\)";
        private Regex _mulRegex = new Regex(_mulRegexPattern, RegexOptions.Compiled | RegexOptions.Multiline);

        protected override void ProcessDataLines()
        {
            // nothing to do
        }

        protected override long WorkOneStar_Implementation()
        {
            long total = 0L;
            for (int i = 0; i < DataLines.Length; i++)
            {
                var matches = _mulRegex.Matches(DataLines[i]);
                Logger.Log($"Found {matches.Count} multiplications in line #{i}.");

                foreach (Match match in matches)
                {
                    long val1 = long.Parse(match.Groups["val1"].Value);
                    long val2 = long.Parse(match.Groups["val2"].Value);
                    long result = val1 * val2;
                    total += result;
                    Logger.Log($"{match.Value} => {val1} x {val2} = {result} => total = {total}");
                }
            }

            return total;
        }
    }
}
