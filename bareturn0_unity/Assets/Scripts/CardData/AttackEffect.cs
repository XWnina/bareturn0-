using UnityEngine;

[CreateAssetMenu(fileName = "NewAttackEffect", menuName = "Card Effects/Attack Effect")]
public class AttackEffect : CardEffect

{
    // 设置攻击伤害
    public int damage = 5;
    public override bool RequiresTarget() { return true; }

    public override void ApplyEffect(BattleManager battleManager, CardData cardData, ICharacter caster, ICharacter target = null)
    {
        if (target == null)
        {
            Debug.Log("No target selected for attack.");
            return;
        }

        battleManager.lastAttackDamage = damage;

        if (caster is PlayerController)
        {
            // 将目标存入 BattleManager 供动画事件使用
            battleManager.selectedEnemy = target as EnemyController;
            // 触发玩家攻击动画，动画事件将负责在合适时机调用 TriggerEnemyHit()
            (caster as PlayerController).Attack();
            Debug.Log($"Player uses {cardData.cardName}, scheduled {damage} damage on target.");

        }
        else if (caster is EnemyController)
        {
            (caster as EnemyController).Attack();
        }
    }
}