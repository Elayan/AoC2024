using AoC2024.Structures;
using AoCTools.Loggers;
using AoCTools.Workers;

namespace AoC2024.Workers.Day17;

public class Compiler : WorkerBase
{
    private Computer _computer;
    public override object Data => _computer;

    protected override void ProcessDataLines()
    {
        _computer = new Computer(DataLines);
    }

    protected override string WorkOneStar_String_Implementation()
    {
        var result = _computer.Execute();
        Logger.Log(_computer.ToString());
        return result;
    }
}