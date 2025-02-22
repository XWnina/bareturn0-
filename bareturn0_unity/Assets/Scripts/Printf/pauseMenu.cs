using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenuManager : MonoBehaviour
{
    public GameObject pauseMenu;
    public DialogManager dialogManager;
    public Button continueButton;
    public Button mainMenuButton;

    void Start()
    {
        continueButton.onClick.AddListener(ResumeGame);
        mainMenuButton.onClick.AddListener(ReturnToMainMenu);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePauseMenu();
        }
    }

    public void TogglePauseMenu()
    {
        if (pauseMenu.activeSelf)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }

    public void PauseGame()
    {
        pauseMenu.SetActive(true);
        dialogManager.PauseDialog();
    }

    public void ResumeGame()
    {
        pauseMenu.SetActive(false);
        dialogManager.ResumeDialog();
    }

    public void ReturnToMainMenu()
    {
        ResetGameProgress();
        SceneManager.LoadScene("MainScene");
    }
    
    private void ResetGameProgress()
    {
        // **清空聊天记录**
        DialogManager.chatHistory.Clear();

        // **重置对话队列**
        DialogManager.savedDialogQueue.Clear();
        DialogManager.hasSavedState = false;
        DialogManager.isDialogFinished = false;
    }
}
