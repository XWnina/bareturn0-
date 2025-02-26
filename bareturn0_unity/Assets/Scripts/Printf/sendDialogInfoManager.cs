using UnityEngine;
using UnityEngine.Networking;
using System.Collections;


public class SendDialogInfoManager : MonoBehaviour
{
    private string baseUrl = "http://localhost:3000"; // 你的后端 API 地址
    private string saveName = "save1"; // 存档名称
    //private string userId = "67bf7a133d7ba917fb6bf5bf"; // 用户 ID

    public void SavePlayerData(string playerName, int progress)
    {
        StartCoroutine(UpdatePlayerName(playerName));
        StartCoroutine(UpdateProgress(progress));
    }

    private IEnumerator UpdatePlayerName(string playerName)
    {
        string url = $"{baseUrl}/{saveName}/updatePlayerName";
        string jsonData = JsonUtility.ToJson(new PlayerNameData(playerName));

        using (UnityWebRequest request = UnityWebRequest.Put(url, jsonData))
        {
            request.method = "PUT";
            request.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(jsonData));
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

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
        string url = $"{baseUrl}/{saveName}/updateProgress";
        string jsonData = JsonUtility.ToJson(new ProgressData(progress));

        using (UnityWebRequest request = UnityWebRequest.Put(url, jsonData))
        {
            request.method = "PUT";
            request.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(jsonData));
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

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
