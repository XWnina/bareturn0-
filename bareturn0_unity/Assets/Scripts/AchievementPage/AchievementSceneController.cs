using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using TMPro;

namespace UI
{    public class AchievementSceneController : MonoBehaviour
    {
        public GameObject achievementPrefab; // Achievement UI Prefab
        public Transform contentPanel; // Scrollable Content Panel
        public Button backButton;
        private string _apiBaseUrl = "http://localhost:3000/achievements";
        private string _currentSaveName;

        void Start()
        {
            _currentSaveName = PlayerPrefs.GetString("currentSaveName", "");
            backButton.onClick.AddListener(() => SceneManager.LoadScene("draftMap"));
            StartCoroutine(FetchAchievements());
        }

        IEnumerator FetchAchievements()
        {
            string token = PlayerPrefs.GetString("token", "");

            if (string.IsNullOrEmpty(token))
            {
                Debug.LogError("No token found! Redirecting to Login Page...");
                SceneManager.LoadScene("LoginScene");
                yield break;
            }

            // Fetch all achievements
            UnityWebRequest requestAll = UnityWebRequest.Get($"{_apiBaseUrl}/all");
            requestAll.SetRequestHeader("Authorization", "Bearer " + token);
            yield return requestAll.SendWebRequest();

            List<Achievement> allAchievements = new List<Achievement>();
            if (requestAll.result == UnityWebRequest.Result.Success)
            {
                allAchievements = JsonHelper.FromJson<Achievement>(requestAll.downloadHandler.text);
            }
            else
            {
                Debug.LogError("Failed to fetch achievements: " + requestAll.downloadHandler.text);
                yield break;
            }

            // Fetch unlocked achievements for the save file
            List<UnlockedAchievement> unlockedAchievements = new List<UnlockedAchievement>();
            if (!string.IsNullOrEmpty(_currentSaveName))
            {
                UnityWebRequest requestUnlocked = UnityWebRequest.Get($"{_apiBaseUrl}/{_currentSaveName}/unlocked");
                requestUnlocked.SetRequestHeader("Authorization", "Bearer " + token);
                yield return requestUnlocked.SendWebRequest();

                if (requestUnlocked.result == UnityWebRequest.Result.Success)
                {
                    unlockedAchievements = JsonHelper.FromJson<UnlockedAchievement>(requestUnlocked.downloadHandler.text);
                }
                else
                {
                    Debug.LogWarning("No unlocked achievements found for this save.");
                }
            }

            DisplayAchievements(allAchievements, unlockedAchievements);
        }

        void DisplayAchievements(List<Achievement> allAchievements, List<UnlockedAchievement> unlockedAchievements)
        {
            foreach (Transform child in contentPanel)
            {
                Destroy(child.gameObject);
            }

            foreach (Achievement achievement in allAchievements)
            {
                GameObject newAchievement = Instantiate(achievementPrefab, contentPanel);
                TMP_Text nameText = newAchievement.transform.Find("NameText").GetComponent<TMP_Text>();
                TMP_Text detailText = newAchievement.transform.Find("DetailText").GetComponent<TMP_Text>();

                bool isUnlocked = unlockedAchievements.Exists(a => a.achievementId == achievement._id);
                if (isUnlocked)
                {
                    UnlockedAchievement unlocked = unlockedAchievements.Find(a => a.achievementId == achievement._id);
                    nameText.text = $"<color=green>{achievement.name}</color>";
                    detailText.text = $"Unlocked on: {unlocked.achievedDate}";
                }
                else
                {
                    nameText.text = $"<color=red>{achievement.name}</color>";
                    detailText.text = $"Unlock method: {achievement.method}";
                }
            }
        }

        [System.Serializable]
        private class Achievement
        {
            public string _id;
            public string name;
            public string method;
        }

        [System.Serializable]
        private class UnlockedAchievement
        {
            public string achievementId;
            public string achievedDate;
        }
    }
}
