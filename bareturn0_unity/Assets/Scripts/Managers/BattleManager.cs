using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static LevelButtonManager;

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

    public List<EnemyController> enemies = new List<EnemyController>();
    public LevelData levelData;
    public GameObject enemyStatusUIPrefab;

    public Button endActionButton;  // “结束行动”按钮


    public BattleState state;
    public CardData currentDraggingCardData; //记录当前正在拖拽的卡牌数据

    public int lastAttackDamage;
    public EnemyController selectedEnemy;
    public bool isCardBeingDragged = false;

    private int roundNumber = 0;
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
        levelData = LevelButtonManager.currentLevelData;
        SetupBattle();
        StartCoroutine(RoundLoop());
    }

    private void Update()
    {
        // 其它现有逻辑

        // 调试：按下 A 键时，让第一个敌人受到伤害
        if (Input.GetKeyDown(KeyCode.A))
        {
            if (enemies.Count > 0 && enemies[0] != null)
            {
                enemies[0].TakeDamage(1); // 例如造成 1 点伤害
                Debug.Log("Debug: Pressed A, first enemy takes 1 damage.");
            }
            else
            {
                Debug.LogWarning("No enemy available to take damage.");
            }
        }

        // 调试：按下 S 键时，让玩家攻击第一个敌人并造成 1 点伤害
        if (Input.GetKeyDown(KeyCode.S))
        {
            // 确保有敌人存在
            if (enemies.Count > 0 && enemies[0] != null)
            {
                // 设置伤害为1点
                lastAttackDamage = 3;
                // 将第一个敌人作为选中的目标
                selectedEnemy = enemies[0];
                // 触发玩家的攻击动画（动画事件中会调用 TriggerEnemyHit()，对 selectedEnemy 造成伤害）
                player.Attack();

                Debug.Log("Pressed S: Player attacks first enemy for 1 damage.");
            }
            else
            {
                Debug.LogWarning("No enemy available to be attacked.");
            }
        }

    }

    private void SetupBattle()
    {
        //初始化玩家和敌人血量
        player.currentHealth = player.maxHealth;

        // 生成关卡中的所有敌人
        SpawnEnemies();

        //初始化抽牌堆
        deckManager.SetupInitialDeck();

        state = BattleState.RoundStart;
    }

    // 根据 LevelData 中存储的敌人数据逐个生成敌人
    public void SpawnEnemies()
    {
        if (levelData == null)
        {
            Debug.LogError("LevelData is not assigned in BattleManager!");
            return;
        }

        // 例如，这里我们为每个敌人生成一个位置（可根据实际需求修改）
        // 此处简单安排在一个横向排列的位置
        float startX = 2;
        float offsetX = 2.5f;
        Vector3 spawnPos = Vector3.zero;

        for (int i = 0; i < levelData.enemyDatas.Count; i++)
        {
            EnemyData enemyData = levelData.enemyDatas[i];

            // 根据索引计算生成位置，可以替换为更复杂的算法
            spawnPos = new Vector3(startX + offsetX * i, 0, 0);
            SpawnEnemy(enemyData, spawnPos);
        }
    }

    // 根据单个 EnemyData 实例化敌人预制体并初始化属性
    public void SpawnEnemy(EnemyData enemyData, Vector3 spawnPos)
    {
        // 1. 实例化敌人
        if (enemyData == null || enemyData.enemyPrefab == null)
        {
            Debug.LogError("SpawnEnemy: Invalid EnemyData or prefab is null.");
            return;
        }

        GameObject enemyObj = Instantiate(enemyData.enemyPrefab, spawnPos, Quaternion.identity);
        EnemyController enemyController = enemyObj.GetComponent<EnemyController>();
        if (enemyController != null)
        {
            enemyController.Initialize(enemyData);
            enemies.Add(enemyController);
        }
        else
        {
            Debug.LogWarning("SpawnEnemy: The instantiated prefab does not have an EnemyController.");
        }

        // 2. 实例化敌人状态UI
        if (enemyStatusUIPrefab != null && enemyController != null)
        {
            // 计算血量UI位置（例如在敌人头顶偏移1.5个单位，根据实际情况调整）
            Vector3 uiPos = spawnPos + new Vector3(0, 1.5f, 0);

            // 将状态UI实例化，并设置其父对象为敌人的transform
            GameObject statusUIObj = Instantiate(enemyStatusUIPrefab, uiPos, Quaternion.identity, enemyObj.transform); 

            // 保存到敌人控制器中，便于后续更新
            enemyController.statusUI = statusUIObj;

            // 初始化状态UI
            EnemyStatusUI statusUI = statusUIObj.GetComponent<EnemyStatusUI>();
            if (statusUI != null)
            {
                statusUI.UpdateStatus(enemyController.currentHealth, enemyController.maxHealth, 0);
            }
            else
            {
                Debug.LogWarning("SpawnEnemy: EnemyStatusUI component not found on statusUIObj.");
            }
        }

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

        //敌人动作
        yield return StartCoroutine(enemy.ExecuteTurn());

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
            BattleUIManager.Instance.ShowBattleResult(true);
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

    public ICharacter GetFirstAliveEnemy()
    {
        foreach (EnemyController enemy in enemies)
        {
            if (enemy != null && enemy.currentHealth > 0)
                return enemy;
        }
        return null;
    }

    public EnemyController GetLowestHPEnemy()
    {
        EnemyController lowest = null;
        foreach (EnemyController enemy in enemies)
        {
            if (enemy != null && enemy.currentHealth > 0)
            {
                if (lowest == null || enemy.currentHealth < lowest.currentHealth)
                {
                    lowest = enemy;
                }
            }
        }
        return lowest;
    }




    //当玩家使用一张牌
    public bool UseCard(CardData cardData, CardView cardView, ICharacter targetCharacter)
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
            return false; // 中止，不执行后续
        }
        Debug.Log("Enough Energy");

        // 2. 扣除能量
        player.currentEnergy -= cardData.cost;

        // 3. 执行效果
        if (cardData.targetingType == TargetingType.FirstEnemy)
        {
            targetCharacter = GetFirstAliveEnemy();
        }

        if (cardData.cardEffect != null)
        {
            cardData.cardEffect.ApplyEffect(this, cardData, player, targetCharacter);
        }
        // 4. 把卡从手牌移到弃牌堆
        deckManager.Discard(cardData);

        // 5.Destroy卡牌UI
        Destroy(cardView.gameObject);
        return true;
    }
}