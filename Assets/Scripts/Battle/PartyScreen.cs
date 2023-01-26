using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PartyScreen : SelectionUI<PartyMemberUI>
{
    [SerializeField] Text messageText;

    PartyMemberUI[] memberSlots;
    public List<Pokemon> Pokemons { get; private set; }
    PokemonParty party;

    public void Init()
    {
        memberSlots = GetComponentsInChildren<PartyMemberUI>(true);
        party = PokemonParty.GetPlayerParty();
        party.OnUpdated += SetPartyData;

        SetSelectionSettings(SelectionUIType.Grid);

        SetPartyData();
    }

    public void SetPartyData()
    {
        Pokemons = party.Pokemons;

        for (int i = 0; i < memberSlots.Length; i++)
        {
            if (i < Pokemons.Count)
            {
                memberSlots[i].gameObject.SetActive(true);
                memberSlots[i].Init(Pokemons[i]);
            }
            else
                memberSlots[i].gameObject.SetActive(false);
        }

        SetItems(memberSlots.Take(Pokemons.Count).ToList());

        messageText.text = "Choose a Pokemon";
    }

    public void UpdateSlotData(int slotIndex)
    {
        memberSlots[slotIndex].Init(Pokemons[slotIndex]);
    }

    public void SetMessageText(string message)
    {
        messageText.text = message;
    }
}
