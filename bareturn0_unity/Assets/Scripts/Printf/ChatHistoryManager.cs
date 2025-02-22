using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ChatHistoryManager : MonoBehaviour
{
    public TextMeshProUGUI chatHistoryText;
    public Button backButton;

    void Start()
    {
        DisplayChatHistory();
        backButton.onClick.AddListener(BackToDialog);
    }

    void DisplayChatHistory()
    {
        chatHistoryText.text = "";
        foreach (string entry in DialogManager.chatHistory)
        {   
            chatHistoryText.text += entry + "\n";
        }
    }

    public void BackToDialog()
    {
        SceneManager.LoadScene("printfTeaching");
    }
}
