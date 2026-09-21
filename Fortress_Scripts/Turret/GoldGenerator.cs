using UnityEngine;

public class GoldGenerator : MonoBehaviour
{
	public bool onGetMoney = true;						// 自動でお金を生成するかどうかのフラグ

    [SerializeField] GameObject _backMoneyUIPre;		// 破壊時に表示する返金 UI
    [SerializeField] GameObject _upgradeDestroyUI;		// アップグレード／破壊ボタンをまとめた UI

    [SerializeField] float _nextGetMoneyTime;			// 次にお金を生成する時間
    [SerializeField] float _Timer;						// お金を生成する間隔（秒）
    [SerializeField] int _getMoney;                     // 一度に生成するお金の量	
    [SerializeField] int _half;                         // 破壊時に返す金額の割合（例：2なら半額）
    [SerializeField] float _uiYPos;						// 返金 UI を表示する高さ（Y座標）
    [SerializeField] float _delayTime;                // 遅延タイム
    bool _buttonSwitch = false;                         // UI の開閉状態を管理するフラグ

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
                // 自分自身と UI 以外をクリックした場合は UI を閉じる
                if (_upgradeDestroyUI != null && hit.transform.gameObject != gameObject && hit.transform.gameObject != _upgradeDestroyUI)
				{
					_upgradeDestroyUI.SetActive(false);
				}
			}
		}

        // 一定時間ごとにお金を生成する処理
        if (onGetMoney == true)
		{
            // 次の生成時間を過ぎたらお金を追加する
            if (Time.time > _nextGetMoneyTime)
			{
				UI_Manager.Instance.GetMoney(_getMoney);

                // 次の生成タイミングを設定する
                _nextGetMoneyTime = Time.time + _Timer;
			}
		}
	}

	//================================ Function =================================>
	public void OnMouseDown()
	{
        // UI の開閉を切り替える
        if (_upgradeDestroyUI != null)
		{

			switch (_buttonSwitch)
			{
				case false:
					_upgradeDestroyUI.SetActive(true);
					_buttonSwitch = true;
					break;
				case true:
					_upgradeDestroyUI.SetActive(false);
					_buttonSwitch = false;
					break;
			}

		}

    }

    //---------UpGraded & Destroy Start------------>

    // 銀行がアップグレードメソッド
    public void GoldUpGraded()
	{

        // アップグレードに必要なお金を持っているか確認する
        if (UI_Manager.Instance.GetMyMoney() >= BuildManager.Instance.goldGeneratorData._costUpGraded)
		{
            // お金を消費する
            UI_Manager.Instance.UseMoney(BuildManager.Instance.goldGeneratorData._costUpGraded);

            // Pool に戻す前に位置を保存しておく
            Vector3 _newPos = gameObject.transform.position;

            // 旧オブジェクトを Pool に戻す
            BuildManager.Instance.goldGeneratorPool.Release(gameObject);

            // アップグレード後のオブジェクトを取得
            GameObject UpGradedObj = BuildManager.Instance.goldUpgradePool.Get();

            // 位置を引き継ぐ
            UpGradedObj.transform.position = _newPos;

        }
        // お金が足りない場合のアニメーション
        else if (UI_Manager.Instance.GetMyMoney() < BuildManager.Instance.goldGeneratorData._costUpGraded)
		{
			UI_Manager.Instance.PlayNoMoneyAnime();
		}
	}
	public void GoldDestroy()
	{
        // 旧オブジェクトを Pool に戻す
        BuildManager.Instance.goldGeneratorPool.Release(gameObject);

        // 半額を返金する
        UI_Manager.Instance.GetMoney(BuildManager.Instance.goldGeneratorData.cost / _half);

        // 返金 UI を生成する
        GameObject backMoneyUI = GameObject.Instantiate(_backMoneyUIPre, gameObject.transform.position, Quaternion.identity);

        // UI の表示位置を調整する（Y座標を上げる）
        backMoneyUI.transform.position = new Vector3(gameObject.transform.position.x, _uiYPos, gameObject.transform.position.z);
        // UI を一定時間後に削除する
        Destroy(backMoneyUI, _delayTime);
	}


}
