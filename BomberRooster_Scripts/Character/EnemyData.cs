using System;

public enum EnemyType   // 敵の種類
{
    Enemy_01,           // 敵_01
    Enemy_02,           // 敵_02
    Enemy_03,           // 敵_03
}

[Serializable]
public class EnemyStatsData     // 敵のデータ
{
    public EnemyType EnemyType;
    public float hp;            // 体力
    public float speed;         // 移動速度
    public float findRange;     // プレイヤーを見つかる距離
}

[Serializable]
public class SpawnEnemyData     // 敵が生まれる用のデータ
{
    public EnemyType EnemyType;
    public float spawnRange;    // 生まれる時プレイヤーとの距離
    public int spawnCount;      // 生まれる時の数
    public int spawnCondition;  // 生まれる条件
}
