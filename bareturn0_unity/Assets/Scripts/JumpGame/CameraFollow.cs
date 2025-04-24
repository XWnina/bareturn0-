using UnityEngine;

namespace JumpGame
{
    public class CameraFollow : MonoBehaviour
    {
        public Transform player; // 玩家角色
        public Transform farBackground;   // 远景父对象
        public Transform midBackground;   // 中景父对象
        public Transform closeBackground; // 近景父对象

        public float xOffset = 3f; // 摄像机在玩家右侧一定距离
        public bool lockY = true; // 是否锁定 Y 坐标

        private Vector3 _lastCameraPosition;

        void Start()
        {
            if (player == null)
            {
                Debug.LogError("Player not assigned to CameraFollow script.");
                return;
            }

            _lastCameraPosition = transform.position;
        }

        void LateUpdate()
        {
            // 1. 先更新摄像机位置
            float targetY = lockY ? transform.position.y : player.position.y;
            Vector3 targetPosition = new Vector3(player.position.x + xOffset, targetY, transform.position.z);
            transform.position = targetPosition;

            // 2. 计算摄像机位移差
            Vector3 currentCameraPosition = transform.position;
            Vector3 deltaMovement = currentCameraPosition - _lastCameraPosition;

            // 3. 分别移动各层背景，模拟景深视差
            if (farBackground != null)
                farBackground.position += deltaMovement * 0.7f;

            if (midBackground != null)
                midBackground.position += deltaMovement * 0.5f;

            if (closeBackground != null)
                closeBackground.position += deltaMovement * 0.1f;

            // 4. 更新 last camera position
            _lastCameraPosition = currentCameraPosition;
        }
    }
}