using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BrokerageShopUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _name;     // ショップ名の表示
    [SerializeField] Image _iconUIBT;           // ショップアイコン画像

    ShopsData data;                             // このボタンが持つショップデータ
    int _num;                                   // ショップの番号

    //============================ Start ============================>
    private void Start()
    {
        // 最初のショップ（番号0）は自動的に選択状態にする
        if (_num == 0)
        {
            SelectShopUI();
        }
    }

    //============================ Function ============================>

    // ボタンにショップ情報をセットする
    public void SetupBTIcon(ShopsData shop,int num)
    {
        data = shop;
        _num = num;

        _name.text =shop.name;              // ショップ名を表示
        _iconUIBT.sprite = shop._icon;      // アイコン画像を表示
    }

    // このショップを選択した時の処理
    public void SelectShop()
    {
        // 仲介店管理クラスに選択したショップを渡す
        BrokerageManager.Instance.SetupShop(data,_num);
    }

    // UI 上でこのボタンを選択状態にする
    public void SelectShopUI()
    {
        EventSystem.current.SetSelectedGameObject(gameObject);
    }
}
