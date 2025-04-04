using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;


public class ProfileManager : MonoBehaviour
{
    public Button ExitButton;
    public Button ModificationButton;
    public GameObject CardCollection;
    public Button ChangePasswordButton;

    private string saveName;
    private string token;

    // Basic Info
    public TextMeshProUGUI userNameText;
    public TextMeshProUGUI selectedDeckText;

    // Selected Deck
    public PlayerInfoLoader playerInfoLoader;
    public List<CardData> playerCards = new List<CardData>();
    public GameObject CardPrefab;

    // Username Modification Panel
    public GameObject userNameModificationPanel;
    public TMP_InputField userNameInputField;
    public Button userNameModificationPanelCancelButton;
    public Button userNameModificationPanelConfirmButton;
    public TextMeshProUGUI userNameMessageTMP;

    // Password Modification Panel
    public GameObject passwordModificationPanel;
    public TMP_InputField oldPasswordInputField;
    public TMP_InputField newPasswordInputField;
    public Button passwordModificationPanelCancelButton;
    public Button passwordModificationPanelConfirmButton;
    public TextMeshProUGUI passwordMessageTMP;

    // Savefile Info
    public TextMeshProUGUI saveNameInput;
    public TextMeshProUGUI playerNameInput;
    public TextMeshProUGUI currentLevelInput;
    public TextMeshProUGUI coinsInput;
    public TextMeshProUGUI maxHealthInput;
    public TextMeshProUGUI speedInput;
    public TextMeshProUGUI createdAtInput;


    void Start()
    {
        saveName = PlayerPrefs.GetString("currentSaveName");
        token = PlayerPrefs.GetString("token");

        PlayerPrefs.SetString("PreviousScene", "draftMap");

        userNameModificationPanel.SetActive(false);
        userNameMessageTMP.gameObject.SetActive(false);
        passwordModificationPanel.SetActive(false);
        passwordMessageTMP.gameObject.SetActive(false);

        ExitButton.onClick.AddListener(OnExitButtonClick);
        ModificationButton.onClick.AddListener(OnUserNameModificationButtonClick);

        userNameModificationPanelConfirmButton.onClick.AddListener(OnConfirmUserNameChange);
        userNameModificationPanelCancelButton.onClick.AddListener(() =>
        {
            userNameModificationPanel.SetActive(false);
        });
        ChangePasswordButton.onClick.AddListener(() =>
        {
            passwordModificationPanel.SetActive(true);
            oldPasswordInputField.text = "";
            newPasswordInputField.text = "";
            passwordMessageTMP.gameObject.SetActive(false);
        });

        passwordModificationPanelConfirmButton.onClick.AddListener(OnConfirmPasswordChange);
        passwordModificationPanelCancelButton.onClick.AddListener(() =>
        {
            passwordModificationPanel.SetActive(false);
        });


        playerInfoLoader.LoadPlayerDeck("selectedDeck", () =>
        {
            playerCards = playerInfoLoader.cardList;
            PopulateCollection();
        });

        StartCoroutine(FetchUsernameAndDeck());
        saveNameInput.text = saveName;
        StartCoroutine(FetchAndSet<PlayerNameResponse>($"http://localhost:3000/savefiles/{saveName}/playerName", "playerName", playerNameInput));
        StartCoroutine(FetchAndSet<CoinsResponse>($"http://localhost:3000/savefiles/{saveName}/coins", "coins", coinsInput));
        StartCoroutine(FetchAndSet<MaxHealthResponse>($"http://localhost:3000/savefiles/{saveName}/maxHealth", "maxHealth", maxHealthInput));
        StartCoroutine(FetchAndSet<SpeedResponse>($"http://localhost:3000/savefiles/{saveName}/speed", "speed", speedInput));
        StartCoroutine(FetchAndSet<ProgressResponse>($"http://localhost:3000/savefiles/{saveName}/progress", "progress", currentLevelInput));
        StartCoroutine(FetchAndSet<CreatedAtResponse>($"http://localhost:3000/savefiles/{saveName}", "createdAt", createdAtInput));
    }

    // All Button Clicks
    public void OnExitButtonClick()
    {
        SceneManager.LoadScene(PlayerPrefs.GetString("PreviousScene"));
    }

    public void OnUserNameModificationButtonClick()
    {
        userNameModificationPanel.SetActive(true);
        userNameInputField.text = userNameText.text;
    }

    public void OnConfirmPasswordChange()
    {
        string oldPass = oldPasswordInputField.text.Trim();
        string newPass = newPasswordInputField.text.Trim();

        if (!string.IsNullOrEmpty(oldPass) && !string.IsNullOrEmpty(newPass))
        {
            StartCoroutine(UpdatePassword(oldPass, newPass));
        }
    }

    public void OnConfirmUserNameChange()
    {
        string newUsername = userNameInputField.text.Trim();
        if (!string.IsNullOrEmpty(newUsername))
        {
            StartCoroutine(UpdateUsername(newUsername));
        }
    }

    // Selected Deck Population
    public void PopulateCollection()
    {
        foreach (Transform child in CardCollection.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (var cardData in playerCards)
        {
            GameObject card = Instantiate(CardPrefab, CardCollection.transform);
            CardThumbnailUI cardThumbnail = card.GetComponent<CardThumbnailUI>();
            cardThumbnail.SetCardThumbnail(cardData);
        }
    }

    // Basic Info Fetching
    IEnumerator FetchUsernameAndDeck()
    {
        string username = "Unknown";
        string selectedDeckName = "Unknown";

        // Fetch username
        string userUrl = "http://localhost:3000/users/me";
        UnityWebRequest userRequest = UnityWebRequest.Get(userUrl);
        userRequest.SetRequestHeader("Authorization", "Bearer " + token);
        yield return userRequest.SendWebRequest();

        if (userRequest.result == UnityWebRequest.Result.Success)
        {
            UserResponse userResp = JsonUtility.FromJson<UserResponse>(userRequest.downloadHandler.text);
            username = userResp.username;
        }
        else
        {
            Debug.LogWarning("Failed to fetch username: " + userRequest.error);
        }

        // Fetch selected deck name
        string deckUrl = $"http://localhost:3000/savefiles/{saveName}/selectedDeckName";
        UnityWebRequest deckRequest = UnityWebRequest.Get(deckUrl);
        deckRequest.SetRequestHeader("Authorization", "Bearer " + token);
        yield return deckRequest.SendWebRequest();

        if (deckRequest.result == UnityWebRequest.Result.Success)
        {
            SelectedDeckResponse deckResp = JsonUtility.FromJson<SelectedDeckResponse>(deckRequest.downloadHandler.text);
            selectedDeckName = deckResp.selectedDeckName;
        }
        else
        {
            Debug.LogWarning("Failed to fetch selected deck name: " + deckRequest.error);
        }

        userNameText.text = username;
        selectedDeckText.text = selectedDeckName;
    }

    // Username Modification
    IEnumerator UpdateUsername(string newUsername)
    {
        string url = "http://localhost:3000/users/updateUsername";

        var body = new UpdateUsernameRequest { newUsername = newUsername };
        string jsonBody = JsonUtility.ToJson(body);

        UnityWebRequest request = new UnityWebRequest(url, "PUT");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonBody);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();

        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + token);

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Failed to update username: " + request.error);
            Debug.LogError("Response: " + request.downloadHandler.text);

            // Parse error message and display it
            string errorMessage = ParseErrorMessage(request.downloadHandler.text);
            ShowMessage(errorMessage);
        }
        else
        {
            string responseText = request.downloadHandler.text;
            Debug.Log("Username updated successfully");
            Debug.Log("Response: " + responseText);

            userNameText.text = newUsername;
            ShowMessage("Username updated successfully");
        }
    }

    IEnumerator HideMessageAfterDelay()
    {
        yield return new WaitForSeconds(1f);
        userNameMessageTMP.gameObject.SetActive(false);
        userNameModificationPanel.SetActive(false);
    }

    // Password Modification
    IEnumerator UpdatePassword(string oldPassword, string newPassword)
    {
        string url = "http://localhost:3000/users/updatePassword";

        var body = new UpdatePasswordRequest
        {
            oldPassword = oldPassword,
            newPassword = newPassword
        };

        string jsonBody = JsonUtility.ToJson(body);

        UnityWebRequest request = new UnityWebRequest(url, "PUT");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonBody);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();

        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Authorization", "Bearer " + token);

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            string errMsg = ParseErrorMessage(request.downloadHandler.text);
            ShowPasswordMessage(errMsg);
        }
        else
        {
            ShowPasswordMessage("Password updated successfully");
        }
    }
    void ShowPasswordMessage(string message)
    {
        passwordMessageTMP.gameObject.SetActive(true);
        passwordMessageTMP.text = message;
        StartCoroutine(HidePasswordMessageAfterDelay());
    }

    IEnumerator HidePasswordMessageAfterDelay()
    {
        yield return new WaitForSeconds(1f);
        passwordMessageTMP.gameObject.SetActive(false);
        passwordModificationPanel.SetActive(false);
    }

    // Message Display
    void ShowMessage(string message)
    {
        userNameMessageTMP.gameObject.SetActive(true);
        userNameMessageTMP.text = message;
        StartCoroutine(HideMessageAfterDelay());
    }
    string ParseErrorMessage(string json)
    {
        try
        {
            ErrorResponse error = JsonUtility.FromJson<ErrorResponse>(json);
            return error.error;
        }
        catch
        {
            return "Unexpected error occurred.";
        }
    }

    // Savefile Info Fetching
    public IEnumerator FetchAndSet<T>(string url, string key, TextMeshProUGUI output) where T : IKeyedResponse, new()
{
    UnityWebRequest request = UnityWebRequest.Get(url);
    request.SetRequestHeader("Authorization", "Bearer " + token);
    yield return request.SendWebRequest();

    if (request.result == UnityWebRequest.Result.Success)
    {
        string json = request.downloadHandler.text;

        T response = JsonUtility.FromJson<T>(json);

        if (response.HasKey(key))
        {
            string value = response.GetValue(key);
            output.text = value;
        }
        else
        {
            Debug.LogWarning($"Key '{key}' not found in response from {url}");
        }
    }
    else
    {
        Debug.LogError($"Failed to fetch {key} from {url}: {request.error}");
    }
}

    // ========== Shared Interfaces ==========
    public interface IKeyedResponse
    {
        bool HasKey(string key);
        string GetValue(string key);
    }

    // ========== Response Classes ==========
    [System.Serializable]
    public class ErrorResponse
    {
        public string error;
    }

    [System.Serializable]
    public class UserResponse
    {
        public string username;
    }

    [System.Serializable]
    public class SelectedDeckResponse
    {
        public string selectedDeckName;
    }

    [System.Serializable]
    public class PlayerNameResponse : IKeyedResponse
    {
        public string playerName;

        public bool HasKey(string key) => key == "playerName";
        public string GetValue(string key) => playerName;
    }

    [System.Serializable]
    public class CoinsResponse : IKeyedResponse
    {
        public int coins;

        public bool HasKey(string key) => key == "coins";
        public string GetValue(string key) => coins.ToString();
    }

    [System.Serializable]
    public class MaxHealthResponse : IKeyedResponse
    {
        public int maxHealth;

        public bool HasKey(string key) => key == "maxHealth";
        public string GetValue(string key) => maxHealth.ToString();
    }

    [System.Serializable]
    public class SpeedResponse : IKeyedResponse
    {
        public int speed;

        public bool HasKey(string key) => key == "speed";
        public string GetValue(string key) => speed.ToString();
    }

    [System.Serializable]
    public class ProgressResponse : IKeyedResponse
    {
        public int progress;

        public bool HasKey(string key) => key == "progress";
        public string GetValue(string key) => progress.ToString();
    }

    [System.Serializable]
    public class CreatedAtResponse : IKeyedResponse
    {
        public string createdAt;

        public bool HasKey(string key) => key == "createdAt";
        public string GetValue(string key) => createdAt;
    }

    // ========== Request Classes ==========
    [System.Serializable]
    public class UpdateUsernameRequest
    {
        public string newUsername;
    }

    [System.Serializable]
    public class UpdatePasswordRequest
    {
        public string oldPassword;
        public string newPassword;
    }

}
