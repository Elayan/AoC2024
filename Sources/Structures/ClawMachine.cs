namespace AoC2024.Structures;

public class ClawMachine
{
    public long AX { get; private set; }
    public long AY { get; private set; }
    public long BX { get; private set; }
    public long BY { get; private set; }
    public long PrizeX { get; private set; }
    public long PrizeY { get; private set; }

    public ClawMachine(long ax, long ay, long bx, long by, long prizeX, long prizeY)
    {
        AX = ax;
        AY = ay;
        BX = bx;
        BY = by;
        PrizeX = prizeX;
        PrizeY = prizeY;
    }

    public override string ToString()
    {
        return $"A: {AX} {AY} - B: {BX} {BY} - Prize: {PrizeX} {PrizeY}";
    }
}