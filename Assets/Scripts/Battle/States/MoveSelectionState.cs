using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveSelectionState : State<BattleSystem>
{
    public static MoveSelectionState Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
    }

    BattleSystem _battleSystem;
    MoveSelectionUI moveSelector;
    public override void Enter(BattleSystem battleSystem)
    {
        _battleSystem = battleSystem;
        moveSelector = battleSystem.MoveSelector;

        _battleSystem.DialogBox.EnableDialogText(false);
        moveSelector.gameObject.SetActive(true);
        moveSelector.SetMoveNames(_battleSystem.PlayerUnit.Pokemon.Moves);

        moveSelector.OnSelected += OnMoveSelected;
        moveSelector.OnSelectionChanged += OnSelectionChanged;
        moveSelector.OnBack += OnBack;
    }

    public override void Execute(BattleSystem battleSystem)
    {
        moveSelector.HandleUpdate();
    }

    public override void Exit(BattleSystem entity)
    {
        moveSelector.gameObject.SetActive(false);
        _battleSystem.DialogBox.EnableDialogText(true);

        moveSelector.OnSelected -= OnMoveSelected;
        moveSelector.OnSelectionChanged -= OnSelectionChanged;
        moveSelector.OnBack -= OnBack;
    }

    void OnMoveSelected(int selectedMove)
    {
        _battleSystem.SelectedAction = BattleAction.Move;
        _battleSystem.SelectedMove = selectedMove;

        _battleSystem.StateMachine.ChangeState(RunTurnState.Instance);
    }

    void OnSelectionChanged(int selectedMove)
    {
        moveSelector.SetMoveDetails(_battleSystem.PlayerUnit.Pokemon.Moves[selectedMove]);
    }

    void OnBack()
    {
        _battleSystem.StateMachine.ChangeState(ActionSelectionState.Instance);
    }
}
