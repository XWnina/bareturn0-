using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    [SerializeField] private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>(); // 获取 Animator 组件
        if (animator == null)
        {
            Debug.LogError("PlayerAnimatorController: Animator component missing on Player!");
        }
    }

    // 播放攻击动画
    public void PlayAttackAnimation()
    {
        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }
    }

    // 播放受击动画
    public void PlayHurtAnimation()
    {
        if (animator != null)
        {
            animator.SetTrigger("Hurt");
        }
    }

    // 播放防御动画（如果有）
    public void PlayDefendAnimation()
    {
        if (animator != null)
        {
            animator.SetTrigger("Defend");
        }
    }

    public void TriggerHit()
    {
        // 这里通知敌人受到攻击
        BattleManager.Instance.TriggerEnemyHit();
    }

}
