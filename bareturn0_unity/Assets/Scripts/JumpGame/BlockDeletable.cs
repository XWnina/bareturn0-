using System.Collections;
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
        void ForceRebuildAllParents(Transform target)
        {
            while (target != null)
            {
                RectTransform rt = target as RectTransform;
                if (rt != null)
                    LayoutRebuilder.ForceRebuildLayoutImmediate(rt);

                target = target.parent;
            }
        }
        private IEnumerator ForceLayoutFix()
        {
            // 第 1 帧：等待 UI 元素真正销毁
            yield return null;

            // 第 2 帧：强制 Canvas 更新（否则宽度不变）
            Canvas.ForceUpdateCanvases();

            // 第 3 帧：向上递归所有父级刷新 Layout
            ForceRebuildAllParents(transform);
        }

        private void DeleteSelf()
        {
            Debug.Log($"🗑 删除模块：{gameObject.name}");
            Destroy(gameObject);
            StartCoroutine(ForceLayoutFix());
        }
    }
}