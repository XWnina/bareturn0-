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
    public Button creeateButton;
    public TextMeshProUGUI blankCardNumText;

    public Transform scrollViewContent;
    public GameObject scrollPrefab;

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
        removeScrollButton.onClick.AddListener(OnRemoveScrollClicked);
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
}
