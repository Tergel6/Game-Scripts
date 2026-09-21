using TMPro;
using UnityEngine;

public class GoldUpgraded : MonoBehaviour
{
	public bool onGetMoney = true;                      // 自動でお金を生成するかどうかのフラグ

	[SerializeField] GameObject _backMoneyUIPre;        // 破壊時に表示する返金 UI 
	[SerializeField] GameObject _upgradeDestroyUI;      // アップグレード／破壊ボタンをまとめた UI
	[Space]
	[SerializeField] float _nextGetMoneyTime;           // 次にお金を生成する時間	
	[SerializeField] float _timer;                      // お金を生成する間隔（秒）
	[Space]
	[SerializeField] TextMeshProUGUI _getMoneyUI;       // 現在の生成金額を表示する UI
	[SerializeField] TextMeshProUGUI _upgradeBTText;    // アップグレードボタンのテキスト
	[SerializeField] TextMeshProUGUI _destroyBTText;    // 破壊ボタンのテキスト
	[Space]
	[SerializeField] int _getMoney;                     // 一度に生成するお金の量
	[SerializeField] int _costUpGraded;                 // アップグレードに必要な金額
	[Space]
	[SerializeField] int _yonWari;                      // 破壊時に返す割合（例：4なら 1/4）
	[SerializeField] float _uiYPos;                     // 返金 UI を表示する高さ（Y座標）
	[SerializeField] float _delayTime;                  // UI を削除するまでの時間（秒）
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
			if (Time.time > _nextGetMoneyTime)
			{
				UI_Manager.Instance.GetMoney(_getMoney);

				// 次の生成タイミングを設定する
				_nextGetMoneyTime = Time.time + _timer;
			}
		}
	}
	public void OnMouseDown()
	{
		// UI の開閉を切り替える（トグル）
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

	// 銀行タワーがアップグレードするメソッド
	public void GoldUpGraded()
	{
		// アップグレードに必要なお金を持っているか確認する
		if (UI_Manager.Instance.GetMyMoney() >= _costUpGraded)
		{
			// お金を消費する
			UI_Manager.Instance.UseMoney(_costUpGraded);

			// アップグレード後の金額を増やす
			_costUpGraded += _costUpGraded;
			_getMoney += _getMoney;

			// UI の更新
			_getMoneyUI.text = "$" + _getMoney.ToString();
			_upgradeDestroyUI.SetActive(false);

			// ボタンのテキストを更新する
			_upgradeBTText.text = "Upgrade -$" + _costUpGraded.ToString();
			_destroyBTText.text = "Destroy +$" + (_costUpGraded / _yonWari).ToString();
		}
	}

	// 銀行タワーを削除するメソッド
	public void GoldDestroy()
	{
		// オブジェクトを Pool に戻す
		BuildManager.Instance.goldUpgradePool.Release(gameObject);

		// 返金処理（割合で計算）
		UI_Manager.Instance.GetMoney(_costUpGraded / _yonWari);

		// 返金 UI を生成する
		GameObject backMoneyUI = GameObject.Instantiate(_backMoneyUIPre, gameObject.transform.position, Quaternion.identity);

		// UI の表示位置を調整する（Y座標を上げる）
		backMoneyUI.transform.position = new Vector3(gameObject.transform.position.x, _uiYPos, gameObject.transform.position.z);

		// UI を一定時間後に削除する
		Destroy(backMoneyUI, _delayTime);
	}


}
