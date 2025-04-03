using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using System.Collections;
public class EnhancementManager : MonoBehaviour
{
    public static EnhancementManager Instance;
    [Header("References to UI Elements")]
    public List<CardData> playerCards = new List<CardData>();
    public int coins;
    public TextMeshProUGUI coinsText;

    //[Header("Test Cards")]
    //public List<CardData> testPlayerCards;

    public Transform scrollViewContent;
    public GameObject cardThumbnailPrefab;

    [Header("UI Placeholders")]
    public CardThumbnailUI selectedCardPlaceholder;
    public CardThumbnailUI upgradePlaceholder1;
    public CardThumbnailUI upgradePlaceholder2;
    public TextMeshProUGUI warningText;
    public TextMeshProUGUI costText;

    private CardData currentSelectedCard;
    private CardData upgradeCard1;
    private CardData upgradeCard2;

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
            playerCards = playerInfoLoader.cardList;
            // 2. 生成 UI 列表
            PopulateScrollView();
        });

        UpdatePlayerCoin();

        costText.text = "?";
        selectedCardPlaceholder.SetSymbol("?");
        upgradePlaceholder1.SetSymbol("?");
        upgradePlaceholder2.SetSymbol("?");
        selectedCardPlaceholder.allowHoverEffect = false;
    }

    public void UpdatePlayerCoin()
    {
        playerInfoLoader.LoadPlayerCoins(() =>
        {
            coins = playerInfoLoader.coins;
            coinsText.text = coins.ToString();
        });
    }


    // 生成卡牌列表
    private void PopulateScrollView()
    {
        // 清空原有子物体
        foreach (Transform child in scrollViewContent)
        {
            Destroy(child.gameObject);
        }

        // 遍历卡牌
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
        int cost = FindCost(currentSelectedCard);
        costText.text = cost.ToString();

        // 刷新下方升级选项
        RefreshUpgradeOptions();
    }

    private void RefreshUpgradeOptions()
    {
        // 检查当前选中的卡牌是否存在
        if (currentSelectedCard == null)
        {
            upgradePlaceholder1.SetSymbol("?");
            upgradePlaceholder2.SetSymbol("?");
            return;
        }

        // 假设在 CardData 中，你使用 upgradeOptions 数组来储存两个升级选项
        // 当 upgradeOptions 数组不为空且长度至少为 2 时
        if (currentSelectedCard.upgradeOptions != null && currentSelectedCard.upgradeOptions.Length == 2)
        {
            // 对于第一个升级选项
            if (currentSelectedCard.upgradeOptions[0] != null)
            {
                upgradePlaceholder1.RemoveSymbol();
                upgradeCard1 = currentSelectedCard.upgradeOptions[0];
                upgradePlaceholder1.SetCardThumbnail(currentSelectedCard.upgradeOptions[0]);
                upgradePlaceholder1.button.onClick.AddListener(OnFirstUpgradeSelected);
            }
            else
            {
                // 如果第一个升级选项不存在，则显示默认符号
                upgradePlaceholder1.SetSymbol("\\");
            }

            // 对于第二个升级选项
            if (currentSelectedCard.upgradeOptions[1] != null)
            {
                upgradePlaceholder2.RemoveSymbol();
                upgradeCard2 = currentSelectedCard.upgradeOptions[1];
                upgradePlaceholder2.SetCardThumbnail(currentSelectedCard.upgradeOptions[1]);
                upgradePlaceholder2.button.onClick.AddListener(OnSecondUpgradeSelected);
            }
            else
            {
                // 如果第二个升级选项不存在，则显示默认符号
                upgradePlaceholder2.SetSymbol("\\");
                upgradeCard1 = null;
                upgradeCard2 = null;
            }
        }
        else
        {
            // 如果数组为空或长度不足 2，则两个都显示默认符号
            upgradePlaceholder1.SetSymbol("\\");
            upgradePlaceholder2.SetSymbol("\\");

        }
    }

    public void OnFirstUpgradeSelected()
    {
        if (currentSelectedCard == null || upgradeCard1 == null)
        {
            Debug.Log("左侧升级选项不存在。");
            return;
        }

        // 升级逻辑：将当前卡牌替换为升级后的卡牌 upgradeCard1
        CardData upgradeCard = upgradeCard1;
        CardData removedCard = currentSelectedCard;


        // 0. 计算需要扣除的金币
        int requiredCoins = FindCost(currentSelectedCard);
        if (coins < requiredCoins)
        {
            DisplayWarning("No Enough Coins");
            return;
        }
        Debug.Log("当前卡牌" + currentSelectedCard.cardName);
        // 清理升级选项显示及相关变量
        selectedCardPlaceholder.SetSymbol("?");
        upgradePlaceholder1.SetSymbol("?");
        upgradePlaceholder2.SetSymbol("?");
        costText.text = "?";
        upgradeCard1 = null;
        upgradeCard2 = null;
        Debug.Log("当前卡牌" + currentSelectedCard.cardName);


        // 1. 从本地卡牌集合中移除当前卡牌
        if (playerCards.Contains(currentSelectedCard))
        {
            playerCards.Remove(currentSelectedCard);
        }
        Debug.Log("当前卡牌" + currentSelectedCard.cardName);

        // 2. 调用后端接口移除旧卡
        playerInfoLoader.RemoveCardFromCollection(currentSelectedCard.cardName, 1, () =>
        {
            Debug.Log("后端成功移除旧卡");

            // 3. 将升级后的卡牌加入本地集合
            playerCards.Add(upgradeCard);

            // 4. 调用后端接口添加升级后的卡牌
            playerInfoLoader.AddCardToCollection(upgradeCard.cardName, 1, () =>
            {
                Debug.Log("后端成功添加升级卡：" + upgradeCard.cardName);
                // 从后端移除玩家金币
                // 本地扣除金币并更新显示
                coins = Mathf.Max(coins - requiredCoins, 0);
                coinsText.text = coins.ToString();
                playerInfoLoader.UpdatePlayerCoin(coins, () =>
                {
                    Debug.Log("后端金币更新成功！");
                });
                // 5. 刷新 ScrollView 显示
                PopulateScrollView();

                playerInfoLoader.RefeshAllDecks(removedCard, upgradeCard);
                Debug.Log("左侧升级选择成功");
            });
        });


        currentSelectedCard = null;
    }

    public void OnSecondUpgradeSelected()
    {
        if (currentSelectedCard == null || upgradeCard2 == null)
        {
            Debug.Log("右侧升级选项不存在。");
            return;
        }

        // 升级逻辑：将当前卡牌替换为升级后的卡牌 upgradeCard2
        CardData upgradeCard = upgradeCard2;
        CardData removedCard = currentSelectedCard;


        // 0. 计算需要扣除的金币
        int requiredCoins = FindCost(currentSelectedCard);
        if (coins < requiredCoins)
        {
            DisplayWarning("No Enough Coins");
            return;
        }
        Debug.Log("当前卡牌" + currentSelectedCard.cardName);
        // 清理升级选项显示及相关变量
        selectedCardPlaceholder.SetSymbol("?");
        upgradePlaceholder1.SetSymbol("?");
        upgradePlaceholder2.SetSymbol("?");
        costText.text = "?";
        upgradeCard1 = null;
        upgradeCard2 = null;
        Debug.Log("当前卡牌" + currentSelectedCard.cardName);


        // 1. 从本地卡牌集合中移除当前卡牌
        if (playerCards.Contains(currentSelectedCard))
        {
            playerCards.Remove(currentSelectedCard);
        }
        Debug.Log("当前卡牌" + currentSelectedCard.cardName);

        // 2. 调用后端接口移除旧卡
        playerInfoLoader.RemoveCardFromCollection(currentSelectedCard.cardName, 1, () =>
        {
            Debug.Log("后端成功移除旧卡");

            // 3. 将升级后的卡牌加入本地集合
            playerCards.Add(upgradeCard);

            // 4. 调用后端接口添加升级后的卡牌
            playerInfoLoader.AddCardToCollection(upgradeCard.cardName, 1, () =>
            {
                Debug.Log("后端成功添加升级卡：" + upgradeCard.cardName);
                // 从后端移除玩家金币
                // 本地扣除金币并更新显示
                coins = Mathf.Max(coins - requiredCoins, 0);
                coinsText.text = coins.ToString();
                playerInfoLoader.UpdatePlayerCoin(coins, () =>
                {
                    Debug.Log("后端金币更新成功！");
                });
                // 5. 刷新 ScrollView 显示
                PopulateScrollView();

                playerInfoLoader.RefeshAllDecks(removedCard, upgradeCard);
                Debug.Log("左侧升级选择成功");
            });
        });
        currentSelectedCard = null;
    }


    private void DisplayWarning(string message)
    {
        warningText.text = message;
        warningText.gameObject.SetActive(true);

        // 启动淡出协程
        StartCoroutine(FadeOutWarning());
    }

    private IEnumerator FadeOutWarning()
    {
        warningText.alpha = 1; // 立即变为可见

        yield return new WaitForSeconds(0.5f); // 停留 0.5 秒

        // 渐渐淡出
        float fadeDuration = 0.5f;
        float elapsedTime = 0;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            warningText.alpha = Mathf.Lerp(1, 0, elapsedTime / fadeDuration);
            yield return null;
        }

        warningText.gameObject.SetActive(false); // 完全消失后隐藏
    }

    public int FindCost(CardData currentCard)
    {
        int cost = 0;
        switch (currentCard.quality)
        {
            case CardQuality.Common:
                cost = 50;
                break;
            case CardQuality.Rare:
                cost = 100;
                break;
            case CardQuality.Epic:
                cost = 200;
                break;
            default:
                cost = 0;
                break;
        }
        return cost;
    }
}
