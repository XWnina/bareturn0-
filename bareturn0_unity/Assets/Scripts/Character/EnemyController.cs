using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEngine.GraphicsBuffer;

public class EnemyController : MonoBehaviour, ICharacter
{
    public int currentHealth = 40;
    public int maxHealth = 40;
    public int speed = 8;
    public int attackDamage = 5;
    public int currentArmor = 0;

    public SpriteRenderer spriteRenderer;
    private Material originalMaterial;
    public GameObject hpUI;

    [SerializeField] EnemyAnimator animatorController;

    //敌人牌组
    public List<CardData> enemyDeck = new List<CardData>();

    // 每回合抽取的手牌
    [HideInInspector]
    public List<CardData> enemyHand = new List<CardData>();

    // 每回合抽牌数量
    public int drawCount = 1;

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

       

        // 死亡逻辑
        if (currentHealth <= 0)
        {
            currentHealth = 0;
  
            Debug.Log("Enemy is dead!");
            StartCoroutine(HandleDeath());
        }

        // 更新血量UI
        UpdateHPText();
    }

    private IEnumerator HandleDeath()
    {
        animatorController.EnemyDeathAnimation(); // 播放死亡动画

        yield return new WaitForSeconds(1.2f); // 等待动画播放完成

        Destroy(gameObject); // 移除敌人对象（或者可以替换成游戏胜利界面）
        BattleManager.Instance.CheckWinLose(); // 重新检查战斗胜负
    }


    public void UpdateHPText()
    {
        if (hpUI != null)
        {
            TMP_Text hpText = hpUI.GetComponent<TMP_Text>();
            if (hpText != null)
            {
                hpText.text = $"HP: {currentHealth}/{maxHealth}";
            }
            else
            {
                Debug.LogWarning("UpdateHPText: TMP_Text component not found on hpUI.");
            }
        }
    }

    public IEnumerator ExecuteTurn()
    {
        // 1. 清空手牌
        enemyHand.Clear();

        // 2. 抽取卡牌（随机抽取，如果牌组为空则跳过）
        for (int i = 0; i < drawCount; i++)
        {
            if (enemyDeck.Count > 0)
            {
                int index = Random.Range(0, enemyDeck.Count);
                enemyHand.Add(enemyDeck[index]);
            }
        }

        // 打印抽到的卡牌名称，方便调试
        foreach (CardData card in enemyHand)
        {
            Debug.Log($"{gameObject.name} drew card: {card.cardName}");
        }

        // 3. 按顺序自动出牌，每张卡牌间隔一定时间（例如1秒）
        ICharacter target = null;
        foreach (CardData card in enemyHand)
        {
            yield return new WaitForSeconds(1f);

            // 选择目标
            if (card.cardEffect != null)
            {
                switch (card.targetingType) 
                {
                    case TargetingType.Self:
                        target = this;
                        break;
                    default:
                        target = BattleManager.Instance.player;
                        break;
                }
            }
            // 调用卡牌效果，施法者为当前敌人，目标为上面选定的
            card.cardEffect.ApplyEffect(BattleManager.Instance, card, this, target);
            Debug.Log($"{gameObject.name} used card: {card.cardName}");
        }

        // 4. 清空手牌，为下个回合做准备
        enemyHand.Clear();

        yield break;
    }

    public void GainArmor(int amount)
    {
        currentArmor += amount;
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
    }


}