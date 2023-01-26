using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChoiceSelectionUI : SelectionUI<Text>
{
    [SerializeField] List<Text> choiceTexts;

    private void Start()
    {
        SetSelectionSettings(SelectionUIType.List);
        SetItems(choiceTexts);
    }
}
