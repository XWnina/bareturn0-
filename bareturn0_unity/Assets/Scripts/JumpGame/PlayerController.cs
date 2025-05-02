using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

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
        [HideInInspector] public bool interrupted = false;

        private Rigidbody2D _rb;
        private Animator _animator;
        private bool _isGrounded;

        private readonly Queue<System.Action> _conditionQueue = new Queue<System.Action>();
        private readonly Queue<PlayerAction> _actionQueue = new Queue<PlayerAction>();
        private bool _isExecuting = false;
        private string baseUrl = "http://localhost:3000/";
        private string _token ;
        private string _saveName ;
        

        void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _animator = GetComponent<Animator>();
            _token = PlayerPrefs.GetString("token", "");
            _saveName = PlayerPrefs.GetString("currentSaveName", "");
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
            Vector2 centerOrigin = transform.position;
            Vector2 groundOrigin = groundCheck != null ? groundCheck.position : centerOrigin + Vector2.down * 0.5f;
            Vector2 middleOrigin = (centerOrigin + groundOrigin) / 2f;

            Vector2 dir = Vector2.right * transform.localScale.x;
            float distance = 1.5f;

            // 射线1：中心
            RaycastHit2D centerHit = Physics2D.Raycast(centerOrigin, dir, distance, obstacleLayer);
            Debug.DrawRay(centerOrigin, dir * distance, Color.yellow, 1f);

            // 射线2：中间
            RaycastHit2D middleHit = Physics2D.Raycast(middleOrigin, dir, distance, obstacleLayer);
            Debug.DrawRay(middleOrigin, dir * distance, Color.cyan, 1f);

            // 射线3：脚底
            RaycastHit2D groundHit = Physics2D.Raycast(groundOrigin, dir, distance, obstacleLayer);
            Debug.DrawRay(groundOrigin, dir * distance, Color.green, 1f);

            if (centerHit.collider != null)
            {
                Debug.Log($"🪨 中间上方检测到障碍物：{centerHit.collider.name}");
                return true;
            }
            else if (middleHit.collider != null)
            {
                Debug.Log($"🪨 中间高度检测到障碍物：{middleHit.collider.name}");
                return true;
            }
            else if (groundHit.collider != null)
            {
                Debug.Log($"🪨 脚底检测到障碍物：{groundHit.collider.name}");
                return true;
            }
            else
            {
                Debug.Log("✅ 前方没有障碍物");
                return false;
            }
        }


        void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.collider.CompareTag("Obstacle"))
            {
                Debug.Log("🪨 撞到了石头！");
                Object.FindFirstObjectByType<NpcTeachingDialogue>()?.TriggerObstacleFeedback();
                interrupted = true; 

                // ➤ 后退一点
                float retreatDistance = 0.5f;
                float direction = -Mathf.Sign(transform.localScale.x); // 面朝方向反向
                Vector2 retreat = new Vector2(retreatDistance * direction, 0f);

                _rb.MovePosition(_rb.position + retreat);
                Debug.Log("🔙 已自动后退一点");
                
                
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
            if (other.CompareTag("FinalFlag"))
            {
                Debug.Log("🎯 到达终点 FinalFlag！");
                ForceStop(); // 停止所有行为
                reachedGoal = true;

                var dialogue = Object.FindFirstObjectByType<NpcTeachingDialogue>();
                if (dialogue != null)
                {
                    dialogue.ShowFinalMessageAndHide(); 
                }
            }

            
        }
        // 这里跳转！
        public IEnumerator HandleLevelComplete()
        {
            yield return new WaitForSeconds(3f); // 显示对话

            yield return StartCoroutine(UpdateProgress(9)); // 上传进度值为 2，可自定义
            StartCoroutine(UnlockAchievement(_saveName, "Your Way"));
            PlayerPrefs.SetInt("AchievementUnlock", 9);

            UnityEngine.SceneManagement.SceneManager.LoadScene("DraftMap"); // 切换场景
        }
        private IEnumerator UnlockAchievement(string saveName, string achievementName)
        {
            string url = $"{baseUrl}achievements/{saveName}/unlock";

            string jsonData = "{ \"achievementName\": \"" + achievementName + "\" }";

            using (UnityWebRequest request = new UnityWebRequest(url, "PUT"))
            {
                byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);
                request.uploadHandler = new UploadHandlerRaw(bodyRaw);
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");
                request.SetRequestHeader("Authorization", "Bearer " + _token);

                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    Debug.Log("✅ Achievement unlocked successfully: " + request.downloadHandler.text);
                }
                else
                {
                    Debug.LogError("❌ Failed to unlock achievement: " + request.error);
                    Debug.LogError("Response: " + request.downloadHandler.text);
                }
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
                case "obstacle ahead": callback(IsRockAhead()); break;
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
        public IEnumerator UpdateProgress(int progress)
        {
            string token = PlayerPrefs.GetString("token", "");
            string saveName = PlayerPrefs.GetString("currentSaveName", "");

            if (string.IsNullOrEmpty(token))
            {
                Debug.LogError("❌ No Token Found! Player is not authenticated.");
                yield break;
            }

            if (string.IsNullOrEmpty(saveName))
            {
                Debug.LogError("❌ No SaveName Found! Cannot update progress.");
                yield break;
            }

            string url = $"http://localhost:3000/savefiles/{saveName}/updateProgress";
            string jsonData = $"{{\"progress\":{progress}}}";
            Debug.Log("📤 发送的 JSON 数据：" + jsonData);

            using (UnityWebRequest request = UnityWebRequest.Put(url, jsonData))
            {
                request.method = "PUT";
                request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(jsonData));
                request.downloadHandler = new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");
                request.SetRequestHeader("Authorization", "Bearer " + token);

                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    Debug.Log("✅ Progress updated successfully: " + request.downloadHandler.text);
                }
                else
                {
                    Debug.LogError("❌ Failed to update progress: " + request.error);
                }
            }
        }

        

    }
}
