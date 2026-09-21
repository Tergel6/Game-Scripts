using System.Collections;
using TMPro;
using UnityEngine;

public class UI_Manager : MonoBehaviour
{
    // 他のクラスから簡単に UI を操作できるようにする
    public static UI_Manager Instance { get; private set; }

    [SerializeField] GameObject teach;				// チュートリアル表示用 UI
    [SerializeField] GameObject setting;			// 設定画面 UI
    [SerializeField] GameObject gameOver;			// ゲームオーバー UI
    [SerializeField] TextMeshProUGUI yourScore;     // 最終スコア表示
    [Space]
    [SerializeField] Animator _myMoneyAnime;		// 金額の UI アニメ
    [SerializeField] TextMeshProUGUI myMoneyUI;		// 金額の UI 表示
    [SerializeField] TextMeshProUGUI _deadEnemyUI;	// 倒した敵数の UI 表示
    [SerializeField] int _myMoney;                  // 現在の金額
    [SerializeField] float _showTeachTime;			// チュートリアルを表示するまでの時間
    [SerializeField] int _showGameOverTime;         // ゲームオーバー UI を表示するまでの時間

    int _deadEnemyCount;                            // 倒した敵の数

    //================================ Awake =================================>
    void Awake()
	{
		Instance = this;
	}

	//================================ Start =================================>
	void Start()
	{
        // ゲーム開始後、一定時間過してからチュートリアルを表示する
        Invoke("ShowTeach", _showTeachTime);

    }

	//================================ Update =================================>
	void Update()
	{
        // 金額を毎フレーム UI に更新する
        myMoneyUI.text = _myMoney.ToString();
        // 倒した敵の数を毎フレーム UI に更新する
        _deadEnemyUI.text = _deadEnemyCount.ToString();
	}

    //================================ Function =================================>
    // チュートリアル UI を表示するメソッド
    public void ShowTeach()
	{
        // チュートリアル UI を表示し、ゲームを一時停止する
        teach.SetActive(true);
		GameController.Instance.PauseGame();
	}

    // チュートリアル UI を非表示するメソッド
    public void HideTeach()
	{
        // チュートリアル UI を非表示にし、ゲームを再開する
        teach.SetActive(false);
		GameController.Instance.ContinueGame();
	}

    //------------------------------------------>

    // 現在の金額を返すメソッド
    public int GetMyMoney()
	{
		return _myMoney;
	}
    // 金額を使うメソッド
    public void UseMoney(int costMoney)
	{
		_myMoney -= costMoney;
	}

    // 金額を獲得するメソッド
    public void GetMoney(int _getMoney)
	{
        // 指定した金額を獲得する
        _myMoney += _getMoney;
	}

    // 倒した敵数を増やすメソッド
    public void DeadEnemyCount()
	{
        // 倒した敵数を 1 増やす
        _deadEnemyCount++;
	}

    // 設定画面を表示するメソッド
    public void OnSetting()
	{
        // 設定画面を表示する
        setting.SetActive(true);
	}

    // 設定画面を閉じるメソッド
    public void OffSetting()
	{
        // 設定画面を閉じる
        setting.SetActive(false);
	}

    // ゲームオーバー UI を表示するメソッド
    public void ShowGameOver()
	{
        // ゲームオーバー UI を少し遅れて表示する
        StartCoroutine(DelayShowGameOver());
	}
	IEnumerator DelayShowGameOver()
	{
        // 一定時間待ってからゲームオーバー UI を表示
        yield return new WaitForSeconds(_showGameOverTime);
		gameOver.SetActive(true);

        // 最終スコア（倒した敵数）を表示
        ShowYourScore(_deadEnemyCount);
	}
    // ゲームオーバー UI を非表示にするメソッド
    public void HideGameOver()
	{
        // ゲームオーバー UI を非表示にする
        gameOver.SetActive(false);
	}

    // 最終スコアを UI に更新するメソッド
    public void ShowYourScore(int score)
	{
        // 最終スコアを UI に更新する
        yourScore.text = score.ToString();
	}

    // 金額UIの金額が足りないアニメをプレイメソッド
    public void PlayNoMoneyAnime()
    {
        _myMoneyAnime.SetTrigger("NoMoney");
    }
}
