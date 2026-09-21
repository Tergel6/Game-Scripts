using System.Collections;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SelectMapUI : MonoBehaviour
{
    [SerializeField] GameObject _scorePR1;              //　第1ステージのベストスコアー1
    [SerializeField] GameObject _scorePR2;              //　第1ステージのベストスコアー2
    [SerializeField] GameObject _scorePR1_2;            //　第2ステージのベストスコアー1
    [SerializeField] GameObject _scorePR2_2;            //　第2ステージのベストスコアー2
    [SerializeField] GameObject _scorePR1_3;            //　第3ステージのベストスコアー1
    [SerializeField] GameObject _scorePR2_3;            //　第3ステージのベストスコアー2

    [Space]
    [SerializeField] TextMeshProUGUI _bestScoreText1;   //　第1ステージのベストスコアー1のテクスト
    [SerializeField] TextMeshProUGUI _bestScoreText2;   //　第1ステージのベストスコアー2のテクスト
    [SerializeField] TextMeshProUGUI _bestScoreText1_2; //　第2ステージのベストスコアー1のテクスト
    [SerializeField] TextMeshProUGUI _bestScoreText2_2; //　第2ステージのベストスコアー2のテクスト
    [SerializeField] TextMeshProUGUI _bestScoreText1_3; //　第3ステージのベストスコアー1のテクスト
    [SerializeField] TextMeshProUGUI _bestScoreText2_3; //　第3ステージのベストスコアー2のテクスト

    [Space]
    [SerializeField] GameObject _loadSceneAsync;        //　非同期でシーンを読み込む画面
    [SerializeField] Slider _loadGameSlider;            //　非同期でシーンを読み込むスライダー
    [SerializeField] TextMeshProUGUI _loadGameText;     //　非同期でシーンを読み込むテクスト

    //============================== Start ================================>
    void Start()
    {
        // V-Syncをオフにする
        QualitySettings.vSyncCount = 0;
        // ゲームのフレームレートを60に設定する
        Application.targetFrameRate = 60;

        //　非同期でシーンを読み込む画面非表示する
        _loadSceneAsync.SetActive(false);

        /*
         * 「PlayerPrefs」からベストスコアを個別に指定する
         */
        _bestScoreText1.text = PlayerPrefs.GetInt("bestScore_01").ToString();
        _bestScoreText2.text = PlayerPrefs.GetInt("bestScore_01").ToString();
        _bestScoreText1_2.text = PlayerPrefs.GetInt("bestScore_02").ToString();
        _bestScoreText2_2.text = PlayerPrefs.GetInt("bestScore_02").ToString();
        _bestScoreText1_3.text = PlayerPrefs.GetInt("bestScore_03").ToString();
        _bestScoreText2_3.text = PlayerPrefs.GetInt("bestScore_03").ToString();
    }

    //============================== Function ================================>

    //　第1ステージのベストスコアー1を表示
    public void ShowScore1()
    {
        // 第1ステージのベストスコアー1が非アクティブだったら
        if (!_scorePR1.activeSelf)
        {
            // 第1ステージのベストスコアー1がアクティブ
            _scorePR1.SetActive(true);
            // 第1ステージのベストスコアー2が非アクティブ
            _scorePR2.SetActive(false);
        }
    }

    //　第1ステージのベストスコアー2を表示
    public void ShowScore2()
    {
        // 第1ステージのベストスコアー2が非アクティブだったら
        if (!_scorePR2.activeSelf)
        {
            // 第1ステージのベストスコアー2がアクティブ
            _scorePR2.SetActive(true);
            // 第1ステージのベストスコアー1が非アクティブ
            _scorePR1.SetActive(false);
        }
    }

    //　第2ステージのベストスコアー1を表示
    public void ShowScore1_2()
    {
        // 第2ステージのベストスコアー1が非アクティブだったら
        if (!_scorePR1_2.activeSelf)
        {
            // 第2ステージのベストスコアー1がアクティブ
            _scorePR1_2.SetActive(true);
            // 第2ステージのベストスコアー2が非アクティブ
            _scorePR2_2.SetActive(false);
        }
    }

    //　第2ステージのベストスコアー2を表示
    public void ShowScore2_2()
    {
        // 第2ステージのベストスコアー2が非アクティブだったら
        if (!_scorePR2_2.activeSelf)
        {
            // 第2ステージのベストスコアー2がアクティブ
            _scorePR2_2.SetActive(true);
            // 第2ステージのベストスコアー1が非アクティブ
            _scorePR1_2.SetActive(false);
        }
    }

    //　第3ステージのベストスコアー1を表示
    public void ShowScore1_3()
    {
        // 第3ステージのベストスコアー1が非アクティブだったら
        if (!_scorePR1_3.activeSelf)
        {
            // 第3ステージのベストスコアー1がアクティブ
            _scorePR1_3.SetActive(true);
            // 第3ステージのベストスコアー2が非アクティブ
            _scorePR2_3.SetActive(false);
        }
    }

    //　第3ステージのベストスコアー2を表示
    public void ShowScore2_3()
    {
        // 第3ステージのベストスコアー2が非アクティブだったら
        if (!_scorePR2_3.activeSelf)
        {
            // 第3ステージのベストスコアー2がアクティブ
            _scorePR2_3.SetActive(true);
            // 第3ステージのベストスコアー1が非アクティブ
            _scorePR1_3.SetActive(false);
        }
    }
    //-------------------------------------------------------------->

    // 第1ステージを開始
    public void StartLevel01()
    {
        // ボタンのフォーカスを外す
        EventSystem.current.SetSelectedGameObject(null);
        AudioController.instance.PlayOnSkillSound();
        //　非同期でシーンを読み込む画面をアクティブ
        _loadSceneAsync.SetActive(true);
        //　第2番目のシーン(第1ステージ)を読み込む
        StartCoroutine(LoadLevel(2));
    }

    // 第2ステージを開始
    public void StartLevel02()
    {
        // ボタンのフォーカスを外す
        EventSystem.current.SetSelectedGameObject(null);
        AudioController.instance.PlayOnSkillSound();
        //　非同期でシーンを読み込む画面をアクティブ
        _loadSceneAsync.SetActive(true);
        //　第3番目のシーン(第2ステージ)を読み込む
        StartCoroutine(LoadLevel(3));
    }
    // 第3ステージを開始
    public void StartLevel03()
    {
        // ボタンのフォーカスを外す
        EventSystem.current.SetSelectedGameObject(null);
        AudioController.instance.PlayOnSkillSound();
        //　非同期でシーンを読み込む画面をアクティブ
        _loadSceneAsync.SetActive(true);
        //　第4番目のシーン(第3ステージ)を読み込む
        StartCoroutine(LoadLevel(4));
    }
    //------------------------------------------------------------------>

    // メインメニューに戻る
    public void OnBackHome()
    {
        AudioController.instance.PlayOnSkillSound();
        // 「メインメニュー」シーンを読み込む
        SceneManager.LoadScene("MainMenu");
    }

    //--------------------------------------------------------------------->

    //　シーンを読み込む
    IEnumerator LoadLevel(int loadLevel)
    {
        //　非同期に読み込むシーンを指定する
        AsyncOperation operation = SceneManager.LoadSceneAsync(loadLevel);
        //　読み込むが90%になったらいったん止まる
        operation.allowSceneActivation = false;

        //　読み込むが完了しない場合はずっと読み込む
        while (operation.progress < 0.9f)
        {
            //　進行度を０－１の間に設定する
            float progress = Mathf.Clamp01(operation.progress / 0.9f);

            //　読み込む進行度を非同期でシーンを読み込むスライダーに設定
            _loadGameSlider.value = progress;
            //　非同期に読み込む進行度を読み込むテキストに整数で表示する
            _loadGameText.SetText($"{progress * 100} %");

            yield return null;
        }
        //　シーンを読み込むスライダーの数値が１になる
        _loadGameSlider.value = 1f;
        //　読み込むテキストが100%になる
        _loadGameText.SetText("100%");

        //　入力待ち
        yield return new WaitUntil(() =>
                 //　「キーボードがないかとキーボードのエンターキーが押されたか」或は「コントローラーの指定したボタンが押されたかの判断」
                 Keyboard.current != null && Keyboard.current.enterKey.wasPressedThisFrame || 
                 Input.GetKeyDown(KeyCode.Joystick1Button1)
            );

        //　読み込むが完了
        operation.allowSceneActivation = true;
    }
}
