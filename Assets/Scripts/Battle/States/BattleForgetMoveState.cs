using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BattleForgetMoveState : State<BattleSystem>
{
    [SerializeField] MoveSelectionBoxUI moveSelectionBox;

    public static BattleForgetMoveState Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
    }

    Pokemon currPokemon;
    MoveBase newMove;
    public void SetData(Pokemon pokemon, MoveBase newMove)
    {
        currPokemon = pokemon;
        this.newMove = newMove;
    }

    BattleSystem battleSystem;
    StateMachine<BattleSystem> stateMachine;
    BattleDialogBox dialogBox;
    public override void Enter(BattleSystem battleSystem)
    {
        this.battleSystem = battleSystem;
        stateMachine = battleSystem.StateMachine;
        dialogBox = battleSystem.DialogBox;

        StartCoroutine(EnterAsync());
    }

    IEnumerator EnterAsync()
    {
        yield return dialogBox.TypeDialog("Choose a move you wan't to forget");

        moveSelectionBox.gameObject.SetActive(true);
        moveSelectionBox.SetMoveData(currPokemon.Moves.Select(x => x.Base).ToList(), newMove);
        moveSelectionBox.OnSelected += OnMoveSelected;
    }

    public override void Execute(BattleSystem battleSystem)
    {
        if (moveSelectionBox.gameObject.activeSelf)
            moveSelectionBox.HandleUpdate();
    }

    public override void Exit(BattleSystem battleSystem)
    {
        moveSelectionBox.gameObject.SetActive(false);
        moveSelectionBox.OnSelected -= OnMoveSelected;
    }

    void OnMoveSelected(int moveIndex)
    {
        StartCoroutine(ForgetMove(moveIndex));
    }

    IEnumerator ForgetMove(int moveIndex)
    {
        stateMachine.ChangeState(BattleBusyState.Instance);

        if (moveIndex == PokemonBase.MaxNumOfMoves)
        {
            // Don't learn the new move
            yield return dialogBox.TypeDialog($"{currPokemon.Base.Name} did not learn {newMove.Name}");
        }
        else
        {
            // Forget the selected move and learn new move
            var selectedMove = currPokemon.Moves[moveIndex].Base;
            yield return dialogBox.TypeDialog($"{currPokemon.Base.Name} forgot {selectedMove.Name} and learned {newMove.Name}");

            currPokemon.Moves[moveIndex] = new Move(newMove);
        }

        currPokemon = null;
        newMove = null;
        stateMachine.PopState();
    }
}
