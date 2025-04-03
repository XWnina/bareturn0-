using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

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
    private CardData cardData; public GameObject hoverDescriptionGroup; // HoverDecriptionImage
    public TextMeshProUGUI hoverDescriptionTMP; // HoverDescriptionTMP
    public bool enableHoverDescription = false;




    // Ԥ�費ͬƷ�ʵĿ��Ʊ߿�
    public Sprite commonFrame;
    public Sprite rareFrame;
    public Sprite epicFrame;

    // Ԥ�費ͬƷ�ʵĿ���������ɫ
    private Color commonColor = new Color(0.72f, 0.45f, 0.20f); // ͭɫ
    private Color rareColor = new Color(0.75f, 0.75f, 0.75f);   // ��ɫ
    private Color epicColor = new Color(1f, 0.84f, 0f);         // ��ɫ

    private Vector3 originalScale;   // ��¼��ʼ����


    void Awake()
    {
        originalScale = transform.localScale;
    }

    // �ⲿ���ã����ñ�����ͼ��Ӧ�Ŀ�����
    public void SetCardThumbnail(CardData data)
    {
        cardData = data;

        // ���ÿ���Ʒ��
        UpdateCardQuilityView();

        // ���ÿ���ͼƬ
        if (cardData.artwork != null)
            artworkImage.sprite = cardData.artwork;

        // �����ı�
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

    // �ṩһ���õ�CardData�ķ������Ա���ʱ����
    public CardData GetCardData()
    {
        return cardData;
    }

    // �����ͣ�ӿ�
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!allowHoverEffect) return;
        // �Ŵ�
        transform.localScale = originalScale * 1.2f;

        // Only show the hover description if it's enabled
        if (enableHoverDescription && hoverDescriptionGroup != null && hoverDescriptionTMP != null)
        {
            hoverDescriptionTMP.text = cardData.description;
            hoverDescriptionGroup.SetActive(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!allowHoverEffect) return;
        // �ָ�
        transform.localScale = originalScale;

        // Hide the hover description
        if (enableHoverDescription && hoverDescriptionGroup != null)
        {
            hoverDescriptionGroup.SetActive(false);
        }
    }
}