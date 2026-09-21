using UnityEngine;
using UnityEngine.UI;

public class SettingsUI : MonoBehaviour
{
    [Space]
    [SerializeField] Image _soundBG;            // 音の背景
    [SerializeField] Image _musicBG;            // 音楽の背景
    [Space]
    [SerializeField] GameObject _soundRedText;  //　音スライダーを選択する時の赤い「Sound」
    [SerializeField] GameObject _musicRedText;  //　音楽スライダーを選択する時の赤い「Music」
    [Space]
    [SerializeField] GameObject _soundRedFill;  //　音スライダーを満たす赤色
    [SerializeField] GameObject _musicRedFill;  //　音楽スライダーを満たす赤色
    [Space]
    [SerializeField] Slider _soundSlider;       //　音スライダー
    [SerializeField] Slider _musicSlider;       //　音楽スライダー

    //=============================== Start ===============================>
    void Start()
    {
        //　「PlayerPrefs」から音の数値をゲットして音スライダーに設定
        _soundSlider.value = PlayerPrefs.GetFloat("soundValue");
        //　「PlayerPrefs」から音楽の数値をゲットして音楽スライダーに設定
        _musicSlider.value = PlayerPrefs.GetFloat("bgmValue");
    }
    private void Update()
    {
        AudioController.instance.SetSoundValue(_soundSlider.value);
        AudioController.instance.SetBGMValue(_musicSlider.value);
    }
    //============================= Function =============================>

    //　音の背景を表示
    public void ShowSoundBG()
    {
        //　音の背景を有効にする
        _soundBG.enabled = true;
        //　音スライダーの赤いテキストをアクティブ
        _soundRedText.SetActive(true);
        //　音スライダーを満たす赤色をアクティブ
        _soundRedFill.SetActive(true);
    }

    //　音の背景を非表示
    public void HidSoundBG()
    {
        //　音の背景を無効にする
        _soundBG.enabled = false;
        //　音スライダーの赤いテキストを非アクティブ
        _soundRedText.SetActive(false);
        //　音スライダーを満たす赤色を非アクティブ
        _soundRedFill.SetActive(false);
    }

    //　音楽の背景を表示
    public void ShowMusicBG()
    {
        //　音楽の背景を有効にする
        _musicBG.enabled = true;
        //　音楽スライダーの赤いテキストをアクティブ
        _musicRedText.SetActive(true);
        //　音楽スライダーを満たす赤色をアクティブ
        _musicRedFill.SetActive(true);
    }

    //　音楽の背景を非表示
    public void HidMusicBG()
    {
        //　音楽の背景を無効にする
        _musicBG.enabled = false;
        //　音楽スライダーの赤いテキストを非アクティブ
        _musicRedText.SetActive(false);
        //　音楽スライダーを満たす赤色を非アクティブ
        _musicRedFill.SetActive(false);
    }

}
