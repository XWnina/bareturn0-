using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using UnityEngine.UI;

public class LogoutManager : MonoBehaviour
{
    public Button logoutButton;
    private string logoutUrl = "https://localhost:3000/users/logout"; // Replace with actual API endpoint

    public void Logout(string username)
    {
        StartCoroutine(LogoutRequest(username));
    }

    private IEnumerator LogoutRequest(string username)
    {
        WWWForm form = new WWWForm();
        form.AddField("username", username);

        using (UnityWebRequest www = UnityWebRequest.Post(logoutUrl, form))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Logout successful: " + www.downloadHandler.text);
            }
            else
            {
                Debug.LogError("Logout failed: " + www.error);
            }
        }

        ClearAllPlayerPrefs();
    }

    public void ClearAllPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save(); // Ensure changes are saved
        Debug.Log("All PlayerPrefs data cleared.");

        UnityEngine.SceneManagement.SceneManager.LoadScene("LoginScene");
    }
}
