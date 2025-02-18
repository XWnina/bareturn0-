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

    void Start()
    {
        // ??????
        List<string> chatHistory = ChatData.chatHistory;

        // ??????
        foreach (string message in chatHistory)
        {
            GameObject newMessage = Instantiate(messagePrefab, content);
            newMessage.GetComponent<TMP_Text>().text = message;
        }

        // ???????????
        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 0f;

        // ??????
        backButton.onClick.AddListener(GoBack);
    }

    void GoBack()
    {
        SceneManager.LoadScene("printfTeaching"); // ?????
    }
}
