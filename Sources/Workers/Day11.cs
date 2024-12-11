using AoC2024.Structures;
using AoCTools.Loggers;
using AoCTools.Workers;

namespace AoC2024.Workers.Day11;

public class QuanticStoneBlinker : WorkerBase
{
    private QuanticLine _line;
    public override object Data => _line;

    protected override void ProcessDataLines()
    {
        _line = new QuanticLine(DataLines[0]);
    }

    protected override long WorkOneStar_Implementation()
    {
        return BlinkTheStones(25);
    }

    protected override long WorkTwoStars_Implementation()
    {
        return BlinkTheStones(75);
    }

    private long BlinkTheStones(int times)
    {
        LogStones();
        for (int i = 0; i < times; i++)
        {
            _line.Blink(); 
            LogStones();
        }

        return _line.GetStoneCount(times);
    }

    private void LogStones()
    {
        if (Logger.ShowAboveSeverity > SeverityLevel.Low)
            return;
        
        Logger.Log($"The lines has {_line.RawStoneCount}(?) stones");
        if (_line.RawStoneCount < 10)
            Logger.Log(_line.ToString());
    }
}