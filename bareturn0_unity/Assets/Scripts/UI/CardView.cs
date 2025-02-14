using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class CardView : MonoBehaviour, IPointerClickHandler, IPointerExitHandler, IPointerEnterHandler
{ 
    [Header("UI Components")]
    public Image artworkImage;
    public TextMeshProUGUI cardNameText;
    public TextMeshProUGUI costText;
    public TextMeshProUGUI descriptionText;

    private CardData cardData;

    // 用于记录原始大小和位置
    private Vector3 originalScale;
    private Vector3 originalPosition;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Canvas canvas; // 用于计算拖拽偏移



    // 初始化/更新 UI
    public void SetCard(CardData data)
    {
        cardData = data;
        if (cardData.artwork != null)
            artworkImage.sprite = cardData.artwork;

        cardNameText.text = cardData.cardName;
        costText.text = cardData.cost.ToString();
        descriptionText.text = cardData.description;
    }

    // 初始化
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
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
