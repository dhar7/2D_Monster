using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum SelectionUIType { List, Grid };

public class SelectionUI<T> : MonoBehaviour
{
    protected int selectedItem = 0;
    List<T> items;

    SelectionUIType type;
    int gridWidth = 2;

    public event Action<int> OnSelected;
    public event Action OnBack;

    public event Action<int> OnSelectionChanged;

    public void SetSelectionSettings(SelectionUIType type, int gridWidth = 2)
    {
        this.type = type;
        this.gridWidth = gridWidth;
    }

    public void SetItems(List<T> items)
    {
        this.items = items;
        UpdateSelection();
    }

    public virtual void HandleUpdate()
    {
        int prevSelection = selectedItem;

        if (type == SelectionUIType.List)
            HandleListSelection();
        else
            HandleGridSelection();

        selectedItem = Mathf.Clamp(selectedItem, 0, items.Count - 1);

        if (selectedItem != prevSelection)
            UpdateSelection();

        if (Input.GetKeyDown(KeyCode.Z))
            OnSelected?.Invoke(selectedItem);
        else if (Input.GetKeyDown(KeyCode.X))
            OnBack?.Invoke();
    }

    void HandleListSelection()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
            ++selectedItem;
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            --selectedItem;
    }

    void HandleGridSelection()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
            ++selectedItem;
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            --selectedItem;
        else if (Input.GetKeyDown(KeyCode.DownArrow))
            selectedItem += gridWidth;
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            selectedItem -= gridWidth;
    }

    protected virtual void UpdateSelection()
    {

        for (int i = 0; i < items.Count; ++i)
        {
            if (items[i] is Text)
            {
                var itemText = items[i] as Text;
                itemText.color = (selectedItem == i) ? GlobalSettings.i.HighlightedColor : Color.black;
            }
            else if (items[i] is ISelectableItem)
            {
                var item = items[i] as ISelectableItem;
                item.OnSelectionChanged(selectedItem == i);
            }
        }

        OnSelectionChanged?.Invoke(selectedItem);
    }

    public int SelectedItem => selectedItem;
}
