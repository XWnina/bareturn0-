namespace UI // 添加或修改为 UI
{
    using UnityEngine;
    using UnityEngine.SceneManagement;
    using UnityEngine.UI;

    public class MainPage : MonoBehaviour
    {
        public Button newGameButton;

        void Start()
        {
            newGameButton.onClick.AddListener(StartNewGame);
        }

        void StartNewGame()
        {
            SceneManager.LoadScene("PrintfTeaching"); // 确保该场景在 Build Settings 里
        }
    }
}