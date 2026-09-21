using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MarketItemUI : MonoBehaviour
{

    [SerializeField] Image _iconUI;                         // アイテムのアイコン
    [SerializeField] Image _seasonUI;                       // 季節アイコン（旬の時期に表示）
    [SerializeField] List<Image> _stars=new();              // 品質を示す星アイコン
    [Header("TXT_UI")]
    [SerializeField] TextMeshProUGUI _nameUI;               // アイテム名
    [SerializeField] TextMeshProUGUI _costPriceUI;          // 仕入価格
    [SerializeField] TextMeshProUGUI _shelfLifeUI;          // 賞味期限
    [SerializeField] TextMeshProUGUI _originUI;             // 原産地
    [SerializeField] TextMeshProUGUI _sizeUI;               // サイズ
    [SerializeField] TextMeshProUGUI _buyCountUI;           // 仕入れ数

    [Header("Month")]
    int _february = 2;
    int _march = 3;
    int _may = 5;
    int _june = 6;
    int _august = 8;
    int _september = 9;
    int _november = 11;
    int _december = 12;

    [Space]
    ItemsData data;                                         // この UI が表示するアイテムデータ

    //============================ Function ============================>
    public void Setup(ItemsData item)
    {
        data = item;
        data._id = item._id;
        _nameUI.SetText(item._name);
        _iconUI.sprite = item._icon;
        _originUI.SetText(item._origin);
        _sizeUI.SetText(item._size.ToString());

        SetQuanlityStar(item._quantily);                    // 品質の星を設定
        CheckSeason(item._season);                          // 旬の季節かどうかチェック
        ShowItemUI();                                       // 基本情報を表示
        InitMarketItem();                                   // 仕入れ数を初期化
    }

    //--------------------------------------------->

    // 品質の星を表示する
    void SetQuanlityStar(int num)
    {
        foreach (Image star in _stars) {
            if (star.enabled)
            {
                star.enabled = false;
            }
        }
        _stars[num].enabled = true;
    }

    //--------------------------------------------->

    // 基本情報を UI に表示する
    void ShowItemUI()
    {
        _shelfLifeUI.SetText(data._shelfLife.ToString());
        _costPriceUI.SetText(data._costPrice.ToString());  
    }

    //-------------------------Button---------------------------->

    // 仕入れ数を減らす
    public void SubtractBuyCount()
    {
        if (data._buyCount > 0)
        {
            data._buyCount--;
            _buyCountUI.SetText(data._buyCount.ToString());

            // 合計コストと倉庫容量を更新
            MarketManager.Instance.SetTotalCost(-(data._costPrice));
            MarketManager.Instance.SetWareHouseCapacity(-(data._size));
        }
    }

    // 仕入れ数を増やす
    public void AddBuyCount()
    {
        int myBalance = MarketManager.Instance.GetMyMoney();
        int myCapacity = MarketManager.Instance.GetFreeCapacity();

        // お金または倉庫容量が足りない場合は仕入れできない
        if (myBalance < data._costPrice || myCapacity < data._size) return;
        
            data._buyCount++;
            _buyCountUI.SetText(data._buyCount.ToString());

            MarketManager.Instance.SetTotalCost(data._costPrice);
            MarketManager.Instance.SetWareHouseCapacity(data._size);
    }

    //----------------------------------------------------------->

    // 品物旬のチェック
    void CheckSeason(string season)
    {
        int currentMonth = GameTime.Instance.GetCurrentMonth();
        if (season == "Spring" && currentMonth >= _march && currentMonth <= _may)
        {
            _seasonUI.enabled = true;
        }
        else if (season == "Summer" && currentMonth >= _june && currentMonth <= _august)
        {
            _seasonUI.enabled = true; ;
        }
        else if (season == "Autumn" && currentMonth >= _september && currentMonth <= _november)
        {
            _seasonUI.enabled = true;
        }
        else if (season == "Winter" && (currentMonth >= _december || currentMonth <= _february))
        {
            _seasonUI.enabled = true;
        }
        else
        {
            _seasonUI.enabled = false;
        }

    }

    //---------------------------Init---------------------------->

    // 仕入れ数を初期化する
    public void InitMarketItem()
    {
        data._buyCount = 0;
        _buyCountUI.SetText(data._buyCount.ToString());
    }

    //-------------------------goto shop---------------------------------->

    // 市場で仕入れたアイテムを店に送る
    public void GoToShop()
    {
        if (data._buyCount > 0)
        {
            data._shopCount += data._buyCount;
            ShopManager.Instance.AddNewItems(data);
        }
    }

    // 店にすでにあるアイテムを UI に反映する
    public void InitShop()
    {
        if (data._shopCount > 0)
        {
            ShopManager.Instance.AddShopItems(data);
        }
    }

}
