using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerDeckDTO
{
    public List<CardInDeckDTO> cardDeck;
}

[Serializable]
public class CardInDeckDTO
{
    public string name;   // ¿¨ÅÆÃû×Ö
    public int count;     // ÊýÁ¿
}
