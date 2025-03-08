using UnityEngine;

public abstract class CardEffect : ScriptableObject
{
    public abstract void ApplyEffect(BattleManager battleManager, CardData cardData, ICharacter caster, ICharacter target = null);

    // 返回卡牌效果是否需要玩家选择目标（默认不需要）
    public virtual bool RequiresTarget()
    {
        return false;
    }
}