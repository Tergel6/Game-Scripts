using UnityEngine;


public class AudioController : MonoBehaviour
{
	public static AudioController instance { get; private set; }

	[SerializeField] AudioClip _selectButton;	// 選択した時の音
    [SerializeField] AudioClip _clickButton;	// クリックした時の音
    [SerializeField] AudioClip _bgm;			// 通常のBGM
    [SerializeField] AudioClip _winBGM;			// 勝利のBGM
    [SerializeField] AudioClip _gameOverBGM;	// ゲームオーバーのBGM

    [SerializeField] AudioSource _soundSource;	// 効果音のAudioSource
    [SerializeField] AudioSource _bgmSource;	// BGMのAudioSource

    //===================================== Awake ========================================> 
    void Awake()
	{
		instance = this;
	}

	//===================================== Start ========================================> 
	void Start()
	{
		PlayBGM();	// ゲーム開始時にBGMを流す 
    }

    //===================================== Function ========================================> 

    // 選択音を再生
    public void PlaySelectButton()
	{
		PlaySound(_selectButton);
	}

    // クリック音を再生
    public void PlayClickButton()
	{
		PlaySound(_clickButton);
	}

    // 勝利BGMを再生
    public void PlayWin()
	{
		PlaySound(_winBGM);
	}

    // ゲームオーバーBGMを再生
    public void PlayGameOver()
	{
		PlaySound(_gameOverBGM);
	}

    // BGMのオン・オフ切り替え
    public void ToggleBGM()
	{
		if (_bgmSource.loop)
		{
			StopBGM();
		}
		else
		{
			PlayBGM();
		}
	}

    // 効果音のオン・オフ切り替え
    public void ToggleSound()
	{
		if (_soundSource.enabled)
		{
			_soundSource.enabled = false;
		}
		else
		{
			_soundSource.enabled = true;
		}
	}

    // 通常BGMを再生
    public void PlayBGM()
	{
		PlayBGM(_bgm, true);
	}

    // BGMを停止
    public void StopBGM()
	{
		StopBGM(_bgm, false);
	}

    //-------------------------------------->

    // 効果音を再生
    void PlaySound(AudioClip newClip)
	{
		if (_soundSource.enabled)
		{
			_soundSource.PlayOneShot(newClip);
		}

	}

    // BGMを再生
    void PlayBGM(AudioClip newBGM, bool loop)
	{
		_bgmSource.clip = newBGM;
		_bgmSource.loop = loop;
		_bgmSource.Play();
	}

    // BGMを止める
    void StopBGM(AudioClip newBGM, bool loop)
	{
		_bgmSource.clip = newBGM;
		_bgmSource.loop = loop;
		_bgmSource.Stop();
	}
}
