using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance {  get; private set; }


    //============================ Awake ============================>
    private void Awake()
    {
        Instance = this;
        QualitySettings.vSyncCount = 0;   // VSyncを閉める（絶対）
        Application.targetFrameRate = 60; // FPSを60に設定する
    }

    //============================ Function ============================>

    //　もう一回プレーする
    public void TryAgain()
    {
        Debug.LogWarning("Todo =>TryAgain");
    }
    //　メインメニューに戻る
    public void BackToHome()
    {
        SceneManager.LoadScene("SampleScene");
        Debug.LogWarning("Todo =>BackToHome");
    }

    //　ゲームを終了する
    public void ExitGame()
    {
        Application.Quit();
    }
}
