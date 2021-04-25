using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class InGameUIController : MonoBehaviour
{
    [SerializeField] private RectTransform _scoreContainer;
    [SerializeField] private RectTransform _fuelBlockContainer;
    [SerializeField] private VerticalLayoutGroup _verticalLayoutGroup;
    
    [SerializeField] private GameObject _gameOverGameObject;
    
    [Inject] private SignalBus _signalBus;
    [Inject] private ScoreDataModel _scoreDataModel;
    [Inject] private ScoreItem.Pool _scoreItemPool;
    [Inject] private FuelBlock.Pool _fuelBlockPool;
    [Inject] private MazeColorDataModel _mazeColorDataModel;
    [Inject] private GameManager _gameManager;
    
    private readonly List<ScoreItem> _spawnedScoreItems = new List<ScoreItem>();
    private readonly List<FuelBlock> _spawnedFuelBlocks = new List<FuelBlock>();
    
    public void ShowGameOver()
    {
        _gameOverGameObject.SetActive(true);
    }

    protected void Start()
    {
        _signalBus.Subscribe<ScoreUpdatedSignal>(HandleScoreUpdated);
        _signalBus.Subscribe<FuelUpdateSignal>(HandleFuelUpdated);
    }

    protected void OnDestroy()
    {
        _signalBus.Unsubscribe<ScoreUpdatedSignal>(HandleScoreUpdated);
        _signalBus.Unsubscribe<FuelUpdateSignal>(HandleFuelUpdated);
    }

    private void HandleScoreUpdated()
    {
        foreach (var i in _spawnedScoreItems)
        {
            _scoreItemPool.Despawn(i);   
        }
        _spawnedScoreItems.Clear();

        foreach (var score in _scoreDataModel.score)
        {
            var t = score.Key;
            var s = score.Value;

            for (var i = 0; i < s; i++)
            {
                var scoreItem = _scoreItemPool.Spawn();
                scoreItem.color = _mazeColorDataModel.GetColorForType(t);
                _spawnedScoreItems.Add(scoreItem);
                
                scoreItem.rectTransform.SetParent(_scoreContainer, worldPositionStays: false);
            }
        }
    }

    private void HandleFuelUpdated(FuelUpdateSignal signal)
    {
        if (signal.refresh)
        {
            _verticalLayoutGroup.enabled = true;
            
            foreach (var fb in _spawnedFuelBlocks)
            {
                _fuelBlockPool.Despawn(fb);
            }
            _spawnedFuelBlocks.Clear();

            for (var i = 0; i < _gameManager.maxFuel; i++)
            {
                var fb = _fuelBlockPool.Spawn();
                _spawnedFuelBlocks.Add(fb);
                
                fb.transform.SetParent(_fuelBlockContainer, worldPositionStays: false);
            }

            LayoutRebuilder.ForceRebuildLayoutImmediate(_fuelBlockContainer);
            _verticalLayoutGroup.enabled = false;

            return;
        }

        var index = _gameManager.maxFuel - _gameManager.currentFuel - 1;
        if (index < 0 || index >= _spawnedFuelBlocks.Count) {
            return;
        }
        _spawnedFuelBlocks[index].gameObject.SetActive(false);
    }
}
