
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomerManager : MonoBehaviour
{
    public static CustomerManager Instance { get; private set; }

    [SerializeField] List<Transform> _spawnCustomerPos;     //　お客様を生成する位置

    [Header("Customer")]
    [SerializeField] List<CustomerData> _customers;         //　お客様のデータ

    [Space]
    [SerializeField] int _buyPercent;                       // 購入確率
    [SerializeField] int _delayDestroy;                     // 顧客を消すまでの遅延時間

    [Header("Random")]
    [SerializeField] int _minDelayBuy;                      // 購入までの最短遅延時間
    [SerializeField] int _maxDelayBuy;                      // 購入までの最大遅延時間
    [SerializeField] int _minBuyCount;                      // 最小購入数
    [SerializeField] int _maxBuyCount;                      // 最大購入数    
    [SerializeField] int _minIsBuy;                         // 購入判定の最小値
    [SerializeField] int _maxIsBuy;                         // 購入判定の最大値



    List<int> _elderlyPeopleTimings = new();                //　時間帯で高齢者のお客さんが来るタイミング
    List<int> _officeWorkersTimings = new();                //　時間帯でサラリーマンのお客さんが来るタイミング
    List<int> _houseWivesTimings = new();                   //　時間帯で主婦層のお客さんが来るタイミング
    List<int> _eliteClassTimings = new();                   //　時間帯でエリート層のお客さんが来るタイミング

    HashSet<int> passedMins1 = new();                       //　時間帯で生成した高齢者のお客さんのタイミング
    HashSet<int> passedMins2 = new();                       //　時間帯で生成したサラリーマンのお客さんのタイミング
    HashSet<int> passedMins3 = new();                       //　時間帯で生成した主婦層のお客さんのタイミング
    HashSet<int> passedMins4 = new();                       //　時間帯で生成したエリート層のお客さんのタイミング


    float _currentTimePeoples;                              //　時間帯に来るお客様の数
    int _oneMinutes = 1;
    int _fiftyMinutes = 50;

    int _elderlyPeopleCount;                                //　時間帯に来る高齢者の数
    int _officeWorkersCount;                                //　時間帯に来るサラリーマンの数
    int _houseWivesCount;                                   //　時間帯に来る主婦層の数
    int _eliteClassCount;                                   //　時間帯に来るエリート層の数

    AreasData _currentArea;                                 //　今の営業所
    WorldData _currentWorldData;                            //　今の日付と時間
    int _customerCountOfDay;                                //　今日店に来るお客様の数

    //============================ Awake ============================>
    private void Awake()
    {
        Instance = this;
    }

    //============================ Update ============================>
    private void Update()
    {
        //　店が営業中ならお客様が生成始まる
        if (ShopManager.Instance.GetShopStatus())
        {
            SpawnCustomerPrefabs(passedMins1, _elderlyPeopleTimings, _currentWorldData._minute, _customers[0], _spawnCustomerPos);
            SpawnCustomerPrefabs(passedMins2, _officeWorkersTimings, _currentWorldData._minute, _customers[1], _spawnCustomerPos);
            SpawnCustomerPrefabs(passedMins3, _houseWivesTimings, _currentWorldData._minute, _customers[2], _spawnCustomerPos);
            SpawnCustomerPrefabs(passedMins4, _eliteClassTimings, _currentWorldData._minute, _customers[3], _spawnCustomerPos);
        }
    }

    //============================ Function ============================>

    //　今の営業所をセットする
    public void SetCurrentArea(AreasData area)
    {
        _currentArea = area;
    }

    //　今の日付と時間をセットする
    public void SetCurrentworldData(WorldData data)
    {
        _currentWorldData = data;
    }

    //　今日店に来るお客様の数を計算する（セットする）
    public void SetDayCustomerCount()
    {
        _customerCountOfDay = (int)(_currentArea._populationCount *
            GetWeatherValue(_currentWorldData._weather) *
            GetWeekValue(_currentWorldData._week) *
            GetPaydayValue(GameTime.Instance.GetPayday()) *
            GetStaffValue("") *
            GetShopTypeValue(BrokerageManager.Instance.GetCurrentShop()._stallTypes) *
            GetShopReview(BrokerageManager.Instance.GetCurrentShop()._shopReview._eeeee));

        ShopManager.Instance.SetReportCustomers(_customerCountOfDay);
    }

    //----------------------------------------------------------------->

    //　時間帯でお客さんが来る数をゲット
    public void GetTimeCustomerCount(int hour)
    {
        //　2で割るのは2時間帯にお客様が来る数だから
        _currentTimePeoples = (_customerCountOfDay * GetTimeperiodValue(hour))/2;

        // 各層の割合で人数を計算
        _elderlyPeopleCount = Mathf.RoundToInt(_currentTimePeoples * _currentArea._populationComposition.elderlyPeople);
        _officeWorkersCount = Mathf.RoundToInt(_currentTimePeoples * _currentArea._populationComposition.officeWorkers);
        _houseWivesCount = Mathf.RoundToInt(_currentTimePeoples * _currentArea._populationComposition.houseWives);
        _eliteClassCount = Mathf.RoundToInt(_currentTimePeoples * _currentArea._populationComposition.eliteClass);

        // 前回のデータをリセット
        _elderlyPeopleTimings.Clear();　　
        _officeWorkersTimings.Clear();
        _houseWivesTimings.Clear();
        _eliteClassTimings.Clear();

        passedMins1.Clear();
        passedMins2.Clear();
        passedMins3.Clear();
        passedMins4.Clear();

        // ランダムな来店タイミングを設定
        SetCustomerRandomTiming(_elderlyPeopleCount, _elderlyPeopleTimings);
        SetCustomerRandomTiming(_officeWorkersCount, _officeWorkersTimings);
        SetCustomerRandomTiming(_houseWivesCount, _houseWivesTimings);
        SetCustomerRandomTiming(_eliteClassCount, _eliteClassTimings);

    }

    //　時間帯でお客さんが来るタイミングをランダムでセットする
    void SetCustomerRandomTiming(int Count, List<int> timings)
    {
        for (int i = 0; i < Count; i++)
        {
            int r = Random.Range(_oneMinutes, _fiftyMinutes);
            timings.Add(r);
        }
    }
    int num = 0;
    //　お客様を生成する
    void SpawnCustomerPrefabs(HashSet<int> mins, List<int> timing, int min, CustomerData customer, List<Transform> spawnPos)
    {

        foreach (int newMin in timing)
        {
            // 該当の分になったら生成（重複防止）
            if (newMin == min && !mins.Contains(min))
            {
                mins.Add(min);

                int r1 = Random.Range(0, customer._models.Count);
                int r2 = Random.Range(0, spawnPos.Count);

                GameObject obj = GameObject.Instantiate(customer._models[r1], spawnPos[r2].position, Quaternion.identity);
                obj.transform.SetParent(spawnPos[r2]);
                num++;

                //　遅延して商品を購入タイム
                int second = Random.Range(_minDelayBuy, _maxDelayBuy);    
                StartCoroutine(DelayBuyItem(customer, second, obj));
                return;
            }
        }
    }

    // お客様が遅延して商品を購入する
    IEnumerator DelayBuyItem(CustomerData customer, int second, GameObject obj)
    {
        yield return new WaitForSeconds(second);
        BuyItem(customer);
        StartCoroutine(DestroyOBJ(obj));
    }
    // 遅延してお客様を消す
    IEnumerator DestroyOBJ(GameObject obj)
    {
        yield return new WaitForSeconds(_delayDestroy);
        Destroy(obj);
    }


    //===============================================================>

    //　お客様が商品を購入する
    void BuyItem(CustomerData customer)
    {
        // 店が営業中でなければ購入処理を行わない
        if (!ShopManager.Instance.GetShopStatus())
            return;

        // 店で販売中の商品をすべてチェックする
        foreach (ItemsData item in ShopManager.Instance.GetSaleItems())
        {
            // この商品を購入するかどうか判定する
            if (IsBuyItem(customer, item, _currentWorldData.month, ShopBoxManager.Instance.GetBoxInPosition(item._salePosition)))
            {

                // ランダムで購入数を決める
                int buyCount = Random.Range(_minBuyCount, _maxBuyCount);

                // 在庫より多く買おうとした場合は購入しない
                if (buyCount > item._saleCount) return;

                // 在庫を減らす
                item._saleCount -= buyCount;

                // 商品が置かれている棚を取得
                ShopBox box = ShopBoxManager.Instance.GetBoxInPosition(item._salePosition);

                // 実際に商品を削除
                ShopManager.Instance.DestroyItem(buyCount, box);

                // 売上を追加
                ShopManager.Instance.SetShopMoney(item._salePrice * buyCount);

                // 倉庫容量を減らす
                MarketManager.Instance.SetWareHouseCapacity(-(item._size * buyCount));

                // 在庫がゼロになった場合の処理
                if (item._saleCount == 0)
                {
                    box._haveItem = false;
                    item._salePosition = 0;

                    // 店内の在庫もゼロならチェック処理
                    if (item._shopCount == 0)
                    {
                        ShopManager.Instance.ChackItemInShop(item);
                    }
                }

                // UI に売れたことを表示
                UIController.Instance.ShowAndSetTip("Sold " + buyCount +" "+item._name);

                // 店内と販売中の商品リストを更新
                ShopManager.Instance.GetItemsInShopAndSale();

            }
        }
    }

    //===============================================================>
    //　お客様が商品を購入するかの確認
    bool IsBuyItem(CustomerData customer, ItemsData item, int month, ShopBox box)
    {
        // 顧客の性質 × 商品の状態 を組み合わせて購入値を計算する
        float buyValue = (GetShelfLifeVelue(customer._inShelfLife, item._shelfLife) +
                        GetQualityVelue(customer._inQuality, item._quantily) +
                        GetSeasonVelue(customer._inSeason, item._season, month) +
                        GetSalePriceValue(customer._inSalePrice, item._marketPrice,item._salePrice) +
                        GetSalePositionValue(customer._inSalePosition, item._salePosition) +
                        GetSaleCountValue(customer._inSaleCount, item._saleCount, box.GetBoxSize())) / _buyPercent;

        //　商品販売金額が高すぎる時お客様が買わない
        if (GetSalePriceValue(customer._inSalePrice, item._marketPrice, item._salePrice) == 0)     
        {
            buyValue = 0;
        }

        Debug.Log($"BuyItem: {buyValue}");

        // ランダム値と比較して購入するか決定
        return Random.Range(_minIsBuy, _maxIsBuy) < buyValue;
    }

    //------------------------------------------------------------->
    //　天気をゲット
    float GetWeatherValue(int num)
    {
        switch (num)
        {
            case 0: return _currentArea._weather.sunny;
            case 1: return _currentArea._weather.rain;
            case 2: return _currentArea._weather.typhoon;
            case 3: return _currentArea._weather.snow;
            case 4: return _currentArea._weather.highTemperature;
            case 5: return _currentArea._weather.windy;
            default:
                Debug.LogError("WeatherValue Is Error!");
                return 0;
        }
    }
    //　週をゲット
    float GetWeekValue(int num)
    {
        switch (num)
        {
            case 0: return _currentArea._week.monday;
            case 1: return _currentArea._week.tuesday;
            case 2: return _currentArea._week.wednesday;
            case 3: return _currentArea._week.thursday;
            case 4: return _currentArea._week.friday;
            case 5: return _currentArea._week.saturday;
            case 6: return _currentArea._week.sunday;
            default:
                Debug.LogError("WeekValue Is Error!");
                return 0;
        }
    }
    //　給料日の確認
    float GetPaydayValue(bool isPayDay)
    {
        return isPayDay ? _currentArea._payDay : 1;

    }

    //　TODO（スタッフ）
    float GetStaffValue(string num)
    {
        switch (num)
        {
            case "aaa": return 0.9f;
            case "bbb": return 0.7f;
            case "ccc": return 0.5f;
            default:
                return 1;
        }
    }
    //　店の品出し数
    float GetShopTypeValue(int type)
    {
        switch (type)
        {
            case 4: return 0.1f;
            case 6: return 0.2f;
            case 8: return 0.3f;
            default:
                Debug.LogError("ShopValue Is Error!");
                return 0;
        }
    }
    //　店の評価
    float GetShopReview(int review)
    {
        switch (review)
        {
            case 1: return 0.1f;
            case 2: return 0.2f;
            case 3: return 0.3f;
            case 4: return 0.4f;
            case 5: return 0.5f;
            default:
                Debug.LogError("ShopReview Is Error!");
                return 0;
        }
    }

    //------------------------------------------------------------->
    //　時間帯をゲット
    float GetTimeperiodValue(int num)
    {
        int i = 0;
        if (num < 12)
        {
            i = 1;
        }
        else if (num >= 12 && num < 14)
        {
            i = 2;
        }
        else if (num >= 14 && num < 16)
        {
            i = 3;
        }
        else if (num >= 16 && num < 18)
        {
            i = 4;
        }
        else if (num >= 18)
        {
            i = 5;
        }

        switch (i)
        {
            case 1: return _currentArea._timePeriod.from10to12;
            case 2: return _currentArea._timePeriod.from12to14;
            case 3: return _currentArea._timePeriod.from14to16;
            case 4: return _currentArea._timePeriod.from16to18;
            case 5: return _currentArea._timePeriod.from18to20;
            default:
                Debug.LogError("TimeperiodValue Is Error");
                return 0;
        }
    }

    //===============================================================>

    //　お客様の商品鮮度による購入指数をゲット
    float GetShelfLifeVelue(float inShelfLife, int itemShelfLife)
    {      
        switch (itemShelfLife)
        {
            case 4: return 0.9f * inShelfLife;
            case 3: return 0.7f * inShelfLife;
            case 2: return 0.5f * inShelfLife;
            case 1: return 0.3f * inShelfLife;
            case 0: return 0.2f * inShelfLife;
            default:
                Debug.LogError("ShelfLifeVelue Is Error");
                return 0f;
        }
    }

    //　お客様の商品品質による購入指数をゲット
    float GetQualityVelue(float inQuality, int itemQuality)
    {
        
        switch (itemQuality)
        {

            case 5: return 0.9f * inQuality;
            case 4: return 0.8f * inQuality;
            case 3: return 0.7f * inQuality;
            case 2: return 0.6f * inQuality;
            case 1: return 0.5f * inQuality;
            case 0: return 0.1f * inQuality;
            default:
                Debug.LogError("QualityVelue Is Error");
                return 0f;
        }
    }

    //　お客様の商品旬（季節）による購入指数をゲット
    float GetSeasonVelue(float inSeason, string itemSeason, int month)
    {
        int itemMonth = 0;
        switch (itemSeason)
        {
            case "Spring":
                itemMonth = 1;
                break;
            case "Summer":
                itemMonth = 2;
                break;
            case "Autumn":
                itemMonth = 3;
                break;
            case "Winter":
                itemMonth = 4;
                break;
            default: break;
        }
        if (itemMonth == 1 && month >= 3 && month <= 5)
        {
            return 0.8f * inSeason;
        }
        else if (itemMonth == 2 && month >= 6 && month <= 8)
        {
            return 0.8f * inSeason;
        }
        else if (itemMonth == 3 && month >= 9 && month <= 11)
        {
            return 0.8f * inSeason;
        }
        else if (itemMonth == 4 && month >= 12 && month <= 2)
        {
            return 0.8f * inSeason;
        }
        else
        {
            return 0f;
        }
    }

    //　お客様の商品販売金額による購入指数をゲット
    float GetSalePriceValue(float inSalePrice, int itemMarketPrice,int itemSalePrice)
    {
        if (itemSalePrice < itemMarketPrice * 0.4)
        {
            return 1f;
        }
        else if (itemSalePrice >= itemMarketPrice * 0.4 && itemSalePrice < itemMarketPrice * 0.65)
        {
            return 0.9f * inSalePrice;
        }
        else if (itemSalePrice >= itemMarketPrice * 0.65 && itemSalePrice < itemMarketPrice * 0.8)
        {
            return 0.8f * inSalePrice;
        }
        else if (itemSalePrice >= itemMarketPrice * 0.8 && itemSalePrice < itemMarketPrice * 0.9)
        {
            return 0.7f * inSalePrice;
        }
        else if (itemSalePrice >= itemMarketPrice * 0.9 && itemSalePrice < itemMarketPrice * 0.95)
        {
            return 0.6f * inSalePrice;
        }
        else if (itemSalePrice >= itemMarketPrice * 0.95 && itemSalePrice < itemMarketPrice * 1.05)
        {
            return 0.5f * inSalePrice;
        }
        else if (itemSalePrice >= itemMarketPrice * 1.05 && itemSalePrice < itemMarketPrice * 1.15)
        {
            return 0.4f * inSalePrice;
        }
        else if (itemSalePrice >= itemMarketPrice * 1.15 && itemSalePrice < itemMarketPrice * 1.3)
        {
            return 0.3f * inSalePrice;
        }
        else if (itemSalePrice >= itemMarketPrice * 1.3 && itemSalePrice < itemMarketPrice * 1.5)
        {
            return 0.2f * inSalePrice;
        }
        else if (itemSalePrice >= itemMarketPrice * 1.5 && itemSalePrice < itemMarketPrice * 1.75)
        {
            return 0.1f * inSalePrice;
        }
        else if (itemSalePrice >= itemMarketPrice * 1.75)
        {
            return 0.0f * inSalePrice;
        }
        else
        {
            Debug.LogError("SalePriceValue Is Error");
            return 0f;
        }
    }

    //　お客様の商品並ぶ位置による購入指数をゲット
    float GetSalePositionValue(float inSalePosition, int itemSalePosition)
    {
        switch (itemSalePosition%2)
        {
            case 0: return 0.8f * inSalePosition;
            case 1: return 0.4f * inSalePosition;
            default:
                Debug.LogError("SalePosition Is Error");
                return 0f;
        }
    }
    //　お客様の商品並ぶ数による購入指数をゲット
    float GetSaleCountValue(float inSaleCount, int itemSaleCount, int boxSize)
    {
        if (itemSaleCount < boxSize / 4)    // ４分の１もない
        {
            return 0.5f * inSaleCount;
        }
        else if (itemSaleCount >= boxSize / 4 && itemSaleCount < boxSize / 2)   // 半分もない
        {
            return 0.7f * inSaleCount;
        }
        else if (itemSaleCount >= boxSize / 2)   // 半分以上
        {
            return 0.9f * inSaleCount;
        }
        else
        {
            Debug.LogError("SaleCount Is Error");
            return 0f;
        }
    }


    //=============================================================>
}
