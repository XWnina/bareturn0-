using UnityEngine;

namespace JumpGame
{
    public class InfiniteBackground : MonoBehaviour
    {
        public float mapWidth; // 单张背景宽度（可从SpriteRenderer中获取）
        public int mapNums = 1; // 该背景对象中包含几张拼接图（默认1）
        private float _totalWidth; // 总宽度 = mapWidth * mapNums

        private GameObject _mainCamera;

        void Start()
        {
            // 查找摄像机
            _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");

            // 获取背景宽度
            mapWidth = GetComponent<SpriteRenderer>().sprite.bounds.size.x;
            _totalWidth = mapWidth * mapNums;
        }

        void Update()
        {
            Vector3 tempPosition = transform.position; // 当前背景位置
            float camX = _mainCamera.transform.position.x;

            // 如果背景与摄像机距离超过一半总宽度，就将背景移动一个完整宽度
            if (camX > transform.position.x + _totalWidth / 2f)
            {
                tempPosition.x += _totalWidth;
                transform.position = tempPosition;
            }
            else if (camX < transform.position.x - _totalWidth / 2f)
            {
                tempPosition.x -= _totalWidth;
                transform.position = tempPosition;
            }
        }
    }
}