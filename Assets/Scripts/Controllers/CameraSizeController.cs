using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class CameraSizeController : MonoBehaviour
{
    [SerializeField] private Camera _camera;

    [Inject] private SignalBus _signalBus;
    
    protected void Start()
    {
        _signalBus.Subscribe<MazeGeneratedSignal>(HandleMazeGeneratedSignal);
    }

    protected void OnDestroy()
    {
        _signalBus.Unsubscribe<MazeGeneratedSignal>(HandleMazeGeneratedSignal);
    }

    private void HandleMazeGeneratedSignal(MazeGeneratedSignal signal)
    {
        _camera.orthographicSize = signal.generatedMaze.height * 0.5f;
    }
}
