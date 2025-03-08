using UnityEngine;

public enum CardQuality
{
    Common,   // 普通（铜色）
    Rare,     // 罕见（银色）
    Epic,     // 史诗（金色）
}

public enum TargetingType
{
    Manual,          // 需要玩家手动选择目标
    Self,            // 目标为施法者自身
    Ally,            // 目标为施法者队友（例如敌人的治疗、护盾卡牌，目标为自己或队友）
    FirstEnemy,      // 第一个敌人（列表中排在最前的敌人）
    LowestHPEnemy    // 血量最低的敌人
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

    public TargetingType targetingType;
}