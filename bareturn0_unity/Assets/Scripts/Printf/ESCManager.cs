using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class ESCManager : MonoBehaviour
{
    public GameObject pauseMenu;
    public Button continueButton;
    public Button backButton;
    private bool isPaused = false;
    private DialogManager dialogManager; // 获取 DialogManager

    void Start()
    {
        dialogManager = FindObjectOfType<DialogManager>(); // 获取 DialogManager 组件

        if (continueButton != null)
            continueButton.onClick.AddListener(ContinueGame);

        if (backButton != null)
            backButton.onClick.AddListener(BackToMainPage);

        pauseMenu.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }

        // 拦截鼠标点击，确保只有 Continue 和 Back 按钮可以点击
        if (isPaused && Input.GetMouseButtonDown(0))
        {
            if (!IsClickOnButton(continueButton) && !IsClickOnButton(backButton))
            {
                Debug.Log("点击无效，必须点击 Continue 或 Back");
                return;
            }
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        pauseMenu.SetActive(isPaused);

        if (dialogManager != null)
        {
            dialogManager.isPaused = isPaused; // 同步暂停状态到 DialogManager
        }

        if (isPaused)
        {
            Debug.Log("游戏暂停...");
            Time.timeScale = 0f;
            pauseMenu.transform.SetAsLastSibling();
        }
        else
        {
            Debug.Log("游戏恢复...");
            Time.timeScale = 1f;
        }
    }

    void ContinueGame()
    {
        isPaused = false;
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;

        if (dialogManager != null)
        {
            dialogManager.isPaused = false; // 恢复对话推进
        }
    }

    void BackToMainPage()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainScene");
    }

    bool IsClickOnButton(Button button)
    {
        if (button == null) return false;

        PointerEventData eventData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        foreach (var result in results)
        {
            if (result.gameObject == button.gameObject)
                return true;
        }

        return false;
    }
}
