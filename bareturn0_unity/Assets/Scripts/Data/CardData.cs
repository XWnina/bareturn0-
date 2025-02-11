using UnityEngine;

public enum CardType
{
    Attack,
    Defend,
    Heal,
    // ... 其它类型
}
[CreateAssetMenu(fileName = "CardData", menuName = "Card/New Card")]
public class CardData : ScriptableObject
{
    public string cardName;
    public string description;
    public int cost;    // 卡牌消耗（类似能量）
    public int damage;  // 攻击伤害（若非攻击卡，可不使用）
    public Sprite artwork;  // 卡牌图片
    public CardType cardType;
}