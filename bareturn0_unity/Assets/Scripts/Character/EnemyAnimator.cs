using UnityEngine;

public class EnemyAnimator : MonoBehaviour
{
    [SerializeField] private Animator animator;

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
    }

    public void TriggerPlayerHit()
    {
        BattleManager.Instance.TriggerPlayerHit(); // ÈÃ `BattleManager` ¿ØÖÆÍæ¼ÒÊÜ»÷
    }
}
