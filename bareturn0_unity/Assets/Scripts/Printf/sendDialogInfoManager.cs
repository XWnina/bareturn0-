using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Text;

public class SendDialogInfoManager : MonoBehaviour
{
    private string baseUrl = "http://localhost:3000/"; // 你的后端 API 地址
    private string saveName; // 存档名称
    private string token;

    void Start()
    {
        // 从 PlayerPrefs 读取存档名称
        saveName = PlayerPrefs.GetString("currentSaveName", "");
        token = PlayerPrefs.GetString("token", "");
        if (string.IsNullOrEmpty(saveName))
        {
            Debug.LogError("❌ Save name is not set in PlayerPrefs!");
        }
        if (string.IsNullOrEmpty(token))
        {
            Debug.LogError("❌ Token is not set in PlayerPrefs!");
        }
    }

    public void SavePlayerData(string playerName, int progress)
    {
        StartCoroutine(UpdatePlayerName(playerName));
        StartCoroutine(UpdateProgress(progress));
        StartCoroutine(UnlockAchievement(saveName, "Person You Know Who"));

    }

    private IEnumerator UnlockAchievement(string saveName, string achievementName)
    {
        string url = $"{baseUrl}achievements/{saveName}/unlock";

        string jsonData = "{ \"achievementName\": \"" + achievementName + "\" }";

        using (UnityWebRequest request = new UnityWebRequest(url, "PUT"))
        {
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", "Bearer " + token);

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("✅ Achievement unlocked successfully: " + request.downloadHandler.text);
            }
            else
            {
                Debug.LogError("❌ Failed to unlock achievement: " + request.error);
                Debug.LogError("Response: " + request.downloadHandler.text);
            }
        }
    }


    private IEnumerator UpdatePlayerName(string playerName)
    {
        string token = PlayerPrefs.GetString("token", "");
        //string saveName = PlayerPrefs.GetString("currentSaveName", ""); // ✅ 读取存档名称

        if (string.IsNullOrEmpty(token))
        {
            Debug.LogError("❌ No Token Found! Player is not authenticated.");
            yield break;
        }

        if (string.IsNullOrEmpty(saveName))
        {
            Debug.LogError("❌ No SaveName Found! Cannot update player name.");
            yield break;
        }

        string url = $"{baseUrl}savefiles/{saveName}/updatePlayerName"; // ✅ 修正 URL
        string jsonData = JsonUtility.ToJson(new PlayerNameData(playerName));

        using (UnityWebRequest request = UnityWebRequest.Put(url, jsonData))
        {
            request.method = "PUT";
            request.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(jsonData));
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", "Bearer " + token); // ✅ 添加 Token

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("✅ Player name updated successfully: " + request.downloadHandler.text);
            }
            else
            {
                Debug.LogError("❌ Failed to update player name: " + request.error);
            }
        }
    }

    private IEnumerator UpdateProgress(int progress)
    {
        string token = PlayerPrefs.GetString("token", "");
        //string saveName = PlayerPrefs.GetString("currentSaveName", ""); // ✅ 读取存档名称

        if (string.IsNullOrEmpty(token))
        {
            Debug.LogError("❌ No Token Found! Player is not authenticated.");
            yield break;
        }

        if (string.IsNullOrEmpty(saveName))
        {
            Debug.LogError("❌ No SaveName Found! Cannot update progress.");
            yield break;
        }

        string url = $"{baseUrl}savefiles/{saveName}/updateProgress"; // ✅ 修正 URL
        string jsonData = JsonUtility.ToJson(new ProgressData(progress));

        using (UnityWebRequest request = UnityWebRequest.Put(url, jsonData))
        {
            request.method = "PUT";
            request.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(jsonData));
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", "Bearer " + token); // ✅ 添加 Token

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("✅ Progress updated successfully: " + request.downloadHandler.text);
            }
            else
            {
                Debug.LogError("❌ Failed to update progress: " + request.error);
            }
        }
    }


    // 确保 `saveName` 在游戏内某个地方被正确存入
    public static void SetSaveName(string newSaveName)
    {
        PlayerPrefs.SetString("currentSaveName", newSaveName);
        PlayerPrefs.Save(); // **重要！保存数据**
        Debug.Log("✅ Save name stored: " + newSaveName);
    }

    [System.Serializable]
    private class PlayerNameData
    {
        public string playerName;
        public PlayerNameData(string name) { playerName = name; }
    }

    [System.Serializable]
    private class ProgressData
    {
        public int progress;
        public ProgressData(int progressNum) { progress = progressNum; }
    }
}
