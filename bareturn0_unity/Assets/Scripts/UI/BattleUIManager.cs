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
    public GameObject battleResultPanel; // �������
    public TextMeshProUGUI resultText; // ʤ��/ʧ���ı�
    public Button returnButton; // ���ز˵���ť
    public Button escButton;
    public GameObject pausePanel;

    [Header("Basic Battle Informations")]
    public TextMeshProUGUI playerHealthText;
    public TextMeshProUGUI playerShieldText;
    public TextMeshProUGUI playerEnergyText;
    public TextMeshProUGUI roundText;
    public Slider PlayerHealthBar;

    [Header("Energy Segment Images (in order)")]
    public List<Image> energySegments;
    public Sprite litSegmentSprite;   // ����ʱ�ĸ����ز�
    public Sprite unlitSegmentSprite; // δ��ʱ�ĸ����ز�

    [Header("Warnings")]
    public TextMeshProUGUI energyWarningText;
    public bool passed = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject); // 避免重复
        }

        // ��ʼ����Energy Warning
        if (energyWarningText != null)
        {
            energyWarningText.gameObject.SetActive(false); 
        }
    }


    // ��ʾ���������㡱��ʾ
    public void ShowEnergyWarning()
    {
        if (energyWarningText == null) return;

        StopAllCoroutines(); // ȷ�������ظ�ִ�ж������
        StartCoroutine(FadeOutWarning());
    }

    //����Ч��
    private IEnumerator FadeOutWarning()
    {
        energyWarningText.gameObject.SetActive(true);
        energyWarningText.alpha = 1; // ������Ϊ�ɼ�

        yield return new WaitForSeconds(0.5f); // ͣ�� 0.5 ��

        // ��������
        float fadeDuration = 0.5f;
        float elapsedTime = 0;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            energyWarningText.alpha = Mathf.Lerp(1, 0, elapsedTime / fadeDuration);
            yield return null;
        }

        energyWarningText.gameObject.SetActive(false); // ��ȫ��ʧ������
    }

    // ��ʾս�����
    public void ShowBattleResult(bool isVictory)
    {
        battleResultPanel.SetActive(true); // ��ʾ���a
        resultText.text = isVictory ? "YOU WIN!!!" : "YOU LOSS...";
        resultText.color = isVictory ? Color.green : Color.red;

        if (isVictory)
        {
            BattleManager.Instance.sendProgress();
            passed = true;
        }
        else {
            passed = false;
        }
        // �������ز˵���ť
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
            // ����ɫ������Ը�����Ҫ����RGBֵ��
            fillImage.color = new Color(0.6f, 0.6f, 0.6f, 1f);
        }
        else
        {
            // ��ɫ
            fillImage.color = Color.red;
        }

        for (int i = 0; i < energySegments.Count; i++)
        {
            if (i < BattleManager.Instance.player.currentEnergy)
            {
                // ����
                energySegments[i].sprite = litSegmentSprite;
            }
            else
            {
                // δ��
                energySegments[i].sprite = unlitSegmentSprite;
            }
        }

    }

    private void Update()
    {
        // ��� BattleManager �Ƿ����
        if (BattleManager.Instance != null)
        {
            // �������Ѫ����ʾ����ǰѪ��/���Ѫ����
            playerHealthText.text = $"{BattleManager.Instance.player.currentHealth}/{BattleManager.Instance.player.maxHealth}";

            // ������һ��ܣ����ף���ʾ
            playerShieldText.text = $"{BattleManager.Instance.player.currentArmor}";

            // �������ʣ��������ʾ
            playerEnergyText.text = $"{BattleManager.Instance.player.currentEnergy}";

            // ���µ�ǰ�غ�����ʾ
            roundText.text = $"Round: {BattleManager.Instance.CurrentRoundNumber}";

            //�������Ѫ����������
            UpdatePlayerUIBar();
        }
    }
}
