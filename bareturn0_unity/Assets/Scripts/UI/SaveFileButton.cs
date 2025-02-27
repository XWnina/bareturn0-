using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace UI  // ✅ 确保命名空间匹配 UI 文件夹
{
    public class SaveFileButton : MonoBehaviour
    {
        public TMP_Text saveNameText;
        public TMP_Text playerNameText;
        public TMP_Text progressText;
        public TMP_Text coinsText;
        public Button deleteButton; // ✅ 确保 `DeleteButton` 在 Inspector 绑定

        private string _saveName;
        private GameObject _saveButton;

        public void SetSaveData(string saveName, string playerName, int progress, int coins)
        {
            _saveName = saveName;
            _saveButton = gameObject;

            if (saveNameText != null) saveNameText.text = $"Save: {saveName}";
            if (playerNameText != null) playerNameText.text = $"Player: {playerName}";
            if (progressText != null) progressText.text = $"Progress: {progress}";
            if (coinsText != null) coinsText.text = $"Coins: {coins}";

            if (deleteButton == null)
            {
                deleteButton = transform.Find("Panel/DeleteButton")?.GetComponent<Button>();
            }

            if (deleteButton != null)
            {
                deleteButton.onClick.RemoveAllListeners(); // ✅ 确保 `onClick` 只绑定一次
                deleteButton.onClick.AddListener(DeleteSaveFile);
            }
            else
            {
                Debug.LogError($"❌ DeleteButton is NULL on {saveName}. Check Prefab!");
            }
        }



        private void DeleteSaveFile()
        {
            LoadSceneController loadSceneController = Object.FindFirstObjectByType<LoadSceneController>();
            if (loadSceneController != null)
            {
                // ❌ 不要直接删除，而是调用 `ShowConfirmationDialog`
                loadSceneController.ShowConfirmationDialog(_saveName, 0, _saveButton, true);
            }
            else
            {
                Debug.LogError("❌ LoadSceneController not found in the scene!");
            }
        }

    }
}