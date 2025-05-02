using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class AchievementPopUpManager : MonoBehaviour
{
    [Header("Popup References")]
    public GameObject achievementPopupPanel;     // The full panel
    public TMP_Text achievementText;             // The TMP text inside the panel
    public Button closeButton;                   // The X button to close it

    // Mapping from int (PlayerPrefs) to achievement name
    private Dictionary<int, string> achievementIdToName = new Dictionary<int, string>
    {
        { 1, "Person You Know Who" },
        { 2, "Fighter" },
        { 4, "Live For Your Own" },
        { 9, "Your Way"},        
        // Hidden achievements
        { 10, "Rich Kid"},
        { 11, "Mini Tycoon"},
        { 12, "Millionaire"},
        { 13, "Battle Expert"},

    };

    void Start()
    {
        achievementPopupPanel.SetActive(false); // Hide on start
        // PlayerPrefs.SetInt("AchievementUnlock", 2); // Reset for testing

        if (closeButton != null)
        {
            closeButton.onClick.AddListener(ClosePopup);
        }

        if (PlayerPrefs.HasKey("AchievementUnlock"))
        {
            int id = PlayerPrefs.GetInt("AchievementUnlock");

            if (achievementIdToName.TryGetValue(id, out string achievementName))
            {
                ShowPopup(achievementName);
            }

            // Ensure it only shows once
            PlayerPrefs.DeleteKey("AchievementUnlock");
        }
    }

    void ShowPopup(string achievementName)
    {
        if (achievementText != null)
        {
            achievementText.text = $"Achievement Unlocked:\n<b>{achievementName}</b>";
        }

        achievementPopupPanel.SetActive(true);

        // Optional: auto-close after 3 seconds
        CancelInvoke();
        Invoke(nameof(ClosePopup), 3f);
    }

    void ClosePopup()
    {
        achievementPopupPanel.SetActive(false);
    }
}
