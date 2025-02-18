using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class ReviewManager : MonoBehaviour
{
    public GameObject messagePrefab;
    public Transform content;
    public ScrollRect scrollRect;
    public Button backButton;
    private bool isLoaded = false;

    void Start()
    {
        if (isLoaded) return;
        isLoaded = true;

        // **获取聊天记录**
        List<string> chatHistory = ChatData.chatHistory;

        // **显示聊天记录**
        foreach (string message in chatHistory)
        {
            GameObject newMessage = Instantiate(messagePrefab, content);
            newMessage.GetComponent<TMP_Text>().text = message;
        }

        // **滚动到底部**
        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 0f;

        // **绑定返回按钮**
        backButton.onClick.AddListener(GoBack);
    }

    void GoBack()
    {
        SceneManager.LoadScene("printfTeaching");
    }
}
