using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using System.Collections;

public class MatchLogicManager : MonoBehaviour
{
    public static MatchLogicManager Instance;

    public GameObject matchingButton;

    public Button ExitButton;
    public Transform dataParent;
    public Transform typeParent;

    public TMP_Text statusMessage;
    public float messageDisplayTime = 2f;

    public GameObject winPanel;           // New: assign in Inspector
    public TMP_Text winPanelText;         // New: assign in Inspector

    private MatchCard selectedValue;
    private MatchCard selectedType;

    private int totalMatches = 0;
    private int matchGoal => valuePool.Count;
    private Dictionary<string, string> matchDict = new();

    private Dictionary<string, List<string>> valuePool = new()
    {
        { "char", new List<string> { "'A'", "'B'", "'Z'", "'x'", "'7'" } },
        { "int", new List<string> { "0", "42", "123", "-7", "9999" } },
        { "float", new List<string> { "1.2f", "3.14f", "-0.99f", "0f", "2.5f" } },
        { "double", new List<string> { "2.718281828", "3.141592653", "0.0001", "-7.5", "100000.000001" } },
        { "string", new List<string> { "\"Hello\"", "\"C\"", "\"Hello World\"", "\"barturn0;\"", "\"MatchGame\"" } }
    };

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        PlayerPrefs.SetString("PreviousScene", "draftMap");

        statusMessage.gameObject.SetActive(false);
        winPanel.SetActive(false); // Ensure win panel starts hidden

        List<(string, MatchCard.CardKind)> valueCards = new();
        List<(string, MatchCard.CardKind)> typeCards = new();

        matchDict.Clear();

        foreach (var kvp in valuePool)
        {
            string type = kvp.Key;
            List<string> possibleValues = kvp.Value;
            string chosenValue = possibleValues[Random.Range(0, possibleValues.Count)];

            matchDict[chosenValue] = type;
            valueCards.Add((chosenValue, MatchCard.CardKind.Value));
            typeCards.Add((type, MatchCard.CardKind.Type));
        }

        Shuffle(valueCards);
        Shuffle(typeCards);

        foreach (var (content, kind) in valueCards)
            CreateCard(content, kind, dataParent);

        foreach (var (content, kind) in typeCards)
            CreateCard(content, kind, typeParent);

        ExitButton.onClick.AddListener(() =>
        {
            SceneManager.LoadScene(PlayerPrefs.GetString("PreviousScene"));
        });
    }

    private void CreateCard(string content, MatchCard.CardKind kind, Transform parent)
    {
        var card = Instantiate(matchingButton, parent);
        card.GetComponent<MatchCard>().Initialize(content, kind);
    }

    public void OnCardClicked(MatchCard card)
    {
        if (card.kind == MatchCard.CardKind.Value)
            selectedValue = card;
        else
            selectedType = card;

        if (selectedValue != null && selectedType != null)
        {
            bool correct = matchDict.TryGetValue(selectedValue.content, out string expectedType) &&
                           expectedType == selectedType.content;

            if (correct)
            {
                selectedValue.Hide();
                selectedType.Hide();
                totalMatches++;

                ShowStatus("Correct!", messageDisplayTime);

                if (totalMatches >= matchGoal)
                {
                    StartCoroutine(AddCoins(5));
                    StartCoroutine(UpdateMinigameStatus(0, "1"));
                    ShowWinPanel("You win! You've earned 5 coins!");
                }
            }
            else
            {
                ShowStatus("Incorrect, please try again!", messageDisplayTime);
            }

            selectedValue = null;
            selectedType = null;
        }
    }

    public void ShowStatus(string message, float delay)
    {
        CancelInvoke(nameof(HideStatus));
        statusMessage.text = message;
        statusMessage.gameObject.SetActive(true);
        Invoke(nameof(HideStatus), delay);
    }

    private void HideStatus()
    {
        statusMessage.gameObject.SetActive(false);
    }

    private void ShowWinPanel(string message)
    {
        winPanelText.text = message;
        winPanel.SetActive(true);
        StartCoroutine(HideWinPanelAfterDelay(3f));

    }

    private IEnumerator HideWinPanelAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        winPanel.SetActive(false);
        SceneManager.LoadSceneAsync(PlayerPrefs.GetString("PreviousScene"));
    }


    private IEnumerator AddCoins(int amount)
    {
        string saveName = PlayerPrefs.GetString("currentSaveName");
        string token = PlayerPrefs.GetString("token");

        string url = $"http://localhost:3000/savefiles/{saveName}/coins";
        UnityWebRequest getRequest = UnityWebRequest.Get(url);
        getRequest.SetRequestHeader("Authorization", "Bearer " + token);
        yield return getRequest.SendWebRequest();

        if (getRequest.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Failed to fetch current coins: " + getRequest.error);
            yield break;
        }

        int currentCoins = JsonUtility.FromJson<CoinResponse>(getRequest.downloadHandler.text).coins;
        int newTotal = currentCoins + amount;

        url = $"http://localhost:3000/savefiles/{saveName}/updateCoins";
        UnityWebRequest putRequest = new UnityWebRequest(url, "PUT");
        string jsonBody = JsonUtility.ToJson(new CoinUpdate { coins = newTotal });

        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonBody);
        putRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
        putRequest.downloadHandler = new DownloadHandlerBuffer();
        putRequest.SetRequestHeader("Authorization", "Bearer " + token);
        putRequest.SetRequestHeader("Content-Type", "application/json");

        yield return putRequest.SendWebRequest();
    }

    private IEnumerator UpdateMinigameStatus(int index, string value)
    {
        string saveFileName = PlayerPrefs.GetString("currentSaveName");
        string authToken = PlayerPrefs.GetString("token");

        string url = $"http://localhost:3000/savefiles/{saveFileName}/updateMinigames";
        UnityWebRequest request = new UnityWebRequest(url, "POST");

        MinigameUpdatePayload payload = new MinigameUpdatePayload { index = index, value = value };
        string jsonBody = JsonUtility.ToJson(payload);
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonBody);

        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + authToken);

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("MatchLogicManager Error: updating minigame status: " + request.error);
        }
        else
        {
            Debug.Log("Minigame status updated to: " + value);
        }
    }

    private void Shuffle<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int rand = Random.Range(0, i + 1);
            (list[i], list[rand]) = (list[rand], list[i]);
        }
    }

    [System.Serializable]
    private class MinigameUpdatePayload
    {
        public int index;
        public string value;
    }

    [System.Serializable]
    private class CoinResponse { public int coins; }

    [System.Serializable]
    private class CoinUpdate { public int coins; }
}
