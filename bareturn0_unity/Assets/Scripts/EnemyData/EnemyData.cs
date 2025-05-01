using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemyData", menuName = "Enemies/Enemy Data")]
public class EnemyData : ScriptableObject
{
    public string enemyName;         // 敌人名称
    public int maxHealth;            // 最大血量
    public int speed;                // 敌人速度
    //public int attackDamage;         // 攻击伤害

    public GameObject enemyPrefab;   // 对应的敌人预制体（包含外观、动画、EnemyController等）
}
