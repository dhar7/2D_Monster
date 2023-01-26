using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public enum MenuItem { Pokemon, Bag, Save, Load }

public class MenuController : SelectionUI<Text>
{
    [SerializeField] GameObject menu;

    private void Start()
    {
        SetItems(menu.GetComponentsInChildren<Text>().ToList());
    }
}
