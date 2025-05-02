using UnityEngine;

[CreateAssetMenu(menuName = "Card Effects/Double Poison")]
public class DoublePoisonEffect : CardEffect
{
    public override bool RequiresTarget() => true;

    public override void ApplyEffect(BattleManager bm, CardData card, ICharacter caster, ICharacter target)
    {
        if (target is EnemyController enemy)
        {
            int oldPoison = enemy.poisonLayers;

            if (oldPoison <= 0)
            {
                return;
            }

            enemy.ApplyPoison(oldPoison);

            Debug.Log($"{enemy.name} 中毒层数从 {oldPoison} 提升到 {enemy.poisonLayers}");
        }
    }
}
