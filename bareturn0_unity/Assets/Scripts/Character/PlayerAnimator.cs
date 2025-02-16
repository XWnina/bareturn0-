using UnityEngine;

public class PlayerAnimator : MonoBehaviour
{
    [SerializeField] private Animator animator;

    [Header("Audio")]
    public AudioSource audioSource;  // 角色的音频组件
    public AudioClip attackSound;    // 攻击音效
    public AudioClip hurtSound; //受击音效
    public AudioClip GainEnergySound;

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

    public void PlayGainEnergyAnimation()
    {
        if (animator != null)
        {
            animator.SetTrigger("GainEnergy");
        }
        if (GainEnergySound != null)
        {
            float volumeMultiplier = 2.5f;
            audioSource.PlayOneShot(GainEnergySound, volumeMultiplier);
        }
    }

    public void TriggerHit()
    {

        // 这里通知敌人受到攻击
        BattleManager.Instance.TriggerEnemyHit();
        if (attackSound != null)
        {
            audioSource.PlayOneShot(attackSound); // 播放音效
        }
    }

}
