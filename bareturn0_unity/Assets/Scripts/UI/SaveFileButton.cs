using UnityEngine;
using TMPro;

public class SaveFileButton : MonoBehaviour
{
    public TMP_Text saveNameText;
    public TMP_Text playerNameText;
    public TMP_Text progressText;
    public TMP_Text coinsText;

    public void SetSaveData(string saveName, string playerName, int progress, int coins)
    {
        if (saveNameText != null) saveNameText.text = $"Save: {saveName}";
        if (playerNameText != null) playerNameText.text = $"Player: {playerName}";
        if (progressText != null) progressText.text = $"Progress: {progress}";
        if (coinsText != null) coinsText.text = $"Coins: {coins}";
    }
}