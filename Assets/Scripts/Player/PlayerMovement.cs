using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

public class PlayerMovement : MonoBehaviour
{
    [Inject] private SignalBus _signalBus;
    
    private PlayerControls _playerControls;
    private bool _canMove = true;

    protected void OnEnable()
    {
        if (_playerControls == null)
        {
           _playerControls = new PlayerControls();
        }
        
        _playerControls.Enable();

        _playerControls.Player.Movement.performed += HandleMove;
    }
    
    protected void OnDisable()
    {
        _playerControls.Disable();
    }

    protected void Start()
    {
        _signalBus.Subscribe<PlayerStartMoveSignal>(HandlePlayerStartMove);
        _signalBus.Subscribe<PlayerEndMoveSignal>(HandlePlayerEndMove);
        _signalBus.Subscribe<MazeRedrawStarted>(HandlePlayerStartMove);
        _signalBus.Subscribe<MazeRedrawFinished>(HandlePlayerEndMove);
    }

    protected void OnDestroy()
    {
        _signalBus.Unsubscribe<PlayerStartMoveSignal>(HandlePlayerStartMove);
        _signalBus.Unsubscribe<PlayerEndMoveSignal>(HandlePlayerEndMove);
        _signalBus.Unsubscribe<MazeRedrawStarted>(HandlePlayerStartMove);
        _signalBus.Unsubscribe<MazeRedrawFinished>(HandlePlayerEndMove);
    }

    private void HandleMove(InputAction.CallbackContext ctx)
    {
        if (!_canMove)
        {
            return;
        }
        
        var move = _playerControls.Player.Movement.ReadValue<Vector2>();
        
        _signalBus.Fire(new PlayerMoveSignal { direction = move });
    }

    private void HandlePlayerStartMove()
    {
        _canMove = false;
    }

    private void HandlePlayerEndMove()
    {
        _canMove = true;
    }
}
