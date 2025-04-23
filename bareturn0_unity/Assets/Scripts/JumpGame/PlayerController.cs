// ✅ PlayerController.cs
// 控制 NPC 的自动移动、跳跃，以及环境检测逻辑（如 RockAhead, PlatformAbove）

using UnityEngine;

namespace JumpGame
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerController : MonoBehaviour
    {
        [Header("移动与跳跃参数")]
        public float moveSpeed = 2f;
        public float jumpForce = 7f;

        [Header("地面检测")] 
        public Transform groundCheck;
        public LayerMask groundLayer;
        public float groundRadius = 0.2f;

        private Rigidbody2D _rb;
        private bool _isGrounded;

        void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
        }

        void Update()
        {
            _isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundLayer);
        }

        public void Jump()
        {
            if (_isGrounded)
            {
                _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, jumpForce);
                Debug.Log("🔼 Jump executed");
            }
            else
            {
                Debug.Log("🚫 Can't jump — not grounded");
            }
        }

        public bool IsRockAhead()
        {
            Vector2 direction = Vector2.right * transform.localScale.x;
            float distance = 1f;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, distance);

            if (hit.collider != null && hit.collider.CompareTag("Obstacle"))
            {
                Debug.Log("🪨 Rock ahead detected!");
                return true;
            }
            return false;
        }

        public bool IsPlatformAbove()
        {
            float checkDistance = 1.5f;
            RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.up, checkDistance);

            if (hit.collider != null && hit.collider.CompareTag("Ground"))
            {
                Debug.Log("🪜 Platform above detected!");
                return true;
            }
            return false;
        }
    }
}