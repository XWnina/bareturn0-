using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace CalcuProblemPage
{
    public enum CodeErrorType
    {
        None,
        SyntaxError,
        UndeclaredVariable,
        HardcodedConstant
    }

    public class AnswerChecker : MonoBehaviour
    {
        [Header("References")] public QuestionManager questionManager;
        public CalcuDialogManager dialogManager;
        public GameManager gameManager;
        public GameObject teachingPanel;
        public TMP_InputField inputField;

        [Header("Error UI")] public GameObject errorPanel;
        public TMP_Text errorText;

        [Header("Settings")] public double tolerance = 1.0; // ← double 精度

        private readonly CodeEvaluator _evaluator = new();
        private bool _isShowingErrorPanel;
        private bool _finished = false;


        void Update()
        {
            if (_isShowingErrorPanel && Input.GetKeyDown(KeyCode.Return))
            {
                errorPanel.SetActive(false);
                if (!_finished)
                {
                    teachingPanel.SetActive(true);
                    _isShowingErrorPanel = false;
                }
            }
            if (teachingPanel.activeSelf)
            {
                if (dialogManager.nextButton != null && dialogManager.nextButton.activeSelf)
                {
                    dialogManager.nextButton.SetActive(false);
                }
            }

            if (teachingPanel.activeSelf &&
                (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) &&
                Input.GetKeyDown(KeyCode.Return))
            {
                StartCoroutine(CheckAnswer());
            }
            

            // ✅ DEMO 模式：按 → 键跳过当前题目
            if (questionManager.demoMode && Input.GetKeyDown(KeyCode.RightArrow))
            {
                teachingPanel.SetActive(false);
                inputField.text = "";
                questionManager.MoveToNextQuestion();

                if (questionManager.HasMoreQuestions())
                {
                    StartCoroutine(gameManager.ShowNextQuestion());
                }
                else
                {
                    _finished = true;
                    StartCoroutine(gameManager.ShowNextQuestion()); // 最后自动触发结语和跳转
                }
            }
        }


        public void CheckAnswerWrapper()
        {
            StartCoroutine(CheckAnswer()); // ✅ Unity Button 调用它
        }

        public IEnumerator CheckAnswer()
        {
            string userCode = inputField.text.Trim();

            Debug.Log("=== User code ===");
            Debug.Log(userCode);

            if (string.IsNullOrWhiteSpace(userCode) || !userCode.Contains("="))
            {
                errorText.text = "You haven't written any valid code. Please try again.";
                errorPanel.SetActive(true);
                _isShowingErrorPanel = true;
                yield break;
            }


            bool success = _evaluator.TryEvaluate(userCode, out double userAnswer, out CodeErrorType errorType);

            if (!success)
            {
                string errorMsg = errorType switch
                {
                    CodeErrorType.UndeclaredVariable => "You seem to be using a variable that wasn't declared.",
                    CodeErrorType.HardcodedConstant => "Your final result looks hardcoded. Try using variables.",
                    CodeErrorType.SyntaxError => "I couldn't understand your code. Please check the syntax.",
                    _ => "Something went wrong in your code."
                };

                errorText.text = errorMsg;
                errorPanel.SetActive(true);
                _isShowingErrorPanel = true;
                yield break;
            }

            double correctAnswer = questionManager.GetCurrentAnswer();
            double error = Math.Abs(userAnswer - correctAnswer);
            Debug.Log($"✅ userAnswer: {userAnswer}, correctAnswer: {correctAnswer}");
            Debug.Log($"inaccuracy: {error}, tolerance: {tolerance}, result: {error <= tolerance}");
            double userRounded = Math.Round(userAnswer, 2); 

            if (error <= tolerance)
            {
                teachingPanel.SetActive(false);
                string himHer = questionManager.GetCurrentCharacterGender() == "male" ? "him" : "her";

                string sentence = questionManager.GetCurrentQuestionText().Contains("change should I give")
                    ? $"That's correct! I should give {himHer} {userRounded:F2} coins."
                    : $"That's correct! I should charge {himHer} {userRounded:F2} coins.";

                yield return dialogManager.ShowNpcLineWithDelay(sentence); // ✅ 等 NPC 说完正确提示

                inputField.text = "";
                questionManager.MoveToNextQuestion();

                if (questionManager.HasMoreQuestions())
                {
                    yield return new WaitForSeconds(2f);
                    yield return gameManager.ShowNextQuestion();
                }
                else
                {
                    _finished = true;
                    yield return gameManager.ShowNextQuestion(); // ✅ 最后一题，等结语
                }
            }
            else
            {
                teachingPanel.SetActive(false);

                string retryMessage = $"That's not correct. Your answer is {userRounded:F2}. Try again.";

                yield return StartCoroutine(ShowRetrySequence(retryMessage));
            }
        }


        private IEnumerator ShowRetrySequence(string npcLine)
        {
            if (_finished) yield break;
            teachingPanel.SetActive(false); // ✅ 确保面板关闭（之前你只有外部调用了）

            List<string> lines = new() { $"NPC: {npcLine}" };
            dialogManager.EnqueueDialogLines(lines);
            yield return new WaitUntil(() => dialogManager.IsDialogPlaying() == false);

            if (!_finished)  // ✅ 再次确认没完成才显示教学面板
                teachingPanel.SetActive(true);// ✅ 等说完再打开
        }


        private void ShowNextQuestion()
        {
            StartCoroutine(gameManager.ShowNextQuestion());
        }
    }
}