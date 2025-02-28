using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

public class AchievementManager : MonoBehaviour
{
    private string backendURL = "http://localhost:3000/achievements"; // API base URL
    private string token;
    private string saveFile;

    public GameObject achievementPrefab;
    public Transform contentPanel; // Assign this to the ScrollView Content

    private void Start()
    {
        // Load token and save file name from PlayerPrefs
        token = PlayerPrefs.GetString("token", "");
        saveFile = PlayerPrefs.GetString("currentSaveName", "");

        if (!string.IsNullOrEmpty(token) && !string.IsNullOrEmpty(saveFile))
        {
            StartCoroutine(FetchUnlockedAchievements());
            StartCoroutine(FetchLockedAchievements());
        }
        else
        {
            Debug.LogError("Start: Token or SaveFile missing! Cannot fetch achievements.");
        }
    }

    private IEnumerator FetchLockedAchievements()
    {
        UnityWebRequest lockedRequest = UnityWebRequest.Get($"{backendURL}/{saveFile}/locked");
        lockedRequest.SetRequestHeader("Authorization", $"Bearer {token}");
        yield return lockedRequest.SendWebRequest();

        if (lockedRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("FetchLockedAchievements: Error fetching locked achievements: " + lockedRequest.error);
        }
        else
        {
            LockedAchievementData[] lockedData = JsonHelper.FromJson<LockedAchievementData>(lockedRequest.downloadHandler.text);

            foreach (var lockAch in lockedData)
            {
                CreateAchievementUI(lockAch.name, lockAch.method, "Not Achieved Yet", false);
            }
        }
    }

    private IEnumerator FetchUnlockedAchievements()
    {

        UnityWebRequest unlockedRequest = UnityWebRequest.Get($"{backendURL}/{saveFile}/unlocked");
        unlockedRequest.SetRequestHeader("Authorization", $"Bearer {token}");
        yield return unlockedRequest.SendWebRequest();

        if (unlockedRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("FetchUnlockedAchievements: Error fetching unlocked achievements: " + unlockedRequest.error);
        }
        else
        {
            UnlockedAchievementData[] unlockedData = JsonHelper.FromJson<UnlockedAchievementData>(unlockedRequest.downloadHandler.text);

            foreach (var unlock in unlockedData)
            {
                CreateAchievementUI(unlock.achievementName, unlock.description, unlock.achievedDate, true);
            }
        }
    }
    private void CreateAchievementUI(string name, string description, string status, bool isUnlocked)
    {
        GameObject newAchievement = Instantiate(achievementPrefab, contentPanel);
        newAchievement.transform.localScale = Vector3.one;

        TMP_Text[] textElements = newAchievement.GetComponentsInChildren<TMP_Text>();

        if (textElements.Length >= 3)
        {
            textElements[0].text = name;          // Achievement Name
            textElements[1].text = description;   // Achievement Condition (Unlock Condition)
            textElements[2].text = status;        // Achievement Status (Achieved Date or "Not Achieved Yet")
        }
        else
        {
            Debug.LogError($"CreateAchievementUI: Achievement UI Prefab is missing required text elements! {name}");
        }

        Debug.Log(isUnlocked
            ? $"🟢 Displaying UNLOCKED achievement: {name} | Condition: {description} | Achieved on: {status}"
            : $"🔴 Displaying LOCKED achievement: {name} | Condition: {description} | Status: Not Achieved Yet");
    }


    [System.Serializable]
    public class UnlockedAchievementData
    {
        public string achievementName; // ✅ Name
        public string description;  // ✅ Unlock condition
        public string achievedDate;
    }


    [System.Serializable]
    public class LockedAchievementData
    {
        public string name;
        public string method;  // ✅ Unlock condition
    }


}

// ✅ JSON Helper for array handling in Unity
public static class JsonHelper
{
    public static T[] FromJson<T>(string json)
    {
        string newJson = "{ \"array\": " + json + "}";
        return JsonUtility.FromJson<Wrapper<T>>(newJson).array;
    }

    [System.Serializable]
    private class Wrapper<T>
    {
        public T[] array;
    }
}
