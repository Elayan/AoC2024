using AoCTools.Loggers;
using AoCTools.Workers;
using System.Text.RegularExpressions;

namespace AoC2024.Workers.Day03
{
    public class Multiplicator : WorkerBase
    {
        private static string _mulRegexPattern = @"mul\((?<val1>[0-9]{1,3}),(?<val2>[0-9]{1,3})\)|do\(\)|don't\(\)";
        private Regex _mulRegex = new Regex(_mulRegexPattern, RegexOptions.Compiled);

        protected override void ProcessDataLines()
        {
            // nothing to do
        }

        protected override long WorkOneStar_Implementation()
        {
            return ReadMemory(true);
        }

        protected override long WorkTwoStars_Implementation()
        {
            return ReadMemory(false);
        }

        private long ReadMemory(bool ignoreDoDont)
        {
            long total = 0L;
            bool mulEnabled = true;
            for (int i = 0; i < DataLines.Length; i++)
            {
                var matches = _mulRegex.Matches(DataLines[i]);
                Logger.Log($"Found {matches.Count} instructions in line #{i}.");

                foreach (Match match in matches)
                {
                    if (match.Value[0] == 'm')
                    {
                        if (ignoreDoDont || mulEnabled)
                        {
                            long val1 = long.Parse(match.Groups["val1"].Value);
                            long val2 = long.Parse(match.Groups["val2"].Value);
                            long result = val1 * val2;
                            total += result;
                            Logger.Log($"{match.Value} => {val1} x {val2} = {result} => total = {total}");
                        }
                        else Logger.Log($"{match.Value} => ignored.");
                    }
                    else
                    {
                        mulEnabled = match.Value == "do()";
                        Logger.Log($"Multiplication {(mulEnabled ? "enabled" : "disabled")}.");
                    }
                }
            }

            return total;
        }
    }
}
