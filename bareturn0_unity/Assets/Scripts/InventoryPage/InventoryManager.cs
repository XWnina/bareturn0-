using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Threading;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    public GameObject CardCollection;
    public GameObject CardPrefab;
    public List<CardData> playerCards = new List<CardData>();

    public PlayerInfoLoader playerInfoLoader;
    public GameObject scrollPrefab;
    public GameObject materialPanel;
    public List<string> playerScroll = new List<string>();
    public TextMeshProUGUI BlankNumText;



    public Button MaterialsButton;

    public int blankcardNum;
    public int ifNum;
    public int whileNum;
    public int mathNum;


    void Start()
    {
        playerInfoLoader.LoadPlayerDeck("cardCollection", () =>
        {
            playerCards = playerInfoLoader.cardList;
            populateCollection();
        });

        materialPanel.SetActive(false);
        MaterialsButton.onClick.AddListener(materialsbuttonclicked);
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
            // Destroy(child.gameObject);
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
