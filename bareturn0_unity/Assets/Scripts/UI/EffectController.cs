using System.Collections;
using TMPro;
using UnityEngine;

public enum EffectType
{
    Poison,
    Bleed,
    Damage,
    Armor,    // 建议保持首字母大写
}

public class EffectController : MonoBehaviour
{
    [Header("配置")]
    public Animator animator; // 拖入 Animator
    public GameObject effectAnim;
    public TextMeshProUGUI valueText;        // 拖入子对象的 TMP_Text
    public float floatUpDistance = 0.5f;     // 浮字上升距离
    public float duration = 1.2f;            // 浮字总时长
    public float animDuration = 0.5f;        // 动画时长，可在 Inspector 调整
    public Vector3 positionOffset = new Vector3(1.5f, -10f, 0f);

    private Vector3 startPosition;

    /// <summary>
    /// 同时播放动画 + 浮字
    /// </summary>
    public void PlayFullEffect(Vector3 pos, int value, EffectType type)
    {
        effectAnim.SetActive(true);
        transform.position = pos;
        if (valueText != null)
            valueText.text = value > 0 ? $"-{value}" : $"+{-value}";
        if (animator != null)
            animator.SetTrigger(type.ToString());
        StartCoroutine(FloatAndDestroy());
        StartCoroutine(DestroyAfter(animDuration));
    }

    /// <summary>
    /// 只播放动画
    /// </summary>
    public void PlayEffect(Vector3 pos, EffectType type)
    {
        effectAnim.SetActive(true);
        transform.position = pos;
        if (animator != null)
            animator.SetTrigger(type.ToString());
        StartCoroutine(DestroyAfter(animDuration));
    }

    private IEnumerator DestroyAfter(float delay)
    {
        yield return new WaitForSeconds(delay);
        effectAnim.SetActive(false);
        Destroy(gameObject);
    }

    /// <summary>
    /// 只显示浮字
    /// </summary>
    public void PlayFloatingValue(Vector3 pos, int value)
    {
        effectAnim.SetActive(false);
        Vector3 anchoredPos = pos + positionOffset;
        valueText.transform.position = anchoredPos;
        //transform.position = anchoredPos;
        startPosition = anchoredPos;

        if (valueText != null)
            valueText.text = value > 0 ? $"-{value}" : $"+{-value}";
        StartCoroutine(FloatAndDestroy());
    }

    private IEnumerator FloatAndDestroy()
    {
        float elapsed = 0f;
        Vector3 endPos = startPosition + Vector3.up * floatUpDistance;

        // 让文字立即全不透明
        if (valueText != null)
        {
            var c0 = valueText.color;
            valueText.color = new Color(c0.r, c0.g, c0.b, 1f);
        }

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            valueText.transform.position = Vector3.Lerp(startPosition, endPos, t);

            if (t > 0.5f && valueText != null)
            {
                float alpha = Mathf.Lerp(1f, 0f, (t - 0.5f) * 2f);
                var c = valueText.color;
                valueText.color = new Color(c.r, c.g, c.b, alpha);
            }
            yield return null;
        }
        //Destroy(gameObject);
    }
}
