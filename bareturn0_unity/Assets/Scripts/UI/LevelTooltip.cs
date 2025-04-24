using UnityEngine;
using TMPro;

public class LevelTooltip : MonoBehaviour
{
    public static LevelTooltip Instance;

    public GameObject panel;
    public TextMeshProUGUI tooltipText;

    void Awake()
    {
        Instance = this;
        panel.SetActive(false);
    }

    public void ShowTooltip(string text, Vector3 screenPosition)
    {
        tooltipText.text = text;

        // 鼠标位置转换为世界空间的点
        Ray ray = Camera.main.ScreenPointToRay(screenPosition);

        // 把 Panel 放到鼠标前方一点的地方
        Vector3 worldPos = ray.origin + ray.direction * 10f; // 10是距离canvas的距离，根据你的摄像机调整
        panel.transform.position = worldPos;

        panel.SetActive(true);
    }


    public void HideTooltip()
    {
        panel.SetActive(false);
    }
}
