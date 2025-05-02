using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
public class BackToSceneManager : MonoBehaviour
{
    public Button ChangeSaveFileButton;
    void Start()
    {
        ChangeSaveFileButton.onClick.AddListener(LoadingScene);
    }

    public void LoadingScene()
{
    SceneManager.LoadScene("MainScene");
}
}
