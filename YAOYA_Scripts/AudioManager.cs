using UnityEngine;

public class AudioManager : MonoBehaviour
{
    // UI の音をまとめて管理するシングルトン
    public static AudioManager Instance{get; private set;}

    [SerializeField] AudioClip _selectBTSound;      // ボタンを選択した時の音
    [SerializeField] AudioClip _clickBTSound;       // ボタンをクリックした時の音
    [SerializeField] AudioSource _soundSource;      // 効果音を再生する AudioSourc
    private void Awake()
    {
        Instance = this;
    }

    // ボタンを選択した時の効果音を再生する
    public void PlaySelectBTSound()
    {
        _soundSource.PlayOneShot(_selectBTSound);
    }

    // ボタンをクリックした時の効果音を再生する
    public void PlayClickBTSound()
    {
        _soundSource.PlayOneShot(_clickBTSound);
    }
}
