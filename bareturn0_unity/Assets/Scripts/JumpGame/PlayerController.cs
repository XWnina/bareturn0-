using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JumpGame
{
    public enum PlayerAction
    {
        Jump,
        Walk,
        EvaluateNextCondition, // 新增动作：用于延迟执行条件判断
        WalkLeft
    }

    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerController : MonoBehaviour
    {
        private static readonly int IsJumping = Animator.StringToHash("isJumping");
        private static readonly int IsFalling = Animator.StringToHash("isFalling");
        private static readonly int IsRunning = Animator.StringToHash("isRunning");

        [Header("移动与跳跃参数")]
        public float jumoMoveSpeed = 2f;
        public float runSpeed = 4f;
        public float jumpForce = 7f;

        [Header("地面检测")]
        public Transform groundCheck;
        public LayerMask groundLayer;
        public float groundRadius = 0.2f;

        [Header("是否启用测试模式 (键盘控制)")]
        public bool enableTestInput = true;
        [HideInInspector] public bool reachedGoal = false;

        private Rigidbody2D _rb;
        private Animator _animator;
        private bool _isGrounded;

        private readonly Queue<System.Action> _conditionQueue = new Queue<System.Action>();
        private readonly Queue<PlayerAction> _actionQueue = new Queue<PlayerAction>();
        private bool _isExecuting = false;

        void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _animator = GetComponent<Animator>();
        }

        void Update()
        {
            _isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundLayer);
            _animator.SetBool(IsJumping, !_isGrounded && _rb.linearVelocity.y > 0.1f);
            _animator.SetBool(IsFalling, !_isGrounded && _rb.linearVelocity.y < -0.1f);
            _animator.SetBool(IsRunning, Mathf.Abs(_rb.linearVelocity.x) > 0.1f && _isGrounded);

            if (enableTestInput)
            {
                if (Input.GetKeyDown(KeyCode.Space)) EnqueueAction(PlayerAction.Jump);
                if (Input.GetKeyDown(KeyCode.D)) EnqueueAction(PlayerAction.Walk);
            }
            if (Input.GetKeyDown(KeyCode.A)) EnqueueAction(PlayerAction.WalkLeft);

        }

        public void EnqueueAction(PlayerAction action)
        {
            _actionQueue.Enqueue(action);
            if (!_isExecuting)
                StartCoroutine(ExecuteQueue());
        }

        public void EnqueueCondition(System.Action conditionalEvaluator)
        {
            _conditionQueue.Enqueue(conditionalEvaluator);
            _actionQueue.Enqueue(PlayerAction.EvaluateNextCondition);
            if (!_isExecuting)
                StartCoroutine(ExecuteQueue());
        }

        private IEnumerator ExecuteQueue()
        {
            _isExecuting = true;

            while (_actionQueue.Count > 0)
            {
                PlayerAction action = _actionQueue.Dequeue();

                switch (action)
                {
                    case PlayerAction.Jump:
                        if (_isGrounded)
                        {
                            float facing = transform.localScale.x >= 0 ? 1f : -1f;
                            _rb.linearVelocity = new Vector2(jumoMoveSpeed * facing, jumpForce);
                            Debug.Log("🔼 Jump 开始");

                            yield return new WaitUntil(() => _rb.linearVelocity.y <= 0.1f);
                            yield return new WaitUntil(() => _isGrounded);
                            Debug.Log("🔽 Jump 落地完成");
                        }
                        break;

                    case PlayerAction.Walk:
                        Debug.Log("🏃 Walk 开始");
                        float facingWalk = transform.localScale.x >= 0 ? 1f : -1f;
                        Vector2 start = transform.position;
                        _rb.linearVelocity = new Vector2(runSpeed * facingWalk, _rb.linearVelocity.y);

                        while (Vector2.Distance(transform.position, start) < 1.5f)
                        {
                            if (Mathf.Abs(_rb.linearVelocity.x) < 0.05f) break;
                            yield return null;
                        }

                        _rb.linearVelocity = new Vector2(0f, _rb.linearVelocity.y);
                        Debug.Log("🏁 Walk 结束");
                        break;

                    case PlayerAction.EvaluateNextCondition:
                        if (_conditionQueue.Count > 0)
                        {
                            _conditionQueue.Dequeue()?.Invoke();
                            yield return null;
                        }
                        break;
                    case PlayerAction.WalkLeft:
                        Debug.Log("👈 WalkLeft 开始");
                        float leftFacing = -1f;
                        Vector2 leftStart = transform.position;

                        _rb.linearVelocity = new Vector2(runSpeed * leftFacing, _rb.linearVelocity.y);

                        float leftDistance = 1.5f;
                        while (Vector2.Distance(transform.position, leftStart) < leftDistance)
                        {
                            if (Mathf.Abs(_rb.linearVelocity.x) < 0.05f) break;
                            yield return null;
                        }

                        _rb.linearVelocity = new Vector2(0f, _rb.linearVelocity.y);
                        Debug.Log("🏁 WalkLeft 结束");
                        break;

                }

                yield return null;
            }

            _isExecuting = false;
        }

        

        public LayerMask obstacleLayer; // 请在 Inspector 中勾选包含 Tilemap 的 Layer

        public bool IsRockAhead()
        {
            // ✅ 推荐从角色底部（如 groundCheck）发射射线
            Vector2 origin =  transform.position;

            // ✅ 向角色面朝方向发射
            Vector2 dir = Vector2.right * transform.localScale.x;

            float distance = 1f; // 根据需要调整检测距离

            // ✅ 使用 LayerMask，只检测 obstacleLayer 中的物体
            RaycastHit2D hit = Physics2D.Raycast(origin, dir, distance, obstacleLayer);

            // ✅ 可视化射线（Scene 视图中看到）
            Debug.DrawRay(origin, dir * distance, Color.yellow, 2f);

            // ✅ 调试输出
            if (hit.collider != null)
            {
                Debug.Log($"🪨 检测到 Obstacle：{hit.collider.name}（层：{LayerMask.LayerToName(hit.collider.gameObject.layer)}）");
                return true;
            }
            else
            {
                Debug.Log("✅ 脚前方没有障碍物");
                return false;
            }
        }
        void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.collider.CompareTag("Obstacle"))
            {
                Debug.Log("🪨 撞到了石头！");
                Object.FindFirstObjectByType<NpcTeachingDialogue>()?.TriggerObstacleFeedback();

            }
        }
        private void OnTriggerEnter2D(Collider2D other)
        {
            Debug.Log($"[DEBUG] 触发器命中：{other.name}，tag = {other.tag}, layer = {LayerMask.LayerToName(other.gameObject.layer)}");

            if (other.CompareTag("Goal"))
            {
                Debug.Log("🎌 碰到了终点旗帜！");
                reachedGoal = true; // ✅ 一定要设置为 true！
                var dialogue = Object.FindFirstObjectByType<NpcTeachingDialogue>();
                dialogue?.OnPlayerReachedFlag();
            }

        }

        public void ForceStop()
        {
            // 停止一切移动
            _rb.linearVelocity = Vector2.zero;

            // 清空动作队列
            _actionQueue.Clear();
            _conditionQueue.Clear();

            // 强制终止协程
            StopAllCoroutines();
            _isExecuting = false;

            // 动画状态可选恢复为 idle（或你已有逻辑中会自动处理）
            Debug.Log("🛑 玩家已强制停止");
        }



        
        public bool IsPlatformAbove()
        {
            Vector2 origin = transform.position;
            float angle = 30f * Mathf.Deg2Rad;
            float dirX = Mathf.Cos(angle) * (transform.localScale.x >= 0 ? 1 : -1);
            Vector2 dir = new Vector2(dirX, Mathf.Sin(angle)).normalized;

            RaycastHit2D hit = Physics2D.Raycast(origin, dir, 2.5f, groundLayer);
            Debug.DrawRay(origin, dir * 2.5f, Color.red, 5f);
            return hit.collider != null;
        }
        public IEnumerator EvaluateConditionAsync(string condition, System.Action<bool> callback)
        {
            yield return new WaitUntil(() => !_isExecuting); // 等待之前动作完成

            switch (condition)
            {
                case "platform up": callback(IsPlatformAbove()); break;
                case "rock ahead": callback(IsRockAhead()); break;
                case "always true": callback(true); break;
                case "is grounded": callback(_isGrounded); break;
                default:
                    Debug.LogWarning($"⚠ 未知条件：{condition}");
                    callback(false);
                    break;
            }
        }
        public IEnumerator ExecuteActionAsync(string action)
        {
            switch (action)
            {
                case "jump":
                    EnqueueAction(PlayerAction.Jump);
                    break;
                case "walk":
                    EnqueueAction(PlayerAction.Walk);
                    break;
                default:
                    Debug.LogWarning($"⚠ 未知动作：{action}");
                    yield break;
            }

            yield return new WaitUntil(() => !_isExecuting); // 等待执行完成
        }

    }
}
