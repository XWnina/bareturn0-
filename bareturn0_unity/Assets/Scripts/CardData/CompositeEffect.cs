using UnityEngine;

[CreateAssetMenu(fileName = "NewCompositeEffect", menuName = "Card Effects/Composite Effect")]
public class CompositeEffect : CardEffect
{
    public CardEffect[] subEffects;  // 在 Inspector 里拖多个效果

    public override bool RequiresTarget()
    {
        // 如果其中任何一个需要目标，就返回 true
        foreach (var e in subEffects)
            if (e.RequiresTarget())
                return true;
        return false;
    }

    public override void ApplyEffect(BattleManager bm, CardData cardData, ICharacter caster, ICharacter target = null)
    {
        foreach (var e in subEffects)
        {
            e.ApplyEffect(bm, cardData, caster, target);
        }
    }
}