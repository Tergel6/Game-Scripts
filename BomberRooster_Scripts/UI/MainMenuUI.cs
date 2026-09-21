using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] CanvasGroup _mainCanvasGroup;  // メインメニューキャンパスのグループ
    [SerializeField] GameObject _settingCanvas;     // 設定キャンパス
    [SerializeField] Button _settingsButton;        // 設定ボタン
    [SerializeField] Slider _soundSlider;           // 音のスライダー

    [Space]
    [SerializeField] GameObject _playGameEgg;       // 「playGame」ボタンにあるタマゴ
    [SerializeField] GameObject _settingsEgg;       // 「settings」ボタンにあるタマゴ
    [SerializeField] GameObject _thanksEgg;         // 「thanks」ボタンにあるタマゴ
    [SerializeField] GameObject _exitEgg;           // 「exit」ボタンにあるタマゴ

    //========================= Start =========================>
    void Start()
    {
        // V-Syncをオフにする
        QualitySettings.vSyncCount = 0;
        // ゲームのフレームレートを60に設定する
        Application.targetFrameRate = 60;
    }

    //========================= Function =========================>

    //　「PlayGame」を選択
    public void SelectPlayGame()
    {
        AudioController.instance.PlaySelectUISound();
        //　「playGame」UIにあるタマゴをアクティブ
        _playGameEgg.SetActive(true);
    }

    //　「PlayGame」を選択してない
    public void DeselectPlayGame()
    {
        //　「playGame」UIにあるタマゴを非アクティブ
        _playGameEgg.SetActive(false);
    }

    //　「設定」を選択
    public void SelectSettings()
    {
        AudioController.instance.PlaySelectUISound();
        //　「設定」UIにあるタマゴをアクティブ
        _settingsEgg.SetActive(true);
    }

    //　「設定」を選択してない
    public void DeselectSettings()
    {
        //　「設定」UIにあるタマゴを非アクティブ
        _settingsEgg.SetActive(false);
    }

    //　「Thanks」を選択
    public void SelectThanks()
    {
        AudioController.instance.PlaySelectUISound();
        //　「Thanks」UIにあるタマゴをアクティブ
        _thanksEgg.SetActive(true);
    }

    //　「Thanks」を選択してない
    public void DeselectThanks()
    {
        //　「Thanks」UIにあるタマゴを非アクティブ
        _thanksEgg.SetActive(false);
    }

    //　「Exit」を選択
    public void SelectExit()
    {
        AudioController.instance.PlaySelectUISound();
        //　「Exit」UIにあるタマゴをアクティブ
        _exitEgg.SetActive(true);
    }

    //　「Exit」を選択してない
    public void DeselectExit()
    {
        //　「Exit」UIにあるタマゴを非アクティブ
        _exitEgg.SetActive(false);
    }

    //　ゲーム始まるボタンを押す
    public void OnStartGame()
    {
        AudioController.instance.PlayOnSkillSound();
        //「SelectMap」シーンを読み込む
        SceneLoader.instance.LoadScene("SelectMap");
    }

    //　設定ボタンを押す
    public void OnSettings()
    {
        AudioController.instance.PlayOnSkillSound();
        //　設定キャンパスをアクティブ
        _settingCanvas.SetActive(true);
        //　音のスライダーを選択
        _soundSlider.Select();

        /*
         * メインキャンパスグループを操作禁止なくする
        */
        _mainCanvasGroup.blocksRaycasts = false;
        _mainCanvasGroup.interactable = false;
    }

    //　設定画面を隠す
    public void HidSettings()
    {
        AudioController.instance.PlayOnSkillSound();
        //　設定キャンパスを非アクティブ
        _settingCanvas.SetActive(false);
        //　「設定」ボタンを選択
        _settingsButton.Select();

        /*
         * メインキャンパスグループを操作できるようにする
        */
        _mainCanvasGroup.blocksRaycasts =true;
        _mainCanvasGroup.interactable = true;
    }

    // 「Thanks」に入る
    public void OnThanksButton()　
    {
        AudioController.instance.PlayOnSkillSound();
        //「SpecialThanks」シーンを読み込む
        SceneLoader.instance.LoadScene("SpecialThanks");
    }

    // ゲームが終了
    public void OnApplicationQuit()　
    {
        AudioController.instance.PlayOnSkillSound();
        // アプリケーションが終了
        Application.Quit();
    }
}
