using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.Networking;
using System.Text;

public class storeTemp : MonoBehaviour
{
    public Button backButton;

    void Start()
    {
        backButton.onClick.AddListener(GoToTown);
    }
     void GoToTown ()
    {
        SceneManager.LoadScene("Town");
    }
}

