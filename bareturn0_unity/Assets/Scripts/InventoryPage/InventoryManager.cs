using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.Networking;
using UnityEditor.Rendering;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;
    public Button ExitButton;
    public GameObject CardCollection;
    public GameObject CardDeck;
    public Button MaterialsButton;
    public Button CardCollectionButton;
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
    public Button DeckPanelCloseButton;
    public DeckPanelManager deckPanelManager;

    // New Deck Panel
    public GameObject NewDeckPanel;
    public TMP_InputField DeckNameInput;
    public Button ConfirmCreateDeckButton;
    public Button CancelCreateDeckButton;

    // Card Collection Panel
    public GameObject CardCollectionPanel;
    public Button CardCollectionCloseButton;
    public CardDatabase cardDatabase; // 拖入 AACardDatabase.asset
    public Transform CardCollectionGrid; // ScrollView下的内容区域

    public GameObject CardDetailsPanel;



    void Awake()
    {
        Instance = this;
    }

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

        DeckPanelCloseButton.onClick.AddListener(() =>
        {
            DeckPanel.SetActive(false);
            DeckPanelTitle.text = "";
        });

        DisplayAllDeckButtons();

        NewDeckPanel.SetActive(false);

        NewDeckButton.onClick.AddListener(() =>
        {
            NewDeckPanel.SetActive(true);
            DeckNameInput.text = "";
        });

        CancelCreateDeckButton.onClick.AddListener(() =>
        {
            NewDeckPanel.SetActive(false);
        });

        ConfirmCreateDeckButton.onClick.AddListener(() =>
        {
            string deckName = DeckNameInput.text.Trim();
            if (!string.IsNullOrEmpty(deckName))
            {
                StartCoroutine(CreateNewDeck(deckName));
            }
        });


        CardCollectionPanel.SetActive(false);
        CardCollectionButton.onClick.AddListener(() =>
        {
            CardCollectionPanel.SetActive(true);
            populateCardCollectionPanel();
        });

        CardCollectionCloseButton.onClick.AddListener(() =>
        {
            CardCollectionPanel.SetActive(false);
        });
        CardDetailsPanel.SetActive(false);
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
            // Enable hover description
            cardThumbnail.enableHoverDescription = true;
            cardThumbnail.hoverDescriptionGroup = card.transform.Find("HoverDecriptionImage").gameObject;
            cardThumbnail.hoverDescriptionTMP = card.transform.Find("HoverDecriptionImage/HoverDescriptionTMP").GetComponent<TextMeshProUGUI>();

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

        List<string> materialList = new List<string>();
        for (int i = 0; i < talentList.Count; i++)
        {
            if (!materialList.Contains(talentList[i]))
            {
                materialList.Add(talentList[i]);
            }
        }

        for (int i = 0; i < materialList.Count; i++)
        {
            string scrollName = materialList[i];
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
        Debug.Log("Material Button Clicked");
        MaterialPanel.SetActive(true);
        MaterialBackground.SetActive(true);

        playerInfoLoader.GetAllMaterials(() =>
        {
            talentList.Clear();
            blankcardNum = 0;
            ifNum = 0;
            whileNum = 0;
            mathNum = 0;

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
                buttonObj.GetComponentInChildren<TextMeshProUGUI>().text = deck.name;

                string deckName = deck.name;
                string deckId = deck._id;
                string saveFileId = deckInfoLoader.currentSaveFileId;

                buttonObj.GetComponent<Button>().onClick.AddListener(() =>
                {
                    Debug.Log("Deck Button Clicked: " + deckName);
                    DeckPanel.SetActive(true);
                    DeckPanelTitle.text = deckName;
                    deckPanelManager.LoadDeckEditor(deckId, saveFileId, deckName);
                });
            }
        });
    }
    IEnumerator CreateNewDeck(string deckName)
    {
        string token = PlayerPrefs.GetString("token");
        string saveFileId = deckInfoLoader.currentSaveFileId;
        string url = $"http://localhost:3000/carddecks/create";

        // JSON payload
        CreateDeckRequest payload = new CreateDeckRequest
        {
            name = deckName,
            saveFileId = saveFileId
        };

        string jsonBody = JsonUtility.ToJson(payload);

        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonBody);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();

        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + token);

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Failed to create deck: " + request.error);
            Debug.LogError("Response: " + request.downloadHandler.text);
        }
        else
        {
            NewDeckPanel.SetActive(false);
            DisplayAllDeckButtons(); // Refresh deck list
        }
    }

    public void populateCardCollectionPanel()
    {

        // Clear existing card thumbnails
        foreach (Transform child in CardCollectionGrid)
        {
            Destroy(child.gameObject);
        }

        // Count the number of each card in playerCards
        Dictionary<string, int> cardCounts = new Dictionary<string, int>();
        foreach (var card in playerCards)
        {
            if (!cardCounts.ContainsKey(card.cardName))
                cardCounts[card.cardName] = 1;
            else
                cardCounts[card.cardName]++;
        }

        // Populate the card collection panel
        foreach (var cardData in cardDatabase.allCards)
        {
            try
            {
                GameObject cardObj = Instantiate(CardPrefab, CardCollectionGrid);
                CardThumbnailUI ui = cardObj.GetComponent<CardThumbnailUI>();

                bool isOwned = cardCounts.ContainsKey(cardData.cardName);
                ui.SetCardThumbnail(cardData, isOwned);

                if (isOwned)
                {
                    ui.SetCardCount(cardCounts[cardData.cardName]);

                    Button btn = cardObj.GetComponentInChildren<Button>();
                    if (btn != null)
                    {
                        btn.onClick.AddListener(() =>
                        {
                            CardDetailsPanel.SetActive(true);
                        });
                    }
                   

                }
            }
            catch (System.Exception e)
            {
                Debug.LogError("InventoryManager: Card Building Failed" + cardData.cardName + "\n" + e.Message);
            }
        }

    }


    [System.Serializable]
    public class CreateDeckRequest
    {
        public string name;
        public string saveFileId;
    }


}
