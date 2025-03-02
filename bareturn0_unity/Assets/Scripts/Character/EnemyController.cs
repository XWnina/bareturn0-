using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class EnemyController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public int currentHealth = 40;
    public int maxHealth = 40;
    public int speed = 8;
    public int attackDamage = 5;

    public SpriteRenderer spriteRenderer;
    private Material originalMaterial;
    public Material outlineMaterial;

    [SerializeField] EnemyAnimator animatorController;

    private void Awake()
    {
        currentHealth = maxHealth;
        if (spriteRenderer != null)
        {
            originalMaterial = spriteRenderer.material;
        }
    }

    public void Initialize(EnemyData data)
    {
        this.maxHealth = data.maxHealth;
        this.currentHealth = data.maxHealth;
        this.speed = data.speed;
        this.attackDamage = data.attackDamage;
        // 如果还需要别的初始化逻辑，可以放在这里
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

    // 当指针进入敌人对象时（例如在拖拽状态下），高亮敌人
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (BattleManager.Instance != null && BattleManager.Instance.isCardBeingDragged)
        {
            Highlight(true);
        }
    }

    // 当指针离开时，取消高亮
    public void OnPointerExit(PointerEventData eventData)
    {
        if (BattleManager.Instance != null && BattleManager.Instance.isCardBeingDragged)
        {
            Highlight(false);
        }
    }

    // 高亮处理方法
    public void Highlight(bool flag)
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.material = flag && outlineMaterial != null ? outlineMaterial : originalMaterial;
        }
    }

    // 用于在卡牌使用后清除高亮
    public void ClearHighlight()
    {
        Highlight(false);
    }


}