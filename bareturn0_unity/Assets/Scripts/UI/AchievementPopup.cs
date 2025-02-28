using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class AchievementPopup : MonoBehaviour
{
    public Image achievementImage; // 关联的Image组件
    public TMP_Text achievementText; // 关联的Text组件

    void Start()
    {
        // 先隐藏 Image，避免一开始就显示
        if (achievementImage != null)
        {
            achievementImage.gameObject.SetActive(false);
        }

        // 检查 ProgressOne 是否存在并且为 true
        if (PlayerPrefs.HasKey("ProgressOne") && PlayerPrefs.GetString("ProgressOne") == "true")
        {
            ShowAchievement(); // 显示成就弹窗
            PlayerPrefs.DeleteKey("ProgressOne"); // 删除 ProgressOne，避免重复弹出
            PlayerPrefs.Save();
        }
    }

    void ShowAchievement()
    {
        // 显示成就弹窗
        achievementImage.gameObject.SetActive(true);
        achievementText.text = "Congrates: Level 1 Completed!\nUnlock achievement: Person You Know Who\nReward: Your own Name";

        // 启动协程，在3秒后隐藏
        StartCoroutine(HideAfterSeconds(5f));
    }

    IEnumerator HideAfterSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        achievementImage.gameObject.SetActive(false);
    }
}
