using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    public GameObject CardCollection;
    public GameObject CardPrefab;
    public List<CardData> playerCards = new List<CardData>();

    public PlayerInfoLoader playerInfoLoader;

    void Start()
    {
        playerInfoLoader.LoadPlayerDeck("cardCollection", () =>
        {
            playerCards = playerInfoLoader.cardList;
            populateCollection();
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
}
