using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Video;
using Zenject;

public class MazeTransformationController : MonoBehaviour
{
    [Inject] private SignalBus _signalBus;
    [Inject] private GameManager _gameManager;
    [Inject] private MazesDataModel _mazesDataModel;
    [Inject] private ScoreDataModel _scoreDataModel;
    
    protected void Start()
    {
        _signalBus.Subscribe<PlayerMovedSignal>(HandlePlayerMoved);
    }

    protected void OnDestroy()
    {
        _signalBus.Unsubscribe<PlayerMovedSignal>(HandlePlayerMoved);
    }

    private void HandlePlayerMoved(PlayerMovedSignal signal)
    {
        var position = _gameManager.currentPlayerPosition;
        var maze = _mazesDataModel.currentMaze;

        var type = maze[position.x, position.y];
        if (type == GroundType.Empty || type == GroundType.Door)
        {
            return;
        }
        
        FloodClearMaze(maze, position);
    }

    private void FloodClearMaze(Maze maze, Vector2Int position)
    {
        var queue = new Queue<Tuple<int, Vector2Int>>();
        queue.Enqueue(new Tuple<int, Vector2Int>(0, position));

        var typeToClear = maze[position.x, position.y];
        var visited = new bool[maze.width, maze.height];

        var updatedPositions = new List<Tuple<int, Vector2Int>>();
        
        while (queue.Count != 0)
        {
            var tuple = queue.Dequeue();
            var pos = tuple.Item2;
            
            _scoreDataModel.Add(maze[pos.x, pos.y],1);
            maze[pos.x, pos.y] = GroundType.Empty;

            updatedPositions.Add(tuple);
            
            if (pos.x - 1 >= 0 && maze[pos.x - 1, pos.y] == typeToClear && !visited[pos.x - 1, pos.y])
            {
                visited[pos.x - 1, pos.y] = true;
                queue.Enqueue(new Tuple<int, Vector2Int>(tuple.Item1 + 1, new Vector2Int(pos.x - 1, pos.y)));
            }
            if (pos.x + 1 < maze.width && maze[pos.x + 1, pos.y] == typeToClear && !visited[pos.x + 1, pos.y])
            {
                visited[pos.x + 1, pos.y] = true;
                queue.Enqueue(new Tuple<int, Vector2Int>(tuple.Item1 + 1, new Vector2Int(pos.x + 1, pos.y)));
            }
            if (pos.y - 1 >= 0 && maze[pos.x, pos.y - 1] == typeToClear && !visited[pos.x, pos.y - 1])
            {
                visited[pos.x, pos.y - 1] = true;
                queue.Enqueue(new Tuple<int, Vector2Int>(tuple.Item1 + 1, new Vector2Int(pos.x, pos.y - 1)));
            }
            if (pos.y + 1 < maze.height && maze[pos.x, pos.y + 1] == typeToClear && !visited[pos.x, pos.y + 1])
            {
                visited[pos.x, pos.y + 1] = true;
                queue.Enqueue(new Tuple<int, Vector2Int>(tuple.Item1 + 1, new Vector2Int(pos.x, pos.y + 1)));
            }
        }

        _signalBus.Fire(new MazeUpdatedSignal
        {
            updatedMaze = maze,
            updatedPositions = updatedPositions
        });
    }
}
