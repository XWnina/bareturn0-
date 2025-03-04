using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.Networking;
using System.Text;

public class NavigationManager : MonoBehaviour
{
    public Button GotoButton;
    public Button backButton;
    public GameObject nevigationPanel;
    public Button userProfileButton;
    public Button inventoryButton;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        nevigationPanel.SetActive(false);
        GotoButton.onClick.AddListener(showPanel);
        backButton.onClick.AddListener(closePanel);
        userProfileButton.onClick.AddListener(GotoUserProfile);
        inventoryButton.onClick.AddListener(GoToInventory);
    }

    void showPanel()
    {
        nevigationPanel.SetActive(true);
    }
    void closePanel()
    {
        nevigationPanel.SetActive(false);
    }
    void GoToInventory()
    {
        SceneManager.LoadScene("InventoryScene");
    }
    void GotoUserProfile()
    {
        SceneManager.LoadScene("UserProfile");
    }
    //// Update is called once per frame
    //void Update()
    //{
        
    //}
}
