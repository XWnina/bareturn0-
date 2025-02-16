using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelButtonManager : MonoBehaviour
{
    [System.Serializable]
    public class LevelButton
    {
        public Button button;
        public Image icon;
        public TextMeshProUGUI levelText;
        public GameObject youAreHereIndicator;
        public Vector2 position;
    }

    public List<LevelButton> levelButtons;
    public Sprite passedIcon;
    public Sprite lockedIcon;
    private int currentLevel = 0;

    void Start()
    {
        // 获取 currentLevel
        currentLevel = MapUrlManager.CurrentLevel;
        SetButtonPositions();
        UpdateLevelButtons();
    }

    public void UpdateLevelButtons()
    {
        currentLevel = MapUrlManager.CurrentLevel; // 获取最新的 currentLevel

        for (int i = 0; i < levelButtons.Count; i++)
        {
            LevelButton lb = levelButtons[i];

            lb.levelText.text = "Level " + (i + 1);
            lb.icon.sprite = i < currentLevel ? passedIcon : lockedIcon;

            if (lb.youAreHereIndicator != null)
            {
                lb.youAreHereIndicator.gameObject.SetActive(i == currentLevel);
            }
            else
            {
                Debug.LogError($"Missing You Are Here Indicator in Level {i + 1}");
            }

            int levelIndex = i;
            lb.button.onClick.RemoveAllListeners();
            lb.button.onClick.AddListener(() => OnLevelButtonClick(levelIndex));
        }
    }

    public void OnLevelButtonClick(int levelIndex)
    {
        if (levelIndex <= currentLevel)
        {
            Debug.Log("Starting Level " + (levelIndex + 1));
            // SceneManager.LoadScene("Level" + (levelIndex + 1));
        }
        else
        {
            Debug.Log("This level is locked!");
        }
    }

    public void SetButtonPositions()
{
    foreach (var lb in levelButtons)
    {
        if (lb.button != null)
        {
            RectTransform rt = lb.button.GetComponent<RectTransform>();
            rt.anchoredPosition = lb.position; // 设置手动指定的位置
        }
    }
}

    public void UnlockNextLevel()
    {
        if (currentLevel < levelButtons.Count - 1)
        {
            currentLevel++;
            UpdateLevelButtons();
        }
    }
}
