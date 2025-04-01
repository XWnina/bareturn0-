using System;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using UnityEngine.Networking;

[Serializable]
public class DeckDTO
{
    public string _id;
    public string saveFileId;
    public string name;
    public List<CardInDeckDTO> cards;
}

[Serializable]
public class CardInDeckDTO
{
    public string cardName;
    public int count;
}

[Serializable]
public class SelectedDeckDTO
{
    public DeckDTO selectedDeck;
}

[Serializable]
public class DeckByNameDTO
{
    public string deckId;
    public DeckDTO deck;
}

[System.Serializable]
public class MaxHealthDTO
{
    public int maxHealth;
}

[System.Serializable]
public class SpeedDTO
{
    public int speed;
}

[Serializable]
public class CardCollectionDTO
{
    public DeckDTO cardCollection;
}

[Serializable]
public class CoinsDTO
{
    public int coins;
}

[System.Serializable]
public class CardOperationDTO
{
    public string cardName;
    public int count;
    public CardOperationDTO(string cardName, int count)
    {
        this.cardName = cardName;
        this.count = count;
    }
}

[System.Serializable]
public class CoinUpdate
{
    public int coins;
    public CoinUpdate(int amount)
    {
        this.coins = amount;
    }
}