using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

namespace UI // ✅ 添加正确的命名空间
{
    public static class JsonHelper
    {
        public static List<T> FromJson<T>(string json)
        {
            string newJson = "{\"array\":" + json + "}";
            Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(newJson);
            return wrapper.array;
        }

        [System.Serializable]
        private class Wrapper<T>
        {
            public List<T> array;
        }
    }

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
                Debug.Log("Raw JSON Response: " + request.downloadHandler.text);

                List<SaveFile> savesList = new List<SaveFile>();

                // ✅ 尝试解析为数组
                if (request.downloadHandler.text.StartsWith("["))
                {
                    savesList = JsonHelper.FromJson<SaveFile>(request.downloadHandler.text);
                }
                else // ✅ 解析单个对象
                {
                    SaveFile singleSave = JsonUtility.FromJson<SaveFile>(request.downloadHandler.text);
                    savesList.Add(singleSave);
                }

                if (savesList.Count == 0)
                {
                    Debug.LogError("No save files returned from server!");
                }

                foreach (SaveFile save in savesList)
                {
                    Debug.Log($"Instantiating save: {save.saveName}, Player: {save.playerName}, Progress: {save.progress}, Coins: {save.coins}");

                    GameObject newButton = Instantiate(saveFileButtonPrefab, contentPanel);
                    SaveFileButton buttonComponent = newButton.GetComponent<SaveFileButton>();
                    
                    if (buttonComponent != null)
                    {
                        buttonComponent.SetSaveData(save.saveName, save.playerName, save.progress, save.coins);
                    }
                    else
                    {
                        Debug.LogError("SaveFileButton component not found on prefab!");
                    }
                    
                    newButton.GetComponent<Button>().onClick.AddListener(() => SetCurrentSaveAndLoad(save.saveName));
                }
            }
            else
            {
                Debug.LogError("Failed to fetch save files: " + request.downloadHandler.text);
            }
        }

        void SetCurrentSaveAndLoad(string saveName)
        {
            _currentSaveName = saveName; // ✅ 存储 saveName 在全局变量
            PlayerPrefs.SetString("currentSaveName", saveName);
            SceneManager.LoadScene("draftMap"); // 加载游戏场景
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
