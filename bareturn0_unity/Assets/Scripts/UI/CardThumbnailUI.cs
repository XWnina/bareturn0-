using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro; // 如果你用TMP

public class CardThumbnailUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("UI Components")]
    public Image cardFrame;
    public Image artworkImage;
    public Image imageUI;
    public Image CardNameUI;
    public TextMeshProUGUI cardNameText;
    public Image CostUI;
    public TextMeshProUGUI costText;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI symbol;
    public Button button;

    public bool allowHoverEffect = true;
    private CardData cardData;


    // 预设不同品质的卡牌边框
    public Sprite commonFrame;
    public Sprite rareFrame;
    public Sprite epicFrame;

    // 预设不同品质的卡牌名称颜色
    private Color commonColor = new Color(0.72f, 0.45f, 0.20f); // 铜色
    private Color rareColor = new Color(0.75f, 0.75f, 0.75f);   // 银色
    private Color epicColor = new Color(1f, 0.84f, 0f);         // 金色

    private Vector3 originalScale;   // 记录初始缩放


    void Awake()
    {
        originalScale = transform.localScale;
    }

    // 外部调用：设置本缩略图对应的卡数据
    public void SetCardThumbnail(CardData data)
    {
        cardData = data;

        // 设置卡牌品质
        UpdateCardQuilityView();

        // 设置卡牌图片
        if (cardData.artwork != null)
            artworkImage.sprite = cardData.artwork;

        // 设置文本
        cardNameText.text = cardData.cardName;
        costText.text = cardData.cost.ToString();
        descriptionText.text = cardData.description;
    }

    public void RemoveSymbol()
    {
        artworkImage.gameObject.SetActive(true);
        imageUI.gameObject.SetActive(true);
        cardNameText.gameObject.SetActive(true);
        CardNameUI.gameObject.SetActive(true);
        costText.gameObject.SetActive(true);
        CostUI.gameObject.SetActive(true);
        descriptionText.gameObject.SetActive(true);

        symbol.gameObject.SetActive(false);
    }

    public void SetSymbol(string symbolName)
    {
        symbol.gameObject.SetActive(true);
        symbol.text = symbolName;

        artworkImage.gameObject.SetActive(false);
        imageUI.gameObject.SetActive(false);
        cardNameText.gameObject.SetActive(false);
        CardNameUI.gameObject.SetActive(false);
        costText.gameObject.SetActive(false);
        CostUI.gameObject.SetActive(false);
        descriptionText.gameObject.SetActive(false);
        cardFrame.sprite = commonFrame;
    }
        

    private void UpdateCardQuilityView()
    {
        if (cardFrame == null) return;

        switch (cardData.quality)
        {
            case CardQuality.Common:
                cardFrame.sprite = commonFrame;
                cardNameText.color = commonColor;
                break;
            case CardQuality.Rare:
                cardFrame.sprite = rareFrame;
                cardNameText.color = rareColor;
                break;
            case CardQuality.Epic:
                cardFrame.sprite = epicFrame;
                cardNameText.color = epicColor;
                break;
            default:
                cardFrame.sprite = commonFrame;
                cardNameText.color = commonColor;
                break;
        }
    }

    // 提供一个拿到CardData的方法，以便点击时传出
    public CardData GetCardData()
    {
        return cardData;
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