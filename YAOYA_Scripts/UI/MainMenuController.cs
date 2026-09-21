using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    public static MainMenuController Instance { get; private set; }

    [SerializeField] GameObject _mainMenu;          // メインメニューの Canvas
    [SerializeField] GameObject _mainSetting;       // セットのUI
    [SerializeField] GameObject _mainThanks;        // 「ありがとう」の UI

    //============================ Awake ============================>
    private void Awake()
    {
        Instance = this; 
    }

    //============================ Function ============================>

    // メインメニューを表示する
    public void ShowMainMenu()
    {
        _mainMenu.SetActive(true);
    }
    //　メインメニューを非表示する
    public void HidMainMenu()
    {
        _mainMenu.SetActive(false);
    }

    // セットのUIを表示する
    public void ShowMainSetting()
    {
        _mainSetting.SetActive(true);
    }
    //　セットのUIを非表示する
    public void HidMainSetting()
    {
        _mainSetting.SetActive(false);
    }

    // 「ありがとう」のUIを表示する
    public void ShowMainThanks()
    {
        _mainThanks.SetActive(true);
    }
    //　「ありがとう」のUIを非表示する
    public void HidMainThanks()
    {
        _mainThanks.SetActive(false);
    }
}
