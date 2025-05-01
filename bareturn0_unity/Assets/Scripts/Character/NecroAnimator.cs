using UnityEngine;

public class NecroAnimator : EnemyAnimator
{
    [Header("Necro专属音效")]
    public AudioClip summonSound;  // 召唤小鬼的特殊音效

    // 
    public void NecroAttack1Animation()
    {
        if (animator != null)
            animator.SetTrigger("Attack1");
    }

    // 召唤小鬼动画
    public void NecroSummonAnimation()
    {
        if (animator != null)
            animator.SetTrigger("Summon"); // Animator中必须有"Summon" Trigger

        if (summonSound != null)
            audioSource.PlayOneShot(summonSound);
    }

    // 其它公共行为（受伤、死亡）直接继承父类，无需重写
}
