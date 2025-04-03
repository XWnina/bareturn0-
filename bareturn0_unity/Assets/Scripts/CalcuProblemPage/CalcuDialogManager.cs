using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace CalcuProblemPage
{
    public class CalcuDialogManager : MonoBehaviour
    {
        [Header("UI References")]
        public GameObject playerDialogBox;
        public TMP_Text playerText;

        public GameObject npcDialogBox;
        public TMP_Text npcText;

        public float defaultDisplayTime = 2f;

        // === 新增 ===
        private readonly Queue<string> _dialogQueue = new();
        private bool _isWaitingForEnter = false;

        void Update()
        {
            // 玩家按下 Enter 键继续
            if (_isWaitingForEnter && Input.GetKeyDown(KeyCode.Return))
            {
                ShowNextLine();
            }
        }

        public void ShowPlayerLine(string line)
        {
            npcDialogBox.SetActive(false);
            playerDialogBox.SetActive(true);
            playerText.text = line;
        }

        public void ShowNpcLine(string line)
        {
            playerDialogBox.SetActive(false);
            npcDialogBox.SetActive(true);
            npcText.text = line;
        }

        public void HideAllDialogs()
        {
            playerDialogBox.SetActive(false);
            npcDialogBox.SetActive(false);
        }

        public IEnumerator ShowPlayerLineWithDelay(string line, float duration = -1f)
        {
            ShowPlayerLine(line);
            yield return new WaitForSeconds(duration > 0 ? duration : defaultDisplayTime);
            HideAllDialogs();
        }

        public IEnumerator ShowNpcLineWithDelay(string line, float duration = -1f)
        {
            ShowNpcLine(line);
            yield return new WaitForSeconds(duration > 0 ? duration : defaultDisplayTime);
            HideAllDialogs();
        }

        // === 新增：按 Enter 键继续的版本 ===
        public void EnqueueDialogLines(List<string> lines)
        {
            foreach (string line in lines)
            {
                _dialogQueue.Enqueue(line);
            }

            if (!_isWaitingForEnter && _dialogQueue.Count > 0)
            {
                ShowNextLine();
                _isWaitingForEnter = true;
            }
        }

        private void ShowNextLine()
        {
            HideAllDialogs();

            if (_dialogQueue.Count == 0)
            {
                _isWaitingForEnter = false;
                return;
            }

            string nextLine = _dialogQueue.Dequeue();

            if (nextLine.StartsWith("PLAYER:"))
            {
                ShowPlayerLine(nextLine.Replace("PLAYER:", "").Trim());
            }
            else
            {
                ShowNpcLine(nextLine.Replace("NPC:", "").Trim());
            }
        }

        // 是否正在等待玩家按 Enter
        public bool IsDialogPlaying()
        {
            return _isWaitingForEnter;
        }
    }
}