using System.Linq;
using UnityEngine;
using Zenject;

public class MazesDataModel : MonoBehaviour
{
    [Inject] private SignalBus _signalBus;
    [Inject] private MazeColorDataModel _mazeColorDataModel;

    private Maze _maze;

    public Maze currentMaze => _maze;
    protected void Start()
    {
        _signalBus.Subscribe<GenerateMazeSignal>(HandleGenerateGround);
    }

    protected void OnDestroy()
    {
        _signalBus.Unsubscribe<GenerateMazeSignal>(HandleGenerateGround);
    }

    private void HandleGenerateGround(GenerateMazeSignal signal)
    {
        var mazeGenerator = new BasicGroundGenerator();
        
        var maze = mazeGenerator.GenerateMaze(signal.size.x, signal.size.y, _mazeColorDataModel.groundTypes.Take(signal.availableTypeCount).ToArray());
        maze.SetStart(signal.playerSpace.x, signal.playerSpace.y);
        mazeGenerator.GenerateEnd(signal.size.x, signal.size.y, maze);

        _maze = maze;
        
        _signalBus.Fire(new MazeGeneratedSignal { generatedMaze = maze });
    }
}
