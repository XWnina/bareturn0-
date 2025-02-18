using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Debug = UnityEngine.Debug;

public class ReviewDIalogManager : MonoBehaviour
{
    public Button reviewButton;

    private bool isPaused = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        reviewButton.onClick.AddListener(showDialog);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void showDialog()
    {

    }
}
