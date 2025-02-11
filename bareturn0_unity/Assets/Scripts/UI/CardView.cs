using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class CardView : MonoBehaviour, IPointerClickHandler
{
    [Header("UI Components")]
    public Image artworkImage;
    public TextMeshProUGUI cardNameText;
    public TextMeshProUGUI costText;
    public TextMeshProUGUI descriptionText;

    private CardData cardData;

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
