using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerInfoLoader : MonoBehaviour
{
    public CardDatabase cardDatabase;  // 卡牌库资源

    // 后端 API 地址
    //private const string URL_BASE = "http://localhost:3000/savefiles/";

    // 用于存储加载后转换好的数据
    public List<CardData> cardList = new List<CardData>();
    public int maxHealth;
    public int speed;
    public int coins;


    #region 加载玩家的卡组数据
    public void LoadPlayerDeck(string deckName, System.Action onLoaded)
    {
   
        StartCoroutine(GetPlayerDeckRequest(deckName, onLoaded));
    }

    private IEnumerator GetPlayerDeckRequest(string deckName, System.Action onLoaded)
    {
        string saveFileId = PlayerPrefs.GetString("currentSaveName", "");

        string url = "";
        if (deckName.ToLower() == "selecteddeck" || deckName == "cardCollection")
        {
            url = $"http://localhost:3000/savefiles/{saveFileId}/{deckName}";
        }
        else
        {
            url = $"http://localhost:3000/savefiles/findByName/{saveFileId}/{deckName}";
        }

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

            DeckDTO deck = null;
            if (deckName == "selectedDeck")
            {
                // 解析为 SelectedDeckDTO
                SelectedDeckDTO selectedDTO = JsonUtility.FromJson<SelectedDeckDTO>(json);
                if (selectedDTO != null && selectedDTO.selectedDeck != null)
                {
                    deck = selectedDTO.selectedDeck;
                }
            }
            else if (deckName.ToLower() == "cardcollection")
            {
                // 解析为 CardCollectionDTO
                CardCollectionDTO collectionDTO = JsonUtility.FromJson<CardCollectionDTO>(json);
                if (collectionDTO != null && collectionDTO.cardCollection != null)
                {
                    deck = collectionDTO.cardCollection;
                }
            }
            else
            {
                // 解析为 DeckByNameDTO
                DeckByNameDTO nameDTO = JsonUtility.FromJson<DeckByNameDTO>(json);
                if (nameDTO != null && nameDTO.deck != null)
                {
                    deck = nameDTO.deck;
                }
            }

            if (deck != null)
            {
                cardList = ConvertDeckDTOToCardDataList(deck);
            }
            else
            {
                Debug.LogWarning("反序列化卡组数据失败或数据为空。");
            }
            if (onLoaded != null) onLoaded();
        }
    }

    // 根据 PlayerDeckDTO 转换为 List<CardData>
    private List<CardData> ConvertDeckDTOToCardDataList(DeckDTO deckDTO)
    {
        List<CardData> result = new List<CardData>();

        foreach (CardInDeckDTO cardInfo in deckDTO.cards)
        {
            // 通过卡牌库查找对应的 CardData
            CardData cardData = cardDatabase.GetCardByName(cardInfo.cardName);
            if (cardData != null)
            {
                for (int i = 0; i < cardInfo.count; i++)
                {
                    CardData cardCopy = ScriptableObject.Instantiate(cardData);
                    result.Add(cardCopy);
                }
            }
            else
            {
                Debug.LogWarning("找不到卡牌：" + cardInfo.cardName);
            }
        }

        return result;
    }
    #endregion


    #region 加载玩家数据
    public void LoadPlayerStats(System.Action onStatsLoaded)
    {
        StartCoroutine(GetPlayerStatsRequest(onStatsLoaded));
    }

    private IEnumerator GetPlayerStatsRequest(System.Action onStatsLoaded)
    {
        yield return StartCoroutine(LoadMaxHealth());

        yield return StartCoroutine(LoadSpeed());

        onStatsLoaded?.Invoke();
        Debug.Log("玩家属性已全部加载完毕。");
    }

    private IEnumerator LoadMaxHealth()
    {
        string saveFileId = PlayerPrefs.GetString("currentSaveName", "");
        string url = $"http://localhost:3000/savefiles/{saveFileId}/maxHealth";
        UnityWebRequest request = UnityWebRequest.Get(url);
        string authToken = PlayerPrefs.GetString("token", "");
        request.SetRequestHeader("Authorization", "Bearer " + authToken);

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("加载 maxHealth 失败: " + request.error);
        }
        else
        {
            string json = request.downloadHandler.text;
            Debug.Log("后端返回 maxHealth 数据: " + json);

            MaxHealthDTO dto = JsonUtility.FromJson<MaxHealthDTO>(json);
            if (dto != null)
            {
                maxHealth = dto.maxHealth;
                Debug.Log($"玩家 maxHealth = {dto.maxHealth}");
            }
        }
    }

    private IEnumerator LoadSpeed()
    {
        string saveFileId = PlayerPrefs.GetString("currentSaveName", "");
        string url = $"http://localhost:3000/savefiles/{saveFileId}/speed";
        UnityWebRequest request = UnityWebRequest.Get(url);
        string authToken = PlayerPrefs.GetString("token", "");
        request.SetRequestHeader("Authorization", "Bearer " + authToken);

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("加载 speed 失败: " + request.error);
        }
        else
        {
            string json = request.downloadHandler.text;
            Debug.Log("后端返回 speed 数据: " + json);

            SpeedDTO dto = JsonUtility.FromJson<SpeedDTO>(json);
            if (dto != null)
            {
                speed = dto.speed;
                Debug.Log($"玩家 speed = {dto.speed}");
            }
        }
    }
    #endregion


    #region 加载玩家金币
    public void LoadPlayerCoins(System.Action onCoinsLoaded)
    {
        StartCoroutine(LoadCoinsRequest(onCoinsLoaded));
    }

    private IEnumerator LoadCoinsRequest(System.Action onCoinsLoaded)
    {
        string saveFileId = PlayerPrefs.GetString("currentSaveName", "");
        string url = $"http://localhost:3000/savefiles/{saveFileId}/coins";
        UnityWebRequest request = UnityWebRequest.Get(url);
        string authToken = PlayerPrefs.GetString("token", "");
        request.SetRequestHeader("Authorization", "Bearer " + authToken);

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("加载金币失败: " + request.error);
        }
        else
        {
            string json = request.downloadHandler.text;
            Debug.Log("后端返回金币数据: " + json);

            CoinsDTO dto = JsonUtility.FromJson<CoinsDTO>(json);
            if (dto != null)
            {
                coins = dto.coins;
                Debug.Log($"玩家金币 = {dto.coins}");
            }
        }
        onCoinsLoaded?.Invoke();
    }
    #endregion


    #region Add Card To Collection
    public void AddCardToCollection(string cardName, int count, System.Action onAdded)
    {
        StartCoroutine(AddCardToCollectionRequest(cardName, count, onAdded));
    }
    public IEnumerator AddCardToCollectionRequest(string cardName, int count, System.Action onAdded)
    {
        string saveFileId = PlayerPrefs.GetString("currentSaveName", "");
        string url = $"http://localhost:3000/selectedDeckAndCardCollection/{saveFileId}/addCardToCollection";
        string authToken = PlayerPrefs.GetString("token", "");

        CardOperationDTO payload = new CardOperationDTO(cardName, count);
        string jsonBody = JsonUtility.ToJson(payload);

        

        UnityWebRequest request = new UnityWebRequest(url, "POST");

        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();

        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + authToken);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log($"成功添加卡牌到集合: {request.downloadHandler.text}");
        }
        else
        {
            Debug.LogError($"添加卡牌失败: {request.error} - {request.downloadHandler.text}");
        }
        onAdded?.Invoke();
    }
    #endregion

    #region Remove Card From Collection
    public void RemoveCardFromCollection(string cardName, int count, System.Action onRemoved)
    {
        StartCoroutine(RemoveCardFromCollectionRequest(cardName, count, onRemoved));
    }

    public IEnumerator RemoveCardFromCollectionRequest(string cardName, int count, System.Action onRemoved)
    {
        string saveFileId = PlayerPrefs.GetString("currentSaveName", "");
        string url = $"http://localhost:3000/selectedDeckAndCardCollection/{saveFileId}/removeCardFromCollection";

        string authToken = PlayerPrefs.GetString("token", "");
        CardOperationDTO payload = new CardOperationDTO(cardName, count);
        string jsonBody = JsonUtility.ToJson(payload);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);

        UnityWebRequest request = new UnityWebRequest(url, "POST");
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + authToken);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log($"成功移除卡牌: {request.downloadHandler.text}");
        }
        else
        {
            Debug.LogError($"移除卡牌失败: {request.error} - {request.downloadHandler.text}");
        }
        onRemoved?.Invoke();
    }
    #endregion

    public void InitialBattleDeck()
    {
        DeckManager.Instance.initialDeck = cardList;
        DeckManager.Instance.SetupInitialDeck();
        Debug.Log("卡组加载成功，抽牌堆已更新。");
    }

    public void InitialPlayerStats()
    {
        PlayerController.instance.maxHealth = maxHealth;
        PlayerController.instance.speed = speed;
    }
}
