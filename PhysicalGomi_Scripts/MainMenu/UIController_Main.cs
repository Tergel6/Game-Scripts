using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIController_Main : MonoBehaviour
{
    public static bool _pointerEnterStart;          //　マウスが「Start」ボタンを選択するを確認

    [SerializeField] Transform _startBox;           // StartボタンのTransfor
    [SerializeField] Transform _exitBox;            // ExitボタンのTransform
    [SerializeField] GraphicRaycaster _uiRaycaster; // UIのレイキャスト
    [SerializeField] float _delayTime;              // 遅延時間  
    [SerializeField] float _multiple;               // 大きくする倍数

    Vector3 _oldStartScale;                         // Startの元の大きさ
    Vector3 _newStartScale;                         // Startの大きくなる大きさ

    Vector3 _oldExitScale;                          // Exitの元の大きさ
    Vector3 _newExitScale;                          // Exitの大きくなる大きさ

    //===================================== Start ========================================> 
    void Start()
    {
        _pointerEnterStart = true;                  // 最初はStartが選ばれている

        _uiRaycaster.enabled = true;                // UIのレイキャストをオンにする

        _oldStartScale = _startBox.localScale;      // 元の大きさを保存
        _newStartScale = _oldStartScale * _multiple;     // 大きくするサイズ
        _oldExitScale = _exitBox.localScale;        // 元の大きさを保存
        _newExitScale = _oldExitScale * _multiple;       // 大きくするサイズ
    }

    //===================================== Function ========================================> 
    //　マウスが「Start」ボタンを選択する
    public void PointerEnterStart()
    {
        _pointerEnterStart = true;
        ScaleStartBox();
        AudioController.instance.PlaySelectButton();
    }
    //　マウスが「Exit」ボタンを選択する
    public void PointerEnterExit()
    {
        _pointerEnterStart = false;
        ScaleExitBox();
        AudioController.instance.PlaySelectButton();
    }
    // Startのサイズ変更
    void ScaleStartBox()
    {
        if (_startBox.localScale != _newStartScale)
        {
            _exitBox.localScale = _oldExitScale;
            _startBox.localScale = _newStartScale;
        }
    }

    // Exitのサイズ変更
    void ScaleExitBox()
    {
        if (_exitBox.localScale != _newExitScale)
        {
            _startBox.localScale = _oldStartScale;
            _exitBox.localScale = _newExitScale;
        }
    }

    // Startボタンを押した時
    public void StartGame()
    {
        _uiRaycaster.enabled = false;
        StartCoroutine(LateStartGame());
    }
    // Exitボタンを押した時
    public void ExitGame()
    {
        _uiRaycaster.enabled = false;
        StartCoroutine(LateExitGame());
    }

    // ゲーム開始の遅延処理
    IEnumerator LateStartGame()
    {
        yield return new WaitForSeconds(_delayTime);
        AudioController.instance.StopBGM();
        SceneManager.LoadScene("GamePlay");
    }

    // ゲーム終了の遅延処理
    IEnumerator LateExitGame()
    {
        yield return new WaitForSeconds(_delayTime);
        Application.Quit();
    }

}
