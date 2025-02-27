using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingController : MonoBehaviour 
{
    public Button LogoutButton; // Drag the button into this field in the Inspector
    private void Start()
    {
        if (LogoutButton != null)
        {
            LogoutButton.onClick.AddListener(ClearAllPlayerPrefs);
            // Debug.Log("Button listener successfully assigned.");
        }
        else
        {
            Debug.LogError("LogoutButton is not assigned in the Inspector!");
        }
    }

    public void ClearAllPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save(); // Ensure changes are saved
        Debug.Log("All PlayerPrefs data cleared.");

        UnityEngine.SceneManagement.SceneManager.LoadScene("LoginScene");
    }
}