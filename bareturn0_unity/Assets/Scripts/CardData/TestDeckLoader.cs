using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestDeckLoader : MonoBehaviour
{
    public PlayerInfoLoader playerInfoLoader;

    void Start()
    {
        string saveName = PlayerPrefs.GetString("currentSaveName", "TestSave");
        Debug.Log($"📘 当前存档名: {saveName}");

        StartCoroutine(RunFullTest());
    }

    private IEnumerator RunFullTest()
    {
        yield return StartCoroutine(PrintDeck("Default Deck", "🔍 初始 Default Deck"));

        // 移除 Slash ×1 从 collection
        bool removed = false;
        playerInfoLoader.RemoveCardFromCollection("Slash", 1, () =>
        {
            Debug.Log("🗑️ 从 cardCollection 中移除 1 张 Slash 成功");
            removed = true;
        });

        while (!removed)
            yield return null;

        yield return new WaitForSeconds(1f);

        // 再次打印 Deck 内容，确保没被改动
        yield return StartCoroutine(PrintDeck("Default Deck", "🔁 移除收藏后再次查看 Default Deck"));
    }

    private IEnumerator PrintDeck(string deckName, string title = "")
    {
        Debug.Log($"============== {title} ==============");
        Debug.Log($"📦 正在加载 Deck: {deckName}");

        bool loaded = false;
        playerInfoLoader.LoadPlayerDeck(deckName, () => { loaded = true; });

        while (!loaded)
            yield return null;

        List<CardData> deckCards = playerInfoLoader.cardList;

        if (deckCards == null || deckCards.Count == 0)
        {
            Debug.LogWarning("⚠️ 加载的卡组为空！");
            yield break;
        }

        Debug.Log($"✅ 成功加载 Deck: {deckName}，共 {deckCards.Count} 张卡");

        Dictionary<string, int> countMap = new Dictionary<string, int>();
        foreach (var card in deckCards)
        {
            if (countMap.ContainsKey(card.cardName))
                countMap[card.cardName]++;
            else
                countMap[card.cardName] = 1;
        }

        foreach (var pair in countMap)
        {
            Debug.Log($"🃏 {pair.Key} × {pair.Value}");
        }

        Debug.Log("============== 打印完毕 ==============\n");
    }
}
