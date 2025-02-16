using UnityEngine;

public enum CardQuality
{
    Common,   // 普通（铜色）
    Rare,     // 罕见（银色）
    Epic,     // 史诗（金色）
}

[CreateAssetMenu(fileName = "CardData", menuName = "Card/New Card")]
public class CardData : ScriptableObject
{
    public string cardName;
    public string description;
    public int cost;    // 卡牌消耗（类似能量）
    public Sprite artwork;  // 卡牌图片
    public CardQuality quality;

    // 通过 CardEffect 来决定卡牌效果
    public CardEffect cardEffect;
}