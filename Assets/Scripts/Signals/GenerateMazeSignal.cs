using System;
using System.Collections.Generic;
using UnityEngine;

public class GenerateMazeSignal
{
    public Vector2Int playerSpace;
    
    public Vector2Int size;
    public int availableTypeCount;
}

public class MazeGeneratedSignal
{
    public Maze generatedMaze;
}

public class MazeUpdatedSignal
{
    public Maze updatedMaze;
    public List<Tuple<int, Vector2Int>> updatedPositions;
}

public class MazeFinishedSignal { }

public class MazeRedrawStarted { }

public class MazeRedrawFinished { }
