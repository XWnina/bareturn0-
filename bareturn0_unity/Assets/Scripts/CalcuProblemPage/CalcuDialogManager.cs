using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace CalcuProblemPage
{
    public class CalcuDialogManager : MonoBehaviour
    {
        [Header("UI References")]
        public GameObject playerDialogBox;
        public Text playerText;

        public GameObject npcDialogBox;
        public Text npcText;

        public float defaultDisplayTime = 2f;

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
    }
}