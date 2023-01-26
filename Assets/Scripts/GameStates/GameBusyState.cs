using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBusyState : State<GameController>
{
    public static GameBusyState Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
    }

    public override void Execute(GameController gc)
    {
        // Do nothing in busy state
    }
}
