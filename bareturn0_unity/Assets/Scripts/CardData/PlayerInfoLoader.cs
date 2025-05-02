using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using static DeckInfoLoader;

public class PlayerInfoLoader : MonoBehaviour
{
    public CardDatabase cardDatabase;  // 卡牌库资源

    // 后端 API 地址
    //private const string URL_BASE = "http://localhost:3000/savefiles/";

    // 用于存储加载后转换好的数据
    public List<CardData> cardList = new List<CardData>();
    public List<string> materials = new List<string>();
    public List<PlayerDeckInfo> PlayerDecks = new List<PlayerDeckInfo>();
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
        string saveName = PlayerPrefs.GetString("currentSaveName", "");
        string token = PlayerPrefs.GetString("token", "");
        string url = "";

        string deckNameLower = deckName.ToLower();

        if (deckNameLower == "selecteddeck" || deckNameLower == "cardcollection")
        {
            // 读取 selectedDeck 或 cardCollection 时用 saveName 构造路径
            url = $"http://localhost:3000/savefiles/{saveName}/{deckName}";
        }
        else
        {
            // 读取一般卡组时用 saveFileId
            string saveFileId = null;
            bool saveIdLoaded = false;

            GetSaveId(saveName, (result) =>
            {
                saveFileId = result;
                saveIdLoaded = true;
            });

            while (!saveIdLoaded)
                yield return null;

            if (string.IsNullOrEmpty(saveFileId))
            {
                Debug.LogError("❌ 获取 saveFileId 失败，无法加载卡组");
                yield break;
            }

            url = $"http://localhost:3000/carddecks/findByName/{saveFileId}/{deckName}";
        }

        UnityWebRequest request = UnityWebRequest.Get(url);
        request.SetRequestHeader("Authorization", "Bearer " + token);

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("❌ 加载卡组失败: " + request.error);
            yield break;
        }

        string json = request.downloadHandler.text;
        Debug.Log($"📥 收到 Deck 数据: {json}");

        cardList = new List<CardData>();

        try
        {
            if (deckNameLower == "selecteddeck")
            {
                SelectedDeckResponse response = JsonUtility.FromJson<SelectedDeckResponse>(json);
                cardList = ConvertDeckDTOToCardDataList(response.selectedDeck);
            }
            else if (deckNameLower == "cardcollection")
            {
                CardCollectionResponse response = JsonUtility.FromJson<CardCollectionResponse>(json);
                cardList = ConvertDeckDTOToCardDataList(response.cardCollection);
            }
            else
            {
                DeckResponse response = JsonUtility.FromJson<DeckResponse>(json);
                cardList = ConvertDeckDTOToCardDataList(response.deck);
            }

            Debug.Log($"✅ 卡组 {deckName} 加载完成，共 {cardList.Count} 张卡");

            onLoaded?.Invoke();
        }
        catch (Exception e)
        {
            Debug.LogError($"❌ JSON 解析失败: {e.Message}");
        }
    }

    // 根据 PlayerDeckDTO 转换为 List<CardData>
    private List<CardData> ConvertDeckDTOToCardDataList(DeckDTO deckDTO)
    {
        List<CardData> result = new List<CardData>();

        foreach (var cardInfo in deckDTO.cards)
        {
            CardData cardData = cardDatabase.GetCardByName(cardInfo.cardName);
            if (cardData != null)
            {
                for (int i = 0; i < cardInfo.count; i++)
                {
                    CardData cardCopy = ScriptableObject.Instantiate(cardData);
                    result.Add(cardCopy);
                }
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


    #region 更新玩家金币
    public void UpdatePlayerCoin(int amount, System.Action onCoinUpdated)
    {
        StartCoroutine(UpdateCoins(amount, onCoinUpdated));
    }

    IEnumerator UpdateCoins(int amount, System.Action onCoinUpdated)
    {
        string saveName = PlayerPrefs.GetString("currentSaveName", "");
        string url = $"http://localhost:3000/savefiles/{saveName}/updateCoins";
        string authToken = PlayerPrefs.GetString("token", "");

        // 发送 JSON 数据 { "coins": amount }
        string jsonBody = JsonUtility.ToJson(new CoinUpdate(amount));

        UnityWebRequest request = UnityWebRequest.Put(url, jsonBody);
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonBody);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + authToken);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log($"成功更新金币：+{amount}");
        }
        else
        {
            Debug.LogError($"更新金币失败: {request.error} - {request.downloadHandler.text}");
        }
        onCoinUpdated?.Invoke();
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


    #region AddCardToDeck

    public void AddCardToDeck(string deckId, string cardName, int count, System.Action onComplete = null)
    {
        StartCoroutine(AddCardToDeckRequest(deckId, cardName, count, onComplete));
    }
    public IEnumerator AddCardToDeckRequest(string deckId, string cardName, int count, System.Action onComplete = null)
    {
        string url = $"http://localhost:3000/carddecks/{deckId}/addCard";
        string token = PlayerPrefs.GetString("token", "");

        CardOperationDTO payload = new CardOperationDTO(cardName, count);
        string jsonBody = JsonUtility.ToJson(payload);

        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();

        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + token);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log($"成功向 Deck({deckId}) 添加 {count} 张 {cardName}");
        }
        else
        {
            Debug.LogError($"添加卡失败: {request.error} - {request.downloadHandler.text}");
        }

        onComplete?.Invoke();
    }
    #endregion


    #region RemoveCardFromDeck
    public void RemoveCardFromDeck(string deckName, string cardName, int count, System.Action onComplete = null)
    {
        StartCoroutine(RemoveCardFromDeckRequest(deckName,cardName,count, onComplete));
    }
    public IEnumerator RemoveCardFromDeckRequest(string deckName, string cardName, int count, System.Action onComplete = null)
    {
        string saveFileId = PlayerPrefs.GetString("currentSaveName", "");
        string url = $"http://localhost:3000/carddecks/removeCardByName/{saveFileId}/{deckName}";
        string token = PlayerPrefs.GetString("token", "");

        CardOperationDTO payload = new CardOperationDTO(cardName, count);
        string jsonBody = JsonUtility.ToJson(payload);

        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();

        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + token);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log($"成功从 Deck({deckName}) 移除 {count} 张 {cardName}");
        }
        else
        {
            Debug.LogError($"移除卡失败: {request.error} - {request.downloadHandler.text}");
        }

        onComplete?.Invoke();
    }
    #endregion


    #region 更新材料
    public void UpdateMaterial(string materialName, int newCount, System.Action onUpdated)
    {
        StartCoroutine(UpdateMaterialRequest(materialName, newCount, onUpdated));
    }

    private IEnumerator UpdateMaterialRequest(string materialName, int newCount, System.Action onUpdated)
    {
        string saveName = PlayerPrefs.GetString("currentSaveName", "");
        string url = $"http://localhost:3000/materials/update/{saveName}/{materialName}";
        string authToken = PlayerPrefs.GetString("token", "");

        MaterialUpdateDTO dto = new MaterialUpdateDTO(newCount);
        string jsonBody = JsonUtility.ToJson(dto);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);

        UnityWebRequest request = UnityWebRequest.Put(url, jsonBody);

        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + authToken);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log($"成功更新材料: {request.downloadHandler.text}");
        }
        else
        {
            Debug.LogError($"更新材料失败: {request.error} - {request.downloadHandler.text}");
        }
        onUpdated?.Invoke();
    }
    #endregion


    #region 创建新材料
    public void CreateNewMaterial(string materialName, int count, System.Action onCreated)
    {
        StartCoroutine(CreateNewMaterialRequest(materialName, count, onCreated));
    }

    private IEnumerator CreateNewMaterialRequest(string materialName, int count, System.Action onCreated)
    {
        string saveName = PlayerPrefs.GetString("currentSaveName", "");
        string url = $"http://localhost:3000/materials/create/{saveName}";
        string authToken = PlayerPrefs.GetString("token", "");

        // 创建 JSON payload
        MaterialCreateDTO dto = new MaterialCreateDTO
        {
            name = materialName,
            count = count
        };
        string jsonBody = JsonUtility.ToJson(dto);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonBody);

        UnityWebRequest request = new UnityWebRequest(url, "POST");
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + authToken);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log($"✅ 材料 {materialName} 创建成功: {request.downloadHandler.text}");
            onCreated?.Invoke();
        }
        else
        {
            Debug.LogError($"❌ 创建材料失败: {request.error} - {request.downloadHandler.text}");
            onCreated?.Invoke();
        }
    }
    #endregion


    #region 获取所有材料
    public void GetAllMaterials(System.Action onMaterialsLoaded)
    {
        StartCoroutine(GetAllMaterialsRequest(onMaterialsLoaded));
    }

    private IEnumerator GetAllMaterialsRequest(System.Action onMaterialsLoaded)
    {
        string saveName = PlayerPrefs.GetString("currentSaveName", "");
        string url = $"http://localhost:3000/materials/all/{saveName}";
        UnityWebRequest request = UnityWebRequest.Get(url);
        string authToken = PlayerPrefs.GetString("token", "");
        request.SetRequestHeader("Authorization", "Bearer " + authToken);

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("获取所有材料失败: " + request.error);
        }
        else
        {
            string json = request.downloadHandler.text;
            Debug.Log("后端返回所有材料数据: " + json);

            MaterialsResponseDTO responseDTO = JsonUtility.FromJson<MaterialsResponseDTO>(json);
            if (responseDTO != null && responseDTO.materials != null)
            {
                materials.Clear();
                foreach (var mat in responseDTO.materials)
                {
                    for (int i = 0; i < mat.count; i++)
                    {
                        materials.Add(mat.name);
                    }
                }
                Debug.Log("材料列表更新完成。");
            }
            else
            {
                Debug.LogWarning("解析所有材料数据失败或数据为空。");
            }
        }
        onMaterialsLoaded?.Invoke();
    }
    #endregion


    #region 获取单个材料数量
    public void GetMaterialCount(string materialName, System.Action<int> onCountLoaded)
    {
        StartCoroutine(GetMaterialCountRequest(materialName, onCountLoaded));
    }

    private IEnumerator GetMaterialCountRequest(string materialName, System.Action<int> onCountLoaded)
    {
        string saveName = PlayerPrefs.GetString("currentSaveName", "");
        string url = $"http://localhost:3000/materials/count/{saveName}/{materialName}";
        UnityWebRequest request = UnityWebRequest.Get(url);
        string authToken = PlayerPrefs.GetString("token", "");
        request.SetRequestHeader("Authorization", "Bearer " + authToken);

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("获取材料数量失败: " + request.error);
            onCountLoaded?.Invoke(-1);
        }
        else
        {
            string json = request.downloadHandler.text;
            Debug.Log("后端返回材料数量数据: " + json);

            MaterialCountDTO dto = JsonUtility.FromJson<MaterialCountDTO>(json);
            if (dto != null)
            {
                onCountLoaded?.Invoke(dto.count);
            }
            else
            {
                Debug.LogWarning("解析材料数量数据失败。");
                onCountLoaded?.Invoke(-1);
            }
        }
    }
    #endregion


    #region 获取所有deck
    public void GetALLDecks(string saveFileId, System.Action onDecksLoaded)
    {
         StartCoroutine(GetAllDecksRequest(saveFileId ,onDecksLoaded));
    }

    private IEnumerator GetAllDecksRequest(string saveFileId, System.Action onDecksLoaded)
    {
        Debug.Log($"[GetAllDecksRequest] 当前存档 ID: {saveFileId}");

        string authToken = PlayerPrefs.GetString("token", "");

        string url = $"http://localhost:3000/carddecks/save/{saveFileId}";
        UnityWebRequest request = UnityWebRequest.Get(url);
        request.SetRequestHeader("Authorization", "Bearer " + authToken);

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("加载所有 Deck 失败: " + request.error);
        }
        else
        {
            string json = request.downloadHandler.text;
            Debug.Log("后端返回 Deck 列表: " + json);

            AllDecksDTO allDecks = JsonUtility.FromJson<AllDecksDTO>(json);
            if (allDecks != null && allDecks.decks != null)
            {
                PlayerDecks.Clear();

                foreach (var deck in allDecks.decks)
                {
                    PlayerDecks.Add(new PlayerDeckInfo(deck.name, deck._id));
                    Debug.Log($"加载 Deck: {deck.name}，ID = {deck._id}");
                }
            }
            else
            {
                Debug.LogWarning("解析 Deck 数据失败或为空。");
            }
        }
        onDecksLoaded?.Invoke();
    }
    #endregion


    public void GetSaveId(string saveName, System.Action<string> onLoaded)
    {
        StartCoroutine(GetSaveIdRequest(saveName, onLoaded));
    }

    private IEnumerator GetSaveIdRequest(string saveName, System.Action<string> onLoaded)
    {
        string token = PlayerPrefs.GetString("token", "");
        string url = $"http://localhost:3000/savefiles/{saveName}/id";

        UnityWebRequest request = UnityWebRequest.Get(url);
        request.SetRequestHeader("Authorization", "Bearer " + token);

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("获取 saveFileId 失败: " + request.error);
            onLoaded?.Invoke(null); // 回调 null 表示失败
        }
        else
        {
            string json = request.downloadHandler.text;
            Debug.Log("获取到 saveFileId 返回: " + json);

            SaveIdResponseDTO response = JsonUtility.FromJson<SaveIdResponseDTO>(json);
            if (response != null && !string.IsNullOrEmpty(response.saveFileId))
            {
                onLoaded?.Invoke(response.saveFileId);
            }
            else
            {
                Debug.LogWarning("返回的 saveFileId 为空");
                onLoaded?.Invoke(null);
            }
        }
    }

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


    #region RefeshAllDecks
    public void RefeshAllDecks(CardData removedCard, CardData addedCard)
    {
        StartCoroutine(RefeshAllDecksCoroutine(removedCard, addedCard));
    }

    private IEnumerator RefeshAllDecksCoroutine(CardData removedCard, CardData addedCard)
    {
        Debug.Log($"开始调整decks");

        Debug.Log("🟡 开始通过 saveName 获取 saveFileId...");

        // Step 1: 获取当前保存的 saveName
        string saveName = PlayerPrefs.GetString("currentSaveName", "");
        if (string.IsNullOrEmpty(saveName))
        {
            Debug.LogError("当前未设置 saveName！");
            yield break;
        }

        // Step 2: 获取saveFileId
        string saveFileId = null;
        bool saveIdLoaded = false;

        GetSaveId(saveName, (result) =>
        {
            saveFileId = result;
            saveIdLoaded = true;
        });
        while (!saveIdLoaded)
            yield return null;

        // Step 3: 加载所有卡组
        bool decksLoaded = false;
        GetALLDecks(saveFileId, () =>
        {
            Debug.Log("所有 Deck 加载完毕");
            decksLoaded = true;
        });
        while (!decksLoaded)
            yield return null;



        if (PlayerDecks == null || PlayerDecks.Count == 0)
        {
            Debug.LogWarning("没有找到任何卡组，停止替换流程");
            yield break;
        }

        Debug.Log($"开始遍历 {PlayerDecks.Count} 个卡组替换卡牌...");
        foreach (var deck in PlayerDecks)
        {
            Debug.Log($"检查 deck 是否为空：deck = {(deck == null ? "null" : deck.deckName)}, id = {deck?.deckId}");
            Debug.Log($"🔍 替换参数：removedCard={removedCard?.cardName}, addedCard={addedCard?.cardName}");
            yield return StartCoroutine(CheckAndReplaceInDeck(deck.deckId, deck.deckName, removedCard.cardName, addedCard.cardName));
        }

        Debug.Log("所有卡组替换流程结束。");

    }

    private IEnumerator CheckAndReplaceInDeck(string deckId, string deckName, string oldCardName, string newCardName)
    {
        // Step 1：加载目标 Deck 的卡组数据
        bool loaded = false;

        Debug.Log(deckName);
        LoadPlayerDeck(deckName, () =>
        {
            loaded = true;
        });

        // 等待加载完成
        while (!loaded)
            yield return null;

        List<CardData> deckCardList = new List<CardData>(cardList);


        Debug.Log($"🔍 oldCardName: \"{oldCardName}\", 长度 = {oldCardName.Length}");
        int deckOldCardCount = 0;
        foreach (var card in deckCardList)
        {
            Debug.Log($"🃏 卡牌名: \"{card.cardName}\", 长度 = {card.cardName.Length}");
            if (card.cardName.Trim().Equals(oldCardName.Trim(), System.StringComparison.Ordinal))
            {
                Debug.Log($"equals");
                deckOldCardCount++;
            }
        }
        Debug.Log("deckOldCardCount:"+ deckOldCardCount);

        if (deckOldCardCount == 0)
        {
            Debug.Log($"⏭️ Deck【{deckName}】中没有 {oldCardName}，跳过。");
            yield break;
        }

        // Step 2：加载收藏 collection 卡组（通过 LoadPlayerDeck("collection")）
        int collectionOldCardCount = 0;
        bool collectionLoaded = false;

        LoadPlayerDeck("cardCollection", () =>
        {
            foreach (var card in cardList)
            {
                if (card.cardName == oldCardName)
                {
                    collectionOldCardCount++;
                }
            }
            collectionLoaded = true;
        });

        while (!collectionLoaded)
            yield return null;

        // Step 3：判断是否超出收藏数量
        if (deckOldCardCount != 0)
        {
            Debug.Log($"Deck【{deckName}】发现旧卡 {oldCardName}，准备替换为 {newCardName}");
            // Step 4：移除超出的旧卡
            //yield return StartCoroutine(RemoveCardFromDeckRequest(deckName, oldCardName, 1));
            // Step 5：添加新卡
            yield return StartCoroutine(AddCardToDeckRequest(deckId, newCardName, 1));
            Debug.Log($"Deck【{deckName}】已完成替换。");
        }
        else
        {
            Debug.Log($"Deck【{deckName}】没有多余的 {oldCardName}，跳过。");
        }
    }
    #endregion
}
