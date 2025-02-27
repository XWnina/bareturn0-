namespace UI
{
    using UnityEngine;
    using UnityEngine.SceneManagement;
    using UnityEngine.UI;
    using TMPro;
    using System.Collections;
    using UnityEngine.Networking;
    using System.Text;

    public class MainPage : MonoBehaviour
    {
        public Button newGameButton;
        public Button loadingButton;
        public Button settingButton; // 添加 Setting 按钮
        public GameObject popupWindow;
        public TMP_InputField saveNameInput;
        public Button confirmButton;
        public Button cancelButton;

        private string _apiBaseUrl = "http://localhost:3000/savefiles";

        void Start()
        {
            newGameButton.onClick.AddListener(ShowPopup);
            loadingButton.onClick.AddListener(GoToLoadScene);
            settingButton.onClick.AddListener(GoToSettings); // 绑定 Setting 按钮事件
            confirmButton.onClick.AddListener(StartNewGame);
            cancelButton.onClick.AddListener(ClosePopup);

            popupWindow.SetActive(false);
        }

        void ShowPopup()
        {
            popupWindow.SetActive(true);
        }

        void ClosePopup()
        {
            popupWindow.SetActive(false);
        }

        void StartNewGame()
        {
            string saveName = saveNameInput.text.Trim();
            if (string.IsNullOrEmpty(saveName))
            {
                Debug.LogError("Save name cannot be empty!");
                return;
            }

            PlayerPrefs.SetString("currentSaveName", saveName);
            StartCoroutine(CreateSaveFile(saveName));
        }

        IEnumerator CreateSaveFile(string saveName)
        {
            string token = PlayerPrefs.GetString("token", "");

            if (string.IsNullOrEmpty(token))
            {
                Debug.LogError("No token found! Redirecting to Login Page...");
                SceneManager.LoadScene("LoginScene");
                yield break;
            }

            string jsonData = $"{{\"saveName\": \"{saveName}\", \"progress\": 0, \"coins\": 0}}";
            UnityWebRequest request = new UnityWebRequest(_apiBaseUrl, "POST");
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("Authorization", "Bearer " + token);

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Save File Created Successfully: " + saveName);
                SceneManager.LoadScene("PrintfTeaching");
            }
            else
            {
                Debug.LogError("Failed to create save file: " + request.downloadHandler.text);
            }
        }

        void GoToLoadScene()
        {
            SceneManager.LoadScene("LoadScene");
        }

        void GoToSettings()
        {
            SceneManager.LoadScene("SettingScene");
        }
    }
}
