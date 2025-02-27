using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MapUrlManager : MonoBehaviour
{
    private string apiBaseUrl = "http://localhost:3000/api/savefiles"; // Update API base URL
    public static int CurrentLevel { get; private set; } = 0; // Store user progress level

    private string username;
    private string saveName;

    void Start()
    {
        // Retrieve stored values from PlayerPrefs
        username = PlayerPrefs.GetString("username", "");
        saveName = PlayerPrefs.GetString("saveName", "");

        if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(saveName))
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
    string url = $"{apiBaseUrl}/me"; // Fetch all save files for the user

    UnityWebRequest request = UnityWebRequest.Get(url);
    request.SetRequestHeader("Authorization", "Bearer " + PlayerPrefs.GetString("authToken", "")); // Assuming auth token is stored
    yield return request.SendWebRequest();

    if (request.result == UnityWebRequest.Result.Success)
    {
        string json = request.downloadHandler.text;
        SaveFileData[] saveFiles = JsonHelper.FromJson<SaveFileData>(json); // Deserialize array

        foreach (var save in saveFiles)
        {
            if (save.saveName == saveName) // Match the correct save file
            {
                CurrentLevel = save.progress; // Use progress as current level
                Debug.Log($"[MapUrlManager] Successfully fetched progress: {CurrentLevel} for save: {saveName}");

                // ✅ Notify LevelButtonManager after fetching data
                if (LevelButtonManager.Instance != null)
                {
                    LevelButtonManager.Instance.UpdateLevelUI();
                }
                else
                {
                    Debug.LogError("[MapUrlManager] LevelButtonManager Instance is NULL!");
                }

                yield break; // Stop loop after finding the correct save
            }
        }

        Debug.LogError($"[MapUrlManager] Save file '{saveName}' not found for the current user.");
    }
    else
    {
        Debug.LogError($"[MapUrlManager] Error fetching user progress: {request.error}");
    }
}


    [System.Serializable]
    private class SaveFileData
    {
        public string saveName;
        public int progress; // Assuming progress represents the current level
        public int coins;
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
