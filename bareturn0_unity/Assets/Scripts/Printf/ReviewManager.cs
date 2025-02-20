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

        DisplayChatHistory();

        backButton.onClick.AddListener(GoBack);
    }

    void DisplayChatHistory()
    {
        List<string> chatHistory = ChatData.chatHistory;

        foreach (string message in chatHistory)
        {
            GameObject newMessage = Instantiate(messagePrefab, content);
            newMessage.GetComponent<TMP_Text>().text = message;
        }

        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 0f;
    }

    void GoBack()
    {
        SceneManager.LoadScene("printfTeaching");
    }
}
