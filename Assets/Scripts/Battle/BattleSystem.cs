using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum BattleAction { Move, SwitchPokemon, UseItem, Run }

public class BattleSystem : MonoBehaviour
{
    [SerializeField] BattleUnit playerUnit;
    [SerializeField] BattleUnit enemyUnit;
    [SerializeField] BattleDialogBox dialogBox;
    [SerializeField] Image playerImage;
    [SerializeField] Image trainerImage;
    [SerializeField] GameObject pokeballSprite;
    [SerializeField] PartyScreen partyScreen;
    [SerializeField] MoveSelectionUI moveSelector;

    public event Action<bool> OnBattleOver;

    public PokemonParty PlayerParty { get; private set; }
    public PokemonParty TrainerParty { get; private set; }
    public Pokemon WildPokemon { get; private set; }

    public bool IsTrainerBattle { get; private set; } = false;
    public bool IsBattleOver { get; private set; } = false;

    public PlayerController Player { get; private set; }
    public TrainerController Trainer { get; private set; }

    // Data to pass between states
    public BattleAction SelectedAction { get; set; }
    public int SelectedMove { get; set; }
    public Pokemon NextPokemon { get; set; } // Next pokemon the trainer in going to use

    public int EscapeAttempts { get; set; } = 0;
    MoveBase moveToLearn;

    public StateMachine<BattleSystem> StateMachine { get; private set; }

    public void StartBattle(PokemonParty playerParty, Pokemon wildPokemon)
    {
        this.PlayerParty = playerParty;
        this.WildPokemon = wildPokemon;
        Player = playerParty.GetComponent<PlayerController>();
        IsTrainerBattle = false;

        StartCoroutine(SetupBattle());
    }

    public void StartTrainerBattle(PokemonParty playerParty, PokemonParty trainerParty)
    {
        this.PlayerParty = playerParty;
        this.TrainerParty = trainerParty;

        IsTrainerBattle = true;
        Player = playerParty.GetComponent<PlayerController>();
        Trainer = trainerParty.GetComponent<TrainerController>();

        StartCoroutine(SetupBattle());
    }

    public IEnumerator SetupBattle()
    {
        StateMachine = new StateMachine<BattleSystem>(this);
        StateMachine.ChangeState(BattleBusyState.Instance);

        playerUnit.Clear();
        enemyUnit.Clear();

        if (!IsTrainerBattle)
        {
            // Wild Pokemon Battle
            playerUnit.Setup(PlayerParty.GetHealthyPokemon());
            enemyUnit.Setup(WildPokemon);

            moveSelector.SetMoveNames(playerUnit.Pokemon.Moves);
            yield return dialogBox.TypeDialog($"A wild {enemyUnit.Pokemon.Base.Name} appeared.");
        }
        else
        {
            // Trianer Battle

            // Show trainer and player sprites
            playerUnit.gameObject.SetActive(false);
            enemyUnit.gameObject.SetActive(false);

            playerImage.gameObject.SetActive(true);
            trainerImage.gameObject.SetActive(true);
            playerImage.sprite = Player.Sprite;
            trainerImage.sprite = Trainer.Sprite;

            yield return dialogBox.TypeDialog($"{Trainer.Name} wants to battle");

            // Send out first pokemon of the trainer
            trainerImage.gameObject.SetActive(false);
            enemyUnit.gameObject.SetActive(true);
            var enemyPokemon = TrainerParty.GetHealthyPokemon();
            enemyUnit.Setup(enemyPokemon);
            yield return dialogBox.TypeDialog($"{Trainer.Name} send out {enemyPokemon.Base.Name}");

            // Send out first pokemon of the player
            playerImage.gameObject.SetActive(false);
            playerUnit.gameObject.SetActive(true);
            var playerPokemon = PlayerParty.GetHealthyPokemon();
            playerUnit.Setup(playerPokemon);
            yield return dialogBox.TypeDialog($"Go {playerPokemon.Base.Name}!");
            moveSelector.SetMoveNames(playerUnit.Pokemon.Moves);
        }

        IsBattleOver = false;
        EscapeAttempts = 0;
        partyScreen.Init();

        StateMachine.ChangeState(ActionSelectionState.Instance);
    }

    public void HandleUpdate()
    {
        StateMachine.HandleUpdate();
    }

    public void BattleOver(bool won)
    {
        IsBattleOver = true;
        PlayerParty.Pokemons.ForEach(p => p.OnBattleOver());
        OnBattleOver(won);
    }

    public IEnumerator SwitchPokemon(Pokemon newPokemon)
    {
        if (playerUnit.Pokemon.HP > 0)
        {
            yield return dialogBox.TypeDialog($"Come back {playerUnit.Pokemon.Base.Name}");
            playerUnit.PlayFaintAnimation();
            yield return new WaitForSeconds(2f);
        }

        playerUnit.Setup(newPokemon);
        moveSelector.SetMoveNames(newPokemon.Moves);
        yield return dialogBox.TypeDialog($"Go {newPokemon.Base.Name}!");
    }

    public IEnumerator SendNextTrainerPokemon()
    {
        var nextPokemon = TrainerParty.GetHealthyPokemon();
        enemyUnit.Setup(nextPokemon);
        yield return dialogBox.TypeDialog($"{Trainer.Name} send out {nextPokemon.Base.Name}!");
    }

    public BattleUnit PlayerUnit => playerUnit;
    public BattleUnit EnemyUnit => enemyUnit;

    public BattleDialogBox DialogBox => dialogBox;
    public PartyScreen PartyScreen => partyScreen;
    public MoveSelectionUI MoveSelector => moveSelector;
}