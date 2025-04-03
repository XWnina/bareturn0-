using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewLevelData", menuName = "Level/Level Data")]
public class LevelData : ScriptableObject
{
    // 关卡中所有敌人的数据列表
    public List<EnemyData> enemyDatas;
    public int processNum;

    // 可选：如果你希望为每个敌人指定一个特定的生成位置，也可以添加一个位置列表，
    // 或者在 EnemyData 中添加一个 Vector3 spawnPosition 字段（如果每个敌人都在预定位置生成）
}
