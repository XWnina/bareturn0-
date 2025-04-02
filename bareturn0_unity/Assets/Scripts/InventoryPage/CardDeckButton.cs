using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeckButtonUI : MonoBehaviour
{
    public TextMeshProUGUI deckNameText;
    private string deckId;

    public void Initialize(string name, string id)
    {
        deckNameText.text = name;
        deckId = id;
    }

    public void OnClick()
    {
        Debug.Log("Deck clicked: " + deckNameText.text + " (ID: " + deckId + ")");
        // You can load deck details here, like showing cards in the deck
    }
}
