using UnityEngine;

public class AudioController : MonoBehaviour
{
    public static AudioController instance { get; private set; }

    [SerializeField] AudioClip _placeEggSound;          // タマゴを置く音
    [SerializeField] AudioClip _bombBoomSound;          // タマゴが爆発音
    [SerializeField] AudioClip _levelUpSound;           // プレイヤーがレベルアップ音
    [SerializeField] AudioClip _gameOverSound;          // ゲームが終わる音（プレイヤーが死ぬ音）
    [SerializeField] AudioClip _onSkillSound;           // スキルを選択音
    [SerializeField] AudioClip _selectSkillSound;       // スキルを選択を確認する音
    [SerializeField] AudioClip _selectUISound;          // UIでの選択音
    [SerializeField] AudioClip _clickUISound;           // UIでの選択を確認する音

    [SerializeField] AudioSource _gamePlayBGMSource;    // ゲームの遊ぶ時の音楽
    [SerializeField] AudioSource _gameOverBGMSource;    // ゲームが終わる時の音楽（失敗した音楽）

    [SerializeField] AudioSource _seAudioSource;        // 音のSource

    //============================ Awake ============================>
    void Awake()
    {
        instance = this;
    }

    //============================ Start ============================>
    void Start()
    {
        // 音の音量をplayerPrefsから読み出す
        SetSoundValue(PlayerPrefs.GetFloat("soundValue"));
        // 音楽の音量をplayerPrefsから読み出す
        SetBGMValue(PlayerPrefs.GetFloat("bgmValue"));
    }

    //============================ Functions ============================>
    // プレー「タマゴを置く音」
    public void PlayPlaceEggSound()     
    {
        _seAudioSource.PlayOneShot(_placeEggSound);
    }

    // プレー「タマゴが爆発音」
    public void PlayBombBoomSound()     
    {
        _seAudioSource.PlayOneShot(_bombBoomSound);
    }

    // プレー「プレイヤーがレベルアップ音」
    public void PlayLevelUpSound()      
    {
        _seAudioSource.PlayOneShot(_levelUpSound);
    }

    // プレー「ゲームが終わる音（プレイヤーが死ぬ音）」
    public void PlayGameOverSound()     
    {
        _seAudioSource.PlayOneShot(_gameOverSound);
    }

    // プレー「スキルを選択音」
    public void PlayOnSkillSound()      
    {
        _seAudioSource.PlayOneShot(_onSkillSound);
    }

    // プレー「スキルを選択を確認する音」
    public void PlaySelectSkillSound()  
    {
        _seAudioSource.PlayOneShot(_selectSkillSound);
    }

    // プレー「UIでの選択音」
    public void PlaySelectUISound()     
    {
        _seAudioSource.PlayOneShot(_selectUISound);
    }

    // プレー「UIでの選択を確認する音」
    public void PlayClickUISound()      
    {
        _seAudioSource.PlayOneShot(_clickUISound);
    }

    //------------------- BGM ------------------------>

    // プレー「ゲームの遊ぶ時の音楽」
    public void PlayGamePlayBGM()       
    {
        _gamePlayBGMSource.Play();
    }

    // ストップ「ゲームの遊ぶ時の音楽」
    public void StopGamePlayBGM()       
    {
        _gamePlayBGMSource.Stop();
    }

    // プレー「ゲームが終わる時の音楽（失敗した音楽）」
    public void PlayGameOverBGM()       
    {
        _gameOverBGMSource.Play();
    }

    // ストップ「ゲームが終わる時の音楽（失敗した音楽）」
    public void StopGameOverBGM()       
    {
        _gameOverBGMSource.Stop();
    }

    //----------------------- Set Value ------------------------>
    // 音音量の調整
    public void SetSoundValue(float soundValue)             
    {
        _seAudioSource.volume = soundValue;
        // 音の音量をplayerPrefsに設定する
        PlayerPrefs.SetFloat("soundValue", soundValue);
        // 音の音量をplayerPrefsに保存する
        PlayerPrefs.Save();                                 
    }

    // 音楽音量の調整
    public void SetBGMValue(float bgmValue)                 
    {
        _gamePlayBGMSource.volume = bgmValue;
        _gameOverBGMSource.volume = bgmValue;
        // 音楽の音量をplayerPrefsに設定する
        PlayerPrefs.SetFloat("bgmValue", bgmValue);
        // 音楽の音量をplayerPrefsに保存する
        PlayerPrefs.Save();                                 
    }
}
