using System;
using System.Collections.Generic;
using UnityEngine;

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