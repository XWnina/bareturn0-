using System.Collections;
using UnityEngine;

public class NecroController : EnemyController
{
    [Header("Necro Animator 专用")]
    [SerializeField] private NecroAnimator necroAnimator;

    protected override void Awake()
    {
        base.Awake();
        // Necro专属初始化
    }

    public override IEnumerator ExecuteTurn()
    {
        enemyHand.Clear();

        for (int i = 0; i < drawCount; i++)
        {
            if (enemyDeck.Count > 0)
            {
                int index = Random.Range(0, enemyDeck.Count);
                enemyHand.Add(enemyDeck[index]);
            }
        }

        foreach (CardData card in enemyHand)
        {
            yield return new WaitForSeconds(1f);

            ICharacter target = BattleManager.Instance.player;

            // 根据卡牌名称选择不同动画
            //if (card.cardName.Contains("SummonDarg"))
            //{
            //    necroAnimator.NecroSummonAnimation();
            //}
            //else if (card.cardName.Contains("BurnBurst"))   //  燃爆
            //{
            //    necroAnimator.NecroAttack1Animation();
            //}

            yield return new WaitForSeconds(1f); // 等动画演出时间

            // 出牌逻辑
            card.cardEffect.ApplyEffect(BattleManager.Instance, card, this, target);

            if (statusUI != null)
            {
                var ui = statusUI.GetComponent<EnemyStatusUI>();
                if (ui != null)
                    ui.ShowCardName(card.cardName);
            }
        }

        enemyHand.Clear();
    }

    public override void Attack()
    {
        // Necro的小火球攻击
        if (necroAnimator != null)
            necroAnimator.EnemyAttackAnimation(); // 这里可以专门给NecroAnimator加小火球方法，如果要区分Attack0

        //Debug.Log($"{gameObject.name} uses small fireball attack!");
    }

    public void Attack1()
    {
        if (necroAnimator != null)
            necroAnimator.NecroAttack1Animation();
    }

    public void Summon()
    {
        necroAnimator.NecroSummonAnimation();
    }

    public override void TakeDamage(int damage)
    {
        int effectiveDamage = Mathf.Max(damage - currentArmor, 0);
        currentArmor = Mathf.Max(currentArmor - damage, 0);
        currentHealth -= effectiveDamage;

        Vector3 pos = transform.position;
        BattleManager.Instance.ShowFloatingValue(pos, effectiveDamage);

        if (necroAnimator != null)
            necroAnimator.EnemyHurtAnimation();

        if (statusUI != null)
        {
            var ui = statusUI.GetComponent<EnemyStatusUI>();
            if (ui != null)
                ui.UpdateStatus(currentHealth, maxHealth, currentArmor);
        }

        if (currentHealth <= 0)
        {
            StartCoroutine(HandleDeath());
        }

        if (BattleManager.Instance.markActive && BattleManager.Instance.markedEnemy == this)
        {
            BattleManager.Instance.player.ApplySharpness(1);
            Debug.Log("追踪生效：玩家获得1层锐利");
        }
    }

    protected override IEnumerator HandleDeath()
    {
        if (necroAnimator != null)
            necroAnimator.EnemyDeathAnimation();

        yield return new WaitForSeconds(1.5f); // 播放死亡动画时间
        Destroy(gameObject);
    }


}
