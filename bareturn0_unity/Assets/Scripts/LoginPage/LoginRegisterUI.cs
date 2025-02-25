using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using TMPro; // Import TextMeshPro namespace for TMP_InputField support
using System.Text; // Required for encoding JSON data

public class LoginRegisterUI : MonoBehaviour
{
    // Menus
    public GameObject mainMenu;
    public GameObject loginMenu;
    public GameObject registerMenu;

    // Login UI Elements
    public TMP_InputField loginUsernameInput;
    public TMP_InputField loginPasswordInput;
    public TextMeshProUGUI loginPromptText;
    public TextMeshProUGUI loginResponseMessage;

    // Register UI Elements
    public TMP_InputField registerUsernameInput;
    public TMP_InputField registerPasswordInput;
    public TextMeshProUGUI registerPromptText;
    public TextMeshProUGUI registerResponseMessage;

    private string apiBaseURL = "http://localhost:3000/users"; // Backend API URL

    void Start()
    {
        ShowMainMenu(); // Start with the main menu visible
    }

    // Display Main Menu
    public void ShowMainMenu()
    {
        mainMenu.SetActive(true);
        loginMenu.SetActive(false);
        registerMenu.SetActive(false);
    }

    // Display Login Form
    public void ShowLoginMenu()
    {
        mainMenu.SetActive(false);
        loginMenu.SetActive(true);
        registerMenu.SetActive(false);

        loginPromptText.text = "Please input your username and password to login.";
        loginUsernameInput.text = "";
        loginPasswordInput.text = "";
        loginResponseMessage.text = "";
    }

    // Display Register Form
    public void ShowRegisterMenu()
    {
        mainMenu.SetActive(false);
        loginMenu.SetActive(false);
        registerMenu.SetActive(true);

        registerPromptText.text = "Please input your username and password to create an account.";
        registerUsernameInput.text = "";
        registerPasswordInput.text = "";
        registerResponseMessage.text = "";
    }

    // Handle Login Form Submission
    public void SubmitLogin()
    {
        StartCoroutine(LoginRequest());
    }

    // Handle Register Form Submission
    public void SubmitRegister()
    {
        StartCoroutine(RegisterRequest());
    }

    // Coroutine for sending login request
    private IEnumerator LoginRequest()
    {
        // Create JSON payload
        string jsonData = JsonUtility.ToJson(new UserCredentials
        {
            username = loginUsernameInput.text,
            password = loginPasswordInput.text
        });

        UnityWebRequest request = new UnityWebRequest(apiBaseURL + "/login", "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            var jsonResponse = JsonUtility.FromJson<LoginResponse>(request.downloadHandler.text);
            PlayerPrefs.SetString("token", jsonResponse.token);
            loginResponseMessage.text = "Login successful!";
            yield return new WaitForSeconds(3);
            UnityEngine.SceneManagement.SceneManager.LoadScene("MainScene");
        }
        else
        {
            var errorResponse = JsonUtility.FromJson<ErrorResponse>(request.downloadHandler.text);
            loginResponseMessage.text = "Login failed: " + errorResponse.error;
        }

    }


    // Coroutine for sending register request
    private IEnumerator RegisterRequest()
    {
        // Create JSON payload
        string jsonData = JsonUtility.ToJson(new UserCredentials
        {
            username = registerUsernameInput.text,
            password = registerPasswordInput.text
        });

        UnityWebRequest request = new UnityWebRequest(apiBaseURL + "/register", "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            registerResponseMessage.text = "Registration successful!";
            yield return new WaitForSeconds(3);
            UnityEngine.SceneManagement.SceneManager.LoadScene("MainScene");
        }
        else
        {
            var errorResponse = JsonUtility.FromJson<ErrorResponse>(request.downloadHandler.text);
            registerResponseMessage.text = "Registration failed: " + errorResponse.error;
        }
    }


    // JSON class for sending user credentials
    [System.Serializable]
    private class UserCredentials
    {
        public string username;
        public string password;
    }

    // JSON class for handling login token response
    [System.Serializable]
    private class LoginResponse
    {
        public string token;
    }

    // JSON class for handling error response
    [System.Serializable]
    private class ErrorResponse
    {
        public string error;
    }

}
