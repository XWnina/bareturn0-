using System.Collections.Generic;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour
{
    public TextMeshProUGUI textP; // 玩家文本框
    public TextMeshProUGUI textN; // NPC文本框
    public TMP_InputField userInputField;
    public GameObject playerDialog;
    public GameObject npcDialog;
    public GameObject pauseMenu;
    public GameObject inputPanel;
    public Button submitButton;
    public Button escButton; // ESC 按钮
    public Button reviewDialogButton; // 聊天记录按钮
    public SendDialogInfoManager sendDialogInfoManager;


    private Queue<string> dialogQueue;
    private bool isPaused = false;
    public string playerName = "You"; // 玩家默认名字
    public int processNum = 0;
    private bool isNamingTask = false; // 是否正在输入名字任务
    private bool isFirstSentenceComplete = false;
    private bool isTeachingMode = false; // ✅ 是否在教学模式
    private int teachingStep = 0; // ✅ 教学当前进行到哪一步

    public static Queue<string> savedDialogQueue = new Queue<string>(); // 存储对话队列状态
    public static bool hasSavedState = false; // 标记是否已有保存状态
    public static bool isDialogFinished = false; // 记录对话是否已结束
    public static List<string> chatHistory = new List<string>();

    void Start()
    {
        //sendDialogInfoManager = FindFirstObjectByType<SendDialogInfoManager>();
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
            if (PlayerPrefs.HasKey("teachingStep"))
            {
                teachingStep = PlayerPrefs.GetInt("teachingStep");
            }
            if (PlayerPrefs.HasKey("isNamingTask"))
            {
                isNamingTask = PlayerPrefs.GetInt("isNamingTask") == 1;
            }
        }
        else
        {
            //dialogQueue = new Queue<string>();
            LoadSampleDialog(); // 只在初次加载时调用
        }
        ShowNextSentence();

        inputPanel.SetActive(false);
        // 绑定按钮事件
        escButton.onClick.AddListener(PauseDialog);
        reviewDialogButton.onClick.AddListener(GoToChatHistory);
        submitButton.onClick.AddListener(ValidateUserInput);
    }

    void Update()
    {
        if (inputPanel.activeSelf) return;

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
        if (chatHistory.Count == 0)
        {
            dialogQueue.Enqueue("You: dhwiadhacnwiodi(Huh, What happened?)");
            dialogQueue.Enqueue("Natasha: Oh, finally you are here.");
            dialogQueue.Enqueue("You: wejiowjdijvkw(Who are you? Where am I?)");
            dialogQueue.Enqueue("Natasha: Welcome to Bareturn0's world!");
            dialogQueue.Enqueue("Natasha: Oh, I almost forgot that you cannot talk for now.");
            dialogQueue.Enqueue("You: ??");
            dialogQueue.Enqueue("Natasha: Try to type in your words in printf(\"\"); Like, printf(\"Hello!\");.");
        }
    }

    public void ShowNextSentence()
    {
        //if (isDialogFinished) return; // **如果对话结束，不再执行**
        if (dialogQueue.Count == 0 && isDialogFinished)
        {
            //isDialogFinished = true; // **对话结束，标记为 true**
            savedDialogQueue.Clear();
            playerDialog.SetActive(false);
            processNum = 1;
            npcDialog.SetActive(false);
            //sendDialogInfoManager.SavePlayerData(playerName, processNum);
            if (sendDialogInfoManager != null)
            {
                sendDialogInfoManager.SavePlayerData(playerName, processNum);
            }
            else
            {
                Debug.LogError("❌ SendDialogInfoManager is NULL! Could not save data.");
            }

            StartCoroutine(LoadSceneAfterDelay("draftMap", 1f));
            return;
        }

        string sentence = dialogQueue.Dequeue();

        if (sentence.Contains("##SHOW_INPUT##"))
        {
            sentence = sentence.Replace("##SHOW_INPUT##", ""); // ✅ 移除标记
            inputPanel.SetActive(true); // ✅ 显示输入框
        }
        else
        {
            inputPanel.SetActive(false); // ✅ 确保其他情况下 `inputPanel` 不显示
        }
        if (!chatHistory.Contains(sentence))
        {
            chatHistory.Add(sentence);
        }
        if (sentence.StartsWith("Natasha: Great " + playerName + ", now let's learn more about printf magic!"))
        {
            playerDialog.SetActive(false);
            npcDialog.SetActive(true);
            textN.text = sentence;
            isTeachingMode = true; // ✅ 开启教学模式
            teachingStep = 1; // ✅ 开始 `char` 教学

            StartCoroutine(ShowTeachingStepAfterDelay(1f)); // ✅ 开始教学
            return;
        }

        if (sentence.StartsWith("Natasha: Try to type in your words in printf(\"\");"))
        {
            playerDialog.SetActive(false);
            npcDialog.SetActive(true);
            textN.text = sentence;
            inputPanel.SetActive(true); // 显示输入框
            return;
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

    public void ValidateUserInput()
    {
        string userInput = userInputField.text.Trim();
        System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(
        @"^printf\(\s*""([^""]+)""\s*(?:,\s*(.+))?\s*\);$"
    );
        System.Text.RegularExpressions.Match match = regex.Match(userInput);

        if (match.Success)
        {
            string extractedText = match.Groups[1].Value;
            string formatSpecifier = match.Groups[1].Value;
            string argument = match.Groups[2].Success ? match.Groups[2].Value.Trim() : ""; // 获取参数（如果存在）
            inputPanel.SetActive(false);
            userInputField.text = "";
            playerDialog.SetActive(true);
            npcDialog.SetActive(false);

            if (isNamingTask) // ✅ 进入名字输入阶段
            {
                playerName = extractedText; // ✅ 存储玩家名字
                chatHistory.Add("You: " + playerName);
                textP.text = "You: " + playerName;
                isNamingTask = false; // ✅ 结束名字任务

                //Invoke(nameof(ShowNextSentence), 0.5f);
                StartCoroutine(ShowNPCBeforeTeaching(1f));
                return;
            }
            if (!isFirstSentenceComplete)
            {
                isFirstSentenceComplete = true;
                textP.text = "You: " + extractedText;
                chatHistory.Add("You: " + extractedText);

                // ✅ 让 Natasha 说 "很好，那你现在告诉一下我你的名字"
                StartCoroutine(ShowNPCAndAskForName(1f));
                return;
            }

            // ✅ 教学模式逻辑
            if (isTeachingMode)
            {
                bool correctInput = false;
                string outputText = argument;
                isNamingTask = false;

                switch (teachingStep)
                {
                    case 1: // `%c`
                        correctInput = System.Text.RegularExpressions.Regex.IsMatch(argument, @"^'.{1}'$"); // 任意单个字符
                        if (correctInput) outputText = argument.Trim('\''); // 去掉引号
                        break;
                    case 2: // `%s`
                        correctInput = System.Text.RegularExpressions.Regex.IsMatch(argument, "^\".*\"$"); // 任意字符串
                        if (correctInput) outputText = argument.Trim('\"'); // 去掉引号
                        break;
                    case 3: // `%d`
                        correctInput = int.TryParse(argument, out _); // 检查是否是整数
                        break;
                    case 4: // `%lf`
                        correctInput = double.TryParse(argument, out _); // 检查是否是浮点数
                        break;
                }
                if (correctInput)
                {
                    textP.text = "You: " + outputText;
                    chatHistory.Add("You: " + outputText);

                    teachingStep++;
                    StartCoroutine(ShowTeachingStepAfterDelay(1f));
                    return;
                }
                else
                {
                    textN.text = "Natasha: Try again using the correct printf syntax.";
                    chatHistory.Add("Natasha: Try again using the correct printf syntax.");
                    StartCoroutine(ShowInputPanelAfterDelay(1f));
                    return;
                }
            }
            //textP.text = "You: " + extractedText;
            //chatHistory.Add("You: " + extractedText);
            Invoke(nameof(ShowNextSentence), 0.5f);
        }
        else
        {
            string randomGarbage = GenerateRandomGarbage(); // 生成乱码

            inputPanel.SetActive(false); // **隐藏输入框**
            playerDialog.SetActive(true); // **先显示玩家的对话**
            npcDialog.SetActive(false);

            textP.text = "You: " + randomGarbage; // ✅ **玩家显示乱码**
            chatHistory.Add("You: " + randomGarbage); // ✅ **存入聊天记录**


            // **1秒后让 Natasha 说话，并再次显示输入框**
            StartCoroutine(ShowNPCAfterDelay(1f));
        }
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
        PlayerPrefs.SetInt("teachingStep", teachingStep);
        PlayerPrefs.SetInt("isNamingTask", isNamingTask ? 1 : 0); // bool 不能直接存，转成 int
        SceneManager.LoadScene("ReviewDialog");
    }

    public static List<string> GetChatHistory()
    {
        return chatHistory;
    }

    private IEnumerator ShowInputPanelAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay); // **等待 `delay` 秒**
        inputPanel.SetActive(true); // **重新显示 `InputField`**
    }

    private IEnumerator ShowNPCAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay); // **等待 `delay` 秒**

        playerDialog.SetActive(false); // **隐藏玩家对话框**
        npcDialog.SetActive(true); // **NPC 开始说话**
        textN.text = "Natasha: You must enter in the format printf(\"some text\");. Try again!";
        chatHistory.Add("Natasha: You must enter in the format printf(\"some text\");. Try again!"); // **存入聊天记录**

        yield return new WaitForSeconds(1f); // **再等1秒后重新显示输入框**
        inputPanel.SetActive(true);
    }

    private IEnumerator ShowNPCAndAskForName(float delay)
    {
        yield return new WaitForSeconds(delay);

        playerDialog.SetActive(false);
        npcDialog.SetActive(true);
        textN.text = "Natasha: Perfect! By the way, what's your name?";
        chatHistory.Add("Natasha: Perfect! By the way, what's your name?");

        yield return new WaitForSeconds(1f);
        inputPanel.SetActive(true); // ✅ 让用户输入名字
        isNamingTask = true; // ✅ 进入名字输入任务
    }
    private IEnumerator ShowNPCBeforeTeaching(float delay)
    {
        yield return new WaitForSeconds(delay);

        playerDialog.SetActive(false);
        npcDialog.SetActive(true);
        textN.text = "Natasha: Great " + playerName + ", now let's learn more about printf magic!";
        chatHistory.Add("Natasha: Great " + playerName + ", now let's learn more about printf magic!");

        yield return new WaitForSeconds(1f);

        // ✅ 开启教学模式
        isTeachingMode = true;
        teachingStep = 1;
        StartCoroutine(ShowTeachingStepAfterDelay(1f));
    }


    private IEnumerator ShowTeachingStepAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        playerDialog.SetActive(false);
        npcDialog.SetActive(true);

        switch (teachingStep)
        {
            case 1:
                // ✅ 逐步加入三句话
                dialogQueue.Enqueue("Natasha: Welcome to C Magic! In this world, printf() is the key to talk.");
                dialogQueue.Enqueue("Natasha: Let's start simple! The `%c` format specifier prints a single character.");
                dialogQueue.Enqueue("Natasha: Try: printf(\"%c\", 'A'); and see what happens! ##SHOW_INPUT##");

                //chatHistory.Add("Natasha: Welcome to C Magic! In this world, printf() is the key to communication.");
                //chatHistory.Add("Natasha: Let's start simple! The `%c` format specifier prints a single character.");
                //chatHistory.Add("Natasha: Try: printf(\"%c\", 'A'); and see what happens! ");

                ShowNextSentence(); // ✅ 触发 Natasha 开始逐步说出三句话
                break;

            case 2:
                dialogQueue.Enqueue("Natasha: Great! You just printed a single character.");
                dialogQueue.Enqueue("Natasha: Now, let's print a full word.");
                dialogQueue.Enqueue("Natasha: The `%s` format specifier is used for strings (a sequence of characters).");
                dialogQueue.Enqueue("Natasha: Try: printf(\"%s\", \"Hello\"); to send a full message! ##SHOW_INPUT##");

                //chatHistory.Add("Natasha: Great! You just printed a single character.");
                //chatHistory.Add("Natasha: Now, let's print a full word.");
                //chatHistory.Add("Natasha: Natasha: The `%s` format specifier is used for strings (a sequence of characters).");
                //chatHistory.Add("Natasha: Try: printf(\"%s\", \"Hello\"); to send a full message!");

                ShowNextSentence();
                break;

            case 3:
                dialogQueue.Enqueue("Natasha: Well done! But sometimes, we need to work with numbers.");
                dialogQueue.Enqueue("Natasha: To print an integer (a whole number), we use `%d`.");
                dialogQueue.Enqueue("Natasha: Try: printf(\"%d\", 123); to display a number. ##SHOW_INPUT##");

                //chatHistory.Add("Natasha: Well done! But sometimes, we need to work with numbers.");
                //chatHistory.Add("Natasha: To print an integer (a whole number), we use `%d`.");
                //chatHistory.Add("Natasha: Try: printf(\"%d\", 123); to display a number.");

                ShowNextSentence();
                break;

            case 4:
                dialogQueue.Enqueue("Natasha: You're getting the hang of it! Now let's deal with decimal numbers.");
                dialogQueue.Enqueue("Natasha: For floating-point numbers (numbers with decimals), we use `%lf`.");
                dialogQueue.Enqueue("Natasha: Try: printf(\"%lf\", 3.14); to print a decimal value. ##SHOW_INPUT##");

                //chatHistory.Add("Natasha: You're getting the hang of it! Now let's deal with decimal numbers.");
                //chatHistory.Add("Natasha: For floating-point numbers (numbers with decimals), we use `%lf`.");
                //chatHistory.Add("Natasha: Try: printf(\"%lf\", 3.14); to print a decimal value.");

                ShowNextSentence();
                break;

            case 5:
                dialogQueue.Enqueue("Natasha: Awesome! You've completed the lesson on printf Magic.");
                dialogQueue.Enqueue("Natasha: Now you understand how to print basic types!");
                dialogQueue.Enqueue("Natasha: You can go explore this world first, and we will meet again.");

                //chatHistory.Add("Natasha: Awesome! You've completed the lesson on C Magic.");
                //chatHistory.Add("Natasha: Now you understand how to print basic types!");
                //chatHistory.Add("Natasha: You can go explore this world first, and we will meet again.");
                isDialogFinished = true;
                ShowNextSentence();

                isTeachingMode = false; // ✅ 结束教学
                teachingStep = 0;
                //isDialogFinished = true;
                inputPanel.SetActive(false);
                //SceneManager.LoadScene("draftMap");
                break;
        }


        yield return new WaitForSeconds(1f);
        //inputPanel.SetActive(true); // ✅ 让用户输入
    }

    private IEnumerator LoadSceneAfterDelay(string sceneName, float delay)
    {
        yield return new WaitForSeconds(delay); // ✅ 等待指定时间
        SceneManager.LoadScene(sceneName); // ✅ 跳转到指定场景
    }

    private string GenerateRandomGarbage()
    {
        string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*()";
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        System.Random random = new System.Random();

        for (int i = 0; i < 15; i++) // **生成 15 个随机字符**
        {
            sb.Append(chars[random.Next(chars.Length)]);
        }

        return sb.ToString();
    }


}
