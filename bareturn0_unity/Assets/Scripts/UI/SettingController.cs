using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class SettingController : MonoBehaviour 
{
    public Button LogoutButton; // Drag the button into this field in the Inspector
    private string logoutUrl = "http://localhost:3000/users/logout"; // Replace with your actual API endpoint

    private void Start()
    {
        if (LogoutButton != null)
        {
            LogoutButton.onClick.AddListener(HandleLogout);
        }
        else
        {
            Debug.LogError("LogoutButton is not assigned in the Inspector!");
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
        UnityWebRequest request = new UnityWebRequest(logoutUrl, "POST");
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

    public void ClearAllPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        SceneManager.LoadScene("LoginScene");
    }
}
