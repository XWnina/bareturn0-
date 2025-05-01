using UnityEngine;
using UnityEngine.UI;

namespace JumpGame
{
    public class ClearAllBlocks : MonoBehaviour
    {
        [Header("清除区域")]
        public Transform codeWorkspace;

        [Header("按钮")]
        public Button clearButton;

        void Start()
        {
            if (clearButton != null)
            {
                clearButton.onClick.AddListener(ClearAll);
            }
            else
            {
                Debug.LogWarning("❗ ClearAllButton 未绑定！");
            }
        }

        void ClearAll()
        {
            foreach (Transform child in codeWorkspace)
            {
                Destroy(child.gameObject);
            }

            Debug.Log("🧹 所有 Block 已清除");
        }
    }
}