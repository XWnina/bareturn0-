using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerDeckLoader : MonoBehaviour
{
    public CardDatabase cardDatabase;  // 卡牌库资源

    public DeckManager deckManager;

    // 后端 API 地址
    private const string URL_BASE = "http://yourServerAddress/";
    private List<CardData> cardList = new List<CardData>();
    // 调用此方法开始加载卡组数据
    public void LoadPlayerDeck(string saveFileId)
    {
   
        StartCoroutine(GetPlayerDeckRequest(saveFileId));
    }

    private IEnumerator GetPlayerDeckRequest(string saveFileId)
    {
        string url = URL_BASE + saveFileId;

        UnityWebRequest request = UnityWebRequest.Get(url);
        string authToken = PlayerPrefs.GetString("token", "");
        request.SetRequestHeader("Authorization", "Bearer " + authToken);

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("加载卡组数据失败: " + request.error);
        }
        else
        {
            string json = request.downloadHandler.text;
            Debug.Log("后端返回卡组数据: " + json);

            // 使用 JsonUtility 反序列化成 PlayerDeckDTO 对象
            PlayerDeckDTO deckDTO = JsonUtility.FromJson<PlayerDeckDTO>(json);
            if (deckDTO != null && deckDTO.cardDeck != null)
            {
                // 将后端返回的卡组转换为 List<CardData>
                cardList = ConvertDTOToCardDataList(deckDTO);
            }
            else
            {
                Debug.LogWarning("反序列化卡组数据失败或数据为空。");
            }
        }
    }

    // 根据 PlayerDeckDTO 转换为 List<CardData>
    private List<CardData> ConvertDTOToCardDataList(PlayerDeckDTO deckDTO)
    {
        List<CardData> result = new List<CardData>();

        foreach (CardInDeckDTO cardInfo in deckDTO.cardDeck)
        {
            // 通过卡牌库查找对应的 CardData
            CardData cardData = cardDatabase.GetCardByName(cardInfo.name);
            if (cardData != null)
            {
                // 根据 count 生成对应数量的卡牌实例
                for (int i = 0; i < cardInfo.count; i++)
                {
                    CardData cardCopy = ScriptableObject.Instantiate(cardData);
                    result.Add(cardCopy);
                }
            }
            else
            {
                Debug.LogWarning("找不到卡牌：" + cardInfo.name);
            }
        }

        return result;
    }

    // 将生成的卡牌赋值给 DeckManager 并初始化抽牌堆
    public void initialBattleDeck()
    {
        if (deckManager != null)
        {
            deckManager.initialDeck = cardList;
            deckManager.SetupInitialDeck();
            Debug.Log("卡组加载成功，抽牌堆已更新。");
        }
    }
}
