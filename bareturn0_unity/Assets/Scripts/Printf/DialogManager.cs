using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogManager : MonoBehaviour
{
    public GameObject playerDialog;
    public GameObject npcDialog;
    public TextMeshProUGUI playerText;
    public TextMeshProUGUI npcText;

    private Queue<string> playerLines;
    private Queue<string> npcLines;
    private bool isPlayerTurn = true;
    private bool enterPressed = false; // 防止长按Enter触发多次

    void Start()
    {
        playerLines = new Queue<string>();
        npcLines = new Queue<string>();

        playerLines.Enqueue("Huh, where am I?");
        npcLines.Enqueue("Oh, finally you are here, welcome.");
        playerLines.Enqueue("What's THIS world??");
        npcLines.Enqueue("This is ...");

        npcDialog.SetActive(false);
        playerDialog.SetActive(true);
        playerText.text = playerLines.Dequeue();
    }

    void Update()
    {
        // 检测鼠标点击
        if (Input.GetMouseButtonDown(0))
        {
            ShowNextDialogue();
        }

        // 检测按键按下
        if (Input.GetKeyDown(KeyCode.Return) && !enterPressed)
        {
            enterPressed = true;
            ShowNextDialogue();
        }

        // 检测按键松开，防止长按触发多次
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
                playerDialog.SetActive(false);
                npcDialog.SetActive(true);
                npcText.text = npcLines.Dequeue();
                isPlayerTurn = false;
            }
        }
        else
        {
            if (playerLines.Count > 0)
            {
                npcDialog.SetActive(false);
                playerDialog.SetActive(true);
                playerText.text = playerLines.Dequeue();
                isPlayerTurn = true;
            }
            else
            {
                playerDialog.SetActive(false);
                npcDialog.SetActive(false);
            }
        }
    }
}
