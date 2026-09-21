using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_Home : MonoBehaviour
{
    [SerializeField] Button play;       // プレイボタン
    [SerializeField] Button exit;       // 終了ボタン　
    [SerializeField] int _frameRate;    // プレイボタン

    //================================ Start =================================>
    void Start()
    {
        // ゲームのフレームレートを設定する
        Application.targetFrameRate = _frameRate;
    }

    //================================ Function =================================>

    // ゲームシーンへ移動する（Scene 1）
    public void OnPlay()
    {
        SceneManager.LoadScene(1);
    }

    // アプリケーションを終了する
    public void OnExit()
    {
        Application.Quit();
    }
}
