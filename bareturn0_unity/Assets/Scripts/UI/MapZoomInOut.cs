using UnityEngine;
using UnityEngine.EventSystems;


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
        HandlePan();
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

    void HandlePan()
{
    // 如果鼠标指在 UI 上，直接退出，不做拖动
    if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
    {
        return;
    }

    if (Input.GetMouseButtonDown(0))
    {
        dragOrigin = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    if (Input.GetMouseButton(0))
    {
        Vector3 difference = dragOrigin - Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3 newPosition = Camera.main.transform.position + difference;

        float camHeight = Camera.main.orthographicSize;
        float camWidth = camHeight * Camera.main.aspect;

        newPosition.x = Mathf.Clamp(newPosition.x, panLimitMin.x + camWidth, panLimitMax.x - camWidth);
        newPosition.y = Mathf.Clamp(newPosition.y, panLimitMin.y + camHeight, panLimitMax.y - camHeight);

        Camera.main.transform.position = new Vector3(newPosition.x, newPosition.y, Camera.main.transform.position.z);
    }
}

}
