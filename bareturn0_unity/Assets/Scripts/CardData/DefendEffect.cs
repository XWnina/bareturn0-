using UnityEngine;

[CreateAssetMenu(fileName = "NewDefendEffect", menuName = "Card Effects/Defend Effect")]
public class DefendEffect : CardEffect
{
    // 设置获得护甲的数值
    public int armorGain = 5;
    public override bool RequiresTarget() { return true; }


    public override void ApplyEffect(BattleManager battleManager, CardData cardData, ICharacter caster, ICharacter target = null)
    {
        if (caster is PlayerController)
        {
            // 给玩家增加护甲
            battleManager.player.GainArmor(armorGain);
            Debug.Log($"Player gains {armorGain} armor.");
        }
        else if (caster is EnemyController)
        {
            EnemyController shieldTarget = null;
            switch (cardData.targetingType)
            {
                case TargetingType.Self:
                    shieldTarget = caster as EnemyController;
                    break;
                case TargetingType.Ally:
                    shieldTarget = battleManager.GetLowestHPEnemy();
                    break;
                default:
                    // 若未指定其他规则，默认给予施法者自己
                    shieldTarget = caster as EnemyController;
                    break;
            }

            if (shieldTarget != null)
            {
                shieldTarget.GainArmor(armorGain);
                Debug.Log($"{shieldTarget.name} gains {armorGain} armor.");
            }
            else
            {
                Debug.Log("No valid enemy target found for shield effect.");
            }
        }

    }
}