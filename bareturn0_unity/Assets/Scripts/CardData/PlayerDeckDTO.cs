using System;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using UnityEngine.Networking;

[System.Serializable]
public class DeckResponse
{
    public string deckId;
    public DeckDTO deck;
}

[System.Serializable]
public class SelectedDeckResponse
{
    public DeckDTO selectedDeck;
}

[System.Serializable]
public class CardCollectionResponse
{
    public DeckDTO cardCollection;
}

[System.Serializable]
public class DeckDTO
{
    public string _id;
    public string name;
    public List<CardCountDTO> cards;
}

[System.Serializable]
public class CardCountDTO
{
    public string cardName;
    public int count;
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

[System.Serializable]
public class MaterialUpdateDTO
{
    public int count;
    public MaterialUpdateDTO(int count)
    {
        this.count = count;
    }
}

[System.Serializable]
public class MaterialDTO
{
    public string name;
    public int count;
}

[System.Serializable]
public class MaterialsResponseDTO
{
    public List<MaterialDTO> materials;
}

[System.Serializable]
public class MaterialCountDTO
{
    public int count;
}

[System.Serializable]
public class AllDecksDTO
{
    public List<DeckDTO> decks;
}

[System.Serializable]
public class PlayerDeckInfo
{
    public string deckName;
    public string deckId;

    public PlayerDeckInfo(string name, string id)
    {
        deckName = name;
        deckId = id;
    }
}

[System.Serializable]
public class SaveIdResponseDTO
{
    public string saveFileId;
}

[System.Serializable]
public class MaterialCreateDTO
{
    public string name;
    public int count;
}