using UnityEngine;

public class EnemyController : MonoBehaviour
{
    static readonly int isEnemtyDamage = Animator.StringToHash("EnemyDamage");  //「敵の傷つく」アニメの名前をIDに変える
    static readonly int isEnemtyDie = Animator.StringToHash("EnemyDie");        //「敵の死ぬ」アニメの名前をIDに変える　

    [SerializeField] EnemyStatsData _statsData; // 敵の統計データ

    [Space]
    [SerializeField] Renderer _renderer;        // 敵のRenderer
    [SerializeField] Animator _anime;           // 敵のアニメ
    [SerializeField] ParticleSystem _deadFX;    // 敵の死ぬときのエフェクト
    [SerializeField] Collider _collider;        // 敵のCollider

    public bool _isEnemyJured = false;          // 敵爆弾に当たる

    float _startSpeed;                          // 敵が生まれる時の移動速度

    Transform _transform;                       // 敵自身のtransform

    Material _material;                         // 敵のマテリアル
    Color _color;                               // 敵の色

    //======================== Start =========================>
    void Start()
    {
        _transform = transform;                 // 敵自身のtransform を指定
        _startSpeed = _statsData.speed;         // 初期のスピードを保存
        _material = _renderer.material;         // 初期マテリアルを保存
        _color = _material.color;               // 初期マテリアルの色を保存
    }

    //======================== Update =========================>
    void Update()
    {　
        //　プレイヤーがいるかの確認
        if (PlayerController.instance != null)
        {
            //　プレイヤーとの距離の判断
            if ((_transform.position - PlayerController.instance.transform.position).sqrMagnitude < _statsData.findRange * _statsData.findRange)
            {
                // プレイヤーへ向かって移動する
                MoveToPlayer();
            }
        }
    }

    //======================== Functions =========================>

    // プレイヤーへ向かって移動する
    void MoveToPlayer()
    {
        // プレイヤーの方向
        Vector3 toPlayer = PlayerController.instance.transform.position - _transform.position;
        // 回転
        _transform.forward = toPlayer;
        // 前進
        _transform.position += _transform.forward * _statsData.speed * Time.deltaTime;
    }

    // 爆弾に当たる
    public void InJured(float damage)
    {
        //　爆弾に当たのかの確認
        if (_isEnemyJured)
        {
            return;
        }

        _isEnemyJured = true;
        //　体力を減らす
        _statsData.hp -= damage;
        //　傷つくアニメを放送
        _anime.SetTrigger(isEnemtyDamage);
        //　HP が 0 以下で、オブジェクトが存在していてアクティブの判断
        if (_statsData.hp <= 0 && gameObject.activeSelf && gameObject != null)
        {
            //　敵が死亡
            GameController.instance.EnemyDead();
            //　敵を生み出す条件で敵を生み出す
            GameController.instance.SpawnCondition();
            //　スピードを０に設定
            _statsData.speed = 0;
            //　スピードを０に設定
            _collider.enabled = false;
            //　死亡時のエフェクトをプレー
            _deadFX.Play();
            //　死亡時のアニメをプレー
            _anime.SetTrigger(isEnemtyDie);
            GamePlayUIController.instance.PlayEnmeyDeadAnime();

            //　どの敵かの判断
            switch (_statsData.EnemyType)
            {
                case EnemyType.Enemy_01:
                    Invoke(nameof(Enemy_01Die), 1.2f);
                    GameController.instance.SpawnEnemy(PoolManager.instance._enemy_01Pool,2);
                    break;
                case EnemyType.Enemy_02:
                    Invoke(nameof(Enemy_02Die), 1.2f);
                    GameController.instance.SpawnEnemy(PoolManager.instance._enemy_02Pool,2);
                    break;
                case EnemyType.Enemy_03:
                    Invoke(nameof(Enemy_03Die), 1.2f);
                    GameController.instance.SpawnEnemy(PoolManager.instance._enemy_03Pool,2);
                    break;
            }
        }
        //　0.5秒あと爆弾にあってないになる
        Invoke(nameof(NoJured), 0.5f);
    }

    //------------------------EnemyDie-------------------------->

    // 「敵_01」が死ぬ
    public void Enemy_01Die()
    {
        //　敵が死ぬ
        Enemy_die();
        //　「敵_01」をプールに戻す
        PoolManager.instance._enemy_01Pool.Release(gameObject);
    }

    // 「敵_02」が死ぬ
    public void Enemy_02Die()
    {
        //　敵が死ぬ
        Enemy_die();
        //　「敵_02」をプールに戻す
        PoolManager.instance._enemy_02Pool.Release(gameObject);
    }

    // 「敵_03」が死ぬ
    public void Enemy_03Die()
    {
        //　敵が死ぬ
        Enemy_die();
        //　「敵_03」をプールに戻す
        PoolManager.instance._enemy_03Pool.Release(gameObject);
    }

    // 敵が死ぬ
    void Enemy_die()
    {
        //　「collider」をオン
        _collider.enabled = true;
        //　色のアルファードを１にする
        _color.a = 1f;
        //　マテリアルの色を設定する
        _material.color = _color;
        //　元の移動スピードに戻す
        _statsData.speed = _startSpeed;
        //　死亡エフェクトをストップ
        _deadFX.Stop();
    }
    //------------------------------------------------------->
    // 爆弾にあたってない
    public void NoJured()
    {
        //　爆弾にあたってない
        _isEnemyJured = false;
    }
}