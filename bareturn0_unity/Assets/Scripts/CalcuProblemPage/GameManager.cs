using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.Networking;

namespace CalcuProblemPage
{
    public class GameManager : MonoBehaviour
    {
        [Header("Managers & Controllers")] public CalcuDialogManager dialogManager;
        public NpcController npcController;
        public QuestionManager questionManager;

        [Header("UI References")] public GameObject teachingPanel;
        public TMP_Text questionText;

        private string _token;
        private string _saveName;


        void Start()
        {
            _token = PlayerPrefs.GetString("token", "");
            _saveName = PlayerPrefs.GetString("currentSaveName", "");

            Debug.Log("🔐 Token: " + _token);
            Debug.Log("📂 SaveName: " + _saveName);

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
                yield return StartCoroutine(ShowQuestionWithNpcDialog(question));
            }
            else
            {
                List<string> endLines = new()
                {
                    "NPC: Thanks! You helped me complete all the tasks.",
                    "NPC: Feel free to stop by my store anytime. You're always welcome here."
                };

                dialogManager.EnqueueDialogLines(endLines);
                yield return new WaitUntil(() => dialogManager.IsDialogPlaying() == false);

                // 等结束语播放完后再跳转或更新进度
                StartCoroutine(UpdateProgressAndGoToMap(4));
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

        private IEnumerator UpdateProgressAndGoToMap(int progress)
        {
            if (string.IsNullOrEmpty(_token) || string.IsNullOrEmpty(_saveName))
            {
                Debug.LogError("❌ Token 或 SaveName 缺失，无法更新进度！");
                yield break;
            }

            string url = $"http://localhost:3000/savefiles/{_saveName}/updateProgress";
            string jsonData = JsonUtility.ToJson(new ProgressWrapper(progress));
            Debug.Log("📤 正在更新进度：" + jsonData);

            using (UnityWebRequest request = UnityWebRequest.Put(url, jsonData))
            {
                request.method = "PUT";
                request.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(jsonData));
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");
                request.SetRequestHeader("Authorization", "Bearer " + _token);

                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    Debug.Log("✅ Progress updated: " + request.downloadHandler.text);
                }
                else
                {
                    Debug.LogError("❌ Failed to update progress: " + request.error);
                }
            }

            // 跳转场景
            Invoke(nameof(LoadDraftMapScene), 2f);
        }

        private void LoadDraftMapScene()
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("draftMap");
        }

        [Serializable]
        private class ProgressWrapper
        {
            public int progress;

            public ProgressWrapper(int p)
            {
                progress = p;
            }
        }
    }
}