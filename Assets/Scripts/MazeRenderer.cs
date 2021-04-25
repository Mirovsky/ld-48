using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

public class MazeRenderer : MonoBehaviour
{
    [Inject] private SignalBus _signalBus;
    [Inject] private MazeObject.Pool _mazeObjectPool;
    [Inject] private MazeColorDataModel _mazeColorDataModel;
    [Inject] private GameManager _gameManager;
    
    private readonly Dictionary<Vector2Int, MazeObject> _spawnedMazeObjects = new Dictionary<Vector2Int, MazeObject>();
    private Maze _currentMaze;
    
    protected void Start()
    {
        _signalBus.Subscribe<MazeGeneratedSignal>(HandleFirstMazeGenerated);
        _signalBus.Subscribe<MazeUpdatedSignal>(HandleMazeUpdated);
        _signalBus.Subscribe<EmptyFuelSignal>(HandleEmptyFuel);
    }

    protected void OnDestroy()
    {
        _signalBus.TryUnsubscribe<MazeGeneratedSignal>(HandleFirstMazeGenerated);
        _signalBus.Unsubscribe<MazeUpdatedSignal>(HandleMazeUpdated);
        _signalBus.TryUnsubscribe<MazeGeneratedSignal>(HandleNextMazeGenerated);
    }

    private void HandleFirstMazeGenerated(MazeGeneratedSignal mazeGeneratedSignal)
    {
        _currentMaze = mazeGeneratedSignal.generatedMaze;

        var offset = ObjectPlacementHelper.GetBlockOffset(_currentMaze.width, _currentMaze.height);
        
        for (var x = 0; x < _currentMaze.width; x++)
        {
            for (var y = 0; y < _currentMaze.height; y++)
            {
                var obj = _currentMaze[x, y];
                var mazeObject = _mazeObjectPool.Spawn(obj, new Vector2Int(x, y),  _mazeColorDataModel.GetColorForType(obj));
                _spawnedMazeObjects.Add(new Vector2Int(x, y),  mazeObject);

                mazeObject.name = $"MazeObject [{x}, {y}]";

                mazeObject.transform.localPosition = offset + ObjectPlacementHelper.GetObjectPosition(x, y);
                mazeObject.Show(false);
            }
        }
        
        _signalBus.Unsubscribe<MazeGeneratedSignal>(HandleFirstMazeGenerated);
        _signalBus.Subscribe<MazeGeneratedSignal>(HandleNextMazeGenerated);
    }

    private void HandleNextMazeGenerated(MazeGeneratedSignal signal)
    {
        _signalBus.Fire<MazeRedrawStarted>();
        
        StartCoroutine(RedrawMaze(signal.generatedMaze));
    }

    private void HandleMazeUpdated(MazeUpdatedSignal signal)
    {
        StartCoroutine(UpdateMaze(signal));
    }

    private IEnumerator UpdateMaze(MazeUpdatedSignal signal)
    {
        var maze = signal.updatedMaze;

        var playerPosition = _gameManager.currentPlayerPosition;

        var depth = 0;
        foreach (var updated in signal.updatedPositions)
        {
            if (depth != updated.Item1)
            {
                depth = updated.Item1;
                yield return new WaitForSeconds(0.15f);
            }
            
            var mazeObject = _spawnedMazeObjects[updated.Item2];
            
            var pos = mazeObject.position;
            var type = maze[pos.x, pos.y];

            var prevObjectType = mazeObject.groundType;
            mazeObject.UpdateObject(type, _mazeColorDataModel.GetColorForType(maze[pos.x, pos.y]));
            if (type == GroundType.Empty && prevObjectType != type)
            {
                mazeObject.Hide(playerPosition != pos, withoutSound: false);
            }
        }   
    }
    
    private IEnumerator RedrawMaze(Maze newMaze)
    {
        yield return null;

        foreach (var mazeObject in _spawnedMazeObjects)
        {
            mazeObject.Value.Hide(false, withoutSound: true);
        }
        
        yield return new WaitForSeconds(0.3f);
        
        ClearMaze();

        if (newMaze == null)
        {
            yield break;
        }
        
        _currentMaze = newMaze;
        
        var offset = ObjectPlacementHelper.GetBlockOffset(_currentMaze.width, _currentMaze.height);
        
        for (var x = 0; x < _currentMaze.width; x++)
        {
            for (var y = 0; y < _currentMaze.height; y++)
            {
                var obj = _currentMaze[x, y];
                var mazeObject = _mazeObjectPool.Spawn(obj, new Vector2Int(x, y),  _mazeColorDataModel.GetColorForType(obj));
                _spawnedMazeObjects.Add(new Vector2Int(x, y), mazeObject);

                mazeObject.name = $"MazeObject [{x}, {y}]";

                mazeObject.transform.localPosition = offset + ObjectPlacementHelper.GetObjectPosition(x, y);
                mazeObject.Show(false);
            }
            
            yield return null;
        }
        
        yield return new WaitForSeconds(0.3f);
        
        _signalBus.Fire<MazeRedrawFinished>();
    } 

    private void HandleEmptyFuel()
    {
        StartCoroutine(RedrawMaze(null));
        StartCoroutine(SwapLevels());
    }

    private IEnumerator SwapLevels()
    {
        yield return new WaitForSeconds(0.4f);
        
        SceneManager.LoadScene("Menu");
    }
    
    private void ClearMaze()
    {
        foreach (var mazeObject in _spawnedMazeObjects)
        {
            _mazeObjectPool.Despawn(mazeObject.Value);
        }
        _spawnedMazeObjects.Clear();
    }
}
