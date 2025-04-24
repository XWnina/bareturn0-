using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;


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
            //if (index == 3)
            //{
            //    levels[i].levelButton.onClick.AddListener(() => SceneManager.LoadScene("calcuTeaching"));
            //}
            EventTrigger trigger = levels[i].levelButton.gameObject.AddComponent<EventTrigger>();

            // mouse on
            EventTrigger.Entry enterEntry = new EventTrigger.Entry();
            enterEntry.eventID = EventTriggerType.PointerEnter;
            enterEntry.callback.AddListener((data) =>
            {
                string tooltip = GetTooltipTextForLevel(index);
                LevelTooltip.Instance.ShowTooltip(tooltip, Input.mousePosition);
            });
            trigger.triggers.Add(enterEntry);

            // mouse off
            EventTrigger.Entry exitEntry = new EventTrigger.Entry();
            exitEntry.eventID = EventTriggerType.PointerExit;
            exitEntry.callback.AddListener((data) =>
            {
                LevelTooltip.Instance.HideTooltip();
            });
            trigger.triggers.Add(exitEntry);
        }
    }

    // 点击某个关卡按钮时
    private void OnClickLevelButton(int levelIndex)
    {
        if (levelIndex > currentLevelIndex)
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

            if ((levelIndex + 1) == 3)
            {
                SceneManager.LoadScene("calcuTeaching");
            }
            else if ((levelIndex + 1 == 4))
            {
                SceneManager.LoadScene("calcuProblem");
            }
            else if ((levelIndex + 1 == 6))
            {
                SceneManager.LoadScene("JumpGame");
            }
        }
    }
    public void UpdateLevelUI()
    {
        currentLevelIndex = MapUrlManager.CurrentLevel;
        // Debug.Log($"LevelButtonController: Updating Level UI: Current Level = {currentLevelIndex}");

        for (int i = 0; i < levels.Count; i++)
        {
            //Debug.Log($"Levels.count {levels.Count}, i {i}");
            LevelButtonData level = levels[i];

            if (i < currentLevelIndex) // Levels that have been completed
            {
                //Debug.Log($"button {i} color change, currentLevelIndex is {currentLevelIndex}");
                level.icon.sprite = level.passedSprite;
                // if(level.icon.sprite == level.passedSprite){
                //     Debug.Log($"change successful!");
                // }
            }
            else if (i == currentLevelIndex) // The current level (where the indicator should be)
            {
                //Debug.Log($"currentLevelIndex is {currentLevelIndex}");
                Vector3 buttonPos = level.levelButton.transform.position;

                // 这里假设向上偏移 1 个单位
                Vector3 offset = new Vector3(0, 1f, 0);

                // 将指示器放置到按钮上方
                youAreHereIndicator.transform.position = buttonPos + offset;

                youAreHereIndicator.SetActive(true);
            }
            else if (i > currentLevelIndex) // Locked levels
            {
                level.icon.sprite = level.lockedSprite;
            }
            //levels[0].icon.sprite = level.lockedSprite;
        }
        // 控制交互范围
        int unlockThreshold = MapUrlManager.CurrentLevel;

        for (int i = 0; i < levels.Count; i++)
        {
            bool interactable = false;

            if (unlockThreshold >= 8)
            {
                interactable = true; // 所有关卡
            }
            else if (unlockThreshold >= 4 && i < 8)
            {
                interactable = true; // 1~8 关
            }
            else if (unlockThreshold < 4 && i < 4)
            {
                interactable = true; // 1~4 关
            }

            levels[i].levelButton.interactable = interactable;
        }

    }
    private string GetTooltipTextForLevel(int index)
    {
        switch (index)
        {
            case 0: return "Level 1:\nTopic: printf in C\nDetails: different typs of printf(), %s,%f, etc.";
            case 1: return "Level 2:\nTopic: Battle!\nDetails: card game\nRewards: +100 Coins";
            case 2: return "Level 3:\nTopic: Calculation level (1/2)\nDetails: different datatypes for math";
            case 3: return "Level 4:\nTopic: Calculation level (2/2)\nDetails: more complex senaieros with calculation";
            case 4: return "Level 5:\nTopic: Battle!\nDetails: card game\nRewards: +100 Coins";
            case 5: return "Level 6:\nTopic:Jump game!\nDetails: dataflow(if-else)";
            default: return "Level ?:\nTopic:???\nDetails: ???";
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