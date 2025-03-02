using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class CardView : MonoBehaviour, IPointerClickHandler, IPointerExitHandler, IPointerEnterHandler,IBeginDragHandler,IDragHandler,IEndDragHandler
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
    // 记录卡牌的原始位置，用于拖拽失败时返回
    private Vector3 originalPosition;
    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;
    private Canvas canvas; // 用于计算拖拽偏移（如果是UI Canvas）



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
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        originalScale = transform.localScale;
        //originalPosition = transform.position;

        // 找到父级Canvas（如果是嵌套Canvas，要找最上层用于正确计算拖拽偏移）
        canvas = GetComponentInParent<Canvas>();

    }

    #region 拖拽接口实现
    public void OnBeginDrag(PointerEventData eventData)
    {
        originalPosition = rectTransform.anchoredPosition;
        // 让卡牌在拖拽时半透明并允许射线穿透
        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;

        // 开始拖拽时，设置标记
        if (cardData.cardEffect.RequiresTarget())
        {
            BattleManager.Instance.isCardBeingDragged = true;
        }
        
    }

    public void OnDrag(PointerEventData eventData)
    {
        // 在UI Canvas中拖拽，需要考虑Canvas的缩放
        if (canvas != null)
        {
            rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
        }
        else
        {
            // 如果没有Canvas，可直接用世界坐标
            rectTransform.position = Input.mousePosition;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // 恢复透明度和射线检测
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;

        // 拖拽结束后，清除标记（不论是否成功使用卡牌）
        BattleManager.Instance.isCardBeingDragged = false;

        // 判断pointerEnter（鼠标指针结束时所指向的对象）是否为目标敌人
        if (eventData.pointerEnter != null)
        {
            EnemyController enemy = eventData.pointerEnter.GetComponent<EnemyController>();
            if (enemy != null)
            {
                //使用卡牌，如果使用卡牌失败，则恢复原位
                bool success = BattleManager.Instance.UseCard(cardData, this, enemy);
                if (!success)
                {
                    rectTransform.anchoredPosition = originalPosition;
                }
                return; // 成功使用后，不需要恢复卡牌位置
            }
        }

        // 如果拖拽到的不是敌人，则返回原位置
        rectTransform.anchoredPosition = originalPosition;
    }
    #endregion

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
        if (cardData.cardEffect.RequiresTarget())
        {
            return;
        }
        BattleManager.Instance.UseCard(cardData, this, null);
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
