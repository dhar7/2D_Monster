using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : SelectionUI<ItemSlotUI>
{
    [SerializeField] Image itemIcon;
    [SerializeField] Text itemDescription;
    [SerializeField] GameObject itemList;
    [SerializeField] ItemSlotUI itemSlotUI;
    [SerializeField] Image upArrow;
    [SerializeField] Image downArrow;

    [SerializeField] PartyScreen partyScreen;

    public event Action<ItemBase> OnItemUsed;

    // Setting
    const int itemsInViewport = 8;

    public enum InventoryUIState { ItemSelection, PartySelection, Busy };
    InventoryUIState state;

    Inventory inventory;
    List<ItemSlotUI> slotUIList = new List<ItemSlotUI>();
    RectTransform itemListRect;
    private void Awake()
    {
        inventory = Inventory.GetInventory();
        itemListRect = itemList.GetComponent<RectTransform>();
    }

    private void Start()
    {
        UpdateItemList();
    }

    void UpdateItemList()
    {
        // Clear previously shown items
        foreach (Transform child in itemList.transform)
            Destroy(child.gameObject);

        slotUIList = new List<ItemSlotUI>();
        foreach (var itemSlot in inventory.Slots)
        {
            var itemUIObj = Instantiate(itemSlotUI, itemList.transform);
            itemUIObj.SetData(itemSlot);
            slotUIList.Add(itemUIObj);
        }

        SetItems(slotUIList);
    }

    void OnPokemonSelected(int selectedPokemon)
    {
        state = InventoryUIState.Busy;

        var pokemon = partyScreen.Pokemons[selectedPokemon];
        StartCoroutine(UseItem(pokemon));
    }

    protected override void UpdateSelection()
    {
        base.UpdateSelection();

        itemIcon.sprite = inventory.Slots[selectedItem].Item.Icon;
        itemDescription.text = inventory.Slots[selectedItem].Item.Description;

        HandleScrolling(selectedItem);
    }

    void HandleScrolling(int selectedItem)
    {
        if (slotUIList.Count <= itemsInViewport) return;

        float scrollPos = Mathf.Clamp(selectedItem - itemsInViewport / 2, 0, selectedItem) * slotUIList[0].Height;
        itemListRect.localPosition = new Vector2(itemListRect.localPosition.x, scrollPos);

        bool showUpArrow = selectedItem > itemsInViewport / 2;
        upArrow.gameObject.SetActive(showUpArrow);

        bool showDownArrow = selectedItem + itemsInViewport / 2 < slotUIList.Count;
        downArrow.gameObject.SetActive(showDownArrow);
    }

    public IEnumerator UseItem(Pokemon selectedPokemon)
    {
        var usedItem = inventory.UseItem(SelectedItem, selectedPokemon);
        if (usedItem != null)
        {
            UpdateItemList();
            partyScreen.UpdateSlotData(partyScreen.SelectedItem);
            yield return DialogManager.Instance.ShowDialogText($"Player used {usedItem.name}", true);
        }
        else
        {
            yield return DialogManager.Instance.ShowDialogText("It won't have any affect", true);
        }

        OnItemUsed?.Invoke(usedItem);
    }
}
