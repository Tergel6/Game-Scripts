using System;
using System.Collections.Generic;
using UnityEngine;

public class GunUpgrade : MonoBehaviour
{
	public static GunUpgrade Instance { get; private set; }

    public List<GameObject> detectedEnemyList = new List<GameObject>();	// 射程内に入った敵を保存するリスト
    [Space]
    [SerializeField] GameObject _backMoney;								// 破壊時に表示する返金 UI
    [SerializeField] GameObject _upgradeDestroyUI;                      // アップグレード／破壊ボタンをまとめた UI
    [Space]
    [SerializeField] GameObject _bulletPos;								// 弾を生成する位置
    [SerializeField] GameObject _gunDirectionOBJ;						// 砲台の向きを制御するオブジェクト
    [SerializeField] Animator _fireAnime02;								// 発射アニメーション	
    [SerializeField] GameObject _lineRender;							// 射程ラインの表示オブジェクト
    [Space]
    [SerializeField] int _getMoney;										// 破壊時に返す金額
    [SerializeField] float _uiYPos;										// 返金 UI を表示する高さ（Y座標）
    [SerializeField] float _delayTime;                                  // 遅延タイム
    [SerializeField] float _zeroSpawnTime;                              // 発射後に spawnTime をリセットする値
    [Space]
    [SerializeField] float _bulletTime;									// 弾を発射する間隔（秒）
    [SerializeField] float _spawnTime;									// 弾の発射タイミングを管理する時間
    [SerializeField] float _enemyRange;									// 敵を検知する射程距離（平方距離で比較）

    bool _buttonSwitch = false;                                         // UI の開閉状態を管理するフラグ

    //================================ Awake =================================>
    void Awake()
	{
		Instance = this;
	}

	//================================ Start =================================>
	void Start()
	{
        // アップグレード／破壊 UI を初期状態で非表示にする
        if (_upgradeDestroyUI != null)
		{
			_upgradeDestroyUI.SetActive(false);
		}
	}

	//================================ Update =================================>
	void Update()
	{
        // 左クリックした時、他のオブジェクトを押したら UI を閉じる処理
        if (Input.GetMouseButtonDown(0))
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if (Physics.Raycast(ray, out hit))
			{
				if (_upgradeDestroyUI != null && hit.transform.gameObject != gameObject && hit.transform.gameObject != _upgradeDestroyUI)
				{
					_upgradeDestroyUI.SetActive(false);

                    // 射程ラインも閉じる
                    if (_lineRender.gameObject.activeSelf)
					{
						_lineRender.gameObject.SetActive(false);
					}
				}
			}
		}
        //--------------------Find Enemy ------------------------->

        // 砲台タワーが設置済みの場合のみ敵を検知する
        if (transform.Find("Upgrade_Destroy") != null)
		{
			detectedEnemyList.Clear();

            // 射程内の敵を検知する（平方距離で高速比較）
            foreach (GameObject enemy in GameController.Instance.enemyList)
			{
				if (((gameObject.transform.position - enemy.transform.position).sqrMagnitude) < _enemyRange)
				{
					detectedEnemyList.Add(enemy);
				}
			}
			if (detectedEnemyList.Count > 0)
			{

                // 射程外に出た敵をリストから削除する
                for (int i = detectedEnemyList.Count - 1; i >= 0; i--)
				{
					if (((gameObject.transform.position - detectedEnemyList[i].transform.position).sqrMagnitude) > _enemyRange)
					{
						detectedEnemyList.RemoveAt(i);
					}
				}
			}
		}
		//-------------------SpawnBullet--------------->
		if (detectedEnemyList.Count > 0)
		{
            // 最も近い敵の方向を向く
            _gunDirectionOBJ.transform.LookAt(detectedEnemyList[0].transform.position);

            // 弾の発射タイミングを管理する
            _spawnTime += Time.deltaTime;
			if (_spawnTime > _bulletTime)
			{
                // 弾を Pool から取得して発射する
                GameObject newBullet = BuildManager.Instance.bulletPool.Get();
				newBullet.transform.position = _bulletPos.transform.position;
				newBullet.GetComponent<Bullet>().TakeTarget(detectedEnemyList[0]);
				BuildManager.Instance.PlayGunFireFX(_bulletPos.transform.position);
				_fireAnime02.SetTrigger("IsFire");

                // 発射タイミングをリセット
                _spawnTime = _zeroSpawnTime;
			}
		}

	}

    //================================ Function =================================>

    public void OnMouseDown()
    {
        // UI が存在しない場合は何もしない
        if (_upgradeDestroyUI == null) return;

        // UI の開閉状態を反転する（トグル）
        _buttonSwitch = !_buttonSwitch;
        _upgradeDestroyUI.SetActive(_buttonSwitch);

        // 射程ラインの表示／非表示を反転する（トグル）
        _lineRender.SetActive(!_lineRender.activeSelf);
    }
    //---------Destroy Start------------>

    public void GunDestroy()
	{
        // 砲台を Pool に戻す
        BuildManager.Instance.gunUpgradePool.Release(this.gameObject);

        // 返金処理
        UI_Manager.Instance.GetMoney(_getMoney);

        // 返金 UI を生成する
        GameObject backMoneyUI = GameObject.Instantiate(_backMoney, gameObject.transform.position, Quaternion.identity);

        // UI の表示位置を調整する
        backMoneyUI.transform.position = new Vector3(gameObject.transform.position.x, _uiYPos, gameObject.transform.position.z);

        // UI を一定時間後に削除する
        Destroy(backMoneyUI, _delayTime);
	}
	//----------------------------------->
}
