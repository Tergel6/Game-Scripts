using UnityEngine;

public class SpecialThanksUI : MonoBehaviour
{
    //　メインメニューに戻る
    public void BackMainMenu()  
    {
        //　「メインメニュー」シーンを読み込む
        SceneLoader.instance.LoadScene("MainMenu");
    }
}
