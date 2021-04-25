using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

public class UIController : MonoBehaviour
{
    [SerializeField] private Button _playGameButton;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private RectTransform _scoreContainer;

    [Inject] private ScoreDataModel _scoreDataModel;
    [Inject] private ScoreItem.Pool _scoreItemPool;
    [Inject] private MazeColorDataModel _mazeColorDataModel;

    private readonly List<ScoreItem> _spawnedScoreItems = new List<ScoreItem>();
    
    protected void Start()
    {
        if (_scoreDataModel.score.Count != 0)
        {
            ShowScore();    
        }
        
        _playGameButton.onClick.AddListener(HandlePlayButtonClick);
    }

    protected void OnDestroy()
    {
        _playGameButton.onClick.RemoveListener(HandlePlayButtonClick);
    }

    private void HandlePlayButtonClick()
    {
        _scoreDataModel.Clear();
        _audioSource.Play();
        
        SceneManager.LoadScene("Game");
    }

    private void ShowScore()
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
}
