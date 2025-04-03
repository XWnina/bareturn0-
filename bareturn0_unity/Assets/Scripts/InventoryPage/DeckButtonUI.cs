using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;

public class DeckCardButtonUI : MonoBehaviour
{
    public TextMeshProUGUI cardNameTMP;
    private string cardName;
    private Action<string> onClick;

    public void Setup(string name, Action<string> callback)
    {
        cardName = name;
        cardNameTMP.text = name;
        onClick = callback;

        GetComponent<Button>().onClick.AddListener(() =>
        {
            onClick?.Invoke(cardName);
        });
    }
}
