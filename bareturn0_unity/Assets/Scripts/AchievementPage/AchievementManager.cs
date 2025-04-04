using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Text;

public class AchievementManager : MonoBehaviour
{
    // Call this method with the achievement name you want to unlock
    public void UnlockAchievement(string achievementName)
    {
        string saveName = PlayerPrefs.GetString("currentSaveName", null);

        if (string.IsNullOrEmpty(saveName))
        {
            Debug.LogError("Save name not set in PlayerPrefs.");
            return;
        }

        StartCoroutine(SendUnlockRequest(saveName, achievementName));
    }

    private IEnumerator SendUnlockRequest(string saveName, string achievementName)
    {
        string url = $"http://localhost:3000/achievements/{saveName}/unlock";

        string jsonBody = JsonUtility.ToJson(new AchievementUnlockRequest
        {
            achievementName = achievementName
        });

        using (UnityWebRequest request = UnityWebRequest.Put(url, jsonBody))
        {
            request.method = UnityWebRequest.kHttpVerbPUT;
            request.SetRequestHeader("Content-Type", "application/json");

            // Add token if you're using auth
            if (PlayerPrefs.HasKey("Token"))
            {
                string token = PlayerPrefs.GetString("Token");
                request.SetRequestHeader("Authorization", "Bearer " + token);
            }

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log($"✅ Achievement '{achievementName}' unlocked!");
            }
            else
            {
                Debug.LogError($"❌ Failed to unlock achievement: {request.error}\n{request.downloadHandler.text}");
            }
        }
    }

    // Small helper class for JSON body
    [System.Serializable]
    public class AchievementUnlockRequest
    {
        public string achievementName;
    }
}
