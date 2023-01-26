using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionSelectionState : State<BattleSystem>
{
    [SerializeField] ActionSelectionUI selectionUI;

    public static ActionSelectionState Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
    }

    BattleSystem _battleSystem;
    PartyScreen partyScreen;
    public override void Enter(BattleSystem battleSystem)
    {
        _battleSystem = battleSystem;
        partyScreen = battleSystem.PartyScreen;

        selectionUI.gameObject.SetActive(true);
        selectionUI.OnSelected += OnActionSelected;
        partyScreen.OnSelected += OnPokemonSelected;

        battleSystem.DialogBox.SetDialog("Choose an action");
    }

    public override void Execute(BattleSystem battleSystem)
    {
        selectionUI.HandleUpdate();
    }

    public override void Exit(BattleSystem entity)
    {
        selectionUI.gameObject.SetActive(false);
        selectionUI.OnSelected -= OnActionSelected;
        partyScreen.OnSelected -= OnPokemonSelected;
    }

    void OnActionSelected(int selection)
    {
        if (selection == 0)
        {
            // Fight
            _battleSystem.StateMachine.ChangeState(MoveSelectionState.Instance);
        }
        else if (selection == 1)
        {
            // Bag
            _battleSystem.StateMachine.ChangeState(BattleItemState.Instance);
        }
        else if (selection == 2)
        {
            // Pokemon
            _battleSystem.StateMachine.PushState(BattlePartyState.Instance);
        }
        else if (selection == 3)
        {
            // Run
            _battleSystem.SelectedAction = BattleAction.Run;
            _battleSystem.StateMachine.ChangeState(RunTurnState.Instance);
        }
    }

    void OnPokemonSelected(int selectedPokemon)
    {
        // Switch Pokemon
        var selectedMember = _battleSystem.PlayerParty.Pokemons[selectedPokemon];
        if (selectedMember.HP <= 0)
        {
            partyScreen.SetMessageText("You can't send out a fainted pokemon");
            return;
        }
        if (selectedMember == _battleSystem.PlayerUnit.Pokemon)
        {
            partyScreen.SetMessageText("You can't switch with the same pokemon");
            return;
        }

        _battleSystem.StateMachine.PopState(); // Pop the party state

        _battleSystem.SelectedAction = BattleAction.SwitchPokemon;
        _battleSystem.StateMachine.ChangeState(RunTurnState.Instance);
    }
}
