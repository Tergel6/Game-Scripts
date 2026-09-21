using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public static GameController Instance { get; private set; }     // GameController のインスタンスを共有するためのプロパティ（シングルトン）
    [SerializeField] int _frameRate;
    [Header("POOL")]
    [SerializeField] int _startPoolCount;                           // 初期プール数
    [SerializeField] int _maxPoolCount;                             // 最大プール数
    public ObjectPool<GameObject> enemyPool;                        // 敵のPool
    public GameObject enemy_01Pre;                                  // 敵_01のオブジェクト
    public int enemyCountInactive;                                  // 非アクティブ状態の敵の数
    public int enemyCountActive;                                    // アクティブ状態の敵の数
    public int enemyCountAll;                                       // 敵の総数

    public List<GameObject> enemyList = new();                      // 敵オブジェクトのリスト                 

    [Space]
    [SerializeField] float _enemyTime;                              // 敵の出現テーム
    [SerializeField] float _spawnTime;                              // スポーン間隔
    [SerializeField] int _spawnCount;                               // 一度にスポーンする敵の数
    [SerializeField] float _newEnemyHP;                             // 新しくスポーンする敵のHP
    [SerializeField] float _gameTime;                               // ゲーム経過時間                           

    // 敵のスポーン位置（X座標とZ座標の範囲）
    [SerializeField] float _enemySpawnX01Min, _enemySpawnX01Max, _enemySpawnX02Min, _enemySpawnX02Max;
    [SerializeField] float _enemySpawnZ01Min, _enemySpawnZ01Max, _enemySpawnZ02Min, _enemySpawnZ02Max;

    [SerializeField] float _enemySpawnY;                            // 敵をスポーンさせる高さ（Y座標）
    [SerializeField] float _random;                                 // ランダム値として使うための変数
    [SerializeField] bool isGameOver;                               // ゲームオーバー状態かどうか
    
    int _stopGameTime = 0;                                          // ゲームを停止するときに使う時間設定（0で停止）
    int _continueGameTime = 1;                                      // ゲームを再開するときの時間設定（1で通常速度）


    //================================ Awake =================================>
    void Awake()
    {
        // シングルトンのインスタンスを設定する
        Instance = this;

        // 敵オブジェクトのプールを作成する
        // 第1引数：生成処理
        // 第2引数：取得時の処理
        // 第3引数：解放時の処理
        // 第4引数：破棄時の処理
        // 第5引数：コレクションチェックの有無
        // 第6引数：初期プール数
        // 第7引数：最大プール数
        enemyPool = new ObjectPool<GameObject>(createEnemy_01Func, actionOnGet, actionOnRelease, actionOnDestroy, true, _startPoolCount, _maxPoolCount);
    }

	//================================ Start =================================>
	void Start()
    {
        // ゲームのフレームレートを設定する
        Application.targetFrameRate = _frameRate;
    }

	//================================ Update =================================>
	void Update()
    {
        enemyCountInactive = enemyPool.CountInactive;               // 非アクティブ状態の敵の数を更新
        enemyCountActive = enemyPool.CountActive;                   // アクティブ状態の敵の数を更新
        enemyCountAll = enemyPool.CountAll;                         // 敵の総数を更新

        // ゲームオーバーではない場合に敵をスポーンする処理
        if (isGameOver == false)
        {
            // 一定時間が過したかどうかを確認する
            if (_gameTime > _spawnTime)
            {
                // 指定された数だけ敵を生成する
                for (int i = 0; i < _spawnCount; i++)
                {
                    SpawnEnemy();
                }
                _spawnTime = _spawnTime + _enemyTime;               // 次のスポーン時間を更新する
                _spawnCount++;                                      // スポーン数を増やす（ゲームが進むほど敵が増える）　
                _newEnemyHP++;                                      // 新しく出現する敵のHPを少しずつ上げる
            }
        }

        _gameTime += Time.deltaTime;                                // ゲームの経過時間を加算する
    }

	//================================ Function =================================>
	void SpawnEnemy()
    {
        // X座標のスポーン範囲をランダムで選ぶ（左側か右側）
        bool XRange = Random.value > _random;
        float spawnPosX = XRange ? Random.Range(_enemySpawnX01Min, _enemySpawnX01Max) : Random.Range(_enemySpawnX02Min, _enemySpawnX02Max);
        // Z座標のスポーン範囲をランダムで選ぶ（前方か後方）
        bool ZRange = Random.value > _random;
        float spawnPosZ = ZRange ? Random.Range(_enemySpawnZ01Min, _enemySpawnZ01Max) : Random.Range(_enemySpawnZ02Min, _enemySpawnZ02Max);
        // XかZのどちらかを広い範囲でランダムに決める位置（パターン1）
        Vector3 spawnPos_01 = new Vector3(spawnPosX, _enemySpawnY, Random.Range(_enemySpawnZ01Min, _enemySpawnZ02Max));
        // XかZのどちらかを広い範囲でランダムに決める位置（パターン2）
        Vector3 spawnPos_02 = new Vector3(Random.Range(_enemySpawnX01Min, _enemySpawnX02Max), _enemySpawnY, spawnPosZ);
        // どちらのスポーン位置パターンを使うかランダムで決める
        bool spawnPosRange = Random.value > _random;
        Vector3 SpawnPos = spawnPosRange ? spawnPos_01 : spawnPos_02;

        // オブジェクトプールから敵を取得する
        GameObject newEnemy = enemyPool.Get();
        // 敵の位置を設定する
        newEnemy.transform.position = SpawnPos;
        // 敵リストに追加する（管理用）
        enemyList.Add(newEnemy);
    }


    //--------------Pool-------------->

    // 敵オブジェクトを生成するメソッド
    GameObject createEnemy_01Func()
    {
        // プレハブから新しい敵を作り、現在のオブジェクトの子として配置する
        GameObject newOBJ = GameObject.Instantiate(enemy_01Pre, transform);
        return newOBJ;
    }

    // プールから取り出したときの処理
    void actionOnGet(GameObject obj)
    {
        // 敵をアクティブにして、ゲーム内で見えるようにする
        obj.SetActive(true);
    }

    // プールに戻すときの処理
    void actionOnRelease(GameObject obj)
    {
        // 敵を非アクティブにして、画面から消す
        obj.SetActive(false);
    }

    // プールから完全に削除するときの処理
    void actionOnDestroy(GameObject obj)
    {
        // オブジェクトを削除する
        Destroy(obj);
    }
    //-----------------Pool------------->
    // 敵をプールに戻すメソッド
    public void DestroyEnemy(GameObject enemyPre)
    {
        // null ではなく、あるいは非アクティブ状態ならプールに返す
        if (enemyPre != null || !enemyPre.activeSelf)
        {
            enemyPool.Release(enemyPre);
        }
    }
    //------------------------GameManager---------------------->

    // ゲームオーバー状態にするメソッド
    public void IsGameOver()
    {
        isGameOver=true;
    }
    // ゲームを一時停止する（時間の進行を止める）
    public void PauseGame()
    {
        Time.timeScale = _stopGameTime;
    }
    // ゲームを再開する
    public void ContinueGame()
    {
        Time.timeScale = _continueGameTime;
    }

    //-----------------------Scene---------------------------->
    // ホーム画面（シーン0）に戻る処理
    public void OnHome()
    {
        SceneManager.LoadScene(0);      // シーンを読み込む
        ContinueGame();                 // 時間を通常に戻す
    }
    // ゲームを再プレイする（シーン1を読み込む）
    public void OnRePlay()
    {
        SceneManager.LoadScene(1);      // シーンを読み込む
        ContinueGame();                 // ゲームを再開する
    }
    //---------------------------------------------------->

    // 新しくスポーンする敵のHPを返すメソッド
    public float NewEnemyHP()
    {
        return _newEnemyHP;
    }


}
