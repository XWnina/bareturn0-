using UnityEngine;

[CreateAssetMenu(fileName = "NewSharpnessEffect", menuName = "Card Effects/Sharpness Effect")]
public class SharpnessEffect : CardEffect
{
    // 增加的锐利层数
    public int layers = 1;

    // 玩家获得锐利不需要选择目标
    public override bool RequiresTarget() { return false; }

    public override void ApplyEffect(BattleManager battleManager, CardData cardData, ICharacter caster, ICharacter target = null)
    {
        if (caster is PlayerController player)
        {
            player.ApplySharpness(layers);
            Debug.Log("Player gains " + layers + " layers of Sharpness.");
        }
        else if (caster is EnemyController enemy)
        {
            if (cardData.targetingType == TargetingType.Self)
            {
                enemy.ApplySharpness(layers); // 你需要在EnemyController中增加类似的字段和方法
                Debug.Log(enemy.name + " gains " + layers + " layers of Sharpness.");
            }
        }
    }
}
