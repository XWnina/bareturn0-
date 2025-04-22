using UnityEngine;
using UnityEngine.EventSystems;


namespace JumpGame
{
    public class DropSlot : MonoBehaviour, IDropHandler
    {
        public void OnDrop(PointerEventData eventData)
        {
            GameObject dropped = eventData.pointerDrag;
            if (dropped != null)
            {
                // 放入本 slot，并重设位置
                dropped.transform.SetParent(transform);
                dropped.transform.localPosition = Vector3.zero;

                // 可选：居中对齐
                RectTransform rect = dropped.GetComponent<RectTransform>();
                if (rect != null)
                    rect.anchoredPosition = Vector2.zero;
            }
        }
    }
}