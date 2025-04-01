using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CreateCardManage : MonoBehaviour
{
    [Header("References to UI Elements")]
    public List<string> playerScroll = new List<string>();
    public int blankcardNum;
    public int coins;
    public TextMeshProUGUI coinsText;
    public Button removeScrollButton;
    public Button createButton;
    public TextMeshProUGUI blankCardNumText;

    public Transform scrollViewContent;
    public GameObject scrollPrefab;
    public GameObject resultPanel;
    public Button confirmButton;

    [Header("Creation Card List")]
    public List<CardData> normalCards = new List<CardData>();
    public List<CardData> ifCards = new List<CardData>();
    public List<CardData> whileCards = new List<CardData>();
    public List<CardData> mathCards = new List<CardData>();

    [Header("Warning UI")]
    public TextMeshProUGUI warningText;



    [Header("UI Placeholders")]
    public ScrollUI selectedScrollPlaceholder;
    public CardThumbnailUI blankCardPlaceholder;
    public CardThumbnailUI createdCardPlaceholder;

    private CardData createdCard;

    public PlayerInfoLoader playerInfoLoader;

    private string currentSelectedScrollName;

    public void SetCreation()
    {
        createdCardPlaceholder.SetSymbol("?");
        selectedScrollPlaceholder.gameObject.SetActive(false);
        PopulateScrollView();
        coinsText.text = coins.ToString();
        blankCardNumText.text = "(" + blankcardNum.ToString() + "/1)";

        selectedScrollPlaceholder.allowHoverEffect = false;
        blankCardPlaceholder.allowHoverEffect = false;
        createdCardPlaceholder.allowHoverEffect = false;
        removeScrollButton.gameObject.SetActive(false);
        resultPanel.SetActive(false);
        warningText.gameObject.SetActive(false);

        removeScrollButton.onClick.AddListener(OnRemoveScrollClicked);
        createButton.onClick.AddListener(OnCreateButtonClicked);
        confirmButton.onClick.AddListener(OnConfirmButtonClicked);
    }

    public void PopulateScrollView()
    {
        // 清空原有子物体
        foreach (Transform child in scrollViewContent)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < playerScroll.Count; i++)
        {
            string scrollName = playerScroll[i];

            GameObject scrollObject = Instantiate(scrollPrefab, scrollViewContent);
            ScrollUI scrollUI = scrollObject.GetComponent<ScrollUI>();

            scrollUI.setScroll(scrollName);

            Button btn = scrollObject.GetComponentInChildren<Button>();
            if (btn != null)
            {
                int index = i;
                btn.onClick.AddListener(() =>
                {
                    Debug.Log("Button clicked, ScrollIndex=" + index);
                    OnScrollClicked(index);
                });
            }
        }
    }

    public void OnScrollClicked(int index)
    {
        currentSelectedScrollName = playerScroll[index];
        selectedScrollPlaceholder.gameObject.SetActive(true);
        removeScrollButton.gameObject .SetActive(true);
        selectedScrollPlaceholder.setScroll(currentSelectedScrollName);
    }

    public void OnRemoveScrollClicked()
    {
        currentSelectedScrollName = null;
        selectedScrollPlaceholder.gameObject.SetActive(false);
        removeScrollButton.gameObject.SetActive(false);
    }

    public void OnCreateButtonClicked()
    {
        // 检查空白卡牌是否足够
        if (blankcardNum <= 0)
        {
            DisplayWarning("Not Enough Blank Card");
            Debug.Log("没有空白卡牌，无法制作！");
            return;
        }

        CardData newCard = null;

        // 根据 currentSelectedScrollName 判断随机卡牌来源
        if (string.IsNullOrEmpty(currentSelectedScrollName))
        {
            // 没有选择特定 scroll，随机获得 normalCards 中的一张
            if (normalCards.Count > 0)
            {
                int randomIndex = Random.Range(0, normalCards.Count);
                newCard = normalCards[randomIndex];
            }
            else
            {
                Debug.LogError("normalCards 列表为空！");
            }
        }
        else
        {
            // 根据 currentSelectedScrollName 的值来判断
            string scrollKey = currentSelectedScrollName.ToLower();
            if (scrollKey == "if")
            {
                if (ifCards.Count > 0)
                {
                    int randomIndex = Random.Range(0, ifCards.Count);
                    newCard = ifCards[randomIndex];
                }
                else
                {
                    Debug.LogError("ifCards 列表为空！");
                }
            }
            else if (scrollKey == "while")
            {
                if (whileCards.Count > 0)
                {
                    int randomIndex = Random.Range(0, whileCards.Count);
                    newCard = whileCards[randomIndex];
                }
                else
                {
                    Debug.LogError("whileCards 列表为空！");
                }
            }
            else if (scrollKey == "math")
            {
                if (mathCards.Count > 0)
                {
                    int randomIndex = Random.Range(0, mathCards.Count);
                    newCard = mathCards[randomIndex];
                }
                else
                {
                    Debug.LogError("mathCards 列表为空！");
                }
            }
            else
            {
                // 如果 currentSelectedScrollName 的值不匹配预设类型，则默认从 normalCards 获取
                if (normalCards.Count > 0)
                {
                    int randomIndex = Random.Range(0, normalCards.Count);
                    newCard = normalCards[randomIndex];
                }
                else
                {
                    Debug.LogError("normalCards 列表为空！");
                }
            }
        }

        if (newCard != null)
        {
            // 制作成功，保存新卡牌
            createdCard = newCard;
            resultPanel.SetActive(true);
            createdCardPlaceholder.RemoveSymbol();
            createdCardPlaceholder.SetCardThumbnail(createdCard);
            
            createdCardPlaceholder.SetCardThumbnail(createdCard);
            Debug.Log("成功制作卡牌：" + createdCard.cardName);

            // 扣除一张空白卡牌，并更新显示
            blankcardNum--;
            blankCardNumText.text = "(" + blankcardNum.ToString() + "/1)";

            // 如果使用了 scroll（currentSelectedScrollName 不为空），则移除该 scroll
            if (!string.IsNullOrEmpty(currentSelectedScrollName))
            {
                // 从玩家拥有的 scroll 列表中移除（这里默认移除第一个匹配项）
                if (playerScroll.Contains(currentSelectedScrollName))
                {
                    playerScroll.Remove(currentSelectedScrollName);
                }
                // 重置当前选中的 scroll，并隐藏相关 UI
                currentSelectedScrollName = null;
                selectedScrollPlaceholder.gameObject.SetActive(false);
                removeScrollButton.gameObject.SetActive(false);
                // 刷新滚动列表（使移除的 scroll 不再显示）
                PopulateScrollView();
            }
        }
        else
        {
            Debug.LogError("制作卡牌失败！");
        }
        
        //初始化
        createdCard = null;
        currentSelectedScrollName = null;
        selectedScrollPlaceholder.gameObject.SetActive(false);
        removeScrollButton.gameObject.SetActive(false);
    }

    public void OnConfirmButtonClicked()
    {
        resultPanel.SetActive(false);
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
}
