using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using TMPro;

namespace UI
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
        public GameObject saveFileButtonPrefab;
        public Transform contentPanel;
        public Button backButton;
        public TMP_Text noSaveMessage;
        public GameObject confirmationDialog;
        public TMP_Text confirmationMessage;
        public Button confirmButton;
        public Button cancelButton;
        private string _apiBaseUrl = "http://localhost:3000/savefiles/me";
        private string _currentSaveName;
        private int _currentProgress;
        private bool _isDeleteAction = false; // ✅ 区分加载 or 删除

        void Start()
        {
            backButton.onClick.AddListener(() => SceneManager.LoadScene("MainScene"));
            confirmButton.onClick.AddListener(() => ConfirmAction());

            cancelButton.onClick.AddListener(() => confirmationDialog.SetActive(false));
            confirmationDialog.SetActive(false);
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

                if (request.downloadHandler.text.StartsWith("["))
                {
                    savesList = JsonHelper.FromJson<SaveFile>(request.downloadHandler.text);
                }
                else
                {
                    SaveFile singleSave = JsonUtility.FromJson<SaveFile>(request.downloadHandler.text);
                    savesList.Add(singleSave);
                }

                if (savesList.Count == 0)
                {
                    Debug.Log("No save files returned from server. Displaying message.");
                    if (noSaveMessage != null)
                    {
                        noSaveMessage.text = "There is no archive for this account";
                        noSaveMessage.gameObject.SetActive(true);
                    }
                    yield break;
                }

                if (noSaveMessage != null)
                {
                    noSaveMessage.gameObject.SetActive(false);
                }

                foreach (SaveFile save in savesList)
                {
                    Debug.Log($"Instantiating save: {save.saveName}, Player: {save.playerName}, Progress: {save.progress}, Coins: {save.coins}");

                    GameObject newButton = Instantiate(saveFileButtonPrefab, contentPanel);
                    SaveFileButton buttonComponent = newButton.GetComponent<SaveFileButton>();
                    
                    if (buttonComponent != null)
                    {
                        buttonComponent.SetSaveData(save.saveName, save.playerName, save.progress, save.coins);
                        
                        Button deleteButton = newButton.transform.Find("Panel/DeleteButton").GetComponent<Button>();
                        if (deleteButton != null)
                        {
                            deleteButton.onClick.AddListener(() => ShowConfirmationDialog(save.saveName, save.progress, newButton, true));



                        }
                    }
                    else
                    {
                        Debug.LogError("SaveFileButton component not found on prefab!");
                    }
                    
                    newButton.GetComponent<Button>().onClick.AddListener(() => ShowConfirmationDialog(save.saveName, save.progress, newButton, false));

                }
            }
            else
            {
                Debug.LogError("Failed to fetch save files: " + request.downloadHandler.text);
            }
        }

        private GameObject _selectedSaveButton; // ✅ 存储被点击的按钮


        public void ShowConfirmationDialog(string saveName, int progress, GameObject saveButton, bool isDelete)
        {
            _currentSaveName = saveName;
            _currentProgress = progress;
            _isDeleteAction = isDelete;
            _selectedSaveButton = saveButton; // ✅ 存储当前存档按钮

            confirmationMessage.text = isDelete 
                ? $"Are you sure you want to delete the archive \"{saveName}\" ?" 
                : $"Are you sure you want to load the archive \"{saveName}\" ?";

            confirmationDialog.SetActive(true);
        }







        void ConfirmAction()
        {
            confirmationDialog.SetActive(false);

            if (_isDeleteAction)
            {
                Debug.Log($"🗑️ Confirmed DELETE: {_currentSaveName}");
                DeleteSaveFile(_currentSaveName, _selectedSaveButton);
            }
            else
            {
                Debug.Log($"✅ Confirmed LOAD: {_currentSaveName}");
                LoadSelectedSave();
            }

            // ✅ 确保状态被重置，不影响下一次操作
            _isDeleteAction = false;
            _selectedSaveButton = null;
        }





        void LoadSelectedSave()
        {
            PlayerPrefs.SetString("currentSaveName", _currentSaveName);
            
            if (_currentProgress == 0)
            {
                SceneManager.LoadScene("PrintfTeaching");
            }
            else
            {
                SceneManager.LoadScene("draftMap");
            }
        }

        private bool isDeleting = false;

        public void DeleteSaveFile(string saveName, GameObject saveButton)
        {
            if (isDeleting) return;
            isDeleting = true;

            StartCoroutine(DeleteSaveFileCoroutine(saveName, saveButton));
        }

        IEnumerator DeleteSaveFileCoroutine(string saveName, GameObject saveButton)
        {
            string url = $"http://localhost:3000/savefiles/{saveName}";
            UnityWebRequest request = UnityWebRequest.Delete(url);
            request.SetRequestHeader("Authorization", "Bearer " + PlayerPrefs.GetString("token", ""));

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log($"✅ Save file {saveName} deleted successfully.");

                if (saveButton != null)
                {
                    Destroy(saveButton);
                    Debug.Log($"🗑️ {saveName} UI button deleted.");
                }
                else
                {
                    Debug.LogWarning($"⚠️ Warning: Save button for {saveName} was already null. Skipping destroy.");
                }
            }
            else
            {
                Debug.LogError($"❌ Failed to delete save file {saveName}: {request.downloadHandler.text}");
            }

            isDeleting = false;
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
