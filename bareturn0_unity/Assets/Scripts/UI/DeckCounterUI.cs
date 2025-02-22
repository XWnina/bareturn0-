using UnityEngine;
using TMPro;
using UnityEngine.InputSystem.Controls;

public class DeckCounterUI : MonoBehaviour
{
    [Header("UI References")]
    // 用于显示抽牌堆卡牌数量的文本
    public TextMeshProUGUI drawPileCountText;
    // 用于显示弃牌堆卡牌数量的文本
    public TextMeshProUGUI discardPileCountText;

    [Header("Deck Manager Reference")]
    public DeckManager deckManager;

    void Update()
    {
        if (deckManager != null)
        {
            // 更新文本内容，显示当前卡牌数量
            drawPileCountText.text = deckManager.drawPile.Count.ToString();
            discardPileCountText.text = deckManager.discardPile.Count.ToString();
        }
    }
}