using UnityEngine;
using Zenject;

public class PlayerFuelController : MonoBehaviour
{
    [SerializeField] private AudioClip _emptyFuelAudioClip;
    [SerializeField] private AudioSource _audioSource;
    
    [Inject] private readonly SignalBus _signalBus;
    [Inject] private readonly GameManager _gameManager;
    [Inject] private readonly MazesDataModel _mazesDataModel;
    
    protected void Start()
    {
        _signalBus.Subscribe<PlayerMovedSignal>(HandlePlayerMoved);
        _signalBus.Subscribe<MazeGeneratedSignal>(HandleMazeGenerated);
        _signalBus.Subscribe<PlayerEndMoveSignal>(HandlePlayerEndMove);
    }

    protected void OnDestroy()
    {
        _signalBus.Unsubscribe<PlayerMovedSignal>(HandlePlayerMoved);
        _signalBus.Unsubscribe<MazeGeneratedSignal>(HandleMazeGenerated);
        _signalBus.Unsubscribe<PlayerEndMoveSignal>(HandlePlayerEndMove);
    }

    private void HandlePlayerMoved(PlayerMovedSignal signal)
    {
        var position = _gameManager.currentPlayerPosition;
        var maze = _mazesDataModel.currentMaze;
        var type = maze[position.x, position.y];
        
        if (type != GroundType.Empty && type != GroundType.Door && type != GroundType.Fuel && type != GroundType.Gold)
        {
            _gameManager.currentFuel -= _gameManager.fuelPerBlock;
            _signalBus.Fire<FuelUpdateSignal>();
        }
    }

    private void HandlePlayerEndMove(PlayerEndMoveSignal signal)
    {
        if (_gameManager.currentFuel == 0)
        {
            _audioSource.Stop();
            _audioSource.clip = _emptyFuelAudioClip;
            _audioSource.Play();
            
            _signalBus.Fire<EmptyFuelSignal>();
        }
    }

    private void HandleMazeGenerated(MazeGeneratedSignal signal)
    {
        var maze = signal.generatedMaze;

        var start = maze.start;
        var end = maze.end;

        /* var x1 = Mathf.Min(start.x, end.x);
        var x2 = Mathf.Max(start.x, end.x);
        var y1 = Mathf.Min(start.y, end.y);
        var y2 = Mathf.Max(start.y, end.y);
        
        var colorSet = new HashSet<GroundType>();
        
        var n = 2 * (y2 - y1);
        var slope_error = n - (x2 - x1);
        for (var (x, y) = (x1, y1); x <= x2; x++)
        {
            var type = maze[x, y];
            if (type != GroundType.Empty && type != GroundType.Door && type != GroundType.Fuel && type != GroundType.Gold)
            {
                colorSet.Add(type);
            }
            
            slope_error += n;

            if (slope_error >= 0)
            {
                y++;
                slope_error -= 2 * (x2 - x1);
            }
        } */

        _gameManager.maxFuel = _gameManager.fuelPerBlock * Mathf.RoundToInt(Vector2Int.Distance(start, end)) + 1;
        _gameManager.currentFuel = _gameManager.maxFuel;
        
        _signalBus.Fire(new FuelUpdateSignal { refresh = true });
    }
}
