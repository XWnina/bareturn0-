using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;

public class ESCManager : MonoBehaviour
{
    public GameObject pauseMenu;
    public Button continueButton;
    public Button backButton;

    private bool isPaused = false;

    void Start()
    {
        Debug.Log("PauseMenu: " + (pauseMenu != null ? "OK" : "NULL"));
        Debug.Log("ContinueButton: " + (continueButton != null ? "OK" : "NULL"));
        Debug.Log("BackButton: " + (backButton != null ? "OK" : "NULL"));

        if (pauseMenu == null)
        {
            Debug.LogError("ERROR: PauseMenu is NOT assigned in the Inspector!");
        }
        if (continueButton == null)
        {
            Debug.LogError("ERROR: ContinueButton is NOT assigned in the Inspector!");
        }
        if (backButton == null)
        {
            Debug.LogError("ERROR: BackButton is NOT assigned in the Inspector!");
        }

        pauseMenu.SetActive(false);  // 确保开始时隐藏

        // 绑定按钮点击事件
        continueButton.onClick.AddListener(ContinueGame);
        backButton.onClick.AddListener(BackToMainPage);
    }


    void Update()
    {
        Debug.Log("ESCManager Update is running...");
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("ESC Pressed!");
            TogglePause();
        }
    }


    void TogglePause()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            Debug.Log("Game Paused!");
            pauseMenu.SetActive(true);  // 显示暂停菜单
            Time.timeScale = 0f;  // 暂停游戏
        }
        else
        {
            ContinueGame();
        }
    }

    void ContinueGame()
    {
        Debug.Log("Continue Button Clicked!");
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    void BackToMainPage()
    {
        Debug.Log("Back to Main Page Clicked!");
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainPage");
    }
}
