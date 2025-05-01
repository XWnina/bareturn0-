using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.EventSystems;
using static UnityEditor.PlayerSettings;
using static UnityEngine.GraphicsBuffer;

public class EnemyController : MonoBehaviour, ICharacter, IPointerEnterHandler, IPointerExitHandler
{
    public string enemyName = "";
    public int currentHealth = 40;
    public int maxHealth = 40;
    public int speed = 8;
    public int attackDamage = 5;
    public int currentArmor = 0;

    public SpriteRenderer spriteRenderer;
    private Vector3 originalScale;
    public float enlargeFactor = 1.1f; // 放大10%
    public GameObject statusUI;

    private bool needEnlarge = false;
    private bool needResetScale = false;

    public int sharpnessLayers = 0;
    public int poisonLayers = 0;
    public int bleedLayers = 0;
    public int burnLayers = 0;


    [SerializeField] EnemyAnimator animatorController;

    //敌人牌组
    public List<CardData> enemyDeck = new List<CardData>();

    // 每回合抽取的手牌
    [HideInInspector]
    public List<CardData> enemyHand = new List<CardData>();

    // 每回合抽牌数量
    public int drawCount = 1;

    protected virtual void Awake()
    {
        currentHealth = maxHealth;
        // 保存初始的局部缩放
        originalScale = transform.localScale;
    }

    public void Initialize(EnemyData data)
    {
        this.enemyName = data.enemyName;
        this.maxHealth = data.maxHealth;
        this.currentHealth = data.maxHealth;
        this.speed = data.speed;
        //this.attackDamage = data.attackDamage;
        // 如果还需要别的初始化逻辑，可以放在这里
    }

    public virtual void Attack()
    {
        animatorController?.EnemyAttackAnimation();
    }

    public virtual void Cast()
    {
        animatorController?.CastingAnimation();
    }

    public virtual void TakeDamage(int damage)
    {
        int effectiveDamage = Mathf.Max(damage - currentArmor, 0);
        currentArmor = Mathf.Max(currentArmor - damage, 0);
        currentHealth -= effectiveDamage;
        animatorController?.EnemyHurtAnimation();
        Vector3 pos = transform.position;
        BattleManager.Instance.ShowFloatingValue(pos, effectiveDamage);
        Debug.Log("Enemy takes " + damage + " damage. HP=" + currentHealth);

       

        // 死亡逻辑
        if (currentHealth <= 0)
        {
            currentHealth = 0;
  
            Debug.Log("Enemy is dead!");
            StartCoroutine(HandleDeath());
        }

        // 更新状态UI
        UpdateStatusUI();
    }

    // 更新状态UI
    public void UpdateStatusUI()
    {
        if (statusUI != null)
        {
            EnemyStatusUI statusUIComponent = statusUI.GetComponent<EnemyStatusUI>();
            if (statusUIComponent != null)
            {
                statusUIComponent.UpdateStatus(currentHealth, maxHealth, currentArmor);
            }
        }
    }
    public void GainArmor(int amount)
    {
        currentArmor += amount;
        Debug.Log($"{gameObject.name} now has {currentArmor} armor.");
        UpdateStatusUI();
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        Vector3 pos = transform.position;
        BattleManager.Instance.ShowEffectOnly(pos, EffectType.Heal);
        BattleManager.Instance.ShowFloatingValue(pos, -amount);

        UpdateStatusUI();
    }


    protected virtual IEnumerator HandleDeath()
    {
        animatorController.EnemyDeathAnimation(); // 播放死亡动画

        yield return new WaitForSeconds(1.2f); // 等待动画播放完成

        Destroy(gameObject); // 移除敌人对象（或者可以替换成游戏胜利界面）
        BattleManager.Instance.CheckWinLose(); // 重新检查战斗胜负
    }

    public virtual IEnumerator ExecuteTurn()
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
            yield return new WaitForSeconds(2f);

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

            // 显示出牌卡牌名称，并在状态UI中淡出显示
            if (statusUI != null)
            {
                EnemyStatusUI statusUIComponent = statusUI.GetComponent<EnemyStatusUI>();
                if (statusUIComponent != null)
                {
                    statusUIComponent.ShowCardName(card.cardName);
                }
            }
        }

        // 4. 清空手牌，为下个回合做准备
        enemyHand.Clear();

        yield break;
    }

    public IEnumerator ProcessStartOfTurnBuffs()
    {
        if (sharpnessLayers > 0)
        {

            sharpnessLayers--;
            Debug.Log("Player's Sharpness reduced by 1, now: " + sharpnessLayers);
        }

        if (poisonLayers > 0)
        {
            int damage = poisonLayers;
            yield return new WaitForSeconds(0.5f);
            TakeDamage(damage);
            poisonLayers = Mathf.Max(poisonLayers - 1, 0);
            Debug.Log(name + " takes " + damage + " poison damage, remaining poison: " + poisonLayers);
        }

        if (bleedLayers > 0)
        {
            int bleedDamage = bleedLayers;
            yield return new WaitForSeconds(0.5f); // 延时
            TakeDamage(bleedDamage);
            bleedLayers = Mathf.Max(bleedLayers - 1, 0);
            Debug.Log($"{name} takes {bleedDamage} bleed damage, remaining bleed: {bleedLayers}");
        }
        if (burnLayers > 0)
        {
            int burnDamage = burnLayers;
            yield return new WaitForSeconds(0.5f);
            TakeDamage(burnDamage);
            burnLayers = Mathf.Max(burnLayers - 1, 0);
            Debug.Log(name + " takes " + burnDamage + " burn damage, remaining burn: " + burnLayers);
        }
        else
        {
            yield return null;
        }
        UpdateBuffUI();
    }

    public void ApplySharpness(int layersToAdd)
    {  
        sharpnessLayers += layersToAdd;
        UpdateBuffUI();
    }

    public void ApplyPoison(int layersToAdd)
    {
        poisonLayers += layersToAdd;
        UpdateBuffUI();
    }

    public void ApplyBleed(int layersToAdd)
    {
        bleedLayers += layersToAdd;
        UpdateBuffUI();
        Debug.Log($"{name} gains {layersToAdd} layers of Bleed. Total bleed layers: {bleedLayers}");
    }

    public void ApplyBurn(int layersToAdd)
    {
        burnLayers += layersToAdd;
        UpdateBuffUI();
        Debug.Log($"{name} gains {layersToAdd} burn layers. Total burn: {burnLayers}");
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        if (BattleManager.Instance != null && BattleManager.Instance.isCardBeingDragged)
        {
            // 检查当前拖拽卡牌的数据是否存在
            CardData draggingCard = BattleManager.Instance.currentDraggingCardData;
            if (draggingCard != null)
            {
                // 如果目标类型是 FirstEnemy，则只有第一个存活敌人放大
                if (draggingCard.targetingType == TargetingType.FirstEnemy)
                {
                    ICharacter firstEnemy = BattleManager.Instance.GetFirstAliveEnemy();
                    if (firstEnemy != null && firstEnemy is EnemyController enemyRef && enemyRef == this)
                    {
                        needEnlarge = true;
                    }

                }
                else if (draggingCard.targetingType == TargetingType.Manual)
                {
                    // 没有特殊限制时，正常放大
                    needEnlarge = true;
                }
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // 设置标记，在 LateUpdate 中处理
        needResetScale = true;
        needEnlarge = false;
    }

    public void UpdateBuffUI()
    {
        if (statusUI != null)
        {
            EnemyStatusUI ui = statusUI.GetComponent<EnemyStatusUI>();
            if (ui != null)
            {
                ui.updateBuffUI(poisonLayers, burnLayers, bleedLayers, sharpnessLayers);
            }
        }
    }


    private void LateUpdate()
    {
        if (needEnlarge)
        {
            transform.localScale = originalScale * enlargeFactor;
            needEnlarge = false;
        }
        if (needResetScale)
        {
            transform.localScale = originalScale;
            needResetScale = false;
        }
    }

}