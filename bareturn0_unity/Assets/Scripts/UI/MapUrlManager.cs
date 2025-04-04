using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
public class MapUrlManager : MonoBehaviour
{
    //private string apiBaseUrl = "http://localhost:3000/savefiles"; // Update API base URL
    public static int CurrentLevel { get; private set; } = 0; // Store user progress level
    public PlayerInfoLoader playerInfoLoader;
    public int coins;

    //private string username;
    private string saveName;
    public Button settingButton; // 添加 Setting 按钮
    public Button townButton;
    public GameObject rewardPanel; 
    public TextMeshProUGUI coinsText;
    public Button okButton;
    //public BattleUIManager battleUIManager;

    void Start()
    {
        townButton.onClick.AddListener(GoToTown);
        saveName = PlayerPrefs.GetString("currentSaveName", "");
        settingButton.onClick.AddListener(GoToSettings);
        rewardPanel.SetActive(false);
        okButton.onClick.AddListener(closeReward);
        if (!string.IsNullOrEmpty(saveName))
        {
            StartCoroutine(GetUserLevel());
        }
        else
        {
            Debug.LogError("Username or SaveName is missing in PlayerPrefs!");
        }
        //CheckLevelAndReward(); 
        StartCoroutine(CheckLevelAndReward());
    }

    IEnumerator GetUserLevel()
    {
        saveName = PlayerPrefs.GetString("currentSaveName", "");
        if (string.IsNullOrEmpty(saveName))
        {
            Debug.LogError("MapUrlManager: SaveName is missing in PlayerPrefs!");
            yield break;
        }

        string url = $"http://localhost:3000/savefiles/{saveName}/progress";
        //  Debug.Log($"[MapUrlManager] Requesting: {url}");

        UnityWebRequest request = UnityWebRequest.Get(url);
        string authToken = PlayerPrefs.GetString("token", "");
        request.SetRequestHeader("Authorization", "Bearer " + authToken);
        //  Debug.Log($"[MapUrlManager] Using auth token: {authToken}");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string json = request.downloadHandler.text;
            //  Debug.Log($"✅ [MapUrlManager] Server Response: {json}");

            ProgressResponse progressData = JsonUtility.FromJson<ProgressResponse>(json);
            if (progressData != null)
            {
                CurrentLevel = progressData.progress;
                Debug.Log($"MapUrlManager: Successfully fetched progress: {CurrentLevel} for save: {saveName}");

                if (LevelButtonManager.Instance != null)
                {
                    LevelButtonManager.Instance.UpdateLevelUI();
                }
                else
                {
                    Debug.LogError("[MapUrlManager] ❌ LevelButtonManager Instance is NULL!");
                }
            }
            else
            {
                Debug.LogError("[MapUrlManager] ❌ Failed to parse JSON response.");
            }
        }
        else
        {
            Debug.LogError($"[MapUrlManager] ❌ Error fetching user progress: {request.error}");
        }
    }

    [System.Serializable]
    private class ProgressResponse
    {
        public int progress;
    }

    [System.Serializable]
    private class CoinUpdateRequest
    {
        public int coins;
        public CoinUpdateRequest(int amount)
        {
            this.coins = amount;
        }
    }

    IEnumerator CheckLevelAndReward()
    {   
        yield return StartCoroutine(GetUserLevel());
        string previousScene = PlayerPrefs.GetString("PreviousScene", "");
        //BattleUIManager uiManager = FindObjectOfType<BattleUIManager>();
        //bool didPass = BattleUIManager.Instance.passed;
        //int curProcess = GetUserLevel();

        int curProcess = CurrentLevel;
        //Debug.Log($"上一个场景是: {previousScene}");
        //previousScene == "BattleScenes"
        if (BattleUIManager.Instance != null && BattleUIManager.Instance.passed && curProcess == 2)  // 确保是从 level2 进入的
        {
            playerInfoLoader.LoadPlayerCoins(() =>
            {
                coins = playerInfoLoader.coins;
                Debug.Log("🎉 通过 level2，奖励 100 coins！");
                coins += 100;
                StartCoroutine(UpdateCoins(coins)); // 更新数据库
                ShowRewardPanel(2, 100); // 弹出奖励界面
                BattleUIManager.Instance.passed = false;
            });
        }
        if (BattleUIManager.Instance != null && BattleUIManager.Instance.passed && curProcess == 5)
        {
            coins = playerInfoLoader.coins;
            Debug.Log("🎉 通过 level5，奖励 150 coins！");
            coins += 150;
            StartCoroutine(UpdateCoins(coins)); // 更新数据库
            ShowRewardPanel(5, 150); // 弹出奖励界面
            BattleUIManager.Instance.passed = false;
        }
    }

    IEnumerator UpdateCoins(int amount)
    {
        string url = $"http://localhost:3000/savefiles/{saveName}/updateCoins";
        string authToken = PlayerPrefs.GetString("token", "");

        // 发送 JSON 数据 { "coins": amount }
        string jsonBody = JsonUtility.ToJson(new CoinUpdateRequest(amount));

        UnityWebRequest request = UnityWebRequest.Put(url, jsonBody);
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonBody);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + authToken);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log($"✅ 成功更新金币：+{amount}");
        }
        else
        {
            Debug.LogError($"❌ 更新金币失败: {request.error} - {request.downloadHandler.text}");
        }
    }


    void ShowRewardPanel(int level, int coins)
    {
        if (rewardPanel != null && coinsText != null)
        {
            rewardPanel.SetActive(true);
            coinsText.text = $"Congratulations! You have already passed (battle) level {level}. You got {coins} coins!";
        }
        else
        {
            Debug.LogError("❌ Reward Panel 或 Coinstxt 未绑定！");
        }
    }

    // Helper class to deserialize JSON arrays
    public static class JsonHelper
    {
        public static T[] FromJson<T>(string json)
        {
            string newJson = "{ \"array\": " + json + "}";
            Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(newJson);
            return wrapper.array;
        }

        [System.Serializable]
        private class Wrapper<T>
        {
            public T[] array;
        }
    }
    void GoToSettings()
    {
        PlayerPrefs.SetString("PreviousScene", SceneManager.GetActiveScene().name);
        PlayerPrefs.Save(); // 确保数据持久化
        SceneManager.LoadScene("SettingScene");
    }
    void GoToTown()
    {
        SceneManager.LoadScene("Town");
    }
    void closeReward()
    {
        //okButton.interactable = true;
        Debug.Log("✅ Okay clicked!");
        rewardPanel.SetActive(false);
    }
}
