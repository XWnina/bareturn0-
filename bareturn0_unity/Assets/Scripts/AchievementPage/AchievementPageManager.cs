using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class AchievementPageManager : MonoBehaviour
{
    public Transform unlockedParent; // UnlockedScrollView/Viewport/Content
    public Transform lockedParent;   // lockedScrollView/Viewport/Content
    public GameObject achievementPrefab; // Prefab containing: name, method, unlockedInfo

    public Button ExitButton;
    private string apiBaseUrl = "http://localhost:3000/achievements";

    void Start()
    {
        PlayerPrefs.SetString("PreviousScene", "draftMap");

        string saveName = PlayerPrefs.GetString("currentSaveName", "");
        if (!string.IsNullOrEmpty(saveName))
        {
            StartCoroutine(LoadAchievements(saveName));
        }

        ExitButton.onClick.AddListener(() =>
        {
            SceneManager.LoadScene(PlayerPrefs.GetString("PreviousScene"));
        });
    }

    IEnumerator LoadAchievements(string saveName)
    {
        string url = $"{apiBaseUrl}/{saveName}/all-status";
        UnityWebRequest request = UnityWebRequest.Get(url);
        request.SetRequestHeader("Authorization", "Bearer " + PlayerPrefs.GetString("token"));

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string json = request.downloadHandler.text;
            AchievementListWrapper wrapper = JsonUtility.FromJson<AchievementListWrapper>("{\"array\":" + json + "}");

            // Clear previous UI elements
            ClearScrollView(unlockedParent);
            ClearScrollView(lockedParent);

            foreach (AchievementData a in wrapper.array)
            {
                // Make sure UI parents still exist
                if (unlockedParent == null || lockedParent == null || achievementPrefab == null)
                    yield break;

                GameObject obj = Instantiate(achievementPrefab);
                obj.transform.SetParent(a.unlocked ? unlockedParent : lockedParent, false);

                TMP_Text nameText = obj.transform.Find("AchievementName")?.GetComponent<TMP_Text>();
                TMP_Text methodText = obj.transform.Find("Method")?.GetComponent<TMP_Text>();
                TMP_Text infoText = obj.transform.Find("UnlockedInfo")?.GetComponent<TMP_Text>();

                if (nameText) nameText.text = a.name;
                if (methodText) methodText.text = a.method;

                if (a.unlocked && infoText != null)
                {
                    infoText.text = $"Unlocked: {a.achievedDate}";
                }
                else if (!a.unlocked && infoText != null && !a.hidden)
                {
                    infoText.text = "Locked"; // Show locked status
                }
                else if (a.hidden && infoText != null)
                {
                    infoText.text = "???"; // Show locked status
                }
                else if (infoText != null)
                {
                    infoText.text = "???"; // Default text for hidden achievements
                }
            }
        }
        else
        {
            Debug.LogError("Failed to load achievements: " + request.error);
        }
    }

    void ClearScrollView(Transform parent)
    {
        List<Transform> toDestroy = new List<Transform>();
        foreach (Transform child in parent)
        {
            toDestroy.Add(child);
        }

        foreach (Transform child in toDestroy)
        {
            Destroy(child.gameObject);
        }
    }

    void OnDestroy()
    {
        StopAllCoroutines(); // Prevents errors if object is destroyed during coroutine
    }

    [System.Serializable]
    private class AchievementListWrapper
    {
        public AchievementData[] array;
    }

    [System.Serializable]
    private class AchievementData
    {
        public string name;
        public string method;
        public bool unlocked;
        public bool hidden;
        public string achievedDate;
    }
}
