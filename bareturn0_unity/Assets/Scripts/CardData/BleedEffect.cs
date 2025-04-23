using UnityEngine;

[CreateAssetMenu(fileName = "NewBleedEffect", menuName = "Card Effects/Bleed Effect")]
public class BleedEffect : CardEffect
{
    // 设定施加的流血层数
    public int layers = 1;

    // 中毒效果需要目标，一般流血效果也需要目标
    public override bool RequiresTarget() { return true; }

    public override void ApplyEffect(BattleManager battleManager, CardData cardData, ICharacter caster, ICharacter target = null)
    {
        if (target == null)
        {
            Debug.Log("No target selected for Bleed Effect.");
            return;
        }

        if (target is EnemyController enemy)
        {
            enemy.ApplyBleed(layers);
            Debug.Log($"{enemy.name} gains {layers} layers of Bleed.");
        }
        else if (target is PlayerController player)
        {
            player.ApplyBleed(layers);
            Debug.Log($"Player gains {layers} layers of Bleed.");
        }
        else
        {
            Debug.Log("BleedEffect: Target is not a valid ICharacter.");
        }
    }
}
