using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public int currentHealth = 40;
    public int maxHealth = 40;

    // ËÙ¶È
    public int speed = 8;

    public int attackDamage = 5;

    [SerializeField] EnemyAnimator animatorController;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void Attack()
    {
        animatorController?.EnemyAttackAnimation();
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        animatorController?.EnemyHurttAnimation();
        Debug.Log("Enemy takes " + damage + " damage. HP=" + currentHealth);

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Debug.Log("Enemy is dead!");
            // ´¥·¢ËÀÍöÂß¼­
        }
    }

    public void PerformAction()
    {
        Debug.Log($"Enemy attacks player for {attackDamage}");
        Attack(); // ´¥·¢µÐÈË¹¥»÷¶¯»­
        // ´æ´¢¹¥»÷ÉËº¦£¬ÉÔºó `TriggerPlayerHit()` ²Å»áÔì³ÉÉËº¦
        BattleManager.Instance.lastAttackDamage = attackDamage;
    }
}