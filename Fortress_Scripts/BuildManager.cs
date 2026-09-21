using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Pool;
using UnityEngine.UI;

public class BuildManager : MonoBehaviour
{
	public static BuildManager Instance { get; private set; }   // BuildManager のシングルトン。どこからでもアクセスできるようにする

    [Header("Turret Data")]
	public TurretData gunTurretData;                            // 砲台タワーのデータ
    public TurretData goldGeneratorData;						// 銀行タワーのデータ

    [Header("Pool")]
    public ObjectPool<GameObject> gunTurretPool;				// 砲台タワーオブジェクトのプール
    public ObjectPool<GameObject> goldGeneratorPool;			// 銀行タワーオブジェクトのプール
    public ObjectPool<GameObject> gunUpgradePool;				// 砲台タワーアップグレードオブジェクトのプール
    public ObjectPool<GameObject> goldUpgradePool;				// 銀行タワーアップグレードオブジェクトのプール
    public ObjectPool<GameObject> bulletPool;					// 弾丸のプール

    public ObjectPool<GameObject> turretBoomFXPool;				// タワー爆発エフェクトのプール
    public ObjectPool<GameObject> droneBoomFXPool;				// 敵（ドロン）の死亡する時のエフェクトプール
    public ObjectPool<GameObject> hitEnemyFXPool;				// 敵（ドロン）にヒットした時のエフェクトプール
    public ObjectPool<GameObject> gunFireFXPool;				// 砲台の発射エフェクトプール

    [Header("FX")]
	[SerializeField] GameObject turretBoomFX;					// 砲台爆発エフェクト		
    [SerializeField] GameObject droneBoomFX;					// 敵（ドロン）爆発エフェクト
    [SerializeField] GameObject hitEnemyFX;						// 敵（ドロン）にヒットした時のエフェクト
    [SerializeField] GameObject gunFireFX;                      // 砲台の発射エフェクト
    [SerializeField] int _fxTime;								// エフェクト続ける時間

    [Space]
    [SerializeField] Toggle gunTurretToggle;					// 砲台に使うトグル
    [SerializeField] Toggle goldGeneratorToggle;                // 銀行に使うトグル

    [Space]
    [SerializeField] GameObject bulletPre;						// 弾丸のプレハブ	
    [SerializeField] GameObject centralTower;					// 中央タワーのオブジェクト
    [SerializeField] GameObject centralLineRender;              // タワーのラインレンダラー
	

    TurretData _selectedData;                                   // 選択されているタワーデータ

    GameObject _followObj;                                      // プレイヤーが配置中のタワーオブジェクト

    Ray _followRay;                                             // 配置位置を調べるためのレイ
    RaycastHit _followHit;                                      // レイが当たった位置情報

    float _newRadius;                                           // タワーの設置範囲や半径の計算に使う値

    //================================ Awake =================================>
    void Awake()
	{
        // シングルトンのインスタンスを設定する
        Instance = this;

        // オブジェクトのプールを作成する
        // 第1引数：生成処理
        // 第2引数：取得時の処理
        // 第3引数：解放時の処理
        // 第4引数：破棄時の処理
        // 第5引数：コレクションチェックの有無
        // 第6引数：初期プール数
        // 第7引数：最大プール数
        gunTurretPool = new ObjectPool<GameObject>(createGunTurretFunc, actionOnGet, actionOnRelease, actionOnDestroy, true, 5, 10);            // 砲台タワーオブジェクトのプール					
        goldGeneratorPool = new ObjectPool<GameObject>(createGoldGeneratorFunc, actionOnGet, actionOnRelease, actionOnDestroy, true, 5, 10);    // 銀行タワーオブジェクトのプール
        gunUpgradePool = new ObjectPool<GameObject>(createGunUpgradeFunc, actionOnGet, actionOnRelease, actionOnDestroy, true, 5, 20);          // 砲台タワーアップグレードオブジェクトのプール
        goldUpgradePool = new ObjectPool<GameObject>(createGoldUpgradeFunc, actionOnGet, actionOnRelease, actionOnDestroy, true, 5, 10);        // 銀行タワーアップグレードオブジェクトのプール
        bulletPool = new ObjectPool<GameObject>(createBulletFunc, actionOnGet, actionOnRelease, actionOnDestroy, true, 50, 1000);               // 弾丸のプール
        turretBoomFXPool = new ObjectPool<GameObject>(createTurretBoomFXPoolFunc, actionOnGet, actionOnRelease, actionOnDestroy, true, 5, 10);  // タワー爆発エフェクトのプール
        droneBoomFXPool = new ObjectPool<GameObject>(createDroneBoomFXPoolFunc, actionOnGet, actionOnRelease, actionOnDestroy, true, 5, 10);    // 敵（ドロン）の死亡する時のエフェクトプール
        hitEnemyFXPool = new ObjectPool<GameObject>(createHitEnemyFXPoolFunc, actionOnGet, actionOnRelease, actionOnDestroy, true, 5, 10);      // 敵（ドロン）にヒットした時のエフェクトプール
        gunFireFXPool = new ObjectPool<GameObject>(createGunFireFXPoolFunc, actionOnGet, actionOnRelease, actionOnDestroy, true, 5, 10);        // 砲台の発射エフェクトプール
    }

	//================================ Update =================================>
	void Update()
	{
		if (centralTower != null)
		{
            // 中央タワーが存在する場合、タレット設置可能な半径を計算する
            // 半径の二乗を使うことで、距離判定を高速化している（sqrMagnitude 用）
            _newRadius = CentralLineRender.Instance.GetRadius()* CentralLineRender.Instance.GetRadius();
		}

        // タワーが選択されている時だけ、配置処理を行う
        if (_selectedData != null)
		{
            // マウス位置に追従するオブジェクトを動かす
            FollowMouse(_followObj);

            // 左クリックした時の処理
            if (Input.GetMouseButtonDown(0))
			{
                // UI の上をクリックした場合はキャンセルする
                if (EventSystem.current.IsPointerOverGameObject() == true)
				{
					_selectedData = null;
					return;
				}

                // 地面をクリックした場合
                else if (_followObj != null && _followHit.transform.tag == "Floor")
				{
                    // 中央タワーの設置範囲内かどうかを判定（距離の二乗で比較）
                    if ((centralTower.transform.position- _followObj.transform.position).sqrMagnitude < _newRadius)
					{
                        // 砲台タワーを設置する場合
                        if (_selectedData == gunTurretData && gunTurretData.cost <= UI_Manager.Instance.GetMyMoney())
						{
							GameObject newObj = gunTurretPool.Get();
							newObj.transform.position = _followObj.transform.position;

                            // お金を消費する
                            UI_Manager.Instance.UseMoney(gunTurretData.cost);

                            // タワーのラインレンダーを非表示にする
                            GameObject _lineRender = newObj.transform.Find("LineRender").transform.gameObject;
							if (_lineRender.activeSelf)
							{
								_lineRender.SetActive(false);
							}
						}

                        // 銀行タワーを設置する場合
                        else if (_selectedData == goldGeneratorData && goldGeneratorData.cost <= UI_Manager.Instance.GetMyMoney())
						{
							GameObject newObj = goldGeneratorPool.Get();
							newObj.transform.position = _followObj.transform.position;
							UI_Manager.Instance.UseMoney(goldGeneratorData.cost);
						}

                        // お金が足りない場合
                        else if (gunTurretData.cost > UI_Manager.Instance.GetMyMoney() || goldGeneratorData.cost > UI_Manager.Instance.GetMyMoney())
						{
							GameObject _myMoney = GameObject.Find("MyMoney");
							TextMeshProUGUI tmp = _myMoney.GetComponent<TextMeshProUGUI>();

                            // アニメーションで「お金が足りない」演出を出す
                            _myMoney.GetComponent<Animator>().SetTrigger("NoMoney");
						}
					}

                    // プレイヤー自身をクリックした場合（タワー選択解除）
                    else if (_followHit.transform.tag == "Player")
					{
						if (_followObj != null)
						{
							if (_selectedData == gunTurretData)
							{
								gunTurretToggle.GetComponent<Toggle>().isOn = false;
							}
							else if (_selectedData == goldGeneratorData)
							{
								goldGeneratorToggle.GetComponent<Toggle>().isOn = false;
							}
						}
					}
				}
			}

            // 右クリックでタレット選択を解除する
            if (Input.GetMouseButtonDown(1))
			{
				if (_followObj != null)
				{
					if (_selectedData == gunTurretData)
					{
						gunTurretToggle.GetComponent<Toggle>().isOn = false;
					}
					else if (_selectedData == goldGeneratorData)
					{
						goldGeneratorToggle.GetComponent<Toggle>().isOn = false;
					}
				}
			}
		}

	}

    //================================ Function =================================>

    // 砲台タワーが選択されるメソッド
    public void OngunTurretSelected(bool isOn)
	{
		if (isOn == true)
		{
            // 砲台タワーが選択されたので、データをセットする
            _selectedData = gunTurretData;

            // プールから仮配置用のタワーを取得する
            _followObj = gunTurretPool.Get();

            // 初期位置をマウス位置に設定（実際の配置は FollowMouse で行う）
            _followObj.transform.position = Input.mousePosition;

            // 既存のアップグレードUIが付いている場合は削除する
            if (_followObj.transform.Find("Upgrade_Destroy") != null)
			{
				Transform newObjUI = _followObj.transform.Find("Upgrade_Destroy");
				Destroy(newObjUI.gameObject);

                // 配置前はコライダーを無効化して、誤判定を防ぐ
                _followObj.transform.GetComponent<BoxCollider>().enabled = false;
				_followObj.transform.Find("Sphere").GetComponent<SphereCollider>().enabled = false;
				_followObj.transform.Find("Cube").GetComponent<BoxCollider>().enabled = false;
				_followObj.transform.Find("Sphere/Cylinder").GetComponent<CapsuleCollider>().enabled = false;
			}
		}
		else if (isOn == false)
		{
            // 選択解除されたので、データをクリアしてプールに戻す
            _selectedData = null;
			gunTurretPool.Release(_followObj);
		}
	}

    // 銀行タワーが選択されるメソッド
    public void OnGoldGeneratorSelected(bool isOn)
	{
		if (isOn == true)
		{
            // 銀行タワーが選択されたので、データをセットする
            _selectedData = goldGeneratorData;

            // プールから仮配置用のタワーを取得する
            _followObj = goldGeneratorPool.Get();

            // 配置前はお金生成を止めておく
            _followObj.transform.GetComponent<GoldGenerator>().onGetMoney = false;

            // 初期位置をマウス位置に設定
            _followObj.transform.position = Input.mousePosition;

            // アップグレードUIが付いている場合は削除する
            if (_followObj.transform.Find("Upgrade_Destroy") != null)
			{
				Transform newObjUI = _followObj.transform.Find("Upgrade_Destroy");
				Destroy(newObjUI.gameObject);

                // 配置前はコライダーを無効化して誤判定を防ぐ
                _followObj.transform.GetComponent<BoxCollider>().enabled = false;
				_followObj.transform.Find("Cube").GetComponent<BoxCollider>().enabled = false;
				_followObj.transform.Find("coin").GetComponent<CapsuleCollider>().enabled = false;
			}
		}
		else if (isOn == false)
		{
            // 選択解除されたので、データをクリアしてプールに戻す
            _selectedData = null;
			goldGeneratorPool.Release(_followObj);
		}
	}

    // マウスと一緒に移動メソッド
    public void FollowMouse(GameObject obj)
	{
		if (obj != null)
		{
            // マウス位置からレイを飛ばす
            _followRay = Camera.main.ScreenPointToRay(Input.mousePosition);

            // 地面に当たった場合のみ、仮配置オブジェクトを移動させる
            if (Physics.Raycast(_followRay, out _followHit))
			{
				if (_followHit.transform.tag == "Floor")
				{
                    // タワーをレイが当たった位置に移動
                    obj.transform.position = _followHit.point;

                    // 配置範囲を示すラインレンダーを表示する
                    GameObject followLine = obj.transform.Find("LineRender").gameObject;
					if (!followLine.gameObject.activeSelf)
					{
						followLine.gameObject.SetActive(true);
					}
				}
			}
		}
	}

    //----------------------Bullet------------------->

    // 弾の発射メソッド
    public void ReleaseBullet(GameObject bullet)
	{
        // 弾オブジェクトが存在し、かつ非アクティブ状態ならプールに戻す
        // activeSelf が false のときは、すでに画面から消えている状態
        if (bullet != null || !bullet.activeSelf)
		{
			bulletPool.Release(bullet);
		}
	}
    //---------------------------------------------->

    //---------------------Effect---------------------------->

    // 砲台タワーが破壊された時の爆発エフェクトをプレイするメソッド
    public void PlayTurretBoomFX(Vector3 Pos)
	{
        // タワーが破壊された時の爆発エフェクトを再生する
        GameObject newFX = turretBoomFXPool.Get();
        // エフェクトの位置を設定する
        newFX.transform.position = Pos;
        // 一定時間後にプールへ戻す（再利用するため）
        StartCoroutine(DisableAfterSeconds(turretBoomFXPool, newFX, _fxTime));
	}

    // 敵（ドロン）が破壊された時の爆発エフェクトをプレイするメソッド
    public void PlayDroneBoomFX(Vector3 Pos)
	{
        // 敵（ドロン）が破壊された時の爆発エフェクトを再生する
        GameObject newFX = droneBoomFXPool.Get();
        // エフェクトの位置を設定する
        newFX.transform.position = Pos;
        // 一定時間後にプールへ戻す（再利用するため）
        StartCoroutine(DisableAfterSeconds(droneBoomFXPool, newFX, _fxTime));
	}

    // 敵に弾が当たった時のヒットエフェクトをプレイするメソッド
    public void PlayHitEnemyFX(Vector3 Pos)
	{
        // 敵に弾が当たった時のヒットエフェクトを再生する
        GameObject newFX = hitEnemyFXPool.Get();
        // エフェクトの位置を設定する
        newFX.transform.position = Pos;
        // 一定時間後にプールへ戻す（再利用するため）
        StartCoroutine(DisableAfterSeconds(hitEnemyFXPool, newFX, _fxTime));
	}

    // 砲台の発射エフェクトをプレイするメソッド
    public void PlayGunFireFX(Vector3 Pos)
	{
        // 砲台の発射エフェクトを再生する
        GameObject newFX = gunFireFXPool.Get();
        // エフェクトの位置を設定する
        newFX.transform.position = Pos;
        // 一定時間後にプールへ戻す（再利用するため）
        StartCoroutine(DisableAfterSeconds(gunFireFXPool, newFX, _fxTime));
	}

    // 一定時間後にプールへ戻すメソッド
    IEnumerator DisableAfterSeconds(ObjectPool<GameObject> pool, GameObject obj, int seconds)
	{
        // 指定した秒数だけ待つ
        yield return new WaitForSeconds(seconds);
        // エフェクトをプールに戻す
        pool.Release(obj);
	}

    //------------------------------------------------------->

    // 砲台タワーのプレハブを生成する
    GameObject createGunTurretFunc()
	{
		GameObject newPoolObj = GameObject.Instantiate(gunTurretData.turretPre, transform);
		return newPoolObj;
	}

    // 銀行タワーのプレハブを生成する
    GameObject createGoldGeneratorFunc()
	{
		GameObject newPoolObj = GameObject.Instantiate(goldGeneratorData.turretPre, transform);
		return newPoolObj;
	}

    // 砲台タワーのアップグレード版プレハブを生成する
    GameObject createGunUpgradeFunc()
	{
		GameObject newPoolObj = GameObject.Instantiate(gunTurretData.turretUpgradedPre, transform);
		return newPoolObj;
	}

    // 銀行タワーのアップグレード版プレハブを生成する
    GameObject createGoldUpgradeFunc()
	{
		GameObject newPoolObj = GameObject.Instantiate(goldGeneratorData.turretUpgradedPre, transform);
		return newPoolObj;
	}

    // 弾丸のプレハブを生成する
    GameObject createBulletFunc()
	{
		GameObject newPoolObj = GameObject.Instantiate(bulletPre, transform);
		return newPoolObj;
	}

    // 砲台爆発エフェクトを生成する
    GameObject createTurretBoomFXPoolFunc()
	{
		GameObject newPoolObj = GameObject.Instantiate(turretBoomFX, transform);
		return newPoolObj;
	}

    // 敵（ドロン）爆発エフェクトを生成する
    GameObject createDroneBoomFXPoolFunc()
	{
		GameObject newPoolObj = GameObject.Instantiate(droneBoomFX, transform);
		return newPoolObj;
	}

    // 敵ヒット時のエフェクトを生成する
    GameObject createHitEnemyFXPoolFunc()
	{
		GameObject newPoolObj = GameObject.Instantiate(hitEnemyFX, transform);
		return newPoolObj;
	}

    // 砲台タワー発射エフェクトを生成する
    GameObject createGunFireFXPoolFunc()
	{
		GameObject newPoolObj = GameObject.Instantiate(gunFireFX, transform);
		return newPoolObj;
	}

    // プールから取り出した時に呼ばれる処理
    // オブジェクトをアクティブにして、ゲーム内で見えるようにする
    void actionOnGet(GameObject obj)
	{
		obj.SetActive(true);
	}

    // プールに戻す時に呼ばれる処理
    // オブジェクトを非アクティブにして、画面から消す
    void actionOnRelease(GameObject obj)
	{
		obj.SetActive(false);
	}

    // プールから完全に削除する時に呼ばれる処理
    // メモリから破棄する
    void actionOnDestroy(GameObject obj)
	{
		Destroy(obj);
	}
	//---------------------POOL END------------------------>
}
