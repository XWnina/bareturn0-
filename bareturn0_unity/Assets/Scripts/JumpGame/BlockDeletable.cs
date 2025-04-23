using UnityEngine;
using UnityEngine.UI;

namespace JumpGame
{
    public class BlockDeletable : MonoBehaviour
    {
        [Header("模块内的删除按钮")]
        public Button deleteButton;

        void Start()
        {
            if (deleteButton != null)
            {
                deleteButton.onClick.AddListener(DeleteSelf);
            }
            else
            {
                Debug.LogWarning($"❗ {gameObject.name} 没有绑定 DeleteButton！");
            }
        }

        private void DeleteSelf()
        {
            Debug.Log($"🗑 删除模块：{gameObject.name}");
            Destroy(gameObject);
        }
    }
}