using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CardDatabase", menuName = "Card/Card Database")]
public class CardDatabase : ScriptableObject
{
    // 这里存放所有完整定义的卡牌资源
    public List<CardData> allCards;

    // 根据卡牌名字查找对应的 CardData（忽略大小写）
    public CardData GetCardByName(string cardName)
    {
        foreach (CardData card in allCards)
        {
            if (card.cardName.Equals(cardName, System.StringComparison.OrdinalIgnoreCase))
            {
                return card;
            }
        }
        Debug.LogWarning("Card not found: " + cardName);
        return null;
    }
}
