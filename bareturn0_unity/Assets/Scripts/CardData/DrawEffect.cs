using UnityEngine;

[CreateAssetMenu(fileName = "NewDrawEffect", menuName = "Card Effects/Draw Effect")]
public class DrawEffect : CardEffect
{
    public int drawCount = 1; // 设置默认抽牌数量

    public override void ApplyEffect(BattleManager battleManager, CardData cardData)
    {
        battleManager.deckManager.DrawCard(drawCount);
        Debug.Log($"Player draws {drawCount} card(s).");
    }
}