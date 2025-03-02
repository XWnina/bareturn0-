using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum BattleState
{
    RoundStart,
    PlayerAction,
    EnemyAction,
    RoundEnd,
    BattleEnd
}

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance;

    public DeckManager deckManager;        // 管理抽牌、弃牌
    public PlayerController player;        // 玩家控制脚本，包含当前能量、血量等

    public List<EnemyController> enemies;    // 敌人控制脚本，包含血量、AI等

    public Button endActionButton;  // “结束行动”按钮


    public BattleState state;

    private int roundNumber = 0;
    public int lastAttackDamage;
    public EnemyController selectedEnemy;
    public bool isCardBeingDragged = false;


    public int CurrentRoundNumber
    {
        get { return roundNumber; }
    }

    // 用于构建回合顺序的内部类
    private class TurnOrderEntry
    {
        public bool isPlayer;
        public EnemyController enemy; // 如果 isPlayer 为 false，则 enemy 不为 null
        public int speed;
    }


    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        SetupBattle();
        StartCoroutine(RoundLoop());
    }

    private void SetupBattle()
    {
        //初始化玩家和敌人血量
        player.currentHealth = player.maxHealth;

        // 初始化所有敌人：重置血量（你也可以在这里设置外观和动画）
        foreach (var enemy in enemies)
        {
            enemy.currentHealth = enemy.maxHealth;
            // 初始化敌人外观或动画（例如播放 idle 动作）
        }

        //初始化抽牌堆
        deckManager.SetupInitialDeck();

        state = BattleState.RoundStart;
    }

    // 核心：回合循环
    IEnumerator RoundLoop()
    {
        int result = 0;
        while (state != BattleState.BattleEnd)
        {
            roundNumber++;
            Debug.Log("---- Round " + roundNumber + " Start ----");

            //1. 赋予玩家能量
            if (roundNumber == 1)
            {
                // 第一回合给initialEnergy
                player.currentEnergy = player.initialEnergy;
            }
            else
            {
                // 之后的回合给energyGainPerRound
                player.currentEnergy += player.energyGainPerRound;
            }
            Debug.Log($"Player's energy = {player.currentEnergy}");


            // 2. 构建回合顺序列表：包括玩家和所有存活的敌人
            List<TurnOrderEntry> turnOrder = new List<TurnOrderEntry>();
            // 添加玩家
            turnOrder.Add(new TurnOrderEntry { isPlayer = true, enemy = null, speed = player.speed });
            // 添加敌人（只添加存活的敌人）
            foreach (var enemy in enemies)
            {
                if (enemy.currentHealth > 0)
                {
                    turnOrder.Add(new TurnOrderEntry { isPlayer = false, enemy = enemy, speed = enemy.speed });
                }
            }
            // 根据速度从高到低排序
            turnOrder.Sort((a, b) => b.speed.CompareTo(a.speed));

            // 3. 按顺序执行各个参与者的回合
            foreach (var entry in turnOrder)
            {
                if (entry.isPlayer)
                {
                    state = BattleState.PlayerAction;
                    yield return StartCoroutine(PlayerActionPhase());
                    if (CheckWinLose() != 0)
                        break;
                }
                else
                {
                    state = BattleState.EnemyAction;
                    yield return StartCoroutine(EnemyActionPhase(entry.enemy));
                    if (CheckWinLose() != 0)
                        break;
                }
            }
            if (CheckWinLose() != 0) break;



            // 5. 回合结束
            state = BattleState.RoundEnd;
            yield return EndRound();
            // 检查胜负
            result = CheckWinLose();
            if (result != 0)
            {
                break;
            }
        }

        Debug.Log("Battle ended...");
        if (result == 1)
        {
            Debug.Log("player win.");
        }
        else if (result == -1)
        {
            Debug.Log("player lose.");
        }
    }

    // 玩家行动阶段
    IEnumerator PlayerActionPhase()
    {
        Debug.Log(">>> Player Turn <<<");
        bool isPlayerDone = false;

        //抽牌
        deckManager.DrawCard(5);
        // 打开按钮
        endActionButton.gameObject.SetActive(true);
        // 先清除旧的监听，以防残留
        endActionButton.onClick.RemoveAllListeners();
        // 添加新的监听事件
        endActionButton.onClick.AddListener(() =>
        {
            isPlayerDone = true;
        });

        // 等待玩家点击“结束行动”按钮
        while (!isPlayerDone && player.currentHealth > 0 && !AllEnemiesDefeated())
        {
            yield return null;
        }

        // 隐藏按钮
        endActionButton.gameObject.SetActive(false);
        Debug.Log("Player Turn End");

        // 玩家回合结束时，弃掉所有手牌
        deckManager.DiscardAllHand();

    }

    // 敌人行动阶段
    IEnumerator EnemyActionPhase(EnemyController enemy)
    {
        Debug.Log(">>> Enemy Turn: {enemy.name} <<<");
        // 简单示例：敌人动作
        enemy.PerformAction();

        // 等1秒模拟动画
        yield return new WaitForSeconds(2f);
        Debug.Log("Enemy Turn End: {enemy.name}");
    }

    // 回合结束，弃牌等
    IEnumerator EndRound()
    {
        Debug.Log(">>> Round End:");
        //deckManager.DiscardAllHand();

        // TODO这里可以做一些buff计时或毒伤结算

        yield return null;
        Debug.Log("Round End done");
    }

    //检查胜负
    public int CheckWinLose()
    {
        if (player.currentHealth <= 0)
        {
            Debug.Log("Player HP = 0, Lose");
            state = BattleState.BattleEnd;
            BattleUIManager.Instance.ShowBattleResult(false);
            return -1;
        }
        else if (AllEnemiesDefeated())
        {
            Debug.Log("All enemies defeated, Win");
            state = BattleState.BattleEnd;
            return 1;
        }
        return 0;
    }

    private bool AllEnemiesDefeated()
    {
        foreach (var enemy in enemies)
        {
            if (enemy.currentHealth > 0)
                return false;
        }
        return true;
    }

    //Triiger hit
    public void TriggerEnemyHit()
    {
        if (selectedEnemy != null)
        {
            selectedEnemy.TakeDamage(lastAttackDamage);
            Debug.Log($"Enemy {selectedEnemy.name} hit for {lastAttackDamage} damage.");
            selectedEnemy = null;
        }
        else
        {
            Debug.Log("No enemy target for TriggerEnemyHit.");
        }
    }

    public void TriggerPlayerHit()
    {
        player.TakeDamage(lastAttackDamage); // 让玩家播放受击动画并扣血
    }




    //当玩家使用一张牌
    public bool UseCard(CardData cardData, CardView cardView, EnemyController targetEnemy)
    {
        Debug.Log("useCard");
        //0. 检查是否为玩家回合
        if (state != BattleState.PlayerAction)
        {
            return false;
        }
        // 1. 检查能量
        if (player.currentEnergy < cardData.cost)
        {
            Debug.Log("Not enough energy to use " + cardData.cardName);
            BattleUIManager.Instance.ShowEnergyWarning(); //Show Warning
            if (targetEnemy != null)
            {
                targetEnemy.ClearHighlight();
            }
            return false; // 中止，不执行后续
        }
        Debug.Log("Enough Energy");

        // 2. 扣除能量
        player.currentEnergy -= cardData.cost;

        if (targetEnemy != null)
        {
            targetEnemy.ClearHighlight();
        }

        // 3. 执行效果
        if (cardData.cardEffect != null)
        {
            cardData.cardEffect.ApplyEffect(this, cardData, targetEnemy);
        }
        // 4. 把卡从手牌移到弃牌堆
        deckManager.Discard(cardData);

        // 5.Destroy卡牌UI
        Destroy(cardView.gameObject);
        return true;
    }
}