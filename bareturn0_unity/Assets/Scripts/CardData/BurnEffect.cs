using UnityEngine;

[CreateAssetMenu(fileName = "NewBurnEffect", menuName = "Card Effects/Burn Effect")]
public class BurnEffect : CardEffect
{
    public int layers = 1;
    public override bool RequiresTarget() { return true; }

    public override void ApplyEffect(BattleManager battleManager, CardData cardData, ICharacter caster, ICharacter target = null)
    {
        if (target is EnemyController enemy)
        {
            enemy.ApplyBurn(layers);
            //battleManager.ShowEffectOnly(enemy.transform.position, EffectType.Burn);
            Debug.Log(enemy.name + " gains " + layers + " layers of Burn.");
        }
        else if (target is PlayerController player)
        {
            player.ApplyBurn(layers);
            //battleManager.ShowEffectOnly(player.transform.position, EffectType.Burn);
        }
    }
}