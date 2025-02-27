using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MapUrlManager : MonoBehaviour
{
    //private string apiBaseUrl = "http://localhost:3000/savefiles"; // Update API base URL
    public static int CurrentLevel { get; private set; } = 0; // Store user progress level

    //private string username;
    private string saveName;

    void Start()
    {
        saveName = PlayerPrefs.GetString("currentSaveName", "");
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
