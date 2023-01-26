using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePartyState : State<GameController>
{
    public static GamePartyState Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
    }

    GameController _gc;

    public override void Enter(GameController gc)
    {
        _gc = gc;

        _gc.PartyScreen.gameObject.SetActive(true);

        _gc.PartyScreen.OnSelected += OnPokemonSelected;
        _gc.PartyScreen.OnBack += OnPartyScreenBack;
    }

    public override void Execute(GameController gc)
    {
        _gc.PartyScreen.HandleUpdate();   
    }

    public override void Exit(GameController entity)
    {
        _gc.PartyScreen.gameObject.SetActive(false);

        _gc.PartyScreen.OnSelected -= OnPokemonSelected;
        _gc.PartyScreen.OnBack -= OnPartyScreenBack;
    }

    void OnPokemonSelected(int selectedPokemon)
    {
        // Go to summary screen
    }

    void OnPartyScreenBack()
    {
        _gc.StateMachine.PopState();
    }
}
