using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInputManager : SingletonMono<GameInputManager>
{
    private GameInputAction _gameInputAction;
    public Vector2 Movement => _gameInputAction.GameInput.Movement.ReadValue<Vector2>();
    public bool Run => _gameInputAction.GameInput.Run.triggered;
    protected override void Awake()
    {
        base.Awake();
        _gameInputAction??= new GameInputAction();
    }

    private void OnEnable()
    {
        _gameInputAction.Enable();
    }
    private void OnDisable()
    {
        _gameInputAction.Disable();
    }
}
