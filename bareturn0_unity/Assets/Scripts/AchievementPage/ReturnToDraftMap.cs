using UnityEngine;
using UnityEngine.SceneManagement;

public class ReturnToDraftMap : MonoBehaviour
{
    public void LoadDraftMap()
    {
        SceneManager.LoadScene("draftMap"); 
    }
}
