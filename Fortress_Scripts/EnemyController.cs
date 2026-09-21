using UnityEngine;
using UnityEngine.UI;

public class EnemyController : MonoBehaviour
{
	[SerializeField] Slider _enemyHPUI;             // 敵のHPを表示するUIスライダー
    [SerializeField] GameObject _centrelTowerPre;	// 中央タワーのオブジェクト
    [SerializeField] float _enemy_A_Damage;			// 敵がタワーに与えるダメージ量
    [SerializeField] float _minMoveSpeed;			// 敵の最低移動速度
    [SerializeField] float _MaxMoveSpeed;           // 敵の最大移動速度

    float _moveSpeed;								// 実際に使用される移動速度
    float _enemyHP;                                 // 敵のHP
    float _enemyDeadHP = 0;							// HPが0以下で死亡扱い

    //================================ Start =================================>
    void Start()
	{
		_enemyHP = GameController.Instance.NewEnemyHP();            // 新しい敵の初期HPを取得
        _enemyHPUI.maxValue = _enemyHP;                             // HPバーの最大値を設定
        _moveSpeed = Random.Range(_minMoveSpeed, _MaxMoveSpeed);    // ランダムで敵の移動速度を決定
    }
	void OnCollisionEnter(Collision collision)
	{
        // 中央タワーに当たった場合
        if (collision.gameObject.CompareTag("CentralTower"))
		{
            // 敵が死亡する
            _enemyHP = _enemyDeadHP;

            // タワーにダメージを与える
            CentralTower.Instance.CentralTowerTakeDamage(_enemy_A_Damage);
		}

        // 砲台Aに当たった場合
        else if (collision.gameObject.CompareTag("Gun_A"))
		{
            // 敵が死亡する
            _enemyHP = _enemyDeadHP;

            // タワーの爆発エフェクトをプレイする
            BuildManager.Instance.PlayTurretBoomFX(collision.gameObject.transform.position);

            // 砲台Aをプールに戻す
            BuildManager.Instance.gunTurretPool.Release(collision.gameObject);
		}
        // 砲台Bに当たった場合
        else if (collision.gameObject.CompareTag("Gun_B"))
		{
            // 敵が死亡する
            _enemyHP = _enemyDeadHP;

            // タワーの爆発エフェクトをプレイする
            BuildManager.Instance.PlayTurretBoomFX(collision.gameObject.transform.position);
            // 砲台Bをプールに戻す
            BuildManager.Instance.gunUpgradePool.Release(collision.gameObject);
		}
        // 銀行Aに当たった場合
        else if (collision.gameObject.CompareTag("Gold_A"))
		{
            // 敵が死亡する
            _enemyHP = _enemyDeadHP;

            // タワーの爆発エフェクトをプレイする
            BuildManager.Instance.PlayTurretBoomFX(collision.gameObject.transform.position);

            // 銀行Aをプールに戻す
            BuildManager.Instance.goldGeneratorPool.Release(collision.gameObject);
		}
        // 銀行Bに当たった場合
        else if (collision.gameObject.CompareTag("Gold_B"))
		{
            // 敵が死亡する
            _enemyHP = _enemyDeadHP;

            // タワーの爆発エフェクトをプレイする
            BuildManager.Instance.PlayTurretBoomFX(collision.gameObject.transform.position);

            // 銀行Bをプールに戻す
            BuildManager.Instance.goldUpgradePool.Release(collision.gameObject);
		}


	}

	//================================ Update =================================>
	void Update()
	{
        // HPバーを更新する
        _enemyHPUI.value = this._enemyHP;

        // タワーの方向を向く
        transform.LookAt(_centrelTowerPre.transform.position);

        // 前進移動
        transform.Translate(Vector3.forward * _moveSpeed * Time.deltaTime);

        // HPが0以下になったら死亡処理
        if (this.gameObject != null && _enemyHP <= _enemyDeadHP)
		{
            //敵が死亡する
			EnemyDead();

            // 敵のHPをリセット（再利用のため）
            _enemyHP = GameController.Instance.NewEnemyHP();
		}
	}

    //================================ Function =================================>

    // ダメージを受けるメソッド
    public void TakeDamage(float damage)
	{
        // ダメージを受けるエフェクトをプレイする
        BuildManager.Instance.PlayHitEnemyFX(this.gameObject.transform.position);

        // ダメージを受ける
        _enemyHP -= damage;
	}

    // 敵が死亡するメソッド
	void EnemyDead()
	{
        // 敵死亡エフェクトをプレイする
        BuildManager.Instance.PlayDroneBoomFX(this.gameObject.transform.position);

        // 敵を削除（プールへ返す）
        GameController.Instance.DestroyEnemy(this.gameObject);

        // UIの死亡数を更新
        UI_Manager.Instance.DeadEnemyCount();

        // 各リストから敵を削除
        if (GameController.Instance.enemyList != null)
		{
			GameController.Instance.enemyList.Remove(this.gameObject);
		}
		if (GunTurret.Instance != null)
		{
			GunTurret.Instance.detectedEnemyList.Remove(this.gameObject);
		}
		if (GunUpgrade.Instance != null)
		{
			GunUpgrade.Instance.detectedEnemyList.Remove(this.gameObject);
		}
	}
}
