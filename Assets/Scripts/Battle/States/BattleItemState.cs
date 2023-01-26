using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleItemState : State<BattleSystem>
{
    [SerializeField] InventoryUI inventoryUI;

    public static BattleItemState Instance { get; private set; }
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

        inventoryUI.gameObject.SetActive(true);
        inventoryUI.OnSelected += OnItemSelected;
        inventoryUI.OnBack += OnBack;
        inventoryUI.OnItemUsed += OnItemUsed;

        partyScreen.OnSelected += OnPokemonSelected;
    }

    public override void Execute(BattleSystem battleSystem)
    {
        inventoryUI.HandleUpdate();
    }

    public override void Exit(BattleSystem battleSystem)
    {
        inventoryUI.gameObject.SetActive(false);

        inventoryUI.OnSelected -= OnItemSelected;
        inventoryUI.OnBack -= OnBack;
        inventoryUI.OnItemUsed -= OnItemUsed;

        partyScreen.OnSelected -= OnPokemonSelected;
    }

    void OnItemSelected(int selectedItem)
    {
        battleSystem.StateMachine.PushState(BattlePartyState.Instance);
    }

    void OnPokemonSelected(int selectedPokemon)
    {
        var pokemon = partyScreen.Pokemons[selectedPokemon];
        StartCoroutine(inventoryUI.UseItem(pokemon));
    }

    void OnItemUsed(ItemBase usedItem)
    {
        battleSystem.StateMachine.PopState(); // Pop the party screen

        if (usedItem != null)
        {
            battleSystem.SelectedAction = BattleAction.UseItem;
            battleSystem.StateMachine.ChangeState(RunTurnState.Instance);
        }
    }

    void OnBack()
    {
        battleSystem.StateMachine.ChangeState(ActionSelectionState.Instance);
    }
}
