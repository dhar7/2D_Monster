using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleBusyState : State<BattleSystem>
{
    public static BattleBusyState Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
    }

    public override void Execute(BattleSystem entity)
    {
        // Do nothing in busy state
    }
}
