using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

public class MapZoomInOut : MonoBehaviour
{
    public float zoomSpeed = 9f;
    public float minZoom = 4f;
    public float maxZoom = 28f;

    public Vector2 panLimitMin; // 地图左下角限制
    public Vector2 panLimitMax; // 地图右上角限制
    public float panSpeed = 0.5f;

    private Vector3 dragOrigin;

    void Update()
    {
        HandleZoom();
        //HandlePan();
    }

    void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (Mathf.Abs(scroll) > 0.01f)
        {
            float newSize = Camera.main.orthographicSize - scroll * zoomSpeed;
            Camera.main.orthographicSize = Mathf.Clamp(newSize, minZoom, maxZoom);
        }
    }

    // void HandlePan()
    // {
    //     // 准确判断鼠标是否在 UI 上（支持 world-space canvas）
    //     if (IsPointerOverUI())
    //     {
    //         return;
    //     }

    //     if (Input.GetMouseButtonDown(0))
    //     {
    //         dragOrigin = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    //     }

    //     if (Input.GetMouseButton(0))
    //     {
    //         Vector3 currentPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    //         Vector3 difference = dragOrigin - currentPoint;

    //         if (difference.magnitude > 0.05f) // 加个拖拽阈值防止误触
    //         {
    //             Vector3 newPosition = Camera.main.transform.position + difference;

    //             float camHeight = Camera.main.orthographicSize;
    //             float camWidth = camHeight * Camera.main.aspect;

    //             newPosition.x = Mathf.Clamp(newPosition.x, panLimitMin.x + camWidth, panLimitMax.x - camWidth);
    //             newPosition.y = Mathf.Clamp(newPosition.y, panLimitMin.y + camHeight, panLimitMax.y - camHeight);

    //             Camera.main.transform.position = new Vector3(newPosition.x, newPosition.y, Camera.main.transform.position.z);
    //         }
    //     }
    // }

    // private bool IsPointerOverUI()
    // {
    //     PointerEventData eventData = new PointerEventData(EventSystem.current);
    //     eventData.position = Input.mousePosition;

    //     List<RaycastResult> results = new List<RaycastResult>();
    //     GraphicRaycaster raycaster = FindObjectOfType<GraphicRaycaster>();
    //     if (raycaster != null)
    //     {
    //         raycaster.Raycast(eventData, results);
    //     }

    //     return results.Count > 0;
    // }
}
