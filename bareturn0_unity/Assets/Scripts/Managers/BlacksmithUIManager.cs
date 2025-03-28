using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BlacksmithUIManager : MonoBehaviour
{
    public Button createButton;       // “制作”按钮
    public Button upgradeButton;      // “强化”按钮

    public GameObject createPanel;    // “制作”界面Panel
    public GameObject upgradePanel;   // “强化”界面Panel

    public Button createBackButton;
    public Button EhBackButton;

    public TextMeshProUGUI messageText;

    public EnhancementManager enhancementManager;
    void Start()
    {
        createPanel.SetActive(false);
        upgradePanel.SetActive(false);

        createButton.onClick.AddListener(OnCreateButtonClicked);
        upgradeButton.onClick.AddListener(OnUpgradeButtonClicked);
        createBackButton.onClick.AddListener(OnReturnButtonClicked);
        EhBackButton.onClick.AddListener(OnReturnButtonClicked);

        messageText.text = "Welcome to my store, what do you need?";
    }

    // 点击“制作”按钮时执行
    void OnCreateButtonClicked()
    {
        // 显示制作的Panel
        createPanel.SetActive(true);

        // 隐藏两个按钮
        createButton.gameObject.SetActive(false);
        upgradeButton.gameObject.SetActive(false);

        messageText.text = "What do you want to create?";

        // 这里你还可以写其它逻辑，例如初始化制作界面的数据
       // Debug.Log("进入制作界面");
    }


    // 点击“强化”按钮时执行
    void OnUpgradeButtonClicked()
    {
        // 显示强化的Panel
        upgradePanel.SetActive(true);

        // 隐藏两个按钮
        createButton.gameObject.SetActive(false);
        upgradeButton.gameObject.SetActive(false);

        enhancementManager.SetEnhancement();
        messageText.text = "chooce the card that you want to enhance";

        // 这里你还可以写其它逻辑，例如显示可升级的卡牌列表
        //Debug.Log("进入强化界面");
    }

    public void OnReturnButtonClicked()
    {
        // 隐藏当前Panel
        createPanel.SetActive(false);
        upgradePanel.SetActive(false);

        // 显示两个主按钮
        createButton.gameObject.SetActive(true);
        upgradeButton.gameObject.SetActive(true);
        messageText.text = "Welcome to my store, what do you need?";
    }


    void Update()
    {
        
    }
}
