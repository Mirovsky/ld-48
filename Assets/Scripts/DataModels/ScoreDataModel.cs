using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class ScoreDataModel : MonoBehaviour
{
    [Inject] private SignalBus _signalBus;
    
    private readonly Dictionary<GroundType, int> _score = new Dictionary<GroundType, int>();

    public Dictionary<GroundType, int> score => _score;

    public void Add(GroundType type, int score)
    {
        if (_score.ContainsKey(type))
        {
            _score[type] += score;
        }
        else
        {
            _score[type] = score;
        }

        _signalBus.Fire<ScoreUpdatedSignal>();
    }

    public void Clear()
    {
        _score.Clear();
    }
}
