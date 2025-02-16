using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class CardView : MonoBehaviour, IPointerClickHandler, IPointerExitHandler, IPointerEnterHandler
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

    private CardData cardData;

    // 预设不同品质的卡牌边框
    public Sprite commonFrame;
    public Sprite rareFrame;
    public Sprite epicFrame;

    // 预设不同品质的卡牌名称颜色
    private Color commonColor = new Color(0.72f, 0.45f, 0.20f); // 铜色
    private Color rareColor = new Color(0.75f, 0.75f, 0.75f);   // 银色
    private Color epicColor = new Color(1f, 0.84f, 0f);         // 金色

    // 用于记录原始大小和位置
    private Vector3 originalScale;
    private Vector3 originalPosition;
    private RectTransform rectTransform;
    
    private Canvas canvas; // 用于计算拖拽偏移



    // 初始化/更新 UI
    public void SetCard(CardData data)
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

    // 初始化
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();

        originalScale = transform.localScale;
        originalPosition = transform.position;
        canvas = GetComponentInParent<Canvas>();

    }

    #region 鼠标悬停放大效果
    public void OnPointerEnter(PointerEventData eventData)
    {
        // 鼠标进入时放大卡牌
        transform.localScale = originalScale * 1.2f;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // 鼠标离开时恢复原始大小
        transform.localScale = originalScale;
    }
    #endregion



    public void OnPointerClick(PointerEventData eventData)
    {
        BattleManager.Instance.UseCard(cardData, this);
        Debug.Log("Card clicked: " + cardData.cardName);
    }

    public CardData GetCardData()
    {
        return cardData;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
