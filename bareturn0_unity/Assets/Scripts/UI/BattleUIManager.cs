using UnityEngine;
using TMPro; // 如果你使用 TextMeshPro，请确保已经导入 TextMeshPro 包

public class BattleUIManager : MonoBehaviour
{
    [Header("UI 元素引用")]
    public TextMeshProUGUI playerHealthText;
    public TextMeshProUGUI enemyHealthText;
    public TextMeshProUGUI playerShieldText;
    public TextMeshProUGUI playerEnergyText;
    public TextMeshProUGUI roundText;

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
