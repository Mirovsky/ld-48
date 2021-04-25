using UnityEngine;
using Zenject;

public class PlayerCharacterController : MonoBehaviour
{
    [SerializeField] private Transform _playerTransform;

    [Space]
    [SerializeField] private float _transitionDuration;
    
    [Inject] private SignalBus _signalBus;
    [Inject] private GameManager _gameManager;
    [Inject] private MazesDataModel _mazesDataModel;

    private Vector3 _originPosition;
    private Vector3 _targetPosition;
    private bool _move;
    private float _currentT;

    protected void Start()
    {
        _signalBus.Subscribe<PlayerMovedSignal>(HandlePlayerMoved);
        _signalBus.Subscribe<SpawnPlayerSignal>(HandleSpawnPlayer);
    }

    protected void OnDestroy()
    {
        _signalBus.Unsubscribe<PlayerMovedSignal>(HandlePlayerMoved);
        _signalBus.Unsubscribe<SpawnPlayerSignal>(HandleSpawnPlayer);
    }

    protected void Update()
    {
        if (!_move)
        {
            return;
        }

        if (_currentT > _transitionDuration)
        {
            _playerTransform.position = _targetPosition;
            _move = false;
            _signalBus.Fire<PlayerEndMoveSignal>();
            return;
        }

        _playerTransform.position = Vector3.Lerp(_originPosition, _targetPosition, _currentT / _transitionDuration);

        _currentT += Time.deltaTime;
    }

    private void HandleSpawnPlayer(SpawnPlayerSignal signal)
    {
        var maze = _mazesDataModel.currentMaze;
        var position = signal.position;
        
        MovePlayerInstant(position.x, position.y, maze.width, maze.height);
    }

    private void HandlePlayerMoved()
    {
        var maze = _mazesDataModel.currentMaze;
        
        var position = _gameManager.currentPlayerPosition;

        MovePlayer(position.x, position.y, maze.width, maze.height);        
    }

    private void MovePlayerInstant(float x, float y, float mazeWidth, float mazeHeight)
    {
        _playerTransform.position = ObjectPlacementHelper.GetBlockOffset(mazeWidth, mazeHeight) +
                                    ObjectPlacementHelper.GetObjectPosition(x, y);
    }
    
    private void MovePlayer(float x, float y, float mazeWidth, float mazeHeight)
    {
        _signalBus.Fire<PlayerStartMoveSignal>();
        
        _originPosition = _playerTransform.position;
     
        _targetPosition = ObjectPlacementHelper.GetBlockOffset(mazeWidth, mazeHeight) +
                          ObjectPlacementHelper.GetObjectPosition(x, y);

        _currentT = 0.0f;
        _move = true;
    }
}
