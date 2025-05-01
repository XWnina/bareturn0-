using UnityEngine;

public class EnemyAnimator : MonoBehaviour
{
    [SerializeField] protected Animator animator;

    [Header("Audio")]
    public AudioSource audioSource;  // 角色的音频组件
    public AudioClip attackSound;    // 攻击音效
    public AudioClip hurtSound; //受击音效

    protected virtual void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public virtual void EnemyAttackAnimation()
    {
        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }
    }

    public virtual void EnemyHurtAnimation()
    {
        if (animator != null)
        {
            animator.SetTrigger("Hurt");
        }

        if (hurtSound != null)
        {
            audioSource.PlayOneShot(hurtSound);
        }
    }

    public virtual void EnemyDeathAnimation()
    {
        if (animator != null)
        {
            animator.SetTrigger("Death"); // 触发死亡动画
        }
    }

    public virtual void CastingAnimation()
    {
        if (animator != null)
        {
            animator.SetTrigger("Cast"); // 触发引导动画
        }
    }

    public void TriggerPlayerHit()
    {
        BattleManager.Instance.TriggerPlayerHit(); // 让 `BattleManager` 控制玩家受击
        if (attackSound != null)
        {
            audioSource.PlayOneShot(attackSound); // 播放攻击音效
        }
    }
}
