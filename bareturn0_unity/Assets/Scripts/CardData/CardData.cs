using UnityEngine;

[CreateAssetMenu(fileName = "CardData", menuName = "Card/New Card")]
public class CardData : ScriptableObject
{
    public string cardName;
    public string description;
    public int cost;    // 卡牌消耗（类似能量）
    public Sprite artwork;  // 卡牌图片

    // 通过 CardEffect 来决定卡牌效果
    public CardEffect cardEffect;
}