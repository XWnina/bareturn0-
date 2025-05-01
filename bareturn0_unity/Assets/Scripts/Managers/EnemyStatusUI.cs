using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class EnemyStatusUI : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_Text hpText;         // 血量文本
    public Slider hpBar;            // 血条Slider
    public TMP_Text shieldText;     // Shield文本
    public GameObject buffPanel;
    public BuffUIPrefab buffPrefab;

    [Header("New Element: Card Name")]
    public TMP_Text enemyCardNameText;

    // 引用血条 Fill 部分的 Image，用于调整颜色
    public Image hpBarFill;

    /// <summary>
    /// 更新敌人状态显示
    /// </summary>
    /// <param name="currentHP">当前血量</param>
    /// <param name="maxHP">最大血量</param>
    /// <param name="shield">当前盾量</param>
    public void UpdateStatus(int currentHP, int maxHP, int shield)
    {
        if (hpText != null)
        {
            hpText.text = $"{currentHP}/{maxHP}";
        }
        if (hpBar != null)
        {
            hpBar.maxValue = maxHP;
            hpBar.value = currentHP;
        }
        if (shieldText != null)
        {
            shieldText.text = $"{shield}";
        }
        if (hpBarFill != null)
        {
            // 血条颜色：默认红色；当盾量大于1时改为灰色（这里可以根据需要微调）
            if (shield > 0)
                hpBarFill.color = new Color(0.6f, 0.6f, 0.6f, 1f);
            else
                hpBarFill.color = Color.red;
        }
    }
    public void ShowCardName(string cardName)
    {
        if (enemyCardNameText == null)
            return;

        // 设置文本
        enemyCardNameText.text = cardName;
        // 设置颜色为完全不透明
        Color c = enemyCardNameText.color;
        c.a = 1f;
        enemyCardNameText.color = c;

        // 启动淡出
        StartCoroutine(FadeOutCardName());
    }

    private IEnumerator FadeOutCardName()
    {
        float duration = 1f; // 淡出时间
        float elapsed = 0f;
        Color originalColor = enemyCardNameText.color;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            // 逐渐将 alpha 值从 1 淡出到 0
            enemyCardNameText.color = new Color(originalColor.r, originalColor.g, originalColor.b, Mathf.Lerp(1f, 0f, t));
            yield return null;
        }
        // 淡出完成后，将文本设为空（或隐藏该对象）
        enemyCardNameText.text = "";
    }
    public void updateBuffUI(int poisonLayers, int burnLayers, int bleedlayers, int sharpnessLayers)
    {
        foreach (Transform child in buffPanel.transform)
        {
            Destroy(child.gameObject);
        }

        if (poisonLayers > 0)
        {
            BuffUIPrefab newBuff = Instantiate(buffPrefab, buffPanel.transform);
            newBuff.buffImage.sprite = newBuff.poisonSprite;
            newBuff.BuffCount.text = poisonLayers.ToString();
        }

        if (burnLayers > 0)
        {
            BuffUIPrefab newBuff = Instantiate(buffPrefab, buffPanel.transform);
            newBuff.buffImage.sprite = newBuff.burnSprite;
            newBuff.BuffCount.text = burnLayers.ToString();
        }

        if(bleedlayers > 0)
        {
            BuffUIPrefab newBuff = Instantiate(buffPrefab, buffPanel.transform);
            newBuff.buffImage.sprite = newBuff.bleedSprite;
            newBuff.BuffCount.text = bleedlayers.ToString();
        }

        if (sharpnessLayers > 0)
        {
            BuffUIPrefab newBuff = Instantiate(buffPrefab, buffPanel.transform);
            newBuff.buffImage.sprite = newBuff.sharpnessSprite;
            newBuff.BuffCount.text = sharpnessLayers.ToString();
        }

    }
}