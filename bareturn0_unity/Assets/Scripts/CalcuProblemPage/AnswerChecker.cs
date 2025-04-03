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
    [Header("References")]
    public QuestionManager questionManager;
    public CalcuDialogManager dialogManager;
    public GameManager gameManager;
    public GameObject teachingPanel;
    public TMP_InputField inputField;

    [Header("Error UI")]
    public GameObject errorPanel;
    public TMP_Text errorText;

    [Header("Settings")]
    public double tolerance = 1.0; // ← double 精度

    private readonly CodeEvaluator _evaluator = new();
    private bool _isShowingErrorPanel;

    void Update()
    {
        if (_isShowingErrorPanel && Input.GetKeyDown(KeyCode.Return))
        {
            errorPanel.SetActive(false);
            teachingPanel.SetActive(true);
            _isShowingErrorPanel = false;
        }

        if (teachingPanel.activeSelf &&
            (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) &&
            Input.GetKeyDown(KeyCode.Return))
        {
            CheckAnswer();
        }
    }

    public void CheckAnswer()
    {
        string userCode = inputField.text.Trim();

        Debug.Log("=== 用户输入代码 ===");
        Debug.Log(userCode);

        if (string.IsNullOrWhiteSpace(userCode) || !userCode.Contains("="))
        {
            StartCoroutine(dialogManager.ShowNpcLineWithDelay("You haven't written any valid code. Please try again."));
            return;
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
            return;
        }

        double correctAnswer = questionManager.GetCurrentAnswer();
        double error = Math.Abs(userAnswer - correctAnswer);
        Debug.Log($"✅ 用户答案: {userAnswer}, 正确答案: {correctAnswer}");
        Debug.Log($"误差: {error}, 容忍误差: {tolerance}, 判断结果: {error <= tolerance}");


        if (error <= tolerance)
        {
            teachingPanel.SetActive(false);
            StartCoroutine(dialogManager.ShowNpcLineWithDelay("That's correct!"));
            questionManager.MoveToNextQuestion();
            inputField.text = "";
            Invoke(nameof(ShowNextQuestion), 2f);
        }
        else
        {
            teachingPanel.SetActive(false);
            StartCoroutine(ShowRetrySequence("That's not correct. Try again."));
        }
    }

    private IEnumerator ShowRetrySequence(string npcLine)
    {
        List<string> lines = new() { $"NPC: {npcLine}" };
        dialogManager.EnqueueDialogLines(lines);
        yield return new WaitUntil(() => dialogManager.IsDialogPlaying() == false);
        teachingPanel.SetActive(true);
    }

    private void ShowNextQuestion()
    {
        StartCoroutine(gameManager.ShowNextQuestion());
    }
}

}
