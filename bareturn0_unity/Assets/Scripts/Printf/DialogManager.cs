using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour
{
    public TextMeshProUGUI textP; // 玩家文本框
    public TextMeshProUGUI textN; // NPC文本框
    public GameObject playerDialog;
    public GameObject npcDialog;
    public GameObject pauseMenu;
    public Button escButton; // ESC 按钮
    public Button reviewDialogButton; // 聊天记录按钮
    private Queue<string> dialogQueue;
    private bool isPaused = false;
    //private bool isPlayerTurn = true; // 交替控制对话轮次
    public static Queue<string> savedDialogQueue = new Queue<string>(); // 存储对话队列状态
    public static bool hasSavedState = false; // 标记是否已有保存状态
    public static bool isDialogFinished = false; // 记录对话是否已结束
    public static List<string> chatHistory = new List<string>();

    void Start()
    {
        //LoadSampleDialog();
        if (isDialogFinished)
        {
            playerDialog.SetActive(false);
            npcDialog.SetActive(false);
            return; // **如果对话已经结束，不再重新开始**
        }
        dialogQueue = new Queue<string>();
        if (hasSavedState && savedDialogQueue.Count > 0)
        {
            dialogQueue = new Queue<string>(savedDialogQueue); // 恢复对话进度
        }
        else
        {
            //dialogQueue = new Queue<string>();
            LoadSampleDialog(); // 只在初次加载时调用
        }
        ShowNextSentence();

        // 绑定按钮事件
        escButton.onClick.AddListener(PauseDialog);
        reviewDialogButton.onClick.AddListener(GoToChatHistory);
    }

    void Update()
    {
        if (!isPaused && Input.GetKeyDown(KeyCode.Return))
        {
            ShowNextSentence();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseDialog();
        }
    }

    void LoadSampleDialog()
    {
        // 交替对话
        if(chatHistory.Count == 0)
        {
            dialogQueue.Enqueue("You: dhwiadhacnwiodi(Huh, What happened?)");
            dialogQueue.Enqueue("Natasha: Oh, finally you are here.");
            dialogQueue.Enqueue("You: wejiowjdijvkw(Who are you? Where am I?)");
            dialogQueue.Enqueue("Natasha: Welcome to Bareturn0's world!");
            dialogQueue.Enqueue("Natasha: Oh, I almost forgot that you cannot talk for now.");
            dialogQueue.Enqueue("You: ??");
            dialogQueue.Enqueue("Natasha: Try to type in your words in printf(''); Like, printf('Hello!');.");
            
        }
    }

    public void ShowNextSentence()
    {
        if (isDialogFinished) return; // **如果对话结束，不再执行**
        if (dialogQueue.Count == 0)
        {
            isDialogFinished = true; // **对话结束，标记为 true**
            savedDialogQueue.Clear();
            playerDialog.SetActive(false);
            npcDialog.SetActive(false);
            return;
        }
        
        string sentence = dialogQueue.Dequeue();
        if (!chatHistory.Contains(sentence))
        {
            chatHistory.Add(sentence);
        }

        if (!sentence.StartsWith("Natasha:"))
        {
            //isPlayerTurn = false; // 下次轮到 NPC
            playerDialog.SetActive(true);
            npcDialog.SetActive(false);
            textP.text = sentence;
        }
        else
        {
            //isPlayerTurn = true; // 下次轮到用户
            playerDialog.SetActive(false);
            npcDialog.SetActive(true);
            textN.text = sentence;
        }
    }

    public void PauseDialog()
    {
        isPaused = true;
        pauseMenu.SetActive(true);
    }

    public void ResumeDialog()
    {
        isPaused = false;
        pauseMenu.SetActive(false);
    }

    public void GoToChatHistory()
    {
        isPaused = true;
        // **保存对话进度**
        savedDialogQueue = new Queue<string>(dialogQueue);
        hasSavedState = true;
        SceneManager.LoadScene("ReviewDialog");
    }
    public static List<string> GetChatHistory()
    {
        return chatHistory;
    }
}
