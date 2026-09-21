using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
	public static UIController Instance { get; private set; }

    [SerializeField] GameObject _menu;					// メニューUI
    [SerializeField] TextMeshProUGUI _timeText;			// 時間を表示するテキスト
    [SerializeField] float _countDown;                  // ゲームのカウントダウン
	[SerializeField] float _redCountDown;				// ゲームのカウントダウンが赤くなる点
    [Header("Pos&Rot BT")]
	[SerializeField] Button _posBT;						// 移動ボタン
	[SerializeField] Button _rotBT;						// 回転ボタン

    Boolean _isTimeRunning = false;                     // タイマーが動いているかどうか

    //============================== Awake ===============================>
    private void Awake()
    {
        Instance = this;
    }
    //============================== Start ===============================>
    void Start()
	{
		// 移動と回転ボタンを無効化する
		HidePosRotBT();

		//カウントダウンのテキストを白にする
		_timeText.color = Color.white;

		//メニューUIを隠す
		_menu.SetActive(false);

        // タイマーをスタートする
        OnTimeStart();
	}
	//============================== Update ===============================>
	void Update()
	{
        // タイマーが動いている時だけ時間を更新する
        if (_isTimeRunning)
		{
			TimeRun();
		}
	}
    //============================== Function ===============================>
    //-------------- Time ------------->
    // タイマーをスタートする
    public void OnTimeStart()
	{
		_isTimeRunning = true;
	}
    // タイマーを止める
    public void OnTimeStop()
	{
		_isTimeRunning = false;
	}
    // カウントダウンを進めてUIに表示する
    public void TimeRun()
	{
		if (_countDown > 0)
		{
			_countDown -= Time.deltaTime;
			_timeText.SetText(((int)_countDown).ToString());
			if (_countDown <= _redCountDown)
			{
                // 残り時間が少ない時は赤色にする
                _timeText.color = Color.red;
			}
		}
	}

    //------------------ Menu ------------------->
    // メニューの表示・非表示を切り替える
    public void OnMenu()
	{
		if (_menu.activeSelf)
		{
			_menu.SetActive(false);
			GameController.ContinueGame();
		}
		else if (!_menu.activeSelf)
		{
			_menu.SetActive(true);
			GameController.PauseGame();
		}
	}
    // ゲームをリスタートする
    public void Restart()
	{
		GameController.RestartGame();
	}
    // メインメニューへ戻る
    public void ToMainMenu()
	{
		GameController.ToMainMenu();
	}
	// カウントダウンを戻すメソッド
	public float GetCountDown()
	{
		return _countDown;
	}

	// 移動と回転ボタンを有効化するメソッド
	public void ShowPosRotBT()
	{
		_posBT.interactable = true;
		_rotBT.interactable = true;
	}

    // 移動と回転ボタンを無効化するメソッド
    public void HidePosRotBT() 
	{
        _posBT.interactable = false;
        _rotBT.interactable = false;
    }

}
