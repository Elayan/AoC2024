using AoC2024.Structures;
using AoCTools.Loggers;
using AoCTools.Workers;

namespace AoC2024.Workers.Day11;

public class QuanticStoneBlinker : WorkerBase
{
    private QuanticStones _stones;
    public override object Data => _stones;

    protected override void ProcessDataLines()
    {
        _stones = new QuanticStones(DataLines[0]);
    }

    protected override long WorkOneStar_Implementation()
    {
        LogStones();
        for (int i = 0; i < 25; i++)
        {
            _stones.Blink();
            LogStones();
        }
        return _stones.StoneCount;
    }

    private void LogStones()
    {
        if (Logger.ShowAboveSeverity > SeverityLevel.Low)
            return;
        
        Logger.Log(_stones.ToString());
    }
}