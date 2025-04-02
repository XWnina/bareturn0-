using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace CalcuProblemPage
{
    public class AnswerChecker : MonoBehaviour
    {
        [Header("References")]
        public QuestionManager questionManager;
        public CalcuDialogManager dialogManager;
        public GameManager gameManager; // 如果你想答对就继续下一题
        public GameObject teachingPanel;
        public TMP_InputField inputField;

        [Header("Settings")]
        public float tolerance = 0.01f;

        private CodeEvaluator evaluator = new();

        /// <summary>
        /// Called when the player submits their answer.
        /// </summary>
        private bool awaitingRetry = false;

        public void CheckAnswer()
        {
            string userCode = inputField.text.Trim();

            Debug.Log("=== 用户输入代码 ===");
            Debug.Log(userCode);

            if (string.IsNullOrEmpty(userCode))
            {
                StartCoroutine(dialogManager.ShowNpcLineWithDelay("Please enter your code answer."));
                return;
            }

            bool success = evaluator.TryEvaluate(userCode, out double userAnswer);

            if (!success)
            {
                StartCoroutine(dialogManager.ShowNpcLineWithDelay("I couldn't understand your code. Check for syntax or variable errors."));
                return;
            }

            float correctAnswer = questionManager.GetCurrentAnswer();
            Debug.Log($"✅ 用户答案: {userAnswer}, 正确答案: {correctAnswer}");

            if (Mathf.Abs((float)userAnswer - correctAnswer) <= tolerance)
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
                StartCoroutine(ShowRetrySequence());
            }


        }
       
        
        private IEnumerator ShowRetrySequence()
        {
            List<string> lines = new() { "NPC: That's not correct. Try again." };
            dialogManager.EnqueueDialogLines(lines);

            yield return new WaitUntil(() => dialogManager.IsDialogPlaying() == false);

            teachingPanel.SetActive(true); // ✅ 自动弹出面板，无需再按 Enter
        }






        private void ShowNextQuestion()
        {
            gameManager.ShowNextQuestion(); // 调用 GameManager 的函数
        }
    }
}
