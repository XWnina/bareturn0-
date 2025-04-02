using UnityEngine;

namespace CalcuProblemPage
{
    public class NPCController : MonoBehaviour
    {
        [Header("Animator + 位置")]
        public Animator animator;
        public Transform targetPosition; // NPC 要走到哪
        public float moveSpeed = 2f;

        private bool isWalking = false;
        private bool reachedTarget = false;

        void Start()
        {
   
             transform.position = new Vector3(10f, transform.position.y, transform.position.z);
        }

        void Update()
        {
            if (isWalking)
            {
                transform.position = Vector3.MoveTowards(
                    transform.position,
                    targetPosition.position,
                    moveSpeed * Time.deltaTime
                );

                if (Vector3.Distance(transform.position, targetPosition.position) < 0.05f)
                {
                    isWalking = false;
                    reachedTarget = true;
                    animator.Play("idle"); // 切回 idle
                }
            }
        }

        public void WalkIn()
        {
            animator.ResetTrigger("startRead");
            animator.SetTrigger("startWalk");
            isWalking = true;
            reachedTarget = false;
        }

        public void PlayRead()
        {
            animator.ResetTrigger("startWalk");
            animator.SetTrigger("startRead");
        }

        public void PlayIdle()
        {
            animator.Play("idle");
        }

        public bool HasReachedTarget()
        {
            return reachedTarget;
        }
    }
}