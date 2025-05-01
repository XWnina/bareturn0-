using UnityEngine;

[CreateAssetMenu(menuName = "Card Effects/Summon Drag")]
public class SummonDragEffect : CardEffect
{
    public EnemyData dragData;  // 在 Inspector 指向 EnemyData_Drag
    public Vector3 spawnOffset = new Vector3(-2.5f, 0f, 0f);

    public override bool RequiresTarget() => false;

    public override void ApplyEffect(BattleManager bm, CardData cd, ICharacter caster, ICharacter target = null)
    {
        // 在 Boss 身边或场地某处召唤
        Vector3 bossPos = (caster as EnemyController).transform.position;

        if (caster is NecroController necro)
        {
            necro.Summon();
        }
        int existingDragCount = 0;

        foreach (var enemy in bm.enemies)
        {
            if (enemy != null && enemy.enemyName == "Drag")  // 请确保 Drag 敌人Prefab的 enemyName是 "Drag"
            {
                existingDragCount++;
            }
        }

        if (existingDragCount >= 2)
        {
            Debug.Log("已有两只小鬼，无法继续召唤！");
            return;
        }

        Vector3 spawnPos = bossPos + spawnOffset;

        if (existingDragCount == 1)
        {
            spawnPos = bossPos + spawnOffset * 2f;  // 第二只小鬼生成在2倍偏移
        }

        bm.SpawnEnemy(dragData, spawnPos);
    }
}