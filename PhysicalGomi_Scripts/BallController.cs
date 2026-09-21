using System.Collections;
using UnityEngine;

public class BallController : MonoBehaviour
{

    [SerializeField] Transform tr_Parent;       // 元の親オブジェクト
    [SerializeField] Transform tr_newParent;    // 新しい親オブジェクト

    [SerializeField] Collider WinCollider;      // ゴール判定のコライダー
    [SerializeField] GameObject gameOverUI;     // ゲーム終了UI

	[SerializeField] Transform tr_Lid;          // ゴミ箱のカバー
	[SerializeField] float _lidCloseAngle;      // カバーを閉める角度
    [SerializeField] float _closeSpeed;         // カバーを閉めるスピード
    [SerializeField] Vector3 _startRot;         // カバーの最初の角度

	[SerializeField] float _delayRunTime;			// 遅延するタイム

    Rigidbody _ballRigidbody;					// ボールのRigidbody
    Transform _transform;						// ボールのTransform

    Vector3 _ballPos;							// ボールの初期位置
    Quaternion _ballRot;						// ボールの初期回転

    GameObject _winUI;							// 勝利UI
    GameObject _overUI;							// ゲームオーバーUI

    bool _isGameOver;                           // ゲームオーバー状態かどうか
	bool _isClosing=false;						// ゴミ箱のカバーを閉めるか

	
    //===================================== Awake ========================================> 
    void Awake()
	{
        _ballPos = gameObject.transform.position;   // 初期位置を保存
        _ballRot = gameObject.transform.rotation;   // 初期回転を保存
    }

	//===================================== Start ========================================> 
	void Start()
	{
		_transform = transform;
		_ballRigidbody = GetComponent<Rigidbody>();
		_ballRigidbody.isKinematic = true;							// 最初は動かないようにする
        _isGameOver = false;
        //------------------------------------------->
        _winUI = gameOverUI.transform.Find("Win").gameObject;
		_overUI = gameOverUI.transform.Find("Over").gameObject;
		ResetGame();

		tr_Lid.localRotation = Quaternion.Euler(_startRot);         // カバーの最初の角度を設定する

    }

	void OnCollisionEnter(Collision collision)
	{
		if (collision.collider == WinCollider)
		{
			_ballRigidbody.isKinematic = true;                      // ボールを止める 
			_isClosing = true;
            StartCoroutine(WinDeleyTime(_delayRunTime));			// 少し待って勝利処理
        }
	}
	//===================================== Update ========================================> 
	void Update()
	{
		// カバーを閉める動画
        if (_isClosing)
        {
			float current = tr_Lid.localEulerAngles.x;
			Debug.Log(current);
			float next = Mathf.MoveTowards(current, _lidCloseAngle, Time.deltaTime * _closeSpeed);
			tr_Lid.rotation = Quaternion.Euler(next, -90f, 0f);
			if(next>= _lidCloseAngle)
			{
				_isClosing = false;
            }
		}
        

		if (UIController.Instance.GetCountDown() <= 0 && !_isGameOver)   // 時間切れ
        {
			GameOver();
		}
	}

    //===================================== Function ========================================> 

    // ゲーム開始時の処理
    public void OnStart()
	{
		DroneController.OnReleaseHand();
		_ballRigidbody.isKinematic = false;
		_transform.SetParent(tr_newParent);
	}

    // リスタート処理
    public void Restart()
	{
		DroneController.OnClampHand();
		gameObject.transform.position = _ballPos;
		gameObject.transform.rotation = _ballRot;
		_ballRigidbody.isKinematic = true;
		_transform.SetParent(tr_Parent);
	}



    //------------------------------------->
    // 勝利までの遅延
    IEnumerator WinDeleyTime(float time)
	{
		yield return new WaitForSeconds(time);
		GameWin();
		GameController.PauseGame();
	}

    // 勝利処理
    void GameWin()
	{
		AudioController.instance.PlayWin();
        gameOverUI.SetActive(true);
        _winUI.SetActive(true);
    }

    // ゲームオーバー処理
    void GameOver()
	{
		AudioController.instance.PlayGameOver();
		GameController.PauseGame();
		gameOverUI.SetActive(true);
		_overUI.SetActive(true);
		_isGameOver = true;
	}
    //---------------------- Reset Game -------------------->

    //リセットゲーム
    void ResetGame()
	{
		if (gameOverUI.activeSelf)
		{
			gameOverUI.SetActive(false);
		}
		if (_winUI.activeSelf)
		{
			_winUI.SetActive(false);
		}
		if (_overUI.activeSelf)
		{
			_overUI.SetActive(false);
		}
		Time.timeScale = 1;
	}

}
