using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

namespace UI
{
    public class SettingController : MonoBehaviour 
    {
        [SerializeField] private Button logoutButton; // 退出按钮
        [SerializeField] private Button backButton; // 返回按钮

        private string _logoutUrl = "http://localhost:3000/users/logout"; // API 端点
        private string _previousScene; // 存储上一个场景

        private void Start()
        {
            // 记录进入设置前的场景
            if (PlayerPrefs.HasKey("PreviousScene"))
            {
                _previousScene = PlayerPrefs.GetString("PreviousScene");
            }

            if (logoutButton != null)
            {
                logoutButton.onClick.AddListener(HandleLogout);
            }
            else
            {
                Debug.LogError("logoutButton is not assigned in the Inspector!");
            }

            if (backButton != null)
            {
                backButton.onClick.AddListener(HandleBack);
            }
            else
            {
                Debug.LogError("backButton is not assigned in the Inspector!");
            }
        }

        private void HandleLogout()
        {
            string token = PlayerPrefs.GetString("token", "");
            if (!string.IsNullOrEmpty(token))
            {
                StartCoroutine(LogoutRequest(token));
            }
            else
            {
                Debug.LogWarning("No token found. Clearing PlayerPrefs and returning to Login.");
                ClearAllPlayerPrefs();
            }
        }

        private IEnumerator LogoutRequest(string token)
        {
            UnityWebRequest request = new UnityWebRequest(_logoutUrl, "POST");
            request.SetRequestHeader("Authorization", "Bearer " + token);
            request.uploadHandler = new UploadHandlerRaw(new System.Text.UTF8Encoding().GetBytes("{}"));
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Logout successful: " + request.downloadHandler.text);
            }
            else
            {
                Debug.LogError("Logout failed: " + request.error);
            }

            ClearAllPlayerPrefs();
        }

        private void ClearAllPlayerPrefs()
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
            SceneManager.LoadScene("LoginScene");
        }

        private void HandleBack()
        {
            if (!string.IsNullOrEmpty(_previousScene))
            {
                SceneManager.LoadScene(_previousScene);
            }
            else
            {
                Debug.LogWarning("No previous scene stored. Returning to default scene.");
                SceneManager.LoadScene("MainScene"); // 你可以换成合适的默认场景
            }
        }
    }
}
