using System.Text.RegularExpressions;
using AoCTools.Frame.TwoDimensions;
using AoCTools.Loggers;

namespace AoC2024.Structures;

public class ToiletRobot
{
    private static string _lineParserRegexPattern = @"p=(?<px>-?[0-9]+),(?<py>-?[0-9]+) v=(?<vx>-?[0-9]+),(?<vy>-?[0-9]+)";
    private Regex _lineParserRegex = new(_lineParserRegexPattern, RegexOptions.Compiled);

    public Coordinates Position { get; private set; }
    private Coordinates _velocity;
    
    public ToiletRobot(string line)
    {
        var match = _lineParserRegex.Match(line);
        Position = new Coordinates(long.Parse(match.Groups["py"].Value), long.Parse(match.Groups["px"].Value));
        _velocity = new Coordinates(long.Parse(match.Groups["vy"].Value), long.Parse(match.Groups["vx"].Value));
    }

    public void Walk(long steps, Coordinates worldBounds)
    {
        Position += steps * _velocity;
        
        Position.Row %= worldBounds.Row;
        if (Position.Row < 0)
            Position.Row += worldBounds.Row;
        
        Position.Col %= worldBounds.Col;
        if (Position.Col < 0)
            Position.Col += worldBounds.Col;
    }

    public override string ToString()
    {
        return $"p={Position} v={_velocity}";
    }
}