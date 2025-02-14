using UnityEngine;

[CreateAssetMenu(fileName = "NewEnergyEffect", menuName = "Card Effects/Energy Effect")]
public class EnergyEffect : CardEffect
{
    public int energyGain = 1; // 设置默认增加的能量

    public override void ApplyEffect(BattleManager battleManager, CardData cardData)
    {
        battleManager.player.currentEnergy += energyGain;
        Debug.Log($"Player gains {energyGain} energy.");
    }
}