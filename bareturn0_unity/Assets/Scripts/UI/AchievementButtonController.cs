using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AchievementButtonController : MonoBehaviour
{
    public Button achievementButton; // Drag the button into this field in the Inspector

    private void Start()
    {
        if (achievementButton != null)
        {
            achievementButton.onClick.AddListener(LoadAchievementScene);
            Debug.Log("Button listener successfully assigned.");
        }
        else
        {
            Debug.LogError("AchievementButton is not assigned in the Inspector!");
        }
    }

    public void LoadAchievementScene()
    {
        Debug.Log("Achievement button clicked! Loading AchievementScene...");
        SceneManager.LoadScene("AchievementScene");
    }
}
