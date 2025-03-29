using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
public class EnhancementManager : MonoBehaviour
{
    public static EnhancementManager Instance;
    [Header("References to UI Elements")]
    public List<CardData> playerCards = new List<CardData>(); // 这里存玩家所有卡

    //[Header("Test Cards")]
    //public List<CardData> testPlayerCards;

    public Transform scrollViewContent; // Inspector里把CardListScrollView/Viewport/Content拖进来
    public GameObject cardThumbnailPrefab; // Inspector里拖进“卡牌缩略图预制体”

    [Header("UI Placeholders")]
    public CardThumbnailUI selectedCardPlaceholder;
    public CardThumbnailUI upgradeLeftPlaceholder;
    public CardThumbnailUI upgradeRightPlaceholder;

    // 假设这里存了“当前选中的卡牌”，和它的两个升级卡
    private CardData currentSelectedCard;
    private CardData leftUpgradeCard;
    private CardData rightUpgradeCard;

    public PlayerInfoLoader playerInfoLoader;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetEnhancement()
    {
        // 1. 获取玩家卡牌列表
        playerInfoLoader.LoadPlayerDeck("cardCollection", () =>
        {
            playerInfoLoader.InitalenhanceDeck();
            // 2. 生成 UI 列表
            PopulateScrollView();
        });

        


        selectedCardPlaceholder.SetSymbol("?");
        upgradeLeftPlaceholder.SetSymbol("?");
        upgradeRightPlaceholder.SetSymbol("?");
        selectedCardPlaceholder.allowHoverEffect = false;
    }


    // 生成卡牌列表
    private void PopulateScrollView()
    {
        // 清空原有子物体
        foreach (Transform child in scrollViewContent)
        {
            Destroy(child.gameObject);
        }

        // 遍历添加的测试卡牌
        for (int i = 0; i < playerCards.Count; i++)
        {
            CardData cardData = playerCards[i];

            // 实例化缩略图Prefab
            GameObject thumbObj = Instantiate(cardThumbnailPrefab, scrollViewContent);
            CardThumbnailUI thumbUI = thumbObj.GetComponent<CardThumbnailUI>();

            // 设置缩略图
            thumbUI.SetCardThumbnail(cardData);

            // 给缩略图加一个点击事件
            Button btn = thumbObj.GetComponentInChildren<Button>();
            Debug.Log(btn ? "Button found, adding listener" : "Button NOT found!");
            if (btn != null)
            {
                int index = i;
                btn.onClick.AddListener(() =>
                {
                    Debug.Log("Button clicked, cardIndex=" + index);
                    OnThumbnailClicked(index);
                });
            }
        }
    }

    private void OnThumbnailClicked(int cardIndex)
    {
        Debug.Log("选中卡牌");
        currentSelectedCard = playerCards[cardIndex];

        // 显示选中的卡牌
        selectedCardPlaceholder.RemoveSymbol();
        selectedCardPlaceholder.SetCardThumbnail(currentSelectedCard);

        // 刷新下方升级选项
        //RefreshUpgradeOptions();
    }

}
