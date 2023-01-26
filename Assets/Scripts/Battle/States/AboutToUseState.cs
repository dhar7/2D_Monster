using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AboutToUseState : State<BattleSystem>
{
    [SerializeField] ChoiceSelectionUI choiceSelectionUI;

    public static AboutToUseState Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
    }

    BattleSystem battleSystem;
    StateMachine<BattleSystem> stateMachine;
    PartyScreen partyScreen;
    public override void Enter(BattleSystem battleSystem)
    {
        this.battleSystem = battleSystem;
        stateMachine = battleSystem.StateMachine;
        partyScreen = battleSystem.PartyScreen;

        StartCoroutine(EnterAsync());
    }

    IEnumerator EnterAsync()
    {
        yield return battleSystem.DialogBox.TypeDialog($"{battleSystem.Trainer.Name} is about to use {battleSystem.NextPokemon.Base.Name}. Do you want to change pokemon?");

        choiceSelectionUI.gameObject.SetActive(true);
        choiceSelectionUI.OnSelected += OnChoiceSelected;
        choiceSelectionUI.OnBack += OnBack;

        partyScreen.OnSelected += OnPokmeonSelected;
    } 

    public override void Execute(BattleSystem battleSystem)
    {
        if (choiceSelectionUI.gameObject.activeSelf)
            choiceSelectionUI.HandleUpdate();
    }

    public override void Exit(BattleSystem entity)
    {
        battleSystem.NextPokemon = null;

        choiceSelectionUI.gameObject.SetActive(false);
        choiceSelectionUI.OnSelected -= OnChoiceSelected;
        choiceSelectionUI.OnBack -= OnBack;

        partyScreen.OnSelected -= OnPokmeonSelected;
    }

    void OnChoiceSelected(int selectedChoice)
    {
        if (selectedChoice == 0)
        {
            // Yes
            stateMachine.PushState(BattlePartyState.Instance);
        }
        else
        {
            // No
            StartCoroutine(ContinueBattle());
        }
    }

    void OnBack()
    {
        StartCoroutine(ContinueBattle());
    }

    void OnPokmeonSelected(int selectedPokemon)
    {
        var selectedMember = battleSystem.PlayerParty.Pokemons[selectedPokemon];
        if (selectedMember.HP <= 0)
        {
            partyScreen.SetMessageText("You can't send out a fainted pokemon");
            return;
        }
        if (selectedMember == battleSystem.PlayerUnit.Pokemon)
        {
            partyScreen.SetMessageText("You can't switch with the same pokemon");
            return;
        }

        stateMachine.PopState(); // Pop party state
        StartCoroutine(SwitchAndConitnueBattle(partyScreen.Pokemons[selectedPokemon]));
    }

    IEnumerator SwitchAndConitnueBattle(Pokemon newPokemon)
    {
        stateMachine.ChangeState(BattleBusyState.Instance);
        yield return battleSystem.SwitchPokemon(newPokemon);
        yield return battleSystem.SendNextTrainerPokemon();
        stateMachine.PopState(); // Pop busy state
    }

    IEnumerator ContinueBattle()
    {
        stateMachine.ChangeState(BattleBusyState.Instance);
        yield return battleSystem.SendNextTrainerPokemon();
        stateMachine.PopState(); // Pop busy
    }
}
