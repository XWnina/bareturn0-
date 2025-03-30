using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class BattleUIManager : MonoBehaviour
{
    public static BattleUIManager Instance;

    [Header("Battle Result UI")]
    public GameObject battleResultPanel; // 结算面板
    public TextMeshProUGUI resultText; // 胜利/失败文本
    public Button returnButton; // 返回菜单按钮

    [Header("Basic Battle Informations")]
    public TextMeshProUGUI playerHealthText;
    public TextMeshProUGUI playerShieldText;
    public TextMeshProUGUI playerEnergyText;
    public TextMeshProUGUI roundText;
    public Slider PlayerHealthBar;

    [Header("Energy Segment Images (in order)")]
    public List<Image> energySegments;
    public Sprite litSegmentSprite;   // 亮起时的格子素材
    public Sprite unlitSegmentSprite; // 未亮时的格子素材

    [Header("Warnings")]
    public TextMeshProUGUI energyWarningText;

    private void Awake()
    {
        Instance = this;

        // 初始隐藏Energy Warning
        if (energyWarningText != null)
        {
            energyWarningText.gameObject.SetActive(false); 
        }
    }


    // 显示“能量不足”提示
    public void ShowEnergyWarning()
    {
        if (energyWarningText == null) return;

        StopAllCoroutines(); // 确保不会重复执行多个淡化
        StartCoroutine(FadeOutWarning());
    }

    //淡出效果
    private IEnumerator FadeOutWarning()
    {
        energyWarningText.gameObject.SetActive(true);
        energyWarningText.alpha = 1; // 立即变为可见

        yield return new WaitForSeconds(0.5f); // 停留 0.5 秒

        // 渐渐淡出
        float fadeDuration = 0.5f;
        float elapsedTime = 0;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            energyWarningText.alpha = Mathf.Lerp(1, 0, elapsedTime / fadeDuration);
            yield return null;
        }

        energyWarningText.gameObject.SetActive(false); // 完全消失后隐藏
    }

    // 显示战斗结果
    public void ShowBattleResult(bool isVictory)
    {
        battleResultPanel.SetActive(true); // 显示面板a
        resultText.text = isVictory ? "YOU WIN!!!" : "YOU LOSS...";
        resultText.color = isVictory ? Color.green : Color.red;

        if (isVictory)
        {
            BattleManager.Instance.sendProgress();
        }
        // 监听返回菜单按钮
        returnButton.onClick.RemoveAllListeners();
        returnButton.onClick.AddListener(ReturnToMap);
    }

    public void ReturnToMap()
    {
        Debug.Log("Returning to map...");
        SceneManager.LoadScene("draftMap");
    }

    public void UpdatePlayerUIBar()   
    {
        PlayerHealthBar.maxValue = BattleManager.Instance.player.maxHealth;
        PlayerHealthBar.value = BattleManager.Instance.player.currentHealth;

        Image fillImage = PlayerHealthBar.fillRect.GetComponent<Image>();
        if (BattleManager.Instance.player.currentArmor > 0)
        {
            // 灰蓝色（你可以根据需要调整RGB值）
            fillImage.color = new Color(0.6f, 0.6f, 0.6f, 1f);
        }
        else
        {
            // 红色
            fillImage.color = Color.red;
        }

        for (int i = 0; i < energySegments.Count; i++)
        {
            if (i < BattleManager.Instance.player.currentEnergy)
            {
                // 亮起
                energySegments[i].sprite = litSegmentSprite;
            }
            else
            {
                // 未亮
                energySegments[i].sprite = unlitSegmentSprite;
            }
        }

    }

    private void Update()
    {
        // 检查 BattleManager 是否存在
        if (BattleManager.Instance != null)
        {
            // 更新玩家血量显示（当前血量/最大血量）
            playerHealthText.text = $"{BattleManager.Instance.player.currentHealth}/{BattleManager.Instance.player.maxHealth}";

            // 更新玩家护盾（护甲）显示
            playerShieldText.text = $"{BattleManager.Instance.player.currentArmor}";

            // 更新玩家剩余能量显示
            playerEnergyText.text = $"{BattleManager.Instance.player.currentEnergy}";

            // 更新当前回合数显示
            roundText.text = $"Round: {BattleManager.Instance.CurrentRoundNumber}";

            //跟新玩家血条，护盾条
            UpdatePlayerUIBar();
        }
    }
}
