using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

public class DeckPanelManager : MonoBehaviour
{
    public GameObject collectionParent;
    public GameObject deckParent;
    public GameObject cardButtonPrefab;

    public string deckId;
    public string saveFileId;
    public string deckName;

    public Button SelectDeckButton;
    public Button DeleteDeckButton;

    public TextMeshProUGUI hintText;

    private bool isBusy = false;
    void Start()
    {
        if (SelectDeckButton != null)
            SelectDeckButton.onClick.AddListener(OnSelectDeckClicked);

        if (DeleteDeckButton != null)
            DeleteDeckButton.onClick.AddListener(OnDeleteDeckClicked);
        if (hintText != null)
        {
            hintText.gameObject.SetActive(false);
        }
    }
    public void LoadDeckEditor(string deckId, string saveFileId, string deckName)
    {
        this.deckId = deckId;
        this.saveFileId = saveFileId;
        this.deckName = deckName;

        StartCoroutine(LoadCollection());
        StartCoroutine(LoadDeck());
    }

    void ClearChildren(GameObject parent)
    {
        foreach (Transform child in parent.transform)
        {
            Destroy(child.gameObject);
        }
    }

    void SpawnCard(GameObject parent, string name, System.Action<string> onClick)
    {
        GameObject go = Instantiate(cardButtonPrefab, parent.transform);
        go.GetComponent<DeckCardButtonUI>().Setup(name, onClick);
    }

    IEnumerator LoadCollection()
    {
        string token = PlayerPrefs.GetString("token");
        string saveName = PlayerPrefs.GetString("currentSaveName", "");
        string url = $"http://localhost:3000/savefiles/{saveName}/cardCollection";

        UnityWebRequest request = UnityWebRequest.Get(url);
        request.SetRequestHeader("Authorization", "Bearer " + token);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            ShowHint("Failed to load collection: " + request.error);
            yield break;
        }

        ClearChildren(collectionParent);

        string json = request.downloadHandler.text;
        CardCollectionResponse response = JsonUtility.FromJson<CardCollectionResponse>(json);

        foreach (Card c in response.cardCollection.cards)
        {
            for (int i = 0; i < c.count; i++)
            {
                SpawnCard(collectionParent, c.cardName, OnCollectionCardClicked);
            }
        }
    }

    IEnumerator LoadDeck()
    {
        string token = PlayerPrefs.GetString("token");
        string url = $"http://localhost:3000/carddecks/{deckId}";

        UnityWebRequest request = UnityWebRequest.Get(url);
        request.SetRequestHeader("Authorization", "Bearer " + token);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            ShowHint("Failed to load deck: " + request.error);
            yield break;
        }

        ClearChildren(deckParent);

        string json = request.downloadHandler.text;
        DeckResponse response = JsonUtility.FromJson<DeckResponse>(json);

        foreach (Card c in response.deck.cards)
        {
            for (int i = 0; i < c.count; i++)
            {
                SpawnCard(deckParent, c.cardName, OnDeckCardClicked);
            }
        }
    }

    void OnCollectionCardClicked(string cardName)
    {
        if (!isBusy)
            StartCoroutine(AddCardToDeck(cardName));
    }

    void OnDeckCardClicked(string cardName)
    {
        if (!isBusy)
            StartCoroutine(RemoveCardFromDeck(cardName));
    }

    IEnumerator AddCardToDeck(string cardName)
    {
        isBusy = true;

        string token = PlayerPrefs.GetString("token");
        string url = $"http://localhost:3000/carddecks/{deckId}/addCard";

        CardRequestData requestData = new CardRequestData { cardName = cardName, count = 1 };
        string jsonBody = JsonUtility.ToJson(requestData);

        UnityWebRequest request = new UnityWebRequest(url, "POST");
        request.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(jsonBody));
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + token);

        yield return request.SendWebRequest();

        string responseText = request.downloadHandler.text;
        ShowHint(ExtractMessage(responseText, request.result));

        if (!responseText.Contains("error"))
        {
            StartCoroutine(LoadCollection());
            StartCoroutine(LoadDeck());
        }

        isBusy = false;
    }

    IEnumerator RemoveCardFromDeck(string cardName)
    {
        isBusy = true;

        string token = PlayerPrefs.GetString("token");
        string url = $"http://localhost:3000/carddecks/removeCardByName/{saveFileId}/{deckName}";

        CardRequestData requestData = new CardRequestData { cardName = cardName, count = 1 };
        string jsonBody = JsonUtility.ToJson(requestData);

        UnityWebRequest request = new UnityWebRequest(url, "POST");
        request.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(jsonBody));
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + token);

        yield return request.SendWebRequest();

        string responseText = request.downloadHandler.text;
        ShowHint(ExtractMessage(responseText, request.result));

        if (!responseText.Contains("error"))
        {
            StartCoroutine(LoadCollection());
            StartCoroutine(LoadDeck());
        }

        isBusy = false;
    }

    void ShowHint(string message)
    {
        if (hintText == null) return;

        hintText.text = message;
        hintText.gameObject.SetActive(true);
        StopAllCoroutines();
        StartCoroutine(HideHintAfterDelay(2f));
    }

    IEnumerator HideHintAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (hintText != null)
        {
            hintText.gameObject.SetActive(false);
        }
    }

    string ExtractMessage(string json, UnityWebRequest.Result result)
    {
        if (string.IsNullOrEmpty(json)) return result == UnityWebRequest.Result.Success ? "Success" : "Failed";

        if (json.Contains("error"))
        {
            try { return JsonUtility.FromJson<ErrorResponse>(json).error; }
            catch { return "Error"; }
        }

        if (json.Contains("message"))
        {
            try { return JsonUtility.FromJson<SuccessResponse>(json).message; }
            catch { return "Success"; }
        }

        return result == UnityWebRequest.Result.Success ? "Success" : "Failed";
    }
    void OnSelectDeckClicked()
    {
        StartCoroutine(SetDeckAsSelected());
    }

    void OnDeleteDeckClicked()
    {
        StartCoroutine(DeleteCurrentDeck());
    }

    IEnumerator SetDeckAsSelected()
{
    string token = PlayerPrefs.GetString("token");
    string saveName = PlayerPrefs.GetString("currentSaveName", "");
    string url = $"http://localhost:3000/selectedDeckAndCardCollection/{saveName}/setSelectedDeck";

    // Debug.Log("[SetDeckAsSelected] token: " + token);
    // Debug.Log("[SetDeckAsSelected] saveName: " + saveName);
    // Debug.Log("[SetDeckAsSelected] deckId: " + this.deckId);
    // Debug.Log("[SetDeckAsSelected] URL: " + url);

    SetSelectedDeckRequest data = new SetSelectedDeckRequest { deckId = this.deckId };
    string jsonBody = JsonUtility.ToJson(data);

    // Debug.Log("[SetDeckAsSelected] Request Body: " + jsonBody);

    UnityWebRequest request = new UnityWebRequest(url, "PUT");
    request.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(jsonBody));
    request.downloadHandler = new DownloadHandlerBuffer();
    request.SetRequestHeader("Content-Type", "application/json");
    request.SetRequestHeader("Authorization", "Bearer " + token);

    yield return request.SendWebRequest();

    string responseText = request.downloadHandler.text;
    Debug.Log("[SetDeckAsSelected] Response: " + responseText);

    if (request.result != UnityWebRequest.Result.Success || responseText.Contains("error"))
    {
        Debug.LogError("[SetDeckAsSelected] Failed to set selected deck");
    }
    else
    {
        Debug.Log("[SetDeckAsSelected] Successfully updated selected deck");
    }
}

    IEnumerator DeleteCurrentDeck()
    {
        string token = PlayerPrefs.GetString("token");
        string url = $"http://localhost:3000/carddecks/{deckId}";

        UnityWebRequest request = UnityWebRequest.Delete(url);
        request.SetRequestHeader("Authorization", "Bearer " + token);

        yield return request.SendWebRequest();

        string responseText = request.downloadHandler != null ? request.downloadHandler.text : "";

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Delete failed: " + request.error);
        }
        else
        {
            if (InventoryManager.Instance != null)
            {
                InventoryManager.Instance.DisplayAllDeckButtons();
            }
        }

        yield return new WaitForSeconds(1.5f);
        gameObject.SetActive(false);
    }



    [System.Serializable] public class Card { public string cardName; public int count; }
    [System.Serializable] public class CardCollection { public List<Card> cards; }
    [System.Serializable] public class CardCollectionResponse { public CardCollection cardCollection; }
    [System.Serializable] public class Deck { public List<Card> cards; }
    [System.Serializable] public class DeckResponse { public Deck deck; }
    [System.Serializable] public class CardRequestData { public string cardName; public int count; }
    [System.Serializable] public class ErrorResponse { public string error; }
    [System.Serializable] public class SuccessResponse { public string message; }
    [System.Serializable]
    public class SetSelectedDeckRequest
    {
        public string deckId;
    }

}