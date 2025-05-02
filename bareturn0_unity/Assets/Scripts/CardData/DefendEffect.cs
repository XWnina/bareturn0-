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
            target.GainArmor(armorGain);
            caster.Cast();
        }

    }
}