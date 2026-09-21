using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GamePlayUIController : MonoBehaviour
{
    public static GamePlayUIController instance { get; private set; }

    //----------------------------- StringToHash---------------------------->

    static readonly int isEnemyDieHash = Animator.StringToHash("isEnemyDie");   // 敵が死亡アニメの名前をIDに変える
    static readonly int isEXPUpHash = Animator.StringToHash("isEXPUp");         // 経験が上がるアニメの名前をIDに変える
    static readonly int isLevelUpHash = Animator.StringToHash("isLevelUp");     // レベルアップアニメの名前をIDに変える
    static readonly int isPauseHash = Animator.StringToHash("IsPause");         // 一時停止アニメの名前をIDに変える
    static readonly int isCountUpHash = Animator.StringToHash("isCountUp");     // タマゴの数が増えるアニメの名前をIDに変える
    static readonly int isRangeUpHash = Animator.StringToHash("isRangeUp");     // 爆発距離が増えるアニメの名前をIDに変える
    static readonly int isSpeedUpHash = Animator.StringToHash("isSpeedUp");     // 移動スピードが上がるアニメの名前をIDに変える
    static readonly int IsGameOverHash = Animator.StringToHash("IsGameOver");   // ゲームオーバーアニメの名前をIDに変える

    //---------------------------------------------------------------------->

    [Header("_elapsedTime")]
    [SerializeField] TextMeshProUGUI _elapsedTime;          // ゲームプレー時間

    [Header("Skill_UI")]                                    // 以下はスキルＵＩ
    [SerializeField] TextMeshProUGUI moveSpeed;            // プレイヤーの移動スピード
    [SerializeField] TextMeshProUGUI eggCount;             // 置けるタマゴの数
    [SerializeField] TextMeshProUGUI bombRange;            // タマゴの爆発する距離
    [SerializeField] TextMeshProUGUI _playerLevel;          // プレイヤーのレベル
    [SerializeField] TextMeshProUGUI _experienceCount;      // プレイヤーの経験
    [SerializeField] Slider _experienceSlider;              // プレイヤーの経験のスライダー
    [SerializeField] Animator _speedUpAnime;                // プレイヤーの移動スピードが上がる動画
    [SerializeField] Animator _countUpAnime;                // 置けるタマゴの数が増える動画
    [SerializeField] Animator _rangeUpAnime;                // タマゴの爆発する距離が増える動画
    [SerializeField] Animator _levelUpAnime;                // プレイヤーのレベルが上がる動画
    [SerializeField] Animator _expUpAnime;                  // プレイヤーの経験が増える動画

    [Space]
    [SerializeField] Animator _enemyDeadUIAnime;            // 敵が死亡する時のUIの動画
    [SerializeField] TextMeshProUGUI _deadEnemyCount;       // 敵が死亡するUIに表示するテキストの数

    [Header("SkillSelect_UI")]                              // 以下はスキルを選択するＵＩ
    [SerializeField] GameObject _selectSkillCanvas;         // スキル選択するキャンパス
    [SerializeField] GameObject _selectSkillBox_01;         // 一番目のスキル選択するケース
    [SerializeField] GameObject _selectSkillBox_02;         // 二番目のスキル選択するケース
    [SerializeField] GameObject _selectSkillBox_03;         // 三番目のスキル選択するケース

    [Header("Options")]                                     // 以下はオプションUI
    [SerializeField] GameObject _options;                   // オプションUI
    [SerializeField] Animator _optionsAnimetor;             // オプションUIの動画
    [SerializeField] Slider _soundSlider;                   // 音を調整するスライダー
    [SerializeField] Slider _bgmSlider;                     // 音楽を調整するスライダー

    [SerializeField] Image _soundBGImage;                   // 音を調整するスライダーを表示する背景
    [SerializeField] Image _musicBGImage;                   // 音楽を調整するスライダーを表示する背景 
    [SerializeField] GameObject _soundFillRed;              // 音を調整するスライダーのフィル用の赤色
    [SerializeField] GameObject _musicFillRed;              // 音楽を調整するスライダーのフィル用の赤色

    [Header("GameOver_UI")]
    [SerializeField] GameObject _gameOverBG;                // ゲームが終わるUI
    [SerializeField] TextMeshProUGUI _enemyDeadCountText;   // 敵が死亡するUIに表示するテキスト
    [SerializeField] Animator _gameOverAnime;               // ゲームが終わるUIの動画
    [SerializeField] Button _gameOverYesBT;                 // ゲームが終わる時選択されているボタン
    [SerializeField] GameObject _gameOverYesEgg;            // 「YES」ボタンにあるタマゴのイメージ
    [SerializeField] GameObject _gameOverYes;               // 「YES」ボタンが選択されていない
    [SerializeField] GameObject _gameOverYesOk;             // 「YES」ボタンが選択されている
    [SerializeField] GameObject _gameOverNoEgg;             // 「NO」ボタンにあるタマゴのイメージ
    [SerializeField] GameObject _gameOverNo;                // 「NO」ボタンが選択されていない
    [SerializeField] GameObject _gameOverNoOk;              // 「NO」ボタンが選択されている

    [HideInInspector]
    public int uiEggCount;      // UIに表示するたまごの数

    GameObject selectSkill_01;  // 一番目のスキル
    GameObject selectSkill_02;  // 二番目のスキル
    GameObject selectSkill_03;  // 三番目のスキル

    //=============================== Awake =================================>
    void Awake()
    {
        instance = this;

        //  UIタマゴの数を指定
        uiEggCount = PlayerController.instance._skill.eggCount;

        //  音の音量を更新する
        _soundSlider.value = PlayerPrefs.GetFloat("soundValue");
        //  音楽の音量を更新する
        _bgmSlider.value = PlayerPrefs.GetFloat("bgmValue");
    }

    //=============================== Start =================================>
    void Start()
    {
        // 音音量のスライダーが動いたら反応する関数を登録する
        _soundSlider.onValueChanged.AddListener(OnSoundSliderValueChanged);
        // 音楽音量のスライダーが動いたら反応する関数を登録する
        _bgmSlider.onValueChanged.AddListener(OnBGMSliderValueChanged);
    }

    //=============================== Function =================================>

    //------------------------------- Skill --------------------------------->

    //　移動スピードを表示
    public void ShowMoveSpeed(float speed)
    {
        moveSpeed.SetText(speed.ToString());
    }

    //　タマゴの数を表示
    public void ShowEggCount()
    {
        eggCount.SetText(uiEggCount.ToString());
    }

    //　爆発距離を表示
    public void ShowBombRange(float range)
    {
        bombRange.SetText(range.ToString());
    }

    //　レベルを表示
    public void ShowPlayerLevel(int level)
    {
        _playerLevel.SetText(level.ToString());
    }

    //　経験を表示
    public void ShowExpCount(float getExp, float needExp)
    {
        _experienceCount.SetText(getExp + "/" + needExp.ToString());
    }

    //　経験スライダーを表示
    public void ShowExperienceSlider(float value)
    {
        _experienceSlider.value = value;
    }

    //　死亡した敵の数を表示
    public void DeadEnemyCount(int enemyCount)
    {
        _deadEnemyCount.SetText(enemyCount.ToString());
    }

    //　ゲーム実行タイムを表示
    public void ShowElapsedTime(string Timer)
    {
        _elapsedTime.SetText(Timer);
    }

    //　敵が死亡時のアニメをプレー
    public void PlayEnmeyDeadAnime()
    {
        _enemyDeadUIAnime.SetTrigger(isEnemyDieHash);
        _expUpAnime.SetTrigger(isEXPUpHash);
    }

    //　レベルアップアニメをプレー
    public void PlaylevelUpAnime()
    {
        _levelUpAnime.SetTrigger(isLevelUpHash);
    }

    //------------------------------- Sound & Music --------------------------------->

    //　音のスライダーを動く
    void OnSoundSliderValueChanged(float value)
    {
        AudioController.instance.PlaySelectUISound();
        AudioController.instance.SetSoundValue(value);
    }

    //　音楽のスライダーを動く
    void OnBGMSliderValueChanged(float value)
    {
        AudioController.instance.PlaySelectUISound();
        AudioController.instance.SetBGMValue(value);
    }

    //　音UIの背景を表示する
    public void ShowSoundBG()
    {
        //　音UIの背景を表示する
        _soundBGImage.enabled = true;
        _soundFillRed.SetActive(true);
    }

    //　音UIの背景を隠す
    public void HidSoundBG()
    {
        //　音UIの背景を隠す
        _soundBGImage.enabled = false;
        _soundFillRed.SetActive(false);
    }

    //　音楽UIの背景を表示する
    public void ShowMusicBG()
    {
        //　音楽UIの背景を表示する
        _musicBGImage.enabled = true;
        _musicFillRed.SetActive(true);
    }

    //　音楽UIの背景を隠す
    public void HidMusicBG()
    {
        //　音楽UIの背景を隠す
        _musicBGImage.enabled = false;
        _musicFillRed.SetActive(false);
    }

    //------------------------------- Options --------------------------------->

    // オプション設定
    public void Options()
    {
        AudioController.instance.PlayClickUISound();

        // 「オプション設定」が非アクティブ時
        if (!_options.activeSelf)
        {
            //「オプション設定」をアクティブする
            _options.SetActive(true);
            //「一時停止」のアニメをプレー
            _optionsAnimetor.SetBool(isPauseHash, true);

            //「スキル選択キャンバス」がアクティブしてたら
            if (!_selectSkillCanvas.activeSelf)
            {
                PlayerController.instance.OnGamePlayDisable();
                GameController.instance.PauseGame();
            }
        }

        // 「オプション設定」がアクティブ時
        else if (_options.activeSelf)
        {
            //「一時停止」のアニメを禁止する
            _optionsAnimetor.SetBool(isPauseHash, false);
            //「オプション設定」を非アクティブする
            _options.SetActive(false);

            //「スキル選択キャンバス」が非アクティブしてたら
            if (!_selectSkillCanvas.activeSelf)
            {
                PlayerController.instance.OnGamePlayEnable();
                GameController.instance.ResumeGame();
            }
        }
    }

    //-------------------------- SelectSkill -------------------------->

    //　スキル選択を表示
    public void ShowSelectSkill()
    {
        //「スキル選択」キャンパスを表示
        _selectSkillCanvas.SetActive(true);
        //　1番目に出て来るスキルを３つのスキル中で１つを選んで表示する
        selectSkill_01 = _selectSkillBox_01.transform.GetChild(Random.Range(0, 3)).gameObject;
        selectSkill_01.SetActive(true);
        // デフォルト選択
        selectSkill_01.gameObject.GetComponent<Button>().Select();

        //　2番目に出て来るスキルを３つのスキル中で１つを選んで表示する
        selectSkill_02 = _selectSkillBox_02.transform.GetChild(Random.Range(0, 3)).gameObject;
        selectSkill_02.SetActive(true);

        //　3番目に出て来るスキルを３つのスキル中で１つを選んで表示する
        selectSkill_03 = _selectSkillBox_03.transform.GetChild(Random.Range(0, 3)).gameObject;
        selectSkill_03.SetActive(true);
    }

    //　タマゴの数を増やす
    public void OnEggCountUp()
    {
        // UIに表示するタマゴの数を増やす
        uiEggCount++;
        PlayerController.instance.EggCountUp();
        // タマゴの数を増やすアニメをプレー
        _countUpAnime.SetTrigger(isCountUpHash);

        HideSelectSkillBox();
        AudioController.instance.PlaySelectSkillSound();
        GameController.instance.ResumeGame();
        PlayerController.instance.OnGamePlayEnable();
    }

    //　爆発距離を増やす
    public void OnBombRangeUp()
    {
        PlayerController.instance.BombRangeUp();
        // 爆発距離を増やすアニメをプレー
        _rangeUpAnime.SetTrigger(isRangeUpHash);

        HideSelectSkillBox();
        AudioController.instance.PlaySelectSkillSound();
        GameController.instance.ResumeGame();
        PlayerController.instance.OnGamePlayEnable();
    }

    //　移動スピードを上げる
    public void OnMoveSpeedUp()
    {
        PlayerController.instance.MoveSpeedUp();
        // 移動スピードを上げるアニメをプレー
        _speedUpAnime.SetTrigger(isSpeedUpHash);

        HideSelectSkillBox();
        AudioController.instance.PlaySelectSkillSound();
        GameController.instance.ResumeGame();
        PlayerController.instance.OnGamePlayEnable();
    }

    //　スキル選択するボックスを隠す
    public void HideSelectSkillBox()
    {
        //　スキル選択キャンパスを非アクティブ
        _selectSkillCanvas.SetActive(false);
        //　1番目のスキルを非アクティブ
        selectSkill_01.SetActive(false);
        //　2番目のスキルを非アクティブ
        selectSkill_02.SetActive(false);
        //　3番目のスキルを非アクティブ
        selectSkill_03.SetActive(false);
    }

    //------------------------------- GameOver --------------------------------->

    // 「ゲームオーバー」画面を表示
    public void ShowGameOver(int enemyDeadCount)
    {
        // ゲームオーバー背景をアクティブ
        _gameOverBG.SetActive(true);
        // ゲームオーバーアニメをプレー
        _gameOverAnime.SetTrigger(IsGameOverHash);
        // 敵を倒した数を設定する
        _enemyDeadCountText.SetText(enemyDeadCount.ToString());
        // 「YES」ボタンをデフォルト選択にする
        _gameOverYesBT.Select();
    }

    //「YES」を表示
    public void ShowYes()
    {
        //「YES」にあるタマゴのUIが非アクティブだったら
        if (!_gameOverYesEgg.activeSelf)
        {

            //「YES」にあるタマゴのUIをアクティブ
            _gameOverYesEgg.SetActive(true);
            //「YESOK」ボタンをアクティブ
            _gameOverYesOk.SetActive(true);
            //「YES」ボタンを非アクティブ
            _gameOverYes.SetActive(false);
            //「NO」にあるタマゴのUIを非アクティブ
            _gameOverNoEgg.SetActive(false);
            //「NOOK」ボタンを非アクティブ
            _gameOverNoOk.SetActive(false);
            //「NO」ボタンをアクティブ
            _gameOverNo.SetActive(true);
        }
    }

    //「NO」を表示
    public void ShowNo()
    {
        //「NO」にあるタマゴのUIが非アクティブだったら
        if (!_gameOverNoEgg.activeSelf)
        {
            //「NO」にあるタマゴのUIをアクティブ
            _gameOverNoEgg.SetActive(true);
            //「NOOK」ボタンをアクティブ
            _gameOverNoOk.SetActive(true);
            //「NO」ボタンを非アクティブ
            _gameOverNo.SetActive(false);
            //「YES」にあるタマゴのUIを非アクティブ
            _gameOverYesEgg.SetActive(false);
            //「YESOK」ボタンを非アクティブ
            _gameOverYesOk.SetActive(false);
            //「NO」ボタンを非アクティブ
            _gameOverYes.SetActive(true);
        }
    }
}
