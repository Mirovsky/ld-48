using UnityEngine;
using Zenject;

public class GameManager : MonoBehaviour
{
    [SerializeField] private int _fuelPerBlock;
    [SerializeField] private int _maxFuel;
    [SerializeField] private int _currentFuel;
    
    [SerializeField] private Vector2Int _gridSize;
    [SerializeField] private int _groundTypesCount = 2;
    [SerializeField] private float _grow = 0;

    [Space]
    [SerializeField] private Vector2Int _currentPlayerPosition = new Vector2Int(2, 0);
    [SerializeField] private Vector3Int _currentMazePosition = new Vector3Int(0, 0, 0);

    [Inject] private SignalBus _signalBus;
    [Inject] private InGameUIController _inGameUIController;

    public Vector2Int gridSize => _gridSize;
    public int groundTypesCount => _groundTypesCount;
    public Vector2Int currentPlayerPosition
    {
        get => _currentPlayerPosition;
        set => _currentPlayerPosition = value;
    }
    public Vector3Int currentMazePosition
    {
        get => _currentMazePosition;
        set => _currentMazePosition = value;
    }
    public int currentFuel
    {
        get => _currentFuel;
        set => _currentFuel = value;
    }

    public int maxFuel
    {
        get => _maxFuel;
        set => _maxFuel = value;
    }

    public int fuelPerBlock => _fuelPerBlock;

    private Vector2Int _prevGridSize;

    protected void Start()
    {
        _signalBus.Subscribe<MazeFinishedSignal>(HandleGenerateNewMazeSignal);
        _signalBus.Subscribe<EmptyFuelSignal>(HandleEmptyFuelSignal);
        
        _signalBus.Fire<GameStartedSignal>();

        _signalBus.Fire(new GenerateMazeSignal
        {
            playerSpace = _currentPlayerPosition,
            size = _gridSize,
            availableTypeCount = groundTypesCount
        });
        _prevGridSize = _gridSize;
        
        _signalBus.Fire(new SpawnPlayerSignal
        {
            position = _currentPlayerPosition
        });
    }

    protected void OnDestroy()
    {
        _signalBus.Unsubscribe<MazeFinishedSignal>(HandleGenerateNewMazeSignal);
        _signalBus.Unsubscribe<EmptyFuelSignal>(HandleEmptyFuelSignal);
    }

    private void HandleGenerateNewMazeSignal()
    {
        _currentMazePosition += Vector3Int.forward;

        var newGridSize = _gridSize + Vector2Int.one * (int) Mathf.Pow(2, _grow);
        newGridSize = new Vector2Int((int)Mathf.Min(newGridSize.x, 32), (int)Mathf.Min(newGridSize.y, 32));
        
        _signalBus.Fire(new GenerateMazeSignal
        {
            playerSpace = _currentPlayerPosition,
            size = newGridSize,
            availableTypeCount = groundTypesCount + _currentMazePosition.z
        });
        _grow += 0.2f;    
        
        _signalBus.Fire(new SpawnPlayerSignal
        {
            position = _currentPlayerPosition
        });

        _prevGridSize = newGridSize;
    }

    private void HandleEmptyFuelSignal()
    {
        _inGameUIController.ShowGameOver();
    }
}
