using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;   


public class LevelButtonManager : MonoBehaviour
{
    public static LevelButtonManager Instance;
    public static LevelData currentLevelData;

    [System.Serializable]
    public class LevelButtonData
    {
        public Button levelButton; // 关卡按钮
        public Image icon; // 关卡图标
        public Sprite passedSprite; // 通过时的图标
        public Sprite lockedSprite; // 未通过的图标
        public bool isPassed; // 是否通过

        public LevelData levelDataAsset; //关卡数据
    }

    public List<LevelButtonData> levels; // 关卡列表
    public int currentLevelIndex = 0; // 当前关卡索引
    public GameObject youAreHereIndicator; // “You Are Here” 指示器

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        // 给每个关卡按钮添加点击事件
        for (int i = 0; i < levels.Count; i++)
        {
            int index = i; // 必须在循环中用局部变量
            levels[i].levelButton.onClick.AddListener(() => OnClickLevelButton(index));
        }
    }

    // 点击某个关卡按钮时
    private void OnClickLevelButton(int levelIndex)
    {
        if (levelIndex > currentLevelIndex + 1)
        {
            Debug.Log($"Level {levelIndex + 1} is locked.");
            return;
        }

        if (levels[levelIndex].levelDataAsset != null)
        {
            currentLevelData = levels[levelIndex].levelDataAsset;
            // 切换场景
            SceneManager.LoadScene("BattleScenes");
        }
        else
        {
            Debug.LogWarning($"No levelDataAsset assigned for level {levelIndex}."); //TODO
        }
    }
    public void UpdateLevelUI()
{
    currentLevelIndex = MapUrlManager.CurrentLevel;
    Debug.Log($"LevelButtonController: Updating Level UI: Current Level = {currentLevelIndex}");

    for (int i = 0; i < levels.Count; i++)
    {
        LevelButtonData level = levels[i];

        if (i < currentLevelIndex) // Levels that have been completed
        {
            level.icon.sprite = level.passedSprite;
        }

        if (i == currentLevelIndex) // The current level (where the indicator should be)
        {
                Vector3 buttonPos = level.levelButton.transform.position;

                // 这里假设向上偏移 50 个单位
                Vector3 offset = new Vector3(0, 1f, 0);

                // 将指示器放置到按钮上方
                youAreHereIndicator.transform.position = buttonPos + offset;

                youAreHereIndicator.SetActive(true);
            }
        else if (i > currentLevelIndex) // Locked levels
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