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

    private Queue<string> playerLines;
    private Queue<string> npcLines;
    private bool isPlayerTurn = true;
    private bool enterPressed = false;
    public bool isPaused = false; // 由 ESCManager 控制，暂停时禁止对话

    void Start()
    {
        playerLines = new Queue<string>();
        npcLines = new Queue<string>();

        playerLines.Enqueue("wddacwifjv??#$");
        npcLines.Enqueue("Oh, finally you are here, welcome.");

        npcDialog.SetActive(false);
        playerDialog.SetActive(true);

        // **游戏启动时清除旧的聊天记录**
        if (ChatData.currentDialogueIndex == 0)
        {
            ChatData.ResetChatData();
        }

        // **恢复聊天进度**
        int savedIndex = ChatData.currentDialogueIndex;
        for (int i = 0; i < savedIndex; i++)
        {
            if (isPlayerTurn && npcLines.Count > 0) npcLines.Dequeue();
            else if (!isPlayerTurn && playerLines.Count > 0) playerLines.Dequeue();
            isPlayerTurn = !isPlayerTurn;
        }

        // **确保继续对话**
        if (isPlayerTurn && playerLines.Count > 0)
            playerText.text = playerLines.Peek();
        else if (!isPlayerTurn && npcLines.Count > 0)
            npcText.text = npcLines.Peek();

        // 绑定 Review 按钮
        if (reviewButton != null)
            reviewButton.onClick.AddListener(OpenReviewDialog);
    }

    void Update()
    {
        if (isPaused) return;

        if (Input.GetMouseButtonDown(0))
        {
            ShowNextDialogue();
        }

        if (Input.GetKeyDown(KeyCode.Return) && !enterPressed)
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
            if (npcLines.Count > 0)
            {
                string message = npcLines.Dequeue();

                // **只存储新的聊天内容**
                if (ChatData.chatHistory.Count <= ChatData.currentDialogueIndex)
                {
                    ChatData.chatHistory.Add("Natasha: " + message);
                }

                playerDialog.SetActive(false);
                npcDialog.SetActive(true);
                npcText.text = message;
                isPlayerTurn = false;
                ChatData.currentDialogueIndex++;
            }
        }
        else
        {
            if (playerLines.Count > 0)
            {
                string message = playerLines.Dequeue();

                if (ChatData.chatHistory.Count <= ChatData.currentDialogueIndex)
                {
                    ChatData.chatHistory.Add("You: " + message);
                }

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
