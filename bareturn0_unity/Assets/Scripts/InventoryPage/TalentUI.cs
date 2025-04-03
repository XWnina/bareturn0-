using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class TalentUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("UI Components")]
    public Image image;
    public TextMeshProUGUI nameText;
    public Button button;

    public Sprite ifSprite;
    public Sprite whileSprite;
    public Sprite mathSprite;

    public int count;
    public TextMeshProUGUI countText;

    private Vector3 originalScale; // Store the original scale of the object
    public bool allowHoverEffect = true;
    void Awake()
    {
        originalScale = transform.localScale;
    }

    public void setScroll(string name, int num)
    {
        countText.text = num.ToString();
        nameText.text = name;
        //Debug.Log(name);

        if (name.ToLower() == "math")
        {
            Debug.Log("name is math");
            image.sprite = mathSprite;
        }

        if (name.ToLower() == "if")
        {
            image.sprite = ifSprite;
        }

        if (name.ToLower() == "while")
        {
            image.sprite = whileSprite;
        }
    }

    public string getScrollName()
    {
        return nameText.text;
    }
   
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!allowHoverEffect) return;
        // Scale up the object when the pointer enters
        transform.localScale = originalScale * 1.2f;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!allowHoverEffect) return;

        transform.localScale = originalScale;
    }
}
