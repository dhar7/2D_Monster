using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameItemState : State<GameController>
{
    [SerializeField] InventoryUI inventoryUI;

    public static GameItemState Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
    }

    GameController _gc;
    public override void Enter(GameController gc)
    {
        _gc = gc;

        inventoryUI.gameObject.SetActive(true);

        inventoryUI.OnSelected += OnItemSelected;
        inventoryUI.OnBack += OnBack;
    }

    public override void Execute(GameController gc)
    {
        inventoryUI.HandleUpdate();
    }

    public override void Exit(GameController gc)
    {
        inventoryUI.gameObject.SetActive(false);

        inventoryUI.OnSelected -= OnItemSelected;
        inventoryUI.OnBack -= OnBack;
    }

    void OnItemSelected(int selectedItem)
    {
        _gc.StateMachine.PushState(GamePartyState.Instance);
        _gc.PartyScreen.OnSelected += OnPokemonSelected;
        _gc.PartyScreen.OnBack += OnPartyScreenBack;
    }

    void OnPokemonSelected(int selectedPokemon)
    {
        var pokemon = _gc.PartyScreen.Pokemons[selectedPokemon];
        StartCoroutine(UseItem(pokemon));
        _gc.PartyScreen.OnSelected -= OnPokemonSelected;
    }

    IEnumerator UseItem(Pokemon pokemon)
    {
        yield return inventoryUI.UseItem(pokemon);
        yield return new WaitForEndOfFrame(); // Wait for dialog state to pop

        _gc.StateMachine.PopState();
    }

    void OnPartyScreenBack()
    {
        _gc.PartyScreen.OnSelected -= OnPokemonSelected;
        _gc.PartyScreen.OnBack -= OnPartyScreenBack;
    }

    void OnBack()
    {
        _gc.StateMachine.ChangeState(GameMenuState.Instance);
    }
}
