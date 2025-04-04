using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class AchievementManager : MonoBehaviour
{
    public static AchievementManager Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void UnlockAchievement(string achievementName)
    {
        string saveName = PlayerPrefs.GetString("currentSaveName", "");
        if (string.IsNullOrEmpty(saveName)) return;

        Instance.StartCoroutine(SendUnlockRequest(saveName, achievementName));
    }

    private IEnumerator SendUnlockRequest(string saveName, string achievementName)
    {
        string url = $"http://localhost:3000/api/achievements/{saveName}/unlock";

        string jsonBody = JsonUtility.ToJson(new AchievementUnlockRequest { achievementName = achievementName });

        UnityWebRequest request = UnityWebRequest.Put(url, jsonBody);
        request.method = UnityWebRequest.kHttpVerbPUT;
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + PlayerPrefs.GetString("token", ""));

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log($"✅ Achievement unlocked: {achievementName}");
        }
        else
        {
            Debug.LogError("❌ Failed to unlock achievement: " + request.error);
        }
    }

    [System.Serializable]
    private class AchievementUnlockRequest
    {
        public string achievementName;
    }
}
