using UnityEngine;

[CreateAssetMenu(fileName = "NewAttackEffect", menuName = "Card Effects/Attack Effect")]
public class AttackEffect : CardEffect
{
    // 设置攻击伤害
    public int damage = 5;

    public override void ApplyEffect(BattleManager battleManager, CardData cardData)
    {
        // 对敌人造成伤害
        BattleManager.Instance.lastAttackDamage = damage;
        BattleManager.Instance.player.Attack();
        Debug.Log($"Enemy takes {damage} damage.");
    }
}