using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class MatchGameManager : MonoBehaviour
{
    public Button matchGameButton;
    public Image buttonFrame;
    public Image statusOverlay;
    public TMP_Text message;

    public Sprite lockedOverlaySprite;
    public Sprite completedOverlaySprite;
    public Sprite frameSprite;
    private string saveFileName;
    private string authToken;

    private const int matchGameIndex = 0;

    void Start()
    {
        saveFileName = PlayerPrefs.GetString("currentSaveName");
        authToken = PlayerPrefs.GetString("token");

        message.gameObject.SetActive(false);
        statusOverlay.gameObject.SetActive(false);

        StartCoroutine(CheckProgressAndMinigameStatus());
    }

    IEnumerator CheckProgressAndMinigameStatus()
    {
        string progressUrl = $"http://localhost:3000/savefiles/{saveFileName}/progress";
        UnityWebRequest progressRequest = UnityWebRequest.Get(progressUrl);
        progressRequest.SetRequestHeader("Authorization", "Bearer " + authToken);

        yield return progressRequest.SendWebRequest();

        if (progressRequest.result == UnityWebRequest.Result.Success)
        {
            ProgressResponse progressData = JsonUtility.FromJson<ProgressResponse>(progressRequest.downloadHandler.text);

            if (progressData.progress >= 3)
            {
                yield return StartCoroutine(UpdateMinigameStatus(matchGameIndex, "0"));
            }
        }
        else
        {
            Debug.LogError("MatchGameManager Error: getting progress: " + progressRequest.error);
        }

        yield return StartCoroutine(CheckMinigameStatus());
    }

    IEnumerator UpdateMinigameStatus(int index, string value)
    {
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
            Debug.LogError("MatchGameManager Error: updating minigame status: " + request.error);
        }
    }

    IEnumerator CheckMinigameStatus()
    {
        string url = $"http://localhost:3000/savefiles/{saveFileName}/minigamesStatus";
        UnityWebRequest request = UnityWebRequest.Get(url);
        request.SetRequestHeader("Authorization", "Bearer " + authToken);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            MinigameStatusResponse response = JsonUtility.FromJson<MinigameStatusResponse>(request.downloadHandler.text);

            char status;

            if (!string.IsNullOrEmpty(response.status) && response.status.Length > matchGameIndex)
            {
                status = response.status[matchGameIndex];
            }
            else
            {
                status = ' ';
            }

            ApplyButtonStyle(status);
        }
        else
        {
            Debug.LogError("MatchGameManager Error: getting minigame status: " + request.error);
            ApplyButtonStyle(' ');
        }
    }

    private void ApplyButtonStyle(char status)
    {
        matchGameButton.interactable = true;
        matchGameButton.onClick.RemoveAllListeners();

        buttonFrame.sprite = frameSprite;
        statusOverlay.gameObject.SetActive(false);

        if (status == '1') // Completed
        {
            statusOverlay.sprite = completedOverlaySprite;
            statusOverlay.gameObject.SetActive(true);

            matchGameButton.onClick.AddListener(() =>
            {
                SceneManager.LoadScene("MatchGameScene");
            });
        }
        else if (status == '0') // Unlocked
        {
            matchGameButton.onClick.AddListener(() =>
            {
                SceneManager.LoadScene("MatchGameScene");
            });
        }
        else // Locked
        {
            statusOverlay.sprite = lockedOverlaySprite;
            statusOverlay.gameObject.SetActive(true);

            matchGameButton.onClick.AddListener(() =>
            {
                StartCoroutine(ShowLockedMessage());
            });
        }
    }

    IEnumerator ShowLockedMessage()
    {
        message.text = "Hidden Game unlocks after completing level 3!";
        message.gameObject.SetActive(true);
        yield return new WaitForSeconds(2f);
        message.gameObject.SetActive(false);
    }

    [System.Serializable]
    public class MinigameStatusResponse
    {
        public string status;
    }

    [System.Serializable]
    public class ProgressResponse
    {
        public int progress;
    }

    [System.Serializable]
    public class MinigameUpdatePayload
    {
        public int index;
        public string value;
    }
}
