using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameFreeRoamState : State<GameController>
{
    public static GameFreeRoamState Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
    }

    PlayerController playerController;
    public override void Enter(GameController gc)
    {
        playerController = gc.PlayerController;
    }

    public override void Execute(GameController gc)
    {
        playerController.HandleUpdate();

        if (Input.GetKeyDown(KeyCode.Return))
        {
            gc.StateMachine.ChangeState(GameMenuState.Instance);
        }
    }
}
