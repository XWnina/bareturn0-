using System.Collections;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public int currentHealth = 40;
    public int maxHealth = 40;

    // 速度
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
            StartCoroutine(HandleDeath());
        }
    }

    private IEnumerator HandleDeath()
    {
        animatorController.EnemyDeathAnimation(); // 播放死亡动画

        yield return new WaitForSeconds(1.2f); // 等待动画播放完成

        Destroy(gameObject); // 移除敌人对象（或者可以替换成游戏胜利界面）
        BattleManager.Instance.CheckWinLose(); // 重新检查战斗胜负
    }

    public void PerformAction()
    {
        Debug.Log($"Enemy attacks player for {attackDamage}");
        Attack(); // 触发敌人攻击动画
        // 存储攻击伤害，稍后 `TriggerPlayerHit()` 才会造成伤害
        BattleManager.Instance.lastAttackDamage = attackDamage;
    }
}