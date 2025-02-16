using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class MapUrlManager : MonoBehaviour
{
    private string apiBaseUrl = "http://localhost:3000"; // 你的后端 API 地址
    public static int CurrentLevel { get; private set; } = 0; // 供外部访问

    private string username;
    private string saveName;

    void Start()
    {
        StartCoroutine(GetUserInfo());
    }

    IEnumerator GetUserInfo()
    {
        string url = apiBaseUrl + "/wu/save1";
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string json = request.downloadHandler.text;
            UserData data = JsonUtility.FromJson<UserData>(json);

            username = data.username;
            saveName = data.saveName;

            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(saveName))
            {
                StartCoroutine(GetUserLevel());
            }
        }
        else
        {
            Debug.LogError("Error fetching user info: " + request.error);
        }
    }

    IEnumerator GetUserLevel()
    {
        string url = apiBaseUrl + "/progress/wu/save1";
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string json = request.downloadHandler.text;
            UserProgressData data = JsonUtility.FromJson<UserProgressData>(json);
            CurrentLevel = data.currentLevel; // 更新当前关卡

            Debug.Log("Fetched Current Level: " + CurrentLevel);

            // 通知 LevelButtonManager 更新 UI
            LevelButtonManager levelManager = FindFirstObjectByType<LevelButtonManager>();
            if (levelManager != null)
            {
                levelManager.UpdateLevelButtons();
            }
            else
            {
                Debug.LogError("LevelButtonManager not found!");
            }
        }
        else
        {
            Debug.LogError("Error fetching user progress: " + request.error);
        }
    }

    [System.Serializable]
    private class UserData
    {
        public string username;
        public string saveName;
    }

    [System.Serializable]
    private class UserProgressData
    {
        public string username;
        public string saveName;
        public int currentLevel;
    }
}
