using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using UnityEngine.UI;

public class MapController : MonoBehaviour
{
    private string apiBaseUrl = "http://localhost:3000"; // 你的后端地址

    public TextMeshProUGUI progressText; // 用于显示当前关卡
    public GameObject[] levelIcons; // 地图上所有的关卡按钮
    public Color completedColor = Color.green; // 已完成关卡的颜色
    public Color currentColor = Color.yellow; // 当前关卡的颜色
    public Color lockedColor = Color.gray; // 未解锁关卡的颜色

    private string username;
    private string saveName;
    private int currentLevel = 0;

    void Start()
    {
        StartCoroutine(GetUserInfo());
    }

    IEnumerator GetUserInfo()
    {
        string url = apiBaseUrl + "/users/me"; // 这里应该是获取当前用户的 API
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string json = request.downloadHandler.text;
            UserData data = JsonUtility.FromJson<UserData>(json);

            username = data.username;
            saveName = data.saveName; // 这里需要后端返回 `saveName`

            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(saveName))
            {
                StartCoroutine(GetUserLevel());
            }
        }
        else
        {
            Debug.LogError("Error fetching user info: " + request.error);
        }
    }

    IEnumerator GetUserLevel()
    {
        string url = apiBaseUrl + "/progress/" + username + "/" + saveName;
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string json = request.downloadHandler.text;
            UserProgressData data = JsonUtility.FromJson<UserProgressData>(json);
            currentLevel = data.currentLevel;

            if (progressText != null)
            {
                progressText.text = "You are at Level: " + currentLevel;
            }

            UpdateMapUI();
        }
        else
        {
            Debug.LogError("Error fetching user progress: " + request.error);
        }
    }

    void UpdateMapUI()
    {
        for (int i = 0; i < levelIcons.Length; i++)
        {
            Button levelButton = levelIcons[i].GetComponent<Button>();
            Image levelImage = levelIcons[i].GetComponent<Image>();

            if (i < currentLevel)
            {
                levelImage.color = completedColor; // 绿色：已完成
                levelButton.interactable = true;
            }
            else if (i == currentLevel)
            {
                levelImage.color = currentColor; // 黄色：当前关卡
                levelButton.interactable = true;
            }
            else
            {
                levelImage.color = lockedColor; // 灰色：未解锁
                levelButton.interactable = false;
            }
        }
    }

    [System.Serializable]
    private class UserData
    {
        public string username;
        public string saveName;
    }

    [System.Serializable]
    private class UserProgressData
    {
        public string username;
        public string saveName;
        public int currentLevel;
    }
}
