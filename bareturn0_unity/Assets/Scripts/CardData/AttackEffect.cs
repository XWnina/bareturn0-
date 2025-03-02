using UnityEngine;

[CreateAssetMenu(fileName = "NewAttackEffect", menuName = "Card Effects/Attack Effect")]
public class AttackEffect : CardEffect

{
    // 设置攻击伤害
    public int damage = 5;
    public override bool RequiresTarget() { return true; }

    public override void ApplyEffect(BattleManager battleManager, CardData cardData, EnemyController target = null)
    {
        if (target == null)
        {
            Debug.Log("No target selected for attack.");
            return;
        }
        // 对敌人造成伤害
        BattleManager.Instance.lastAttackDamage = damage;
        battleManager.selectedEnemy = target;
        BattleManager.Instance.player.Attack();
        Debug.Log($"Enemy takes {damage} damage.");
    }
}