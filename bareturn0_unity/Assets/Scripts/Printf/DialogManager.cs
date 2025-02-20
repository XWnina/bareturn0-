using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.SceneManagement;

public class DialogManager : MonoBehaviour
{
    public GameObject playerDialog;
    public GameObject npcDialog;
    public TextMeshProUGUI playerText;
    public TextMeshProUGUI npcText;
    public Button reviewButton;

    private bool isPlayerTurn = true;
    private bool enterPressed = false;
    public bool isPaused = false; 

    void Start()
    {
        if (ChatData.currentDialogueIndex == 0)
        {
            ChatData.ResetChatData();
            ChatData.playerLines.Enqueue("wddacwifjv??#$");
            ChatData.npcLines.Enqueue("Oh, finally you are here, welcome.");
        }

        playerDialog.SetActive(false);
        npcDialog.SetActive(false);
        
        RestoreChatProgress();

        if (reviewButton != null)
            reviewButton.onClick.AddListener(OpenReviewDialog);
    }

    void RestoreChatProgress()
    {
        // 恢复对话框状态，并确保历史顺序正确
        if (ChatData.currentDialogueIndex % 2 == 0 && ChatData.playerLines.Count > 0)
        {
            playerDialog.SetActive(true);
            playerText.text = ChatData.playerLines.Peek();
            isPlayerTurn = true;
        }
        else if (ChatData.npcLines.Count > 0)
        {
            npcDialog.SetActive(true);
            npcText.text = ChatData.npcLines.Peek();
            isPlayerTurn = false;
        }
    }

    void Update()
    {
        if (isPaused) return;

        if (Input.GetMouseButtonDown(0) || (Input.GetKeyDown(KeyCode.Return) && !enterPressed))
        {
            enterPressed = true;
            ShowNextDialogue();
        }

        if (Input.GetKeyUp(KeyCode.Return))
        {
            enterPressed = false;
        }
    }

    void ShowNextDialogue()
    {
        if (isPlayerTurn)
        {
            if (ChatData.npcLines.Count > 0)
            {
                string message = ChatData.npcLines.Dequeue();
                
                // **存储对话，确保按顺序插入**
                ChatData.chatHistory.Add("Natasha: " + message);

                playerDialog.SetActive(false);
                npcDialog.SetActive(true);
                npcText.text = message;
                isPlayerTurn = false;
                ChatData.currentDialogueIndex++;
            }
        }
        else
        {
            if (ChatData.playerLines.Count > 0)
            {
                string message = ChatData.playerLines.Dequeue();
                
                // **存储对话，确保按顺序插入**
                ChatData.chatHistory.Add("You: " + message);

                npcDialog.SetActive(false);
                playerDialog.SetActive(true);
                playerText.text = message;
                isPlayerTurn = true;
                ChatData.currentDialogueIndex++;
            }
            else
            {
                playerDialog.SetActive(false);
                npcDialog.SetActive(false);
            }
        }
    }

    public void OpenReviewDialog()
    {
        SceneManager.LoadScene("ReviewDialog");
    }
}
