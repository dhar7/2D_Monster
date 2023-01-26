using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattlePartyState : State<BattleSystem>
{
    public static BattlePartyState Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
    }

    BattleSystem battleSystem;
    PartyScreen partyScreen;
    public override void Enter(BattleSystem battleSystem)
    {
        this.battleSystem = battleSystem;
        partyScreen = battleSystem.PartyScreen;

        partyScreen.gameObject.SetActive(true);
        partyScreen.OnSelected += OnPokemonSelected;
        partyScreen.OnBack += OnBack;

        partyScreen.SetMessageText("Choose a pokemon");
    }

    public override void Execute(BattleSystem battleSystem)
    {
        partyScreen.HandleUpdate();
    }

    public override void Exit(BattleSystem battleSystem)
    {
        partyScreen.gameObject.SetActive(false);
        partyScreen.OnSelected -= OnPokemonSelected;
        partyScreen.OnBack -= OnBack;
    }

    void OnPokemonSelected(int selectedPokemon)
    {
        
    }

    void OnBack()
    {
        battleSystem.StateMachine.PopState();
    }
}
