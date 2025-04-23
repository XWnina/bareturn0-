using UnityEngine;

[CreateAssetMenu(fileName = "NewAttackEffect", menuName = "Card Effects/Attack Effect")]
public class AttackEffect : CardEffect

{
    // …Ë÷√π•ª˜…À∫¶
    public int damage = 5;
    public override bool RequiresTarget() { return true; }

    public override void ApplyEffect(BattleManager battleManager, CardData cardData, ICharacter caster, ICharacter target = null)
    {
        if (target == null)
        {
            Debug.Log("No target selected for attack.");
            return;
        }

        

        if (caster is PlayerController player)
        {
            int totalDamage = damage + player.sharpnessLayers;
            battleManager.lastAttackDamage = totalDamage;

            battleManager.selectedEnemy = target as EnemyController;

            player.Attack();
            Debug.Log($"Player uses {cardData.cardName}, scheduled {damage} damage on target.");

        }
        else if (caster is EnemyController)
        {
            battleManager.lastAttackDamage = damage;
            (caster as EnemyController).Attack();
        }
    }
}