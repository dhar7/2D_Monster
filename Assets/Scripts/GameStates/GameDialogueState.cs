using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameDialogueState : State<GameController>
{

    public static GameDialogueState Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
    }

    GameController _gc;
    public override void Enter(GameController gc)
    {
        _gc = gc;

        DialogManager.Instance.OnCloseDialog += OnDialogueClosed;
    }

    public override void Execute(GameController gc)
    {
        DialogManager.Instance.HandleUpdate();
    }

    public override void Exit(GameController entity)
    {
        DialogManager.Instance.OnCloseDialog -= OnDialogueClosed;
    }

    void OnDialogueClosed()
    {
        _gc.StateMachine.PopState();
    }
}
