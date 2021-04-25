using UnityEngine;

public class BasicGroundGenerator : IGroundGenerator
{
    public Maze GenerateMaze(int width, int height, GroundType[] availableGroundTypes)
    {
        var grid = new GroundType[width, height];
        
        for (var i = 0; i < width; i++)
        {
            for (var j = 0; j < height; j++)
            {
                var randomColor = Random.Range(0, availableGroundTypes.Length);

                grid[i, j] = availableGroundTypes[randomColor];
            }
        }
        
        return new Maze(width, height, grid);
    }

    public void GenerateEnd(int width, int height, Maze maze)
    {
        var x = Random.Range(0, width);
        var y = Random.Range(0, height);

        while (maze[x, y] == GroundType.Empty)
        {
            x = Random.Range(0, width);
            y = Random.Range(0, height);
        }

        maze.SetEnd(x, y);
    }
}
