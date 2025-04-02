using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class InventoryManager : MonoBehaviour
{

    public static InventoryManager Instance;
    public Button ExitButton;
    public GameObject CardCollection;
    public GameObject CardDeck;
    public Button MaterialsButton;
    public Button NewDeckButton;

    // Card Collection
    public PlayerInfoLoader playerInfoLoader;
    public List<CardData> playerCards = new List<CardData>();
    public GameObject CardPrefab;

    // Material Panel
    public GameObject MaterialBackground;
    public GameObject TalentPrefab;
    public List<string> talentList = new List<string>();
    public TextMeshProUGUI BlankNumText;
    public GameObject MaterialPanel;
    public Button MaterialCloseButton;
    public int blankcardNum;
    public int ifNum;
    public int whileNum;
    public int mathNum;

    // Card Deck
    public DeckInfoLoader deckInfoLoader;
    public GameObject DeckButtonPrefab;

    // Deck Panel
    public GameObject DeckPanel;
    public TextMeshProUGUI DeckPanelTitle;



    void Start()
    {
        PlayerPrefs.SetString("PreviousScene", "draftMap");

        playerInfoLoader.LoadPlayerDeck("cardCollection", () =>
        {
            playerCards = playerInfoLoader.cardList;
            populateCollection();
        });

        MaterialPanel.SetActive(false);
        MaterialBackground.SetActive(false);
        DeckPanel.SetActive(false);

        ExitButton.onClick.AddListener(() =>
        {
            SceneManager.LoadScene(PlayerPrefs.GetString("PreviousScene"));
        });

        MaterialsButton.onClick.AddListener(materialButtonClicked);
        MaterialCloseButton.onClick.AddListener(() =>
        {
            MaterialPanel.SetActive(false);
            MaterialBackground.SetActive(false);
        });
        DisplayAllDeckButtons();
    }

    public void populateCollection()
    {
        foreach (Transform child in CardCollection.transform)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < playerCards.Count; i++)
        {
            CardData cardData = playerCards[i];

            GameObject card = Instantiate(CardPrefab, CardCollection.transform);
            CardThumbnailUI cardThumbnail = card.GetComponent<CardThumbnailUI>();
            cardThumbnail.SetCardThumbnail(cardData);

        }
    }

    public void populateMaterialView()
    {
        foreach (Transform child in MaterialPanel.transform)
        {
            if (child.gameObject.name != "CardThumbnial")
            {
                Destroy(child.gameObject);
            }
        }
        for (int i = 0; i < talentList.Count; i++)
        {
            string scrollName = talentList[i];

            GameObject scrollObject = Instantiate(TalentPrefab, MaterialPanel.transform);
            TalentUI scrollUI = scrollObject.GetComponent<TalentUI>();
            if (scrollName == "if" && ifNum != 0)
            {
                scrollUI.setScroll(scrollName, ifNum);
            }
            else if (scrollName == "while" && whileNum != 0)
            {
                scrollUI.setScroll(scrollName, whileNum);
            }
            else if (scrollName == "math" && mathNum != 0)
            {
                scrollUI.setScroll(scrollName, mathNum);
            }

        }
    }
    public void materialButtonClicked()
    {
        MaterialPanel.SetActive(true);
        MaterialBackground.SetActive(true);
        playerInfoLoader.GetAllMaterials(() =>
      {
          talentList.Clear();
          blankcardNum = 0;

          foreach (string material in playerInfoLoader.materials)
          {
              if (material.ToLower().Contains("blank"))
              {
                  blankcardNum++;
              }
              else if (material.ToLower().Contains("math"))
              {
                  mathNum++;
                  talentList.Add(material);

              }
              else if (material.ToLower().Contains("if"))
              {
                  ifNum++;
                  talentList.Add(material);

              }
              else if (material.ToLower().Contains("while"))
              {
                  whileNum++;
                  talentList.Add(material);

              }

          }
          BlankNumText.text = blankcardNum.ToString();
          populateMaterialView();
      });

    }
    public void DisplayAllDeckButtons()
    {
        deckInfoLoader.LoadAllDecks((deckList) =>
        {
            foreach (Transform child in CardDeck.transform)
            {
                Destroy(child.gameObject);
            }

            foreach (var deck in deckList)
            {
                GameObject buttonObj = Instantiate(DeckButtonPrefab, CardDeck.transform);
                buttonObj.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = deck.name;

                string deckName = deck.name;
                buttonObj.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() =>
                {
                    Debug.Log("你点击了卡组：" + deckName);

                    DeckPanel.SetActive(true); // 显示面板
                    DeckPanelTitle.text = deckName; // 设置标题
                });
            }
        });
    }
}
