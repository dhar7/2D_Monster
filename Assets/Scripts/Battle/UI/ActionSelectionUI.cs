using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionSelectionUI : SelectionUI<Text> 
{
    [SerializeField] List<Text> actionTexts;

    private void Start()
    {
        SetSelectionSettings(SelectionUIType.Grid);
        SetItems(actionTexts);
    }
}
