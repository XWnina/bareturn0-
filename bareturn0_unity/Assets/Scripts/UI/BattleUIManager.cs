using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleUIManager : MonoBehaviour
{
    public static BattleUIManager Instance;

    [Header("Battle Result UI")]
    public GameObject battleResultPanel; // 结算面板
    public TextMeshProUGUI resultText; // 胜利/失败文本
    //public Button returnButton; // 返回菜单按钮

    [Header("Basic Battle Informations")]
    public TextMeshProUGUI playerHealthText;
    public TextMeshProUGUI enemyHealthText;
    public TextMeshProUGUI playerShieldText;
    public TextMeshProUGUI playerEnergyText;
    public TextMeshProUGUI roundText;

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
        battleResultPanel.SetActive(true); // 显示面板
        resultText.text = isVictory ? "YOU WIN!!!" : "YOU LOSS...";
        resultText.color = isVictory ? Color.green : Color.red;

        // 监听返回菜单按钮
        //returnButton.onClick.RemoveAllListeners();
        //returnButton.onClick.AddListener(ReturnToMainMenu);
    }

    public void ReturnToMainMenu()
    {
        Debug.Log("Returning to main menu...");
        // 这里可以加载主菜单场景，例如 SceneManager.LoadScene("MainMenu");
    }

    private void Update()
    {
        // 检查 BattleManager 是否存在
        if (BattleManager.Instance != null)
        {
            // 更新玩家血量显示（当前血量/最大血量）
            playerHealthText.text = $"HP: {BattleManager.Instance.player.currentHealth}/{BattleManager.Instance.player.maxHealth}";

            // 更新敌人血量显示
            enemyHealthText.text = $"HP: {BattleManager.Instance.enemy.currentHealth}/{BattleManager.Instance.enemy.maxHealth}";

            // 更新玩家护盾（护甲）显示
            playerShieldText.text = $"Shield: {BattleManager.Instance.player.currentArmor}";

            // 更新玩家剩余能量显示
            playerEnergyText.text = $"Energy: {BattleManager.Instance.player.currentEnergy}";

            // 更新当前回合数显示
            roundText.text = $"Round: {BattleManager.Instance.CurrentRoundNumber}";
        }
    }
}
