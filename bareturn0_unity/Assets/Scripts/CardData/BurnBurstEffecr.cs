using UnityEngine;

[CreateAssetMenu(menuName = "Card Effects/Burn Burst")]
public class BurnBurstEffect : CardEffect
{
    public override bool RequiresTarget() => true;
    public override void ApplyEffect(BattleManager bm, CardData cd, ICharacter caster, ICharacter target)
    {
        if (!(target is PlayerController player)) return;
        int burn = player.burnLayers;
        Vector3 pos = player.transform.position;
        bm.ShowEffectOnly(pos, EffectType.Burst);
        bm.lastAttackDamage = burn;
        (caster as NecroController).Attack1();
    }
}
