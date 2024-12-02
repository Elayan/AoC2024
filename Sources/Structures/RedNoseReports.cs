using AoCTools.Loggers;
using System.Text;

namespace AoC2024.Structures
{
    public class RedNoseReports
    {
        List<List<int>> _reports = new List<List<int>>();

        public RedNoseReports(string[] reportLines)
        {
            foreach (var line in reportLines)
            {
                var split = line.Split(' ');
                var list = new List<int>();
                foreach (var item in split)
                {
                    list.Add(int.Parse(item));
                }
                _reports.Add(list);
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine("=== RED NOSE REPORTS ===");
            foreach (var report in _reports)
            {
                sb.AppendLine(string.Join(" ", report));
            }
            return sb.ToString();
        }

        public long GetValidReportCount()
        {
            long count = 0;
            foreach(var report in _reports)
            {
                if (IsValid(report))
                    count++;
            }
            return count;
        }

        private bool IsValid(List<int> report)
        {
            if (report.Count < 2)
                return true;

            Logger.Log($"> Validating report {string.Join(" ", report)}");

            int start = 0;
            int step = 1;
            int end = report.Count - 1;

            if (report[0] > report[1])
            {
                // if report is decreasing, we check the list as increasing from the end
                Logger.Log(">> Report is detected as decreasing.");
                start = report.Count - 1;
                step = -1;
                end = 0;
            }

            Logger.Log($">>> Start {start}, end {end}, step {step}");
            for (int i = start, next = start + step; i != end; i += step, next += step)
            {
                var difference = report[next] - report[i];
                Logger.Log($">>> Difference between {report[i]} and {report[next]} = {difference}");
                if (difference <= 0 || difference > 3)
                {
                    Logger.Log($">> Invalidated.");
                    return false; // report is changing direction, or moving too fast
                }
            }

            Logger.Log(">> Validated.");
            return true;
        }
    }
}
