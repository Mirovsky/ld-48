using UnityEngine;
using Zenject;

public class PlayerMovementController : MonoBehaviour
{
    [SerializeField] private AudioClip _nextLevelAudioClip;
    [SerializeField] private AudioSource _audioSource;
    
    [Inject] private SignalBus _signalBus;
    [Inject] private GameManager _gameManager;
    [Inject] private MazesDataModel _mazesDataModel;
    
    protected void Start()
    {
        _signalBus.Subscribe<PlayerMoveSignal>(HandlePlayerMove);
        _signalBus.Subscribe<PlayerEndMoveSignal>(HandlePlayerEndMove);
    }

    protected void OnDestroy()
    {
        _signalBus.Unsubscribe<PlayerMoveSignal>(HandlePlayerMove);
        _signalBus.Unsubscribe<PlayerEndMoveSignal>(HandlePlayerEndMove);
    }

    private void HandlePlayerMove(PlayerMoveSignal signal)
    {
        var maze = _mazesDataModel.currentMaze;

        var newPosition = _gameManager.currentPlayerPosition + signal.direction;
        if (newPosition.x < 0 || newPosition.y < 0 || newPosition.x >= maze.width || newPosition.y >= maze.height)
        {
            return;
        }

        _gameManager.currentPlayerPosition = new Vector2Int((int)newPosition.x, (int)newPosition.y);
        
        _signalBus.Fire(new PlayerMovedSignal());
    }

    private void HandlePlayerEndMove()
    {
        var maze = _mazesDataModel.currentMaze;
        
        var type = maze[_gameManager.currentPlayerPosition.x, _gameManager.currentPlayerPosition.y];
        if (type == GroundType.Door)
        {
            _audioSource.Stop();
            _audioSource.clip = _nextLevelAudioClip;
            _audioSource.Play();
            _signalBus.Fire<MazeFinishedSignal>();
        }
    }
}
