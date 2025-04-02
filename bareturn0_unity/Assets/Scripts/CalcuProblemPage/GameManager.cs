using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using CalcuProblemPage;
using UnityEngine.Serialization;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("Managers & Controllers")]
    public CalcuDialogManager dialogManager;
    public NpcController npcController;
    public QuestionManager questionManager;

    [Header("UI References")]
    public GameObject teachingPanel;
    public TMP_Text questionText;

    void Start()
    {
        StartCoroutine(StartQuestSequence());
    }

    private IEnumerator StartQuestSequence()
    {
        // 第一段只有玩家说话 → 播完后再让 NPC 出场
        List<string> opening = new()
        {
            "PLAYER: Is this the store?",
            "PLAYER: Hello? Anyone here?"
        };
        dialogManager.EnqueueDialogLines(opening);

        // ✅ 等玩家按 Enter 播放完前两句
        yield return new WaitUntil(() => dialogManager.IsDialogPlaying() == false);

        // ✅ NPC 走进来
        npcController.WalkIn();
        yield return new WaitUntil(() => npcController.HasReachedTarget());
        npcController.PlayIdle();

        // 接下来继续完整对话
        List<string> rest = new()
        {
            "NPC: Oh, you're new in town?",
            "PLAYER: Yeah, Who are you?",
            "NPC: I'm Alibaba, a merchant, I run this shop.",
            "PLAYER: Oh, Mr. Smith said he finished your computer and asked me to show you how to use it.",
            "NPC: Oh, perfect timing! I was just about to battle with my numbers for today.",
            "NPC: Could you help me do it using the computer?",
            "PLAYER: Of course."
        };
        dialogManager.EnqueueDialogLines(rest);

        // ✅ 再等玩家播完这些句子
        yield return new WaitUntil(() => dialogManager.IsDialogPlaying() == false);

        // 小停顿，然后进入第一题
        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(ShowNextQuestion());
    }

    public IEnumerator ShowNextQuestion()
    {
        if (questionManager.HasMoreQuestions())
        {
            string question = questionManager.GetCurrentQuestionText();
            yield return StartCoroutine(ShowQuestionWithNpcDialog(question)); // ✅ 等待题目对话播放完
        }
        else
        {
            yield return dialogManager.ShowNpcLineWithDelay("Thanks! You helped me complete all the tasks.");
        }
    }

    private IEnumerator ShowQuestionWithNpcDialog(string question)
    {
        npcController.PlayIdle(); 
        yield return new WaitForSeconds(0.2f);

        // ✅ 把题目塞进对话队列，等待玩家按 Enter 播放完
        List<string> qLines = new()
        {
            $"NPC: {question}"
        };

        dialogManager.EnqueueDialogLines(qLines);
        npcController.PlayRead(); // 播放出题动画

        // ✅ 等玩家 Enter 播放完这句题目
        yield return new WaitUntil(() => dialogManager.IsDialogPlaying() == false);

        // ✅ 再显示题面板
        yield return new WaitForSeconds(0.3f);
        questionText.text = question;
        teachingPanel.SetActive(true);
    }

    private IEnumerator ShowNpcLineWithRead(string line, float duration = -1f)
    {
        npcController.PlayRead(); // 播放出题动画
        dialogManager.ShowNpcLine(line); // 显示对话框内容

        float waitTime = duration > 0 ? duration : dialogManager.defaultDisplayTime;
        yield return new WaitForSeconds(waitTime);

        dialogManager.HideAllDialogs();
    }
}
