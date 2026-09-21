using UnityEngine;
using UnityEngine.InputSystem;

public class LoadGame : MonoBehaviour
{
    const int MaxJoystickButtons = 20; 　    //　Unityがサポートする一般的なボタン数（コントローラー）

    //========================= Start =========================>
    void Start()
    {
        Application.targetFrameRate = 60;   //　ゲームのフレームレートを60に設定する
    }
    //========================= Update =========================>
    void Update()
    {
        //　キーを押したのかを確認する（コントローラー）
        bool getKey = false;

        //　Unityがサポートする一般的なボタン数(20)回繰り返す（コントローラー）
        for (int i = 0; i < MaxJoystickButtons; i++)
        {
            //　Unityがサポートする一般的なボタンを押したのかの確認
            if (Input.GetKey(KeyCode.JoystickButton0 + i))
            {
                //　キーを押したに設定（コントローラー）
                getKey = true;
                break;
            }
        }
        //　「キーボードのどれか1つでもキーが押されたか」或は「Unityがサポートする一般的なボタンが押されたかの判断（コントローラー）」
        if (Keyboard.current.anyKey.wasPressedThisFrame || getKey)
        {
            //　「Level_02」シーンを読み込む
            SceneLoader.instance.LoadScene("Level_02");
        }
    }
}
