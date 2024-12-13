using System.Text.RegularExpressions;
using AoC2024.Structures;
using AoCTools.Loggers;
using AoCTools.Workers;

namespace AoC2024.Workers.Day13;

public class ClawMachineHacker : WorkerBase
{
    private List<ClawMachine> _machines = new();
    public override object Data => _machines;

    private static string _clawMachineReaderRegexPattern = 
        @"Button A: X\+(?<xa>[0-9]+), Y\+(?<ya>[0-9]+)\nButton B: X\+(?<xb>[0-9]+), Y\+(?<yb>[0-9]+)\nPrize: X=(?<xp>[0-9]+), Y=(?<yp>[0-9]+)";
    private Regex _clawMachineRegex = new(_clawMachineReaderRegexPattern, RegexOptions.Compiled | RegexOptions.Multiline);

    protected override void ProcessDataLines()
    {
        var fullData = string.Join("\n", DataLines);
        foreach (Match match in _clawMachineRegex.Matches(fullData))
        {
            _machines.Add(new ClawMachine(
                long.Parse(match.Groups["xa"].Value),
                long.Parse(match.Groups["ya"].Value),
                long.Parse(match.Groups["xb"].Value),
                long.Parse(match.Groups["yb"].Value),
                long.Parse(match.Groups["xp"].Value),
                long.Parse(match.Groups["yp"].Value)));
        }
    }

    protected override long WorkOneStar_Implementation()
    {
        var tokens = 0L;
        Logger.Log($"Hacking {_machines.Count} machines!");
        foreach (var machine in _machines)
        {
            Logger.Log($"Hacking machine: {machine}");
            if (!ComputeSolutionForSystemWithTwoEquationsAndTwoUnknowns(
                    machine.AX, machine.BX, machine.PrizeX,
                    machine.AY, machine.BY, machine.PrizeY,
                    out var A, out var B))
            {
                Logger.Log("> this machine doesn't have a solution.");
                continue;
            }

            if (A > 100 || B > 100)
            {
                Logger.Log("> this machine is too expensive.");
                continue;
            }

            var aCost = 3L * A;
            var machineCost = aCost + B;
            tokens += machineCost;
            Logger.Log($"> press A {A} times ({aCost} tokens) and B {B} times => {machineCost} tokens => total tokens = {tokens}.");
        }

        return tokens;
    }

    /// <summary>
    /// A * xa + B * xb = x
    /// A * ya + B * yb = y
    /// </summary>
    private bool ComputeSolutionForSystemWithTwoEquationsAndTwoUnknowns(
        long xa, long xb, long x,
        long ya, long yb, long y,
        out long a, out long b)
    {
        var divisor = xa * yb - xb * ya;
        if (divisor == 0L)
        {
            a = -1;
            b = -1;
            return false;
        }

        var aNumerator = x * yb - xb * y;
        if (aNumerator % divisor != 0)
        {
            a = -1;
            b = -1;
            return false;
        }
        
        var bNumerator = xa * y - x * ya;
        if (bNumerator % divisor != 0)
        {
            a = -1;
            b = -1;
            return false;
        }
        
        a = aNumerator / divisor;
        b = bNumerator / divisor;
        return true;
    }
}