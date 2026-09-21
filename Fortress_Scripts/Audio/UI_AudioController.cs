using UnityEngine;

public class UI_AudioController : MonoBehaviour
{
    public static UI_AudioController Instance { get; private set; }

    [SerializeField] AudioClip _selectUISound;      // UI を選択した時の音
    [SerializeField] AudioClip _clickUISound;       // UI をクリックした時の音
    [SerializeField] AudioSource _soundSource;      // 効果音を再生する AudioSource

    //================================ Awake =================================>
    void Awake()
    {
        Instance = this;
    }

    //================================ Function =================================>

    // UI を選択した時の効果音をプレイするメソッド
    public void playSelectUISound()
    {
        _soundSource.PlayOneShot(_selectUISound);
    }

    // UI をクリックした時の効果音をプレイするメソッド
    public void playClickUISound()
    {
        _soundSource.PlayOneShot(_clickUISound);
    }
}
