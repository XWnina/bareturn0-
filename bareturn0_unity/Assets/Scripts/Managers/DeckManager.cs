using UnityEngine;
using System.Collections.Generic;


public class DeckManager : MonoBehaviour
{
    public List<CardData> drawPile = new List<CardData>();    // 抽牌堆
    public List<CardData> hand = new List<CardData>();        // 玩家手牌
    public List<CardData> discardPile = new List<CardData>(); // 弃牌堆
    public List<CardData> exhaustPile = new List<CardData>(); // 移除堆


    public CardUIManager cardUIManager;

    // 用于测试或初始化用的卡牌数据列表
    // 可以在Inspector里手动拖拽一堆CardData进来，作为初始牌库
    public List<CardData> initialDeck = new List<CardData>();
    void Start()
    {
        //SetupInitialDeck();

        if (cardUIManager == null)
        {
            Debug.LogError("DeckManager: cardUIManager is not assigned!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        // 按 D 键弃掉手牌[0]
        if (Input.GetKeyDown(KeyCode.D))
        {
            if (hand.Count > 0)
            {
                Discard(hand[0]);
                Debug.Log("Discarded first card in hand. Discard pile count: " + discardPile.Count);
            }
        }
    }

    public void SetupInitialDeck()
    {
        drawPile.Clear();
        discardPile.Clear();
        exhaustPile.Clear();
        hand.Clear();

        // 将 initialDeck 中的所有卡复制到 drawPile
        foreach (var cardData in initialDeck)
        {
            drawPile.Add(cardData);
        }

        // 打乱抽牌堆
        Shuffle(drawPile);
    }

    // Fisher-Yates 洗牌算法
    public void Shuffle(List<CardData> deck)
    {
        for (int i = 0; i < deck.Count; i++)
        {
            int randomIndex = Random.Range(i, deck.Count);
            CardData temp = deck[i];
            deck[i] = deck[randomIndex];
            deck[randomIndex] = temp;
        }
    }

    public void DrawCard(int count)
    {
        for (int i = 0; i < count; i++)
        {
            // 如果抽牌堆没有卡了，就把弃牌堆洗回去
            if (drawPile.Count == 0)
            {
                Reshuffle();
            }

            if (drawPile.Count > 0)
            {
                CardData topCard = drawPile[0];
                drawPile.RemoveAt(0);
                hand.Add(topCard);

                cardUIManager.OnDrawCard(topCard);
            }
        }
    }

    // 将弃牌堆洗回抽牌堆
    public void Reshuffle()
    {
        drawPile.AddRange(discardPile);
        discardPile.Clear();
        Shuffle(drawPile);
    }

    // 将使用过的卡转移到弃牌堆
    public void Discard(CardData card)
    {
        if (hand.Contains(card))
        {
            hand.Remove(card);
            discardPile.Add(card);

            // 让 CardUIManager 也移除对应的 UI
            if (cardUIManager != null)
            {
                cardUIManager.RemoveCardView(card);
            }
        }
    }

    // 销毁卡牌
    public void Exhaust(CardData card)
    {
        if (hand.Contains(card))
        {
            hand.Remove(card);
            exhaustPile.Add(card);
        }
    }

    public void DiscardAllHand()
    {
        for (int i = hand.Count - 1; i >= 0; i--)
        {
            discardPile.Add(hand[i]);
            hand.RemoveAt(i);
        }

        // 让 CardUIManager 负责销毁 UI
        if (cardUIManager != null)
        {
            cardUIManager.DestroyAllCardViews();
        }
    }


}