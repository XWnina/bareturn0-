using UnityEngine;

public class EnemyAnimator : MonoBehaviour
{
    [SerializeField] private Animator animator;

    [Header("Audio")]
    public AudioSource audioSource;  // 角色的音频组件
    public AudioClip attackSound;    // 攻击音效
    public AudioClip hurtSound; //受击音效

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void EnemyAttackAnimation()
    {
        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }
    }

    public void EnemyHurttAnimation()
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

    public void EnemyDeathAnimation()
    {
        if (animator != null)
        {
            animator.SetTrigger("Death"); // 触发死亡动画
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
