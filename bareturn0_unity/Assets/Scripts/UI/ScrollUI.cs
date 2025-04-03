using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class ScrollUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("UI Components")]
    public Image image;
    public TextMeshProUGUI nameText;
    public Button button;

    public Sprite ifSprite;
    public Sprite whileSprite;
    public Sprite mathSprite;

    private Vector3 originalScale;   // 记录初始缩放
    public bool allowHoverEffect = true;
    void Awake()
    {
        originalScale = transform.localScale;
    }

    public void setScroll(string name)
    {
        nameText.text = name;
        //Debug.Log(name);

        if (name.ToLower() == "math")
        {
            Debug.Log("name is math");
            image.sprite = mathSprite;
        }

        if (name.ToLower() == "if")
        {
            image.sprite = ifSprite;
        }

        if (name.ToLower() == "while")
        {
            image.sprite = whileSprite;
        }
    }

    public string getScrollName()
    {
        return nameText.text;
    }
    // 鼠标悬停接口
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!allowHoverEffect) return;
        // 放大
        transform.localScale = originalScale * 1.2f;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!allowHoverEffect) return;
        // 恢复
        transform.localScale = originalScale;
    }
}
