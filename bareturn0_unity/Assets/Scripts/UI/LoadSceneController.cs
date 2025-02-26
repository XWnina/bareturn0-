using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

namespace UI // ✅ 添加正确的命名空间
{
    public class LoadSceneController : MonoBehaviour
    {
        public GameObject saveFileButtonPrefab; // 预制体
        public Transform contentPanel; // `Content` 物体
        public Button backButton;
        private string _apiBaseUrl = "http://localhost:3000/savefiles/me"; // 获取当前用户存档
        private string _currentSaveName; // ✅ 添加全局变量

        void Start()
        {
            backButton.onClick.AddListener(() => SceneManager.LoadScene("MainScene"));
            StartCoroutine(FetchSaveFiles());
        }

        IEnumerator FetchSaveFiles()
        {
            string token = PlayerPrefs.GetString("token", "");

            if (string.IsNullOrEmpty(token))
            {
                Debug.LogError("No token found! Redirecting to Login Page...");
                SceneManager.LoadScene("LoginScene");
                yield break;
            }

            UnityWebRequest request = UnityWebRequest.Get(_apiBaseUrl);
            request.SetRequestHeader("Authorization", "Bearer " + token);

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Save Files: " + request.downloadHandler.text);

                // ✅ 先尝试解析为单个对象
                SaveFile singleSave = JsonUtility.FromJson<SaveFile>(request.downloadHandler.text);
    
                List<SaveFile> savesList = new List<SaveFile> { singleSave };

                foreach (SaveFile save in savesList)
                {
                    GameObject newButton = Instantiate(saveFileButtonPrefab, contentPanel);
                    TMP_Text buttonText = newButton.GetComponentInChildren<TMP_Text>();

                    if (buttonText != null)
                    {
                        buttonText.text = $"{save.saveName} - Player: {save.playerName} - Progress: {save.progress}, Coins: {save.coins}";
                    }
                    else
                    {
                        Debug.LogError("TMP_Text component not found on SaveFileButton prefab!");
                    }

                    newButton.GetComponent<Button>().onClick.AddListener(() => SetCurrentSaveAndLoad(save.saveName));
                }
            }

        }

        void SetCurrentSaveAndLoad(string saveName)
        {
            _currentSaveName = saveName; // ✅ 存储 saveName 在全局变量
            PlayerPrefs.SetString("currentSaveName", saveName);
            SceneManager.LoadScene("draftMap"); // 加载游戏场景
        }

        [System.Serializable]
        private class SaveFileResponse
        {
            public List<SaveFile> saves;
        }

        [System.Serializable]
        private class SaveFile
        {
            public string saveName;
            public string playerName;
            public int progress;
            public int coins;
        }
    }
    
}
