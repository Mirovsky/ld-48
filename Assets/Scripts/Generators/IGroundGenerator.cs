using UnityEngine;

public enum GroundType
{
    Empty,
    
    Ground1,
    Ground2,
    Ground3,
    Ground4,
    Ground5,
    
    Door,
    Fuel,
    Gold
}

public class Maze
{
    private readonly GroundType[,] _ground;

    public Maze(int width, int height, GroundType[,] ground)
    {
        this.width = width;
        this.height = height;
        _ground = ground;
    }
    
    public int width { get; }
    public int height { get; }

    public Vector2Int start => _start;
    public Vector2Int end => _end;

    private Vector2Int _start;
    private Vector2Int _end;

    public GroundType this[int i, int j]
    {
        get => _ground[i, j];
        set => _ground[i, j] = value;
    }

    public void SetStart(int x, int y)
    {
        _start = new Vector2Int(x, y);
        _ground[x, y] = GroundType.Empty;
    }

    public void SetEnd(int x, int y)
    {
        _end = new Vector2Int(x, y);
        _ground[x, y] = GroundType.Door;
    }
}

public interface IGroundGenerator
{
    Maze GenerateMaze(int width, int height, GroundType[] availableGroundTypes);
}
