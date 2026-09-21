using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerController : MonoBehaviour
{
    static public PlayerController instance { get; private set; }

    static readonly int isMoveHash = Animator.StringToHash("IsMove");       //「移動」アニメの名前をIDに変える
    static readonly int isDie = Animator.StringToHash("IsDie");             //「死亡」アニメの名前をIDに変える
    static readonly int isPlaceEgg = Animator.StringToHash("IsPlaceEgg");   //「タマゴを置く」アニメの名前をIDに変える

    [SerializeField] ParticleSystem _upgradeFX; // レベルアップエフェクト
    [SerializeField] ParticleSystem _deadFX;    // 死亡エフェクト
    [SerializeField] Animator _anime;           // プレイヤーのアニメ

    public PlayerSkillData _skill;              // スキルのデータ

    PlayerControl _inputActions;                // 新しいインプットシステム（プレイヤー操作用）

    Transform _transform;                       // 自身のtransform

    float _expSliderValue;                      // 経験スライダーの数値

    //============================= Awake ==============================>
    void Awake()
    {
        instance = this;
        _transform = transform; 　                               //　自身のtransformを指定

        _inputActions = new PlayerControl();

        //　Fire が押された瞬間に「PlaceBomb」を呼び出すイベントを登録
        _inputActions.GamePlay.Fire.started += PlaceBomb;
        //　Options が押された瞬間に「OnOptions」を呼び出すイベントを登録
        _inputActions.GamePlay.Options.started += OnOptions;
    }

    //============================= Start ==============================>
    void Start()
    {
        //　スキルUIを初期化する
        InitSkillUI();
    }
    //============================= inputAction ==============================>

    void OnEnable()
    {
        //　入力システムをオン
        _inputActions.Enable();
    }
    void OnDisable()
    {
        //　入力システムをオフ
        _inputActions.Disable();
    }

    //============================= Collision ==============================>
    void OnCollisionEnter(Collision collision)
    {
        //　Colliderと衝突したタッグを判断する
        if (collision.gameObject.CompareTag("Enemy"))
        {
            //プレイヤーが死亡
            PlayerDie();
        }
    }
    //============================= Update ==============================>
    void Update()
    {
        //プレイヤーが移動
        PlayerMove();
        //プレイヤーがマップの範囲から外に出ているのかの確認
        if (_transform.position.x < -20f || _transform.position.x > 20f || _transform.position.z < -20f || _transform.position.z > 20f)
        {
            GameController.instance.SpawnEnemy_OutSide(1);
        }
    }

    //============================= Functions ==============================>

    //　「GamePlay」の入力を有効化する
    public void OnGamePlayEnable()
    {
        //　「GamePlay」の入力を有効化する
        _inputActions.GamePlay.Enable();
    }

    //　「GamePlay」の入力を無効化する
    public void OnGamePlayDisable()
    {
        //　「GamePlay」の入力を無効化する
        _inputActions.GamePlay.Disable();
    }

    // プレイヤーが移動する
    void PlayerMove()
    {
        //　インプットシステムで移動輸入を得る
        Vector3 _inputDirection = _inputActions.GamePlay.Move.ReadValue<Vector3>();　
        if (_inputDirection != Vector3.zero)
        {
            //　基礎速度プラスアップした移動速度
            float _newMoveSpeed = 1.2f + _skill.moveSpeed * 0.2f;
            //　回転
            _transform.forward = _inputDirection;
            //　前進
            _transform.position += _transform.forward * _newMoveSpeed * Time.deltaTime;
            //　「移動」アニメをプレー
            _anime.SetBool(isMoveHash, true);
        }
        else
        {
            //　「移動」アニメをストップ
            _anime.SetBool(isMoveHash, false);
        }
    }

    // プレイヤーがタマゴを置く
    void PlaceBomb(InputAction.CallbackContext context)
    {
        //　置けるタマゴの数を判断
        if (_skill.eggCount > 0)
        {
            //　プールからタマゴを呼び出す
            GameObject newEgg = PoolManager.instance._eggPool.Get();
            //　タマゴの置く位置
            newEgg.transform.position = _transform.position;
            //　タマゴのマテリアルの色を変える
            newEgg.GetComponent<EggController>().ChangeEggColor();
            //　タマゴの「Collider」を有効する
            newEgg.GetComponent<Collider>().enabled = true;
            //　タマゴの数を減らす
            EggCountDown();
            //　タマゴを置く音をプレー
            AudioController.instance.PlayPlaceEggSound();
            //　タマゴを置くアニメをプレー
            _anime.SetTrigger(isPlaceEgg);
        }
    }

    // プレイヤーが死亡
    public void PlayerDie()
    {
        //　入力システムをオフ
        OnDisable();
        //　プレイヤーの「Collider」を無効する
        _transform.GetComponent<Collider>().enabled = false;
        // ゲームオーバーを遅延実行
        GameController.instance.DelayedFunction();
        // ゲームオーバー音をプレー
        AudioController.instance.PlayGameOverSound();
        // 死亡エフェクトをプレー
        _deadFX.Play();
        // 死亡アニメをプレー
        _anime.SetTrigger(isDie);
        // プレイヤーを１秒あと削除
        Destroy(gameObject, 1f);
    }

    //--------------------　UI　----------------------->

    //　オプションを呼び出す
    void OnOptions(InputAction.CallbackContext context)
    {
        //　オプションを呼び出す
        GamePlayUIController.instance.Options();
    }

    //--------------------　PlayerSkill　----------------------->

    // 経験がアップする
    public void ExperienceUp(float newExperience)
    {
        //　プレイヤーがいるかの確認
        if (PlayerController.instance != null)
        {
            //　経験を増やす
            _skill.experience += newExperience;
            //　経験スレイダーを設定
            _expSliderValue = _skill.experience / (_skill.level * 0.1f + 0.3f);

            // プレイヤーの経験が一定の条件になるとレベルアップする
            if (_skill.experience >= (_skill.level * 0.1f * 2f) - 0.05)
            {
                //　レベルアップエフェクトをプレー
                _upgradeFX.Play();
                //　経験を０にする
                _skill.experience = 0;
                //　経験スレイダーを０にする
                _expSliderValue = 0;
                //　レベルアップする
                _skill.level++;

                //　プレイヤーのレベルを更新
                GamePlayUIController.instance.ShowPlayerLevel(_skill.level);
                //　レベルアップ音をプレー
                AudioController.instance.PlayLevelUpSound();
                //　レベルアップアニメをプレー
                GamePlayUIController.instance.PlaylevelUpAnime();

                //　1.8秒あと「レベルアップ」が実行
                Invoke(nameof(LevelUp), 1.8f);
            }

            //　経験を更新する
            GamePlayUIController.instance.ShowExpCount((int)(_skill.experience * 10), (int)((_skill.level * 0.1f * 2f) * 10));
            //　経験スレイダーを更新する
            GamePlayUIController.instance.ShowExperienceSlider(_expSliderValue);
        }
    }

    // プレイヤーがレベルアップする
    void LevelUp()
    {
        //　「GamePlay」の入力を無効化する
        OnGamePlayDisable();
        //　ゲームを一時停止する
        GameController.instance.PauseGame();
        //　スキルを選択する画面を呼び出す
        GamePlayUIController.instance.ShowSelectSkill();
    }

    // プレイヤーの「移動」スキルがアップする
    public void MoveSpeedUp()
    {
        //　「移動スピード」がアップ
        _skill.moveSpeed++;
        //　「移動スピード」を更新する
        GamePlayUIController.instance.ShowMoveSpeed(_skill.moveSpeed);
    }

    // 「爆発距離」スキルがアップする
    public void BombRangeUp()
    {
        //　「爆発距離」スキルがアップ
        _skill.bombRange++;
        //　「爆発距離」を更新する
        GamePlayUIController.instance.ShowBombRange(_skill.bombRange);
    }

    // プレイヤーの「タマゴの数」スキルがアップする
    public void EggCountUp()
    {
        //　「タマゴの数」スキルがアップする
        _skill.eggCount++;
        //　「タマゴの数」スキルを更新する
        GamePlayUIController.instance.ShowEggCount();
    }

    // タマゴの数を減らす
    void EggCountDown()
    {
        //　置けるタマゴの数を減らす
        _skill.eggCount--;
    }

    //-----------------------InitUI-------------------------->

    //　スキルUIを初期化する
    void InitSkillUI()
    {
        //　「GamePlayUIController.instance」を表明する
        var skillUI = GamePlayUIController.instance;
        //　「移動スピード」を初期化する
        skillUI.ShowMoveSpeed(_skill.moveSpeed);
        //　「爆破距離」を初期化する
        skillUI.ShowBombRange(_skill.bombRange);
        //　「タマゴの数」を初期化する
        skillUI.ShowEggCount();
        //　「経験」を初期化する
        skillUI.ShowExpCount(0, 2);
        //　「プレイヤーのレベル」を初期化する
        skillUI.ShowPlayerLevel(_skill.level);
        //　「経験のスレイダー」を初期化する
        skillUI.ShowExperienceSlider(_skill.experience);
    }
}
