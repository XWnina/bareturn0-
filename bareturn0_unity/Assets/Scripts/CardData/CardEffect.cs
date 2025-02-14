using UnityEngine;

public abstract class CardEffect : ScriptableObject
{
    public abstract void ApplyEffect(BattleManager battleManager, CardData cardData);
}