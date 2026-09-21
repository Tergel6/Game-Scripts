
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

//============================== Class ==========================>

// シリアライズ可能なデータクラス（DTO） 
[Serializable]
public class ItemsDataDTO    
{
    public string _id;
    //------------------Market---------------->
    public int _shelfLife;                      // 賞味期限
    public int _quantily;                       // 品質
    public int _costPrice;                      // 仕入れ価格
    public int _buyCount;                       // 仕入れ数
    //-----------------Shop------------------>
    public int _shopCount;                      // 店内在庫
    public int _salePosition;                   // 陳列位置
    public int _salePrice;                      // 販売価格
    public int _saleCount;                      // 販売中の数量

    public ItemsDataDTO(ItemsData item)
    {
        _id = item._id;

        //------------------Market---------------->
        _shelfLife = item._shelfLife;
        _quantily = item._quantily;
        _costPrice = item._costPrice;
        _buyCount = item._buyCount;

        //-----------------Shop------------------>
        _shopCount = item._shopCount;
        _salePosition = item._salePosition;
        _salePrice = item._salePrice;
        _saleCount = item._saleCount;
    }
}

//============================== MonoBehaviour ==========================>
public class MarketManager : MonoBehaviour
{
    public static MarketManager Instance { get; private set; }

    [SerializeField] ItemsData[] _items;                        // 市場に並ぶアイテム一覧
    [SerializeField] GameObject _itemPrefab;                    // アイテム UI Prefab
    [SerializeField] Transform _contentParent;                  // アイテム UI の親オブジェクト
        
    [SerializeField] TextMeshProUGUI _myMoneyUI;                // 所持金表示
    [SerializeField] TextMeshProUGUI _totalCostUI;              // 合計仕入れ額
    [SerializeField] TextMeshProUGUI _wareHouseCapacityUI;      // 倉庫容量表示

    [SerializeField] Button _marketBT;                          // 市場ボタン（開閉）
    List<GameObject> _itemPrefabs = new();                      // 生成したアイテム UI のリスト

    int _myMoney;                                               // 現在の所持金
    int _totalCost;                                             // 現在の仕入れ合計額
    int _wareHouseCapacity;                                     // 倉庫の最大容量
    int _usedCapacity;                                          // 使用中の倉庫容量

    bool _isMarketOpen;                                         // 市場が開いているかどうか

    //============================== Awake ==========================>

    private void Awake()
    {
        Instance = this;

        // アイテムの初期化（在庫・販売情報をリセット）
        InitItems();
    }

    //============================== Start ==========================>
    private void Start()
    {
        // 初期状態では市場を閉じる
        CloseTheMarket();

        // 市場 UI にアイテムを並べる
        foreach (var item in _items)
        {
            var obj = Instantiate(_itemPrefab, _contentParent);
            obj.GetComponent<MarketItemUI>().Setup(item);
            _itemPrefabs.Add(obj);
        }

        // 店にすでにあるアイテムを UI に反映
        InitShops();
    }
    //============================== Function ==========================>

    // 合計仕入れ額を取得
    public int GetTotalCost()
    {
        return _totalCost;
    }

    // 合計仕入れ額を更新
    public void SetTotalCost(int cost)
    {
        _totalCost += cost;
        _totalCostUI.SetText(_totalCost.ToString() + "$");

        // 所持金も同時に更新
        SetMyMoney(cost);
    }

    // 所持金を取得
    public int GetMyMoney()
    {
        return _myMoney;
    }

    // 所持金を更新
    public void SetMyMoney(int money)
    {
        _myMoney -= money;
        _myMoneyUI.SetText(_myMoney.ToString() + "$");
    }

    // 倉庫の空き容量を取得
    public int GetFreeCapacity()
    {
        return _wareHouseCapacity - _usedCapacity;
    }

    // 倉庫容量を更新
    public void SetWareHouseCapacity(int size)
    {
        _usedCapacity += size;
        PlayerData.Instance.SetUsedCapacity(_usedCapacity);
        _wareHouseCapacityUI.SetText(_usedCapacity + "/" + _wareHouseCapacity);
    }

    //---------------------------------------------------->

    // 仕入れ確定ボタン
    public void ClickConfirm()
    {
        // プレイヤーの所持金と倉庫容量を更新
        PlayerData.Instance.SetPlayerMoney(_myMoney);
        PlayerData.Instance.SetUsedCapacity(_usedCapacity);

        // 仕入れたアイテムを店に送る
        foreach (var itemPrefab in _itemPrefabs)
        {
            itemPrefab.GetComponent<MarketItemUI>().GoToShop();
        }
        ShopManager.Instance.ShowItemUI();
        AreaManager.Instance.InteractableAreasBT();

        // 市場画面を初期化
        InitMarket();

        // アイテムデータを保存
        SaveItemsData();
    }

    // 市場画面を初期化する
    public void InitMarket()
    {
        _myMoney = PlayerData.Instance.GetPlayerMoney();
        _totalCost = 0;
        _wareHouseCapacity = PlayerData.Instance.GetPlayerCapacity();
        _usedCapacity = PlayerData.Instance.GetUsedCapacity();

        _myMoneyUI.SetText(_myMoney.ToString());
        _totalCostUI.SetText(_totalCost.ToString());
        _wareHouseCapacityUI.SetText(_usedCapacity + "/" + _wareHouseCapacity);

        // UI の仕入れ数をリセット
        foreach (var itemPrefab in _itemPrefabs)
        {
            itemPrefab.GetComponent<MarketItemUI>().InitMarketItem();
        }
    }

    // 店にすでにあるアイテムを UI に反映する
    public void InitShops()
    {
        foreach (var itemPrefab in _itemPrefabs)
        {
            itemPrefab.GetComponent<MarketItemUI>().InitShop();
        }
    }

    //------------------------------------------------------>

    // 品物の設定を初期状態に戻す
    void InitItems()
    {
        if (_items.Length == 0)
            return;

        for (int i = 0; i < _items.Length; i++)
        {
            _items[i]._buyCount = 0;
            _items[i]._shopCount = 0;
            _items[i]._salePosition = 0;
            _items[i]._salePrice = 0;
            _items[i]._saleCount = 0;
        }
    }

    //-------------------------------------------->

    // 市場を開く
    public void OpenTheMarket()
    {
        _isMarketOpen = true;
        _marketBT.interactable = true;
    }

    // 市場を閉じる
    public void CloseTheMarket()
    {
        _isMarketOpen = false;
        _marketBT.interactable = false;
    }
    public bool GetMarketStatus()
    {
        return _isMarketOpen;
    }

    //============================ SaveAndLoadItemsData ===========================>

    //商品のデータを保存する（DTO に変換）
    public void SaveItemsData()
    {
        GameDataManager.gameData.itemsData = new List<ItemsDataDTO>();
        foreach (ItemsData newData in _items)
        {
            GameDataManager.gameData.itemsData.Add(new ItemsDataDTO(newData));
        }
    }

    //商品のデータを読み込む（DTO → ItemsData）
    public void LoadItemsData()
    {
        foreach (var newItem in GameDataManager.gameData.itemsData)
        {
            foreach (var oldItem in _items)
            {
                if (newItem._id == oldItem._id)
                {
                    oldItem._shelfLife = newItem._shelfLife;
                    oldItem._quantily = newItem._quantily;
                    oldItem._costPrice = newItem._costPrice;
                    oldItem._buyCount = newItem._buyCount;
                    //-----------------Shop------------------>
                    oldItem._shopCount = newItem._shopCount;
                    oldItem._salePosition = newItem._salePosition;
                    oldItem._salePrice = newItem._salePrice;
                    oldItem._saleCount = newItem._saleCount;
                }
            }
        }
    }
}
