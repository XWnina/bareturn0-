using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class MatchCard : MonoBehaviour
{
    public enum CardKind { Value, Type }
    public CardKind kind;
    public string content;
    public TMP_Text label; // Assign via Inspector

    private void Awake()
    {
        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    public void Initialize(string text, CardKind cardKind)
    {
        content = text;
        kind = cardKind;
        label.text = text;
    }

    private void OnClick()
    {
        MatchLogicManager.Instance.OnCardClicked(this);
    }

    // Call this when a correct match is made to hide the card
    public void Hide()
    {
        gameObject.SetActive(false);
    }

    // Optional alternative: completely remove the card from the scene
    public void Remove()
    {
        Destroy(gameObject);
    }
}
