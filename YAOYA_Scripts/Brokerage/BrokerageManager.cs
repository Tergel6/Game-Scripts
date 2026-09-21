
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

//============================ Class ============================>

// 仲介店のデータ
[Serializable]
public class BrokerageData
{
    public int _staff;                                      // スタッフ数
    public List<int> _canNotBuyNums = new List<int>();      // 購入済みショップの番号リスト
}

//============================ MonoBehaviour ============================>
public class BrokerageManager : MonoBehaviour
{
    public static BrokerageManager Instance { get; private set; }

    [SerializeField] ShopsData[] _shops;                    // ショップデータ一覧
    [SerializeField] GameObject _shopPrefab;                // ショップ UI プレハブ
    [SerializeField] Transform _contantParent;              // ショップ一覧の親オブジェクト
    [Space]
    //------------------------------------------->
    [SerializeField] Button _brokerageBT;                   // 仲介店ボタン
    [SerializeField] TextMeshProUGUI _nameUI;               // ショップ名
    [SerializeField] Image _imageUI;                        // ショップ画像
    [SerializeField] TextMeshProUGUI _priceUI;              // 価格
    [SerializeField] TextMeshProUGUI _stallTypesUI;         // 品目数
    [SerializeField] TextMeshProUGUI _stallSizeUI;          // 店舗サイズ
    [SerializeField] TextMeshProUGUI _wareHouseSizeUI;      // 倉庫サイズ
    [SerializeField] Button _buyBT;                         // 購入ボタン
    //------------------------------------------->
    List<ShopsData> _shopPrefabs = new();                   // ショップデータのリスト
    List<GameObject> _shopGameObjects= new();               // ショップ UI オブジェクトのリスト

    BrokerageData _brokerageData = new BrokerageData();     // 仲介店データ本体

    ShopsData _currentShop;                                 // 現在選択中のショップ
    int _shopNum = 0;                                       // 現在のショップ番号

    //============================ Awake ============================>
    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        // ショップ一覧 UI を生成する
        int num = 0;
        foreach (var shop in _shops)
        {
            var obj = Instantiate(_shopPrefab, _contantParent);
            obj.GetComponent<BrokerageShopUI>().SetupBTIcon(shop, num);
            _shopGameObjects.Add(obj);
            _shopPrefabs.Add(shop);
            num++;
        }

        // 最初のショップを表示する
        SetupShop(_shopPrefabs[_shopNum], _shopNum);
    }

    //============================ Function ============================>

    // ショップ情報を UI にセットする
    public void SetupShop(ShopsData shop, int num)
    {
        _shopNum = num;

        // 購入済みならボタンを無効化
        CanNotClickBuyBT();

        _currentShop = shop;

        _nameUI.SetText(shop._name);
        _imageUI.sprite = shop._image;
        _priceUI.SetText(shop._price.ToString());
        _stallTypesUI.SetText(shop._stallTypes.ToString());
        _stallSizeUI.SetText(shop._stallSize.ToString());
        _wareHouseSizeUI.SetText(shop._wareHouseSize.ToString());

        // 選択中のショップをハイライト表示
        _shopGameObjects[num].GetComponent<BrokerageShopUI>().SelectShopUI();
    }

    // 現在選択中のショップを取得する
    public ShopsData GetCurrentShop()
    {
        return _currentShop;
    }

    // 前のショップへ移動
    public void ClickBeforeShop()
    {
        if (_shopNum > 0)
        {
            _shopNum--;
            SetupShop(_shopPrefabs[_shopNum], _shopNum);
        }
        else
        {
            _shopNum = 0;
            SetupShop(_shopPrefabs[_shopNum], _shopNum);
        }
    }

    // 次のショップへ移動
    public void ClickNextShop()
    {
        if (_shopNum < _shopPrefabs.Count - 1)
        {
            _shopNum++;
            SetupShop(_shopPrefabs[_shopNum], _shopNum);
        }
        else
        {
            _shopNum = _shopPrefabs.Count - 1;
            SetupShop(_shopPrefabs[_shopNum], _shopNum);
        }
    }

    // ショップを購入する
    public void ClickBuy()
    {
        bool canBuy = PlayerData.Instance.UseMoney(_shopPrefabs[_shopNum]._price);
        if (canBuy)
        {
            // 購入済みショップとして記録
            _brokerageData._canNotBuyNums.Add(_shopNum);

            // ショップをゲーム内に生成
            ShopManager.Instance.SpawnShopPrefab(_currentShop);

            // 倉庫容量を更新
            PlayerData.Instance.SetPlayerCapacity(_shopPrefabs[_shopNum]._wareHouseSize);

            // UI やマーケットを開く
            ShopManager.Instance.ShowShopUI();
            MarketManager.Instance.OpenTheMarket();
            AreaManager.Instance.InteractableAreasBT();

            CanNotClickBuyBT();

            UIController.Instance.ShowAndSetTip("Bought The " + _currentShop._name);
        }
        else
        {
            UIController.Instance.ShowAndSetTip("Not enough money");
        }
    }

    // 購入済みショップは購入ボタンを押せないようにする
    void CanNotClickBuyBT()
    {
        if (_brokerageData._canNotBuyNums.Count <= 0) return;
        foreach (int num in _brokerageData._canNotBuyNums)
        {
            if (_shopNum == num)
            {
                _buyBT.interactable = false;
                break;
            }
            else
            {
                _buyBT.interactable = true;
            }
        }
    }
    //------------------------------------------------------------------->

    // 仲介店 UI を押せる状態にする
    public void InteractableBrokerageBT()
    {
        _brokerageBT.interactable = true;
    }

    // 仲介店 UI を押せない状態にする
    public void UnInteractableBrokerageBT()
    {
        _brokerageBT.interactable = false;
    }

    //================================== SaveAndLoadBrokerageData ======================================>

    //　仲介店のデータを保存する
    public void SaveBrokerageData()
    {
        GameDataManager.gameData.brokerageData = _brokerageData;
    }

    //　仲介店のデータを読み込む
    public void LoadBrokerageData()
    {
        _brokerageData = GameDataManager.gameData.brokerageData;
    }



}
