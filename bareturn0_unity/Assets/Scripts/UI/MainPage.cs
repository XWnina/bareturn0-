namespace UI
{
    using UnityEngine;
    using UnityEngine.SceneManagement;
    using UnityEngine.UI;

    public class MainPage : MonoBehaviour
    {
        public Button newGameButton;
        public Button loadingButton; // 添加 LoadScene 按钮

        void Start()
        {
            newGameButton.onClick.AddListener(StartNewGame);
            loadingButton.onClick.AddListener(GoToLoadScene); // 绑定 LoadScene 按钮事件
        }

        void StartNewGame()
        {
            SceneManager.LoadScene("PrintfTeaching"); // 确保该场景在 Build Settings 里
        }

        void GoToLoadScene()
        {
            SceneManager.LoadScene("LoadScene"); // 确保 LoadScene 在 Build Settings 里
        }
    }
}