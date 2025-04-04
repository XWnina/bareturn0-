using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.Networking;
using System.Text;

public class TownMapManager : MonoBehaviour
{
    public Button backButton;
    public Button BlacksmithShopButton;
    public Button StoreButton;
    public TextMeshProUGUI close1Txt;
    public TextMeshProUGUI close2Txt;
    public Image BlacksmithShopNPC;
    public Image StoreNPC;
    private string _apiBaseUrl = "http://localhost:3000/savefiles";
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        backButton.onClick.AddListener(GoToMainMap);
        BlacksmithShopButton.onClick.AddListener(GoToSimithShop);
        StoreButton.onClick.AddListener(GoToStore);
        close1Txt.gameObject.SetActive(false);
        close2Txt.gameObject.SetActive(false);
        BlacksmithShopNPC.gameObject.SetActive(false);
        StoreNPC.gameObject.SetActive(false);
        StartCoroutine(GetUserProgress());
    }
    void GoToMainMap ()
    {
        SceneManager.LoadScene("draftMap");
    }
    void GoToSimithShop ()
    {
        SceneManager.LoadScene("BlackSmithScene");
    }
    void GoToStore()
    {
        SceneManager.LoadScene("StoreScene");
    }
    // Update is called once per frame
    //void Update()
    //{ 

    //}
    IEnumerator GetUserProgress()
    {
        Debug.Log("see me here asshole");
        string saveName = PlayerPrefs.GetString("currentSaveName", "");
        if (string.IsNullOrEmpty(saveName))
        {
            Debug.LogError("[TownMapManager] ❌ SaveName is missing in PlayerPrefs!");
            yield break;
        }

        string url = $"{_apiBaseUrl}/{saveName}/progress";
        UnityWebRequest request = UnityWebRequest.Get(url);

        string authToken = PlayerPrefs.GetString("token", "");
        request.SetRequestHeader("Authorization", "Bearer " + authToken);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string json = request.downloadHandler.text;
            ProgressResponse progressData = JsonUtility.FromJson<ProgressResponse>(json);

            if (progressData != null)
            {
                int userProgress = progressData.progress;
                Debug.Log($"[TownMapManager] ✅ Progress fetched: {userProgress}");

                if (userProgress == 3)
                {
                    BlacksmithShopButton.interactable = true;
                    close1Txt.gameObject.SetActive(false);
                    BlacksmithShopNPC.gameObject.SetActive(true);
                    StoreButton.interactable = false;        
                    close2Txt.gameObject.SetActive(true);            
                    StoreNPC.gameObject.SetActive(false);
                }
                else if (userProgress >= 4) {
                    BlacksmithShopButton.interactable = true;
                    close1Txt.gameObject.SetActive(false);
                    BlacksmithShopNPC.gameObject.SetActive(true);
                    StoreButton.interactable = true;        
                    close2Txt.gameObject.SetActive(false);            
                    StoreNPC.gameObject.SetActive(true);
                }
                else
                {
                    BlacksmithShopButton.interactable = false;
                    StoreButton.interactable = false;
                    close1Txt.gameObject.SetActive(true);
                    close2Txt.gameObject.SetActive(true);
                    BlacksmithShopNPC.gameObject.SetActive(false);
                    StoreNPC.gameObject.SetActive(false);
                }
            }
            else
            {
                Debug.LogError("[TownMapManager] ❌ Failed to parse JSON response.");
            }
        }
        else
        {
            Debug.LogError($"[TownMapManager] ❌ Error fetching user progress: {request.error}");
        }
    }

    [System.Serializable]
    private class ProgressResponse
    {
        public int progress;
    }
}

