using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class BattleUIManager : MonoBehaviour
{
    public static BattleUIManager Instance;

    [Header("Battle Result UI")]
    public GameObject battleResultPanel;
    public TextMeshProUGUI resultText;
    public Button returnButton;

    [Header("Pause UI")]
    public Button escButton;
    public GameObject pausePanel;
    public Button closeButton;
    public Button backToMapButton;
    public Button backToMainButton;

    [Header("Basic Battle Informations")]
    public TextMeshProUGUI playerHealthText;
    public TextMeshProUGUI playerShieldText;
    public TextMeshProUGUI playerEnergyText;
    public TextMeshProUGUI roundText;
    public Slider PlayerHealthBar;
    public GameObject buffPanel;
    public BuffUIPrefab buffPrefab;

    [Header("Energy Segment Images (in order)")]
    public List<Image> energySegments;
    public Sprite litSegmentSprite;
    public Sprite unlitSegmentSprite;

    [Header("Warnings")]
    public TextMeshProUGUI energyWarningText;

    private void Awake()
    {
        Instance = this;

        // Energy Warning
        if (energyWarningText != null)
        {
            energyWarningText.gameObject.SetActive(false); 
        }
    }

    private void Start()
    {
        pausePanel.SetActive(false);
        escButton.onClick.AddListener(OnEscClicked);
    }

    public void OnEscClicked()
    {
        pausePanel.SetActive(true);
        closeButton.onClick.RemoveAllListeners();
        backToMapButton.onClick.RemoveAllListeners();
        backToMainButton.onClick.RemoveAllListeners();

        closeButton.onClick.AddListener(OnCloseClicked);
        backToMapButton.onClick.AddListener(ReturnToMap);
        backToMainButton.onClick.AddListener(ReturnToMain);
    }

    public void ReturnToMain()
    {
        Debug.Log("Returning to map...");
        SceneManager.LoadScene("MainScene");
    }

    public void OnCloseClicked()
    {
        pausePanel.SetActive(false);
    }
    public void ShowEnergyWarning()
    {
        if (energyWarningText == null) return;

        StopAllCoroutines();
        StartCoroutine(FadeOutWarning());
    }


    private IEnumerator FadeOutWarning()
    {
        energyWarningText.gameObject.SetActive(true);
        energyWarningText.alpha = 1;

        yield return new WaitForSeconds(0.5f);


        float fadeDuration = 0.5f;
        float elapsedTime = 0;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            energyWarningText.alpha = Mathf.Lerp(1, 0, elapsedTime / fadeDuration);
            yield return null;
        }

        energyWarningText.gameObject.SetActive(false);
    }


    public void ShowBattleResult(bool isVictory)
    {
        battleResultPanel.SetActive(true);
        resultText.text = isVictory ? "YOU WIN!!!" : "YOU LOSS...";
        resultText.color = isVictory ? Color.green : Color.red;

        if (isVictory)
        {
            BattleManager.Instance.sendProgress();
            BattleResultManager.Instance.passed = true;
            BattleResultManager.Instance.CheckPerfectPass();
        }
        else {
            BattleResultManager.Instance.passed = false;
        }

        returnButton.onClick.RemoveAllListeners();
        returnButton.onClick.AddListener(ReturnToMap);
    }

    public void ReturnToMap()
    {
        Debug.Log("Returning to map...");
        SceneManager.LoadScene("draftMap");
    }

    public void UpdatePlayerUIBar()   
    {
        PlayerHealthBar.maxValue = BattleManager.Instance.player.maxHealth;
        PlayerHealthBar.value = BattleManager.Instance.player.currentHealth;

        Image fillImage = PlayerHealthBar.fillRect.GetComponent<Image>();
        if (BattleManager.Instance.player.currentArmor > 0)
        {
            fillImage.color = new Color(0.6f, 0.6f, 0.6f, 1f);
        }
        else
        {
            fillImage.color = Color.red;
        }

        for (int i = 0; i < energySegments.Count; i++)
        {
            if (i < BattleManager.Instance.player.currentEnergy)
            {
                energySegments[i].sprite = litSegmentSprite;
            }
            else
            {
                energySegments[i].sprite = unlitSegmentSprite;
            }
        }

    }

    public void updateBuffUI(int poisonLayers, int burnLayers, int bleedlayers, int sharpnessLayers)
    {
        foreach (Transform child in buffPanel.transform)
        {
            Destroy(child.gameObject);
        }

        if (poisonLayers > 0)
        {
            BuffUIPrefab newBuff = Instantiate(buffPrefab, buffPanel.transform);
            newBuff.buffImage.sprite = newBuff.poisonSprite;
            newBuff.BuffCount.text = poisonLayers.ToString();
        }

        if (burnLayers > 0)
        {
            BuffUIPrefab newBuff = Instantiate(buffPrefab, buffPanel.transform);
            newBuff.buffImage.sprite = newBuff.burnSprite;
            newBuff.BuffCount.text = burnLayers.ToString();
        }

        if (bleedlayers > 0)
        {
            BuffUIPrefab newBuff = Instantiate(buffPrefab, buffPanel.transform);
            newBuff.buffImage.sprite = newBuff.bleedSprite;
            newBuff.BuffCount.text = bleedlayers.ToString();
        }

        if (sharpnessLayers > 0)
        {
            BuffUIPrefab newBuff = Instantiate(buffPrefab, buffPanel.transform);
            newBuff.buffImage.sprite = newBuff.sharpnessSprite;
            newBuff.BuffCount.text = sharpnessLayers.ToString();
        }

    }

    private void Update()
    {
        if (BattleManager.Instance != null)
        {

            playerHealthText.text = $"{BattleManager.Instance.player.currentHealth}/{BattleManager.Instance.player.maxHealth}";


            playerShieldText.text = $"{BattleManager.Instance.player.currentArmor}";


            playerEnergyText.text = $"{BattleManager.Instance.player.currentEnergy}";


            roundText.text = $"Round: {BattleManager.Instance.CurrentRoundNumber}";

            UpdatePlayerUIBar();
        }
    }
}
