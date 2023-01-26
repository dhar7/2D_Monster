using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MoveSelectionBoxUI : SelectionUI<Text>
{
    [SerializeField] List<Text> moveTexts;

    private void Start()
    {
        SetSelectionSettings(SelectionUIType.List);
    }

    public void SetMoveData(List<MoveBase> currentMoves, MoveBase newMove)
    {
        for (int i = 0; i < currentMoves.Count; ++i)
        {
            moveTexts[i].text = currentMoves[i].Name;
        }

        moveTexts[currentMoves.Count].text = newMove.Name;

        SetItems(moveTexts.Take(currentMoves.Count + 1).ToList());
    }
}
