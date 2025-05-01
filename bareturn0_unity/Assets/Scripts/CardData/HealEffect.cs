using UnityEngine;

[CreateAssetMenu(fileName = "NewHealEffect", menuName = "Card Effects/Heal Effect")]
public class HealEffect : CardEffect
{
    public int layers = 1;
    public override bool RequiresTarget() { return true; }

    public override void ApplyEffect(BattleManager battleManager, CardData cardData, ICharacter caster, ICharacter target = null)
    {
        if (target is EnemyController enemy)
        {
            enemy.Heal(layers);
            //battleManager.ShowEffectOnly(enemy.transform.position, EffectType.Heal);
        }
        else if (target is PlayerController player)
        {
            player.Heal(layers);
            //battleManager.ShowEffectOnly(player.transform.position, EffectType.Heal);
        }
    }
}