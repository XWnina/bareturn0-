using UnityEngine;

[CreateAssetMenu(fileName = "NewPoisonEffect", menuName = "Card Effects/Poison Effect")]
public class PoisonEffect : CardEffect
{
    public int layers = 1;
    public override bool RequiresTarget() { return true; }

    public override void ApplyEffect(BattleManager battleManager, CardData cardData, ICharacter caster, ICharacter target = null)
    {
        if (target is EnemyController enemy)
        {
            enemy.ApplyPoison(layers);
            battleManager.ShowEffectOnly(enemy.transform.position, EffectType.Poison);
            Debug.Log(enemy.name + " gains " + layers + " layers of Poison.");
        }
        else if (target is PlayerController player)
        {
            player.ApplyPoison(layers);
            battleManager.ShowEffectOnly(player.transform.position, EffectType.Poison);
        }
    }
}