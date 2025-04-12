using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

namespace CalcuTeaching
{
    public class NpcDialogueController : MonoBehaviour
    {
        public GameObject npcDialog;
        public GameObject playerDialog;
        public GameObject teachingPanel;
        public GameObject pausePanel;
        public Button closeButton;
        public Button escButton;
        public Button backMapButton;
        public Button backMenuButton;

        public TextMeshProUGUI npcText;
        public TextMeshProUGUI playerText;
        public TMP_InputField codeInput;
        public TextMeshProUGUI questionText;
        public CodeEvaluator codeEvaluator;
        public GameObject nextButton;

        [Header("Demo Mode(Press \u2192 to skip)")]
        public bool demoMode;


        private int _dialogueIndex;
        private bool _waitingForInput;
        private bool _waitingForSecondInput;
        private string[] _currentIntroLines;
        private int _currentLineIndex;
        private bool _isPlayingIntro;
        private string _currentQuestionText;


        private readonly string[] _npcLines = new string[]
        {
            "Hi young one, are you the newcomer to this world?",
            "I am Smith, the blacksmith here. I'm building a computer for the shop. Can you help me?",
            "No no no, in this world we use C language to do math. Let me teach you.",
            "In C, there is a variable type called int.",
            "When we want to assign an integer value, like saying an apple is 3 gold, we write: int apple = 3;",
            "Now it's your turn! "
        };

        private readonly string[] _playerLines = new string[]
        {
            "Yes, who are you?",
            "Of course! My math was great in my old world."
        };

        private readonly string[] _doubleIntroLines = new string[]
        {
            "Great! But this world doesn't only deal with integers. What if we need to work with decimal numbers?",
            "That's where other types like double come in.",
            "Double is used for storing floating point numbers, like 3.14 or 0.99.",
            "Now try using the double type. Assign a decimal value. (e.g. double price = 3.14;)"
        };

        private int _doubleLineIndex;
        private bool _playingDoubleIntro;

        private readonly string[] _floatIntroLines = new string[]
        {
            "Excellent! You've learned about int and double.",
            "But there's also a lighter-weight decimal type: float.",
            "Float is useful when precision is less important, and it uses less memory than double.",
            "Try declaring a float variable and assigning it a value like 2.5f."
        };

        private int _floatLineIndex;
        private bool _playingFloatIntro;

        private readonly string[] _additionIntroLines = new string[]
        {
            "Nice! Now let's try some math operations.",
            "In C, we can add numbers using the '+' operator.",
            "For example, you can write: int sum = 2 + 3;",
            "Now it's your turn! Try declaring a variable and assigning it a sum."
        };

        private int _addLineIndex;
        private bool _playingAddIntro;

        private readonly string[] _subtractionIntroLines = new string[]
        {
            "Great job with addition!",
            "Now let's try subtraction using the '-' operator.",
            "In C, you can subtract like this: int result = 7 - 2;",
            "Give it a try! Declare a variable and assign it a subtraction result."
        };

        private int _subLineIndex;
        private bool _playingSubIntro;

        private readonly string[] _multiplicationIntroLines = new string[]
        {
            "You're doing great!",
            "Next, let's look at multiplication using the '*' operator.",
            "In C, we multiply like this: int product = 4 * 5;",
            "Try writing a multiplication statement on your own!"
        };

        private int _mulLineIndex;
        private bool _playingMulIntro;

        private readonly string[] _divisionIntroLines = new string[]
        {
            "Finally, let's try division!",
            "In C, we divide using the '/' operator.",
            "For example: int result = 10 / 2;",
            "Now give it a go — write a division statement!"
        };

        private int _divLineIndex;
        private bool _playingDivIntro;

        private readonly string[] _modIntroLines = new string[]
        {
            "Let's learn about the modulo operator: '%'.",
            "It gives you the remainder after division.",
            "For example: int result = 5 % 2;  // result will be 1",
            "Try writing a modulo expression yourself!"
        };

        private string[] _completionLines = new[]
        {
            "Nice work. You can go teach the store people how to use computers now.",
            "My shop is right in town. Come visit when you have time!"
        };

        private int _completionLineIndex;
        private bool _isShowingCompletion;
        private bool _shouldLoadNextScene;

        private void Start()
        {
            string token = PlayerPrefs.GetString("token", "");
            string saveName = PlayerPrefs.GetString("currentSaveName", "");

            Debug.Log($"🔑 当前Token为: {token}");
            Debug.Log($"📂 当前存档名为: {saveName}");
            nextButton.SetActive(true);
            npcDialog.SetActive(true);
            playerDialog.SetActive(false);
            teachingPanel.SetActive(false);
            pausePanel.SetActive(false);
            escButton.onClick.AddListener(showPausePanel);
            backMapButton.onClick.AddListener(loadMap);
            backMenuButton.onClick.AddListener(loadMenu);
            closeButton.onClick.AddListener(closePausePanel);

            ShowNextLine();
        }

        void loadMap(){
            SceneManager.LoadScene("draftMap");
        }
        void loadMenu(){
            SceneManager.LoadScene("MainScene");
        }
        void showPausePanel(){
            pausePanel.SetActive(true);
        }
        void closePausePanel(){
            pausePanel.SetActive(false);
        }

        private void ShowNextLine()
        {
            npcDialog.SetActive(true);
            playerDialog.SetActive(false);
            playerText.text = "";
            nextButton.SetActive(true);

            if (_dialogueIndex < _npcLines.Length + _playerLines.Length)
            {
                switch (_dialogueIndex)
                {
                    case 0:
                        npcText.text = _npcLines[0];
                        break;
                    case 1:
                        npcDialog.SetActive(false);
                        playerDialog.SetActive(true);
                        playerText.text = _playerLines[0];
                        break;
                    case 2:
                        npcDialog.SetActive(true);
                        playerDialog.SetActive(false);
                        npcText.text = _npcLines[1];
                        break;
                    case 3:
                        npcDialog.SetActive(false);
                        playerDialog.SetActive(true);
                        playerText.text = _playerLines[1];
                        break;
                    case 4:
                        npcDialog.SetActive(true);
                        playerDialog.SetActive(false);
                        npcText.text = _npcLines[2];
                        break;
                    case 5:
                        npcText.text = _npcLines[3];
                        break;
                    case 6:
                        npcText.text = _npcLines[4];
                        break;
                    case 7:
                        npcText.text = _npcLines[5];
                        break;
                }

                _dialogueIndex++;

                if (_dialogueIndex == 8)
                {
                    _waitingForInput = true;
                }
            }
        }

        public void OnNextButtonClicked()
        {
            HandleDialogueAdvance(); // 封装原先 Update() 中 Enter 的逻辑
        }

        private void HandleDialogueAdvance()
        {
            if (_playingDoubleIntro)
            {
                ShowNextDoubleLine();
                return;
            }

            if (_playingFloatIntro)
            {
                ShowNextFloatLine();
                return;
            }

            if (_playingAddIntro)
            {
                ShowNextAdditionLine();
                return;
            }

            if (_playingSubIntro)
            {
                ShowNextSubtractionLine();
                return;
            }

            if (_playingMulIntro)
            {
                ShowNextMultiplicationLine();
                return;
            }

            if (_playingDivIntro)
            {
                ShowNextDivisionLine();
                return;
            }

            if (_isPlayingIntro)
            {
                ShowNextTeachingLine();
                return;
            }

            if (!_waitingForInput && !_waitingForSecondInput)
            {
                ShowNextLine();
            }
            else if (_waitingForInput && codeEvaluator.hasSubmitted && !codeEvaluator.inputCorrect)
            {
                npcDialog.SetActive(false);
                playerDialog.SetActive(false);
                teachingPanel.SetActive(true);
                codeEvaluator.hasSubmitted = false;
            }
            else if (_waitingForSecondInput && codeEvaluator.hasSubmitted && !codeEvaluator.inputCorrect)
            {
                npcDialog.SetActive(false);
                playerDialog.SetActive(false);
                teachingPanel.SetActive(true);
                codeEvaluator.hasSubmitted = false;
            }
        }


        private void Update()
        {
            if (demoMode && Input.GetKeyDown(KeyCode.RightArrow))
            {
                Debug.Log("🎮 Demo Mode 跳过题目");
                SkipToNextQuestion();
            }

            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
            {
                if (_isShowingCompletion)
                {
                    ShowNextCompletionLine(); // ✅ 添加这行
                    return; // ✅ 防止同时触发 HandleDialogueAdvance
                }

                HandleDialogueAdvance();
            }

            // ✅ 防止 Demo Mode 跳题后教学面板自动弹出，只有非 Demo 状态时才弹
            // ✅ 第一题 & 非 Demo 模式下才自动弹出教学面板
            if (_waitingForInput && !teachingPanel.activeSelf && !codeEvaluator.hasSubmitted)
            {
                TriggerNextTeachingDialogue(0);
                _waitingForInput = false;
            }
        }


        private void SkipToNextQuestion()
        {
            int currentIndex = codeEvaluator.GetCurrentQuestionIndex();
            int nextIndex = currentIndex + 1;

            if (nextIndex < 11)
            {
                Debug.Log($"🚀 Demo 跳转到题目 {nextIndex}");

                typeof(CodeEvaluator)
                    .GetField("_currentQuestionIndex",
                        System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                    ?.SetValue(codeEvaluator, nextIndex);

                codeEvaluator.PrepareNextQuestionWithoutPanel(nextIndex);
                TriggerNextTeachingDialogue(nextIndex);

                // ✅ 这里是关键：阻止再次触发 index 0
                _waitingForInput = false;
            }
            else
            {
                Debug.Log("✅ Demo 已到最后一题，调用结语");
                codeEvaluator.SendMessage("ShowCompletionDialogue");
            }
        }


        private void TriggerNextTeachingDialogue(int index)
        {
            switch (index)
            {
                case 0:
                    StartTeachingDialogue(new string[]
                    {
                    }, "Declare an int variable and assign it a value. For example: int apple = 3;");
                    break;
                case 1:
                    StartDoubleTeachingDialogue(index);
                    break;
                case 2:
                    StartFloatTeachingDialogue();
                    break;
                case 3:
                    StartAdditionTeachingDialogue();
                    break;
                case 4:
                    StartSubtractionTeachingDialogue();
                    break;
                case 5:
                    StartMultiplicationTeachingDialogue();
                    break;
                case 6:
                    StartDivisionTeachingDialogue();
                    break;
                case 7:
                    StartModuloTeachingDialogue();
                    break;
                case 8:
                    StartIncrementTeachingDialogue();
                    break;
                case 9:
                    StartDecrementTeachingDialogue();
                    break;
                case 10:
                    StartMixedTeachingDialogue();
                    break;
            }
        }

        public void StartCompletionDialogue()
        {
            _isShowingCompletion = true;
            _completionLineIndex = 0;
            ShowNextCompletionLine();
            // 更新进度
            PlayerPrefs.SetString("PreviousScene", "calcuTeaching");
            PlayerPrefs.Save();
            StartCoroutine(codeEvaluator.UpdateProgress(3));
        }

        private void ShowNextCompletionLine()
        {
            if (_completionLineIndex < _completionLines.Length)
            {
                npcDialog.SetActive(true);
                playerDialog.SetActive(false);
                teachingPanel.SetActive(false);
                npcText.text = _completionLines[_completionLineIndex];
                _completionLineIndex++;
            }
            else
            {
                _isShowingCompletion = false;
                _shouldLoadNextScene = true;
                Invoke(nameof(LoadNextScene), 2f); // 给个缓冲时间
            }
        }

        private void LoadNextScene()
        {
            if (_shouldLoadNextScene)
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene("draftMap");
            }
        }


        public void StartDoubleTeachingDialogue(int questionIndex)
        {
            _playingDoubleIntro = true;
            _doubleLineIndex = 0;
            ShowNextDoubleLine();
        }

        private void ShowNextDoubleLine()
        {
            if (_doubleLineIndex < _doubleIntroLines.Length)
            {
                npcDialog.SetActive(true);
                playerDialog.SetActive(false);
                npcText.text = _doubleIntroLines[_doubleLineIndex];
                nextButton.SetActive(true);
                _doubleLineIndex++;
            }
            else
            {
                // 完成 double 教学对话，切换到输入
                _playingDoubleIntro = false;
                npcDialog.SetActive(false);
                playerDialog.SetActive(false);
                teachingPanel.SetActive(true);
                questionText.text =
                    "Please declare a double variable and assign it a decimal number. (e.g. double price = 3.14;)";
                nextButton.SetActive(false);
                codeEvaluator.hasSubmitted = false;
                codeEvaluator.inputCorrect = false;
                codeInput.text = "";
                _waitingForSecondInput = true;
            }
        }

        public void StartFloatTeachingDialogue()
        {
            _playingFloatIntro = true;
            _floatLineIndex = 0;
            ShowNextFloatLine();
        }

        private void ShowNextFloatLine()
        {
            if (_floatLineIndex < _floatIntroLines.Length)
            {
                npcDialog.SetActive(true);
                playerDialog.SetActive(false);
                npcText.text = _floatIntroLines[_floatLineIndex];
                nextButton.SetActive(true);
                _floatLineIndex++;
            }
            else
            {
                _playingFloatIntro = false;
                npcDialog.SetActive(false);
                playerDialog.SetActive(false);
                teachingPanel.SetActive(true);
                questionText.text =
                    "Please declare a float variable and assign it a value like 2.5f. (e.g. float distance = 2.5f;)";
                nextButton.SetActive(false);
                codeEvaluator.hasSubmitted = false;
                codeEvaluator.inputCorrect = false;
                codeInput.text = "";
            }
        }

        public void StartAdditionTeachingDialogue()
        {
            _playingAddIntro = true;
            _addLineIndex = 0;
            ShowNextAdditionLine();
        }

        private void ShowNextAdditionLine()
        {
            if (_addLineIndex < _additionIntroLines.Length)
            {
                npcDialog.SetActive(true);
                playerDialog.SetActive(false);
                npcText.text = _additionIntroLines[_addLineIndex];
                nextButton.SetActive(true);
                _addLineIndex++;
            }
            else
            {
                _playingAddIntro = false;
                npcDialog.SetActive(false);
                playerDialog.SetActive(false);
                teachingPanel.SetActive(true);
                questionText.text = "Try declaring a variable and assigning it a sum. (e.g. int sum = 2 + 3;)";
                nextButton.SetActive(false);
                codeEvaluator.hasSubmitted = false;
                codeEvaluator.inputCorrect = false;
                codeInput.text = "";
            }
        }

        public void StartSubtractionTeachingDialogue()
        {
            _playingSubIntro = true;
            _subLineIndex = 0;
            ShowNextSubtractionLine();
        }

        private void ShowNextSubtractionLine()
        {
            if (_subLineIndex < _subtractionIntroLines.Length)
            {
                npcDialog.SetActive(true);
                playerDialog.SetActive(false);
                npcText.text = _subtractionIntroLines[_subLineIndex];
                nextButton.SetActive(true);
                _subLineIndex++;
            }
            else
            {
                _playingSubIntro = false;
                npcDialog.SetActive(false);
                playerDialog.SetActive(false);
                teachingPanel.SetActive(true);
                questionText.text =
                    "Give it a try! Declare a variable and assign it a subtraction result. (e.g. int result = 7 - 2;)";
                nextButton.SetActive(false);
                codeEvaluator.hasSubmitted = false;
                codeEvaluator.inputCorrect = false;
                codeInput.text = "";
            }
        }

        public void StartMultiplicationTeachingDialogue()
        {
            _playingMulIntro = true;
            _mulLineIndex = 0;
            ShowNextMultiplicationLine();
        }

        private void ShowNextMultiplicationLine()
        {
            if (_mulLineIndex < _multiplicationIntroLines.Length)
            {
                npcDialog.SetActive(true);
                playerDialog.SetActive(false);
                npcText.text = _multiplicationIntroLines[_mulLineIndex];
                nextButton.SetActive(true);
                _mulLineIndex++;
            }
            else
            {
                _playingMulIntro = false;
                npcDialog.SetActive(false);
                playerDialog.SetActive(false);
                teachingPanel.SetActive(true);
                questionText.text = "Try writing a multiplication statement on your own! (e.g. int product = 4 * 5;)";
                nextButton.SetActive(false);
                codeEvaluator.hasSubmitted = false;
                codeEvaluator.inputCorrect = false;
                codeInput.text = "";
            }
        }

        public void StartDivisionTeachingDialogue()
        {
            _playingDivIntro = true;
            _divLineIndex = 0;
            ShowNextDivisionLine();
        }

        private void ShowNextDivisionLine()
        {
            if (_divLineIndex < _divisionIntroLines.Length)
            {
                npcDialog.SetActive(true);
                playerDialog.SetActive(false);
                npcText.text = _divisionIntroLines[_divLineIndex];
                nextButton.SetActive(true);
                _divLineIndex++;
            }
            else
            {
                _playingDivIntro = false;
                npcDialog.SetActive(false);
                playerDialog.SetActive(false);
                teachingPanel.SetActive(true);
                questionText.text = "Now give it a go — write a division statement! (e.g. int result = 10 / 2;)";
                nextButton.SetActive(false);
                codeEvaluator.hasSubmitted = false;
                codeEvaluator.inputCorrect = false;
                codeInput.text = "";
            }
        }

        public void StartModuloTeachingDialogue()
        {
            StartTeachingDialogue(_modIntroLines,
                "Try writing a modulo expression yourself! (e.g. int result = 5 % 2;)");
        }

        public void StartIncrementTeachingDialogue()
        {
            string[] incIntroLines = new string[]
            {
                "C also supports the increment operator: '++'.",
                "It increases an integer by 1.",
                "For example: int a = 10; a++;",
                "Try writing a line of code that increments a variable."
            };
            string npcQuestionText = "If you have: int a = 10;Try writing a line of code that increments a variable.";
            StartTeachingDialogue(incIntroLines, npcQuestionText);
        }

        public void StartDecrementTeachingDialogue()
        {
            string[] decIntroLines = new string[]
            {
                "Similarly, the decrement operator '--' decreases a value by 1.",
                "If you have: int a = 10; a--; ",
                "Write a line of code that uses the decrement operator."
            };
            string npcQuestionText = "If you have: int a = 10; Write a line of code that uses the decrement operator.";
            StartTeachingDialogue(decIntroLines, npcQuestionText);
        }

        private readonly string[] _mixedIntroLines = new string[]
        {
            "Excellent! You've learned variables and operators.",
            "Now, let's combine them to compute more complex expressions.",
            "For example: int apple = 3; int pear = 4; int sum = apple + pear;",
            "Try writing such a statement yourself!"
        };

        public void StartMixedTeachingDialogue()
        {
            StartTeachingDialogue(_mixedIntroLines,
                "Write a combined expression using variables and +. (e.g. int apple = 3; int pear = 4; int sum = apple + pear;)");
        }

        public void StartTeachingDialogue(string[] introLines, string npcQuestionText)
        {
            _currentIntroLines = introLines;
            _currentLineIndex = 0;
            _currentQuestionText = npcQuestionText;
            _isPlayingIntro = true;
            _waitingForInput = true; // ✅ 这句是关键
            ShowNextTeachingLine();
        }

        private void ShowNextTeachingLine()
        {
            Debug.Log("当前教学行索引：" + _currentLineIndex); // 调试用
            if (_currentLineIndex < _currentIntroLines.Length)
            {
                npcDialog.SetActive(true);
                playerDialog.SetActive(false);
                npcText.text = _currentIntroLines[_currentLineIndex];
                nextButton.SetActive(true);
                _currentLineIndex++;
            }
            else
            {
                _isPlayingIntro = false;
                npcDialog.SetActive(false);
                playerDialog.SetActive(false);
                teachingPanel.SetActive(true);
                questionText.text = _currentQuestionText;
                nextButton.SetActive(false);

                // 反馈计算结果
                if (codeEvaluator.inputCorrect)
                {
                    npcText.text = $"Well done! {codeEvaluator.GetVariableValue()}"; // 显示计算结果
                }

                codeEvaluator.hasSubmitted = false;
                codeEvaluator.inputCorrect = false;
                codeInput.text = "";
            }
        }
    }
}