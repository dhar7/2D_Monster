using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMenuState : State<GameController>
{
    [SerializeField] MenuController menuController;

    public static GameMenuState Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
    }

    GameController _gc;

    public override void Enter(GameController gc)
    {
        _gc = gc;

        menuController.gameObject.SetActive(true);
        menuController.OnSelected += OnMenuItemSelected;
        menuController.OnBack += OnMenuBack;
    }

    public override void Execute(GameController gc)
    {
        menuController.HandleUpdate();
    }

    public override void Exit(GameController gc)
    {
        menuController.gameObject.SetActive(false);
        menuController.OnSelected -= OnMenuItemSelected;
        menuController.OnBack -= OnMenuBack;
    }

    void OnMenuItemSelected(int selection)
    {
        var selectedItem = (MenuItem)selection;
        if (selectedItem == MenuItem.Pokemon)
        {
            _gc.StateMachine.PushState(GamePartyState.Instance);
        }
        else if (selectedItem == MenuItem.Bag)
        {
            _gc.StateMachine.ChangeState(GameItemState.Instance);
        }
    }

    void OnMenuBack()
    {
        _gc.StateMachine.ChangeState(GameFreeRoamState.Instance);
    }
}
