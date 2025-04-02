using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class InventoryManager : MonoBehaviour
{
    // Full setting
    public static InventoryManager Instance;
    public GameObject CardCollection;
    public GameObject CardPrefab;
    public List<CardData> playerCards = new List<CardData>();
    public Button MaterialsButton;
    public Button ExitButton;

    // Card Collection
    public PlayerInfoLoader playerInfoLoader;
    public GameObject scrollPrefab;
    public GameObject materialPanel;

    // Material Panel
    public GameObject materialBackground;
    public List<string> playerScroll = new List<string>();
    public TextMeshProUGUI BlankNumText;
    public Button CloseButton;

    public int blankcardNum;
    public int ifNum;
    public int whileNum;
    public int mathNum;


    void Start()
    {
        PlayerPrefs.SetString("PreviousScene", "draftMap");

        playerInfoLoader.LoadPlayerDeck("cardCollection", () =>
        {
            playerCards = playerInfoLoader.cardList;
            populateCollection();
        });

        materialPanel.SetActive(false);
        materialBackground.SetActive(false);

        ExitButton.onClick.AddListener(() =>
        {
            SceneManager.LoadScene(PlayerPrefs.GetString("PreviousScene"));
        });

        MaterialsButton.onClick.AddListener(materialsbuttonclicked);
        CloseButton.onClick.AddListener(() =>
        {
            materialPanel.SetActive(false);
            materialBackground.SetActive(false);
        });
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

    public void PopulateScrollView()
    {
        foreach (Transform child in materialPanel.transform)
        {
            if (child.gameObject.name != "CardThumbnial")
            {
                Destroy(child.gameObject);
            }
        }
        for (int i = 0; i < playerScroll.Count; i++)
        {
            string scrollName = playerScroll[i];

            GameObject scrollObject = Instantiate(scrollPrefab, materialPanel.transform);
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
    public void materialsbuttonclicked()
    {
        materialPanel.SetActive(true);
        materialBackground.SetActive(true);
        playerInfoLoader.GetAllMaterials(() =>
      {
          playerScroll.Clear();
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
                  playerScroll.Add(material);

              }
              else if (material.ToLower().Contains("if"))
              {
                  ifNum++;
                  playerScroll.Add(material);

              }
              else if (material.ToLower().Contains("while"))
              {
                  whileNum++;
                  playerScroll.Add(material);

              }

          }
          BlankNumText.text = blankcardNum.ToString();
          PopulateScrollView();
      });

    }
}
