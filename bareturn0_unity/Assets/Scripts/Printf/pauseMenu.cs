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
        SceneManager.LoadScene("MainScene");
    }
}
