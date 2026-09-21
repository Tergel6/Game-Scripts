using UnityEngine;
using UnityEngine.UI;

public class NumberPadButton : MonoBehaviour
{
    public string key;          // ボタンの種類（0?9 / DEL / OK）

    //============================ Start ============================>
    void Start()
    {
        // このボタンが押された時に OnClick を呼ぶ
        GetComponent<Button>().onClick.AddListener(OnClick);
    }

    //============================ Click ============================>
    void OnClick()
    {
        // ShopManager が存在しない場合は何もしない
        if (ShopManager.Instance == null) return;

        // ボタンの種類に応じて処理を分ける
        switch (key)
        {
            case "DEL":
                // 入力した数字を削除する
                ShopManager.Instance.Delete();
                break;

            case "OK":
                // 数字入力 UI を閉じる
                ShopManager.Instance.HideInputNumber();
                break;

            default:
                // 数字を入力する（0?9）
                ShopManager.Instance.InputNumber(key);
                break;
        }
    }




}
