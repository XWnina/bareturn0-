using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace JumpGame
{
    public class DraggableBlock : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        private CanvasGroup canvasGroup;
        private RectTransform rectTransform;
        private Transform originalParent;

        void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            rectTransform = GetComponent<RectTransform>();
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            originalParent = transform.parent;
            transform.SetParent(transform.root); // 拖到顶层，避免被遮挡
            canvasGroup.blocksRaycasts = false; // 允许穿透拖拽
        }

        public void OnDrag(PointerEventData eventData)
        {
            rectTransform.position = eventData.position;
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            transform.SetParent(originalParent); // 如果没拖到 DropSlot 就回原位
            canvasGroup.blocksRaycasts = true;
        }
    }
}