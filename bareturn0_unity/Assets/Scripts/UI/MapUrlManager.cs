using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MapUrlManager : MonoBehaviour
{
    private string apiBaseUrl = "http://localhost:3000/savefiles"; // Update API base URL
    public static int CurrentLevel { get; private set; } = 0; // Store user progress level

    //private string username;
    private string saveName;

    void Start()
    {
        // Retrieve stored values from PlayerPrefs
        //username = PlayerPrefs.GetString("username", "");
        saveName = PlayerPrefs.GetString("currentSaveName", "");
        Debug.LogError("12345dsfrdg");
        if (!string.IsNullOrEmpty(saveName))
        {
            StartCoroutine(GetUserLevel());
        }
        else
        {
            Debug.LogError("Username or SaveName is missing in PlayerPrefs!");
        }
    }

    IEnumerator GetUserLevel()
    {
        string url = $"http://localhost:3000/savefiles/{saveName}/progress"; // 使用后端提供的 API

        UnityWebRequest request = UnityWebRequest.Get(url);
        request.SetRequestHeader("Authorization", "Bearer " + PlayerPrefs.GetString("authToken", "")); // 发送身份验证
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string json = request.downloadHandler.text;
            Debug.Log($"✅ [MapUrlManager] Server Response: {json}"); // 确保正确返回 JSON

            // 解析 JSON
            ProgressResponse progressData = JsonUtility.FromJson<ProgressResponse>(json);
            if (progressData != null)
            {
                CurrentLevel = progressData.progress; // 存储当前进度
                Debug.Log($"[MapUrlManager] Successfully fetched progress: {CurrentLevel} for save: {saveName}");

                // ✅ 通知 UI 更新
                if (LevelButtonManager.Instance != null)
                {
                    LevelButtonManager.Instance.UpdateLevelUI();
                }
                else
                {
                    Debug.LogError("[MapUrlManager] LevelButtonManager Instance is NULL!");
                }
            }
            else
            {
                Debug.LogError("[MapUrlManager] Failed to parse JSON response.");
            }
        }
        else
        {
            Debug.LogError($"[MapUrlManager] Error fetching user progress: {request.error}");
        }
    }

    // JSON 解析类
    [System.Serializable]
    private class ProgressResponse
    {
        public int progress;
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
}
