using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class LevelButtonManager : MonoBehaviour
{
    public static LevelButtonManager Instance;

    [System.Serializable]
    public class LevelData
    {
        public Button levelButton; // 关卡按钮
        public Image icon; // 关卡图标
        public Sprite passedSprite; // 通过时的图标
        public Sprite lockedSprite; // 未通过的图标
        public bool isPassed; // 是否通过
    }

    public List<LevelData> levels; // 关卡列表
    public int currentLevelIndex = 0; // 当前关卡索引
    public GameObject youAreHereIndicator; // “You Are Here” 指示器

    private void Awake()
    {
        Instance = this;
        currentLevelIndex = MapUrlManager.CurrentLevel;
        UpdateLevelUI();
    }
    public void UpdateLevelUI()
    {
        for (int i = 0; i < levels.Count; i++)
        {
            LevelData level = levels[i];

            if (i + 1 < currentLevelIndex)
            {
                level.icon.sprite = level.passedSprite;
            }
            else if (i + 1 == currentLevelIndex)
            {
                youAreHereIndicator.transform.position = level.levelButton.transform.position;
                youAreHereIndicator.SetActive(true);
            }
            else
            {
                level.icon.sprite = level.lockedSprite;
            }
            


        }
    }

    public void CompleteLevel(int levelIndex)
    {
        if (levelIndex >= 0 && levelIndex < levels.Count)
        {
            levels[levelIndex].isPassed = true;
            currentLevelIndex = levelIndex + 1; // 进入下一关
            UpdateLevelUI();
        }
    }
}