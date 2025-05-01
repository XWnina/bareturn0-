using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace JumpGame
{
    public class PauseMenuController : MonoBehaviour
    {
        public GameObject pausePanel;
        public Button closeButton;
        public Button escButton;
        public Button backMapButton;
        public Button backMenuButton;

        private void Start()
        {
            pausePanel.SetActive(false);
            escButton.onClick.AddListener(ShowPausePanel);
            closeButton.onClick.AddListener(ClosePausePanel);
            backMapButton.onClick.AddListener(LoadMap);
            backMenuButton.onClick.AddListener(LoadMenu);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                ShowPausePanel();
            }
        }

        public void ShowPausePanel()
        {
            pausePanel.SetActive(true);
        }

        public void ClosePausePanel()
        {
            pausePanel.SetActive(false);
        }

        public static void LoadMap()
        {
            SceneManager.LoadScene("draftMap");
        }

        public static void LoadMenu()
        {
            SceneManager.LoadScene("MainScene");
        }
    }
}