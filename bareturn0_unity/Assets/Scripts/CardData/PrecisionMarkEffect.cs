using UnityEngine;

[CreateAssetMenu(menuName = "Card Effects/PrecisionMarkEffect")]
public class PrecisionMarkEffect : CardEffect
{
    public override bool RequiresTarget() => true;

    public override void ApplyEffect(BattleManager bm, CardData card, ICharacter caster, ICharacter target)
    {
        if (target is EnemyController enemy)
        {
            bm.RegisterMark(enemy);
            enemy.AplplyPrecisionMark();
            Debug.Log($"已标记 {enemy.name}：本回合若其受伤，玩家将获得锐利");
        }
    }
}
