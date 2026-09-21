
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public static GameController instance { get; private set; }

    [SerializeField] int _frameRate;                        // ゲームのフレームレート

    [SerializeField] SpawnEnemyData _spawnEnemy_01Data;     // 敵_01のデータ
    [SerializeField] SpawnEnemyData _spawnEnemy_02Data;     // 敵_02のデータ
    [SerializeField] SpawnEnemyData _spawnEnemy_03Data;     // 敵_03のデータ

    [HideInInspector] public int enemyDeadCount;            // 敵を倒した数
    [HideInInspector] public string elapsedTime;            // ゲームプレー時間

    int _minSpawnPos = -18;                                 // 敵が出現する位置の最小値
    int _maxSpawnPos = 18;                                  // 敵が出現する位置の最大値
    int _minEnemyRot = 0;                                   // 敵が出現時の回転角度の最小値（0度）
    int _maxEnemyRot = 360;                                 // 敵が出現時の回転角度の最大値（360度）

    float _timer;                                           // プレー時間

    //================================ Awake ================================>
    void Awake()
    {
        instance = this;
    }

    //================================ Start ================================>
    void Start()
    {
        // V-Syncをオフにする
        QualitySettings.vSyncCount = 0;
        // ゲームのフレームレートを設定する
        Application.targetFrameRate = _frameRate;

        // ゲームを初期化する
        InitGame();

        // 敵_01を生成する
        SpawnEnemy(PoolManager.instance._enemy_01Pool, _spawnEnemy_01Data.spawnCount);
    }

    //================================ Update ================================>
    void Update()
    {
        // プレー時間が実行する
        ElapsedTime();

        // 実行しているプレー時間がUIに表示する
        GamePlayUIController.instance.ShowElapsedTime(elapsedTime);
    }

    //=============================== Function ===============================>

    //-------------------------- InitEnemy ------------------------>

    // 敵を条件に合わせて生成する
    public void SpawnCondition()
    {
        // 死亡した敵の数が敵_02の生成条件に合ったら
        if (enemyDeadCount == _spawnEnemy_02Data.spawnCondition)
        {
            // 敵_02を生成する
            SpawnEnemy(PoolManager.instance._enemy_02Pool, _spawnEnemy_02Data.spawnCount);
        }
        // 死亡した敵の数が敵_03の生成条件に合ったら
        else if (enemyDeadCount == _spawnEnemy_03Data.spawnCondition)
        {
            // 敵_03を生成する
            SpawnEnemy(PoolManager.instance._enemy_03Pool, _spawnEnemy_03Data.spawnCount);
        }
    }

    public void SpawnEnemy(ObjectPool<GameObject> enemyPool, int enemyCount)
    {
        if (PlayerController.instance == null)
            return;

        for (int i = 0; i < enemyCount; i++)
        {
            // プールから敵_01をゲットする
            GameObject newEnemy = enemyPool.Get();
            // ゲットした敵_01を生成する
            InitEnemy(newEnemy);
        }
    }

    // 敵を初期化する
    void InitEnemy(GameObject newEnemy)
    {
        while (true)
        {
            //　生成位置をランダムでマップの中に設定する
            Vector3 spawnPos = new Vector3(Random.Range(_minSpawnPos, _maxSpawnPos), 0, Random.Range(-_minSpawnPos, _maxSpawnPos));

            //　敵の生成位置がプレイヤーから設定した距離にある時
            if ((spawnPos - PlayerController.instance.transform.position).sqrMagnitude >
                _spawnEnemy_01Data.spawnRange * _spawnEnemy_01Data.spawnRange)
            {
                //　ランダムで回転して生成
                Quaternion spawnRot = Quaternion.Euler(0, Random.Range(_minEnemyRot,_maxEnemyRot), 0);
                //　生成した敵の位置と回転を指定
                newEnemy.transform.SetPositionAndRotation(spawnPos, spawnRot);
                break;
            }
        }

    }

    //----------------------- EnemyDead ----------------------->

    // 敵が死亡
    public void EnemyDead()
    {
        //　敵の死亡数を増やす
        enemyDeadCount++;
        //　死亡した敵の数を更新
        GamePlayUIController.instance.DeadEnemyCount(enemyDeadCount);
        //　プレイヤーの経験を0.1アップする
        PlayerController.instance.ExperienceUp(0.1f);
    }

    //------------------- GameOverDelayed --------------------------->

    // 遅延実行
    public void DelayedFunction()
    {
        //　ゲームオーバーを1.5秒遅延実行
        Invoke(nameof(GameOverDelayedFunction), 1.5f);
    }

    // ゲームオーバーを遅延実行
    void GameOverDelayedFunction()
    {
        // ゲームを停止する
        PauseGame();
        // 「ゲームオーバー」画面を表示
        GamePlayUIController.instance.ShowGameOver(enemyDeadCount);
        //　ストップ「ゲームの遊ぶ時の音楽」
        AudioController.instance.StopGamePlayBGM();
        //　プレー「ゲームが終わる時の音楽（失敗した音楽）」
        AudioController.instance.PlayGameOverBGM();

        //　アクティブシーンが第1ステージだったら
        if (SceneManager.GetActiveScene().name == "Level_01")
        {
            //　第1ステージのベストスコアーをPlayerPrefsから読み込んでPlayerPrefsにスコアーがなかったら0に設定
            int bestScore_01 = PlayerPrefs.GetInt("bestScore_01", 0);
            //　読み込んだスコアーが倒した敵の数より少ない場合
            if (bestScore_01 < enemyDeadCount)
            {
                //　PlayerPrefsの第1ステージのベストスコアーに倒した敵の数を書き込む
                PlayerPrefs.SetInt("bestScore_01", enemyDeadCount);
                //　スコアーをPlayerPrefsに保存する
                PlayerPrefs.Save();
            }
        }
        else if (SceneManager.GetActiveScene().name == "Level_02")
        {
            //　第2ステージのベストスコアーをPlayerPrefsから読み込んでPlayerPrefsにスコアーがなかったら0に設定
            int bestScore_02 = PlayerPrefs.GetInt("bestScore_02", 0);
            //　読み込んだスコアーが倒した敵の数より少ない場合
            if (bestScore_02 < enemyDeadCount)
            {
                //　PlayerPrefsの第2ステージのベストスコアーに倒した敵の数を書き込む
                PlayerPrefs.SetInt("bestScore_02", enemyDeadCount);
                //　スコアーをPlayerPrefsに保存する
                PlayerPrefs.Save();
            }
        }
        else if (SceneManager.GetActiveScene().name == "Level_03")
        {
            //　第3ステージのベストスコアーをPlayerPrefsから読み込んでPlayerPrefsにスコアーがなかったら0に設定
            int bestScore_03 = PlayerPrefs.GetInt("bestScore_03", 0);
            //　読み込んだスコアーが倒した敵の数より少ない場合
            if (bestScore_03 < enemyDeadCount)
            {
                //　PlayerPrefsの第3ステージのベストスコアーに倒した敵の数を書き込む
                PlayerPrefs.SetInt("bestScore_03", enemyDeadCount);
                //　スコアーをPlayerPrefsに保存する
                PlayerPrefs.Save();
            }
        }
    }

    //------------------------ Button ---------------------->

    // もう一回プレイ
    public void RePlayGame()
    {
        //　アクティブシーンのステージ名前を取る
        string stageName = SceneManager.GetActiveScene().name;
        //　取ったステージシーンを読み込む
        SceneManager.LoadScene(stageName);
    }

    // メインメニューに戻る
    public void MainMenu()
    {
        // メインメニューシーンを読み込む
        SceneManager.LoadScene(0);
        // プレー「ゲームの遊ぶ時の音楽」
        AudioController.instance.PlayGamePlayBGM();
        // ストップ「ゲームが終わる時の音楽（失敗した音楽）」
        AudioController.instance.StopGameOverBGM();
        // ゲームを再開する
        ResumeGame();
    }

    //------------------------- OutSide --------------------------->>

    // 範囲以外の敵を生成する
    public void SpawnEnemy_OutSide(int enemyCount)
    {
        //　プレイヤーが存在していない場合
        if (PlayerController.instance == null)
            return;

        for (int i = 0; i < enemyCount; i++)
        {
            //　プールから敵_03を読み出す
            GameObject outSide_NewOBJ = PoolManager.instance._enemy_03Pool.Get();
            // 範囲以外に敵を生成する
            SpawnEnemyOutSide(outSide_NewOBJ);
        }
    }

    // 範囲以外に敵を生成する
    void SpawnEnemyOutSide(GameObject newEnemy)
    {

        Vector3 playerPos = PlayerController.instance.transform.position;
        //　敵を生み出す位置をプレイヤーの近くに設定
        Vector3 outSide_SpawnPos = new Vector3(Random.Range(playerPos.x - 3f, playerPos.x + 3f), 0, Random.Range(playerPos.z - 3f, playerPos.z + 3f));
        //　生み出した敵のtransformを指定
        Transform tr_outSideOBJ = newEnemy.transform;
        //　生み出した敵の位置を指定
        tr_outSideOBJ.position = outSide_SpawnPos;
        //　生み出した敵の回転を指定
        tr_outSideOBJ.rotation = Quaternion.Euler(0, Random.Range(0, 360f), 0);

    }
    //-------------------------- ElapsedTime ------------------------------>

    // プレー時間が実行する
    public void ElapsedTime()
    {
        //　タイマーを進める
        _timer += Time.deltaTime;
        //　タイマーから分を計算する
        int minutes = (int)(_timer / 60);
        //　タイマーから秒を計算する
        int seconds = (int)(_timer % 60);
        // ゲームプレー時間を指定する
        elapsedTime = string.Format("{0:D2}:{1:D2}", minutes, seconds);
    }
    //----------------------- PauseGame ----------------------->

    // ゲームを停止する
    public void PauseGame()
    {
        //　ゲーム内の時間が止まる
        Time.timeScale = 0;
    }

    // ゲームを再開する
    public void ResumeGame()
    {
        //　時間が通常通り進む（再開する）
        Time.timeScale = 1;
    }

    //-------------------------- InitGame ----------------------------->

    // ゲームを初期化する
    void InitGame()
    {
        //　ゲームを再開する
        ResumeGame();
        //　倒した敵の数を0にする
        enemyDeadCount = 0;
        //　ゲーム中UIの死亡した敵の数を倒した敵の数（0）にする
        GamePlayUIController.instance.DeadEnemyCount(enemyDeadCount);
        //　プレー時間を0にする
        _timer = 0;

        // プレー「ゲームの遊ぶ時の音楽」
        AudioController.instance.PlayGamePlayBGM();
        // ストップ「ゲームが終わる時の音楽（失敗した音楽）」
        AudioController.instance.StopGameOverBGM();
    }
}
