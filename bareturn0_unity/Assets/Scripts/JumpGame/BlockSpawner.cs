using UnityEngine;
using UnityEngine.EventSystems;

namespace JumpGame
{
    public class BlockSpawner : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [Header("要生成的模块 prefab")]
        public GameObject blockPrefab;

        [Header("拼接目标区域（比如 CodeWorkspaceContent）")]
        public Transform targetParent;

        private GameObject _spawnedBlock;
        private RectTransform _canvasRoot;
        private RectTransform _scrollViewRect;

        void Start()
        {
            // 找 Canvas
            _canvasRoot = GameObject.Find("Canvas")?.GetComponent<RectTransform>();

            // 自动找拼接区（也可以手动拖进来）
            if (targetParent == null)
                targetParent = GameObject.Find("CodeWorkspaceContent")?.transform;

            // 自动找 CodeWorkspaceScrollView（作为放置判定区域）
            GameObject scrollViewGo = GameObject.Find("CodeWorkspaceScrollView");
            if (scrollViewGo != null)
                _scrollViewRect = scrollViewGo.GetComponent<RectTransform>();
            else
                Debug.LogWarning("❗未找到 CodeWorkspaceScrollView，请检查命名或手动拖入！");
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (blockPrefab == null || _canvasRoot == null) return;

            _spawnedBlock = Instantiate(blockPrefab, _canvasRoot);
            _spawnedBlock.transform.position = eventData.position;
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (_spawnedBlock != null)
                _spawnedBlock.transform.position = eventData.position;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (_spawnedBlock == null || targetParent == null || _scrollViewRect == null)
            {
                Destroy(_spawnedBlock);
                return;
            }

            Vector2 localPoint;
            bool isInside = RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _scrollViewRect,
                eventData.position,
                eventData.pressEventCamera,
                out localPoint
            );

            if (isInside && _scrollViewRect.rect.Contains(localPoint))
            {
                // ✅ 放到拼接区
                _spawnedBlock.transform.SetParent(targetParent, false);
                RectTransform rt = _spawnedBlock.GetComponent<RectTransform>();
                if (rt != null)
                {
                    rt.localScale = Vector3.one;
                    rt.anchoredPosition = Vector2.zero;
                    rt.localRotation = Quaternion.identity;
                }

                Debug.Log($"✅ 模块成功吸收：{_spawnedBlock.name} → {targetParent.name}");
            }
            else
            {
                // ❌ 放错地方自动销毁
                Debug.Log($"❌ 模块放置失败，已销毁：{_spawnedBlock.name}");
                Destroy(_spawnedBlock);
            }
        }
    }
}
