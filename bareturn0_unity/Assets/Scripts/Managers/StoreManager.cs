using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.SceneManagement;


public class StoreManager : MonoBehaviour
{
    [Header("References to PlayerInfoLoader & UI")]
    public PlayerInfoLoader playerInfoLoader;
    public TextMeshProUGUI coinsText;
    public TextMeshProUGUI warningText;
    public Button backButton;

    [System.Serializable]
    public class ShopItem
    {
        public string name;              // 物品名称（如 BlankCard / If / While）
        public int cost;                 // 花费金币
        public TextMeshProUGUI costText; // 显示价格文本
        public Button buyButton;         // 对应购买按钮
    }
    [Header("Shop Items")]
    public List<ShopItem> shopItems = new List<ShopItem>();

    [Header("Player Info")]
    [SerializeField] private int playerCoins;
    [SerializeField] private List<string> playerMaterial = new List<string>();

    void Start()
    {
        playerInfoLoader.LoadPlayerCoins(() =>
        {
            playerCoins = playerInfoLoader.coins;
            coinsText.text = playerCoins.ToString();
        });

        playerInfoLoader.GetAllMaterials(() =>
        {
            playerMaterial = new List<string>(playerInfoLoader.materials); // 拷贝材料
        });
        for (int i = 0; i < shopItems.Count; i++)
        {
            int index = i;
            shopItems[i].costText.text = shopItems[i].cost.ToString();
            shopItems[i].buyButton.onClick.AddListener(() => OnBuyButtonClicked(index));
        }

        warningText.gameObject.SetActive(false);
        backButton.onClick.AddListener(onBackClicked);

    }

    public void onBackClicked()
    {
        SceneManager.LoadScene("Town");
    }

    public void OnBuyButtonClicked(int index)
    {
        Debug.Log($"OnBuyButtonClicked 被调用了！ index = {index}");

        var item = shopItems[index];
        if (playerCoins < item.cost)
        {
            StartCoroutine(ShowWarning("Not enough coins!"));
            return;
        }

        // 扣除金币，更新后端
        playerCoins -= item.cost;
        coinsText.text = playerCoins.ToString();
        playerInfoLoader.UpdatePlayerCoin(playerCoins, () =>
        {
            Debug.Log("金币已更新");
        });

        // 更新材料（先统计当前数量 +1）
        int oldCount = CountMaterial(item.name);
        int newCount = oldCount + 1;
        
        if (oldCount == 0)
        {
            playerInfoLoader.CreateNewMaterial(item.name, 1, () =>
            {
                Debug.Log($"{item.name} 材料数量更新为 {newCount}");
                playerMaterial.Add(item.name); // 更新本地
                StartCoroutine(ShowWarning(
                    $"Successfully purchased {item.name}! \n current count: {newCount}",
                    Color.green
                    ));
            });
        }
        else {
            playerInfoLoader.UpdateMaterial(item.name, newCount, () =>
            {
                Debug.Log($"{item.name} 材料数量更新为 {newCount}");
                playerMaterial.Add(item.name); // 更新本地
                StartCoroutine(ShowWarning(
                    $"Successfully purchased {item.name}! \n current count: {newCount}",
                    Color.green
                    ));
            });
        }

        


        Debug.Log($"点击了 {item.name}，价格为 {item.cost}");
    }

    private int CountMaterial(string materialName)
    {
        int count = 0;
        foreach (string mat in playerMaterial)
        {
            if (mat == materialName)
                count++;
        }
        return count;
    }

    private IEnumerator ShowWarning(string message, Color? colorOverride = null)
    {
        warningText.text = message;
        warningText.color = colorOverride ?? Color.red; // 默认为红色

        warningText.alpha = 1;
        warningText.gameObject.SetActive(true);

        yield return new WaitForSeconds(1f);

        float duration = 0.5f;
        float elapsed = 0;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            warningText.alpha = Mathf.Lerp(1, 0, elapsed / duration);
            yield return null;
        }

        warningText.gameObject.SetActive(false);
    }

}
