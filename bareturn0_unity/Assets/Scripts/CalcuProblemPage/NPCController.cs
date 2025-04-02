using UnityEngine;

namespace CalcuProblemPage
{
    public class NpcController : MonoBehaviour
    {
        [Header("动画与移动目标")]
        public Animator animator;
        public Transform targetPosition;
        public Transform startPosition;
        public float moveSpeed = 2f;

        private bool _isWalking;
        private bool _reachedTarget;
        private Transform _currentTarget;

        void Start()
        {
            if (startPosition != null)
            {
                transform.position = startPosition.position;
            }
            animator.ResetTrigger("startRead");
            animator.ResetTrigger("startWalk");
            animator.ResetTrigger("startIdle");
        }

        void Update()
        {
            if (_isWalking && _currentTarget != null)
            {
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    _currentTarget.position,
                    moveSpeed * Time.deltaTime
                );

                if (Vector3.Distance(transform.position, _currentTarget.position) < 0.05f)
                {
                    _isWalking = false;
                    _reachedTarget = true;

                    // 停止walk动画，切idle
                    animator.ResetTrigger("startRead");
                    animator.ResetTrigger("startWalk");
                    animator.SetTrigger("startIdle");
                }
            }
        }

        public void WalkIn()
        {
            WalkTo(targetPosition);
        }

        public void WalkTo(Transform target)
        {
            _currentTarget = target;
            _isWalking = true;
            _reachedTarget = false;

            animator.ResetTrigger("startRead");
            animator.ResetTrigger("startIdle");
            animator.SetTrigger("startWalk");
        }

        public void PlayRead()
        {
            Debug.Log(">>> PlayRead() 被调用了！");
            animator.ResetTrigger("startIdle");
            animator.ResetTrigger("startWalk");
            animator.SetTrigger("startRead");
        }

        public void PlayIdle()
        {
            animator.ResetTrigger("startRead");
            animator.ResetTrigger("startWalk");
            animator.SetTrigger("startIdle");
        }

        public bool HasReachedTarget()
        {
            return _reachedTarget;
        }
    }
}
