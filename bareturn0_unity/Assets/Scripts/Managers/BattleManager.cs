using System.Collections;
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
    public EnemyController enemy;          // 敌人控制脚本，包含血量、AI等

    public Button endActionButton;  // 场景中的“结束行动”按钮


    public BattleState state;

    private int roundNumber = 0;
    public int lastAttackDamage;

    public int CurrentRoundNumber
    {
        get { return roundNumber; }
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
        enemy.currentHealth = enemy.maxHealth;

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

            //2. 抽牌
            deckManager.DrawCard(5);

            //3. 判断顺序
            int playerSpeed = player.speed;
            int enemySpeed = enemy.speed;
            bool playerGoesFirst = (playerSpeed >= enemySpeed);

            //4. 先手行动
            if (playerGoesFirst)
            {
                //玩家回合
                state = BattleState.PlayerAction;
                yield return StartCoroutine(PlayerActionPhase());
                result = CheckWinLose();
                if (result != 0)
                {
                    break;
                }
                //敌人回合
                state = BattleState.EnemyAction;
                yield return StartCoroutine(EnemyActionPhase());
                result = CheckWinLose();
                if (result != 0)
                {
                    break;
                }
            }
            else
            {
                //敌人回合
                state = BattleState.EnemyAction;
                yield return StartCoroutine(EnemyActionPhase());
                result = CheckWinLose();
                if (result != 0)
                {
                    break;
                }
                //玩家回合
                state = BattleState.PlayerAction;
                yield return StartCoroutine(PlayerActionPhase());
                result = CheckWinLose();
                if (result != 0)
                {
                    break;
                }
            }

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
        while (!isPlayerDone && player.currentHealth > 0 && enemy.currentHealth > 0)
        {
            yield return null;
        }

        // 隐藏按钮
        endActionButton.gameObject.SetActive(false);
        Debug.Log("Player Turn End");

    }

    // 敌人行动阶段
    IEnumerator EnemyActionPhase()
    {
        Debug.Log(">>> Enemy Turn (action phase) <<<");
        // 简单示例：敌人动作
        enemy.PerformAction();

        // 等1秒模拟动画
        yield return new WaitForSeconds(2f);
        Debug.Log("Enemy Turn End");
    }

    // 回合结束，弃牌等
    IEnumerator EndRound()
    {
        Debug.Log(">>> Round End: Discard All Player Cards...");
        deckManager.DiscardAllHand();

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
        else if (enemy.currentHealth <= 0)
        {
            Debug.Log("Enemy HP = 0, Win");
            state = BattleState.BattleEnd;
            BattleUIManager.Instance.ShowBattleResult(true);
            return 1;
        }
        return 0;
    }

    //Triiger hit
    public void TriggerEnemyHit()
    {
        enemy.TakeDamage(lastAttackDamage);
    }

    public void TriggerPlayerHit()
    {
        player.TakeDamage(lastAttackDamage); // 让玩家播放受击动画并扣血
    }




    //当玩家使用一张牌
    public void UseCard(CardData cardData, CardView cardView)
    {
        //0. 检查是否为玩家回合
        if (state != BattleState.PlayerAction)
        {
            return;
        }
        // 1. 检查能量
        if (player.currentEnergy < cardData.cost)
        {
            Debug.Log("Not enough energy to use " + cardData.cardName);
            BattleUIManager.Instance.ShowEnergyWarning(); //Show Warning
            return; // 中止，不执行后续
        }

        // 2. 扣除能量
        player.currentEnergy -= cardData.cost;

        // 3. 执行效果 TODO
        switch (cardData.cardType)
        {
            case CardType.Attack:
                player.Attack();
                lastAttackDamage = cardData.damage;
                break;
            case CardType.Defend:
                // 给玩家增加护甲/防御
                player.GainArmor(cardData.damage);
                // 这里我暂用 cardData.damage当作防御值，你可换成cardData.armor
                break;
            case CardType.Heal:
                player.Heal(cardData.damage);
                break;
                // 其它类型请自行扩展
        }

        // 4. 把卡从手牌移到弃牌堆
        deckManager.Discard(cardData);

        // 5.Destroy卡牌UI
        Destroy(cardView.gameObject);

        // 6. 检查敌人是否死亡
        if (enemy.currentHealth <= 0)
        {
            Debug.Log("Enemy is defeated!");
        }
    }


}