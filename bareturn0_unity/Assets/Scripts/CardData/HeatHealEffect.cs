using UnityEngine;

[CreateAssetMenu(menuName = "Card Effects/Heat Heal")]
public class HeatHealEffect : CardEffect
{
    // 根据玩家身上的 burnLayers 治疗同等血量
    public override bool RequiresTarget() => false;
    public override void ApplyEffect(BattleManager bm, CardData cd, ICharacter caster, ICharacter target = null)
    {
        if (caster is EnemyController enemy)
        {
            if (target is PlayerController player) {
                int burn = player.burnLayers;
                enemy.Heal(burn);      
            }
        }
    }
}