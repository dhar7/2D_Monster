using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBattleState : State<GameController>
{
    [SerializeField] BattleSystem battleSystem;
    [SerializeField] Camera worldCamera;

    public static GameBattleState Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
    }

    GameController _gc;
    public override void Enter(GameController gc)
    {
        _gc = gc;

        battleSystem.gameObject.SetActive(true);
        worldCamera.gameObject.SetActive(false);

        var playerParty = gc.PlayerController.GetComponent<PokemonParty>();

        if (gc.Trainer == null)
        {
            var wildPokemon = gc.CurrentScene.GetComponent<MapArea>().GetRandomWildPokemon();
            var wildPokemonCopy = new Pokemon(wildPokemon.Base, wildPokemon.Level);
            battleSystem.StartBattle(playerParty, wildPokemonCopy);
        }
        else
        {
            var trainerParty = gc.Trainer.GetComponent<PokemonParty>();
            battleSystem.StartTrainerBattle(playerParty, trainerParty);
        }

        battleSystem.OnBattleOver += OnBattleOver;
    }

    public override void Execute(GameController gc)
    {
        battleSystem.HandleUpdate();
    }

    public override void Exit(GameController gc)
    {
        battleSystem.OnBattleOver -= OnBattleOver;

        battleSystem.gameObject.SetActive(false);
        worldCamera.gameObject.SetActive(true);
    }

    void OnBattleOver(bool won)
    {
        if (_gc.Trainer != null && won == true)
        {
            _gc.Trainer.BattleLost();
            _gc.Trainer = null;
        }

        _gc.StateMachine.ChangeState(GameFreeRoamState.Instance);
    }
}
