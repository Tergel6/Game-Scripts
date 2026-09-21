using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

//============================ Class ============================>
[Serializable]
public class ShopData
{
    public int shopMoney;               // 店の売上金
    public string _jobID;               // スタッフの職種ID
}
class ReportData
{
    public string _reportDay;           // 日報：日付
    public string _reportWeather;       // 日報：天気
    public string _reportArea;          // 日報：営業エリア
    public int _reportCustomers;        // 日報：来客数
    public int _reportSales;            // 日報：売上
    public int _reportRepayLoan;        // 日報：返済額
    public int _reportPayRent;          // 日報：家賃
    public int _reportPaySalary;        // 日報：給料
}

//============================ MonoBehaviour ============================>
public class ShopManager : MonoBehaviour
{
    public static ShopManager Instance { get; private set; }

    // 店のメイン UI
    [SerializeField] GameObject _shopUI;
    [SerializeField] GameObject _itemPrefab;
    [SerializeField] Transform _contentParent;

    [Space]
    [SerializeField] TextMeshProUGUI _salesUI;              // 売上表示
    [SerializeField] GameObject _inventoryUI;               // 在庫 UI
    [SerializeField] GameObject _staffUI;                   // スタッフ UI

    [Space]
    [SerializeField] GameObject _goAndBackUI;               // 商品を棚へ置く／戻す UI
    [SerializeField] GameObject _gotoCarUI;                 // 車へ持っていく UI
    [SerializeField] GameObject _backUI;                    // 戻す UI

    //---------------------InputField----------------------->

    [Space]
    [Header("GoUI")]
    [SerializeField] Image _goCarIcon;                      // アイテムアイコン
    [SerializeField] Image _goCarSeason;                    // 季節アイコン
    [SerializeField] TextMeshProUGUI _goCarName;            // アイテム名
    [SerializeField] TextMeshProUGUI _costPrice;            // 仕入れ価格
    [SerializeField] TextMeshProUGUI _sellingQuantity;      // 店内在庫数

    [SerializeField] TMP_InputField _priceInputField;       // 販売価格入力
    [SerializeField] TMP_InputField _quantityInputField;    // 販売数量入力

    [SerializeField] GameObject _inputNumberBG;             // 数字入力 UI
    [SerializeField] GameObject _inputSliderBG;             // スライダー入力 UI
    [SerializeField] Slider _inputSlider;                   // 数量スライダー
    [SerializeField] int _maxInputLength;                   // 入力最大桁数

    [Space]
    [Header("BackUI")]
    [SerializeField] Image _backIMG;                        // 日報 UI
    [SerializeField] TextMeshProUGUI _backName;
    [SerializeField] TextMeshProUGUI _backInSelling;
    [SerializeField] TMP_InputField _backQuantityField;
    [SerializeField] GameObject _backSliderBG;
    [SerializeField] Slider _backSlider;

    [Space]
    [Header("ReportUI")]
    [SerializeField] GameObject _reportBG;
    [SerializeField] TextMeshProUGUI _reportDayUI;
    [SerializeField] TextMeshProUGUI _reportWeatherUI;
    [SerializeField] TextMeshProUGUI _reportAreaUI;
    [SerializeField] TextMeshProUGUI _reportCustomersUI;
    [SerializeField] TextMeshProUGUI _reportSalesUI;
    [SerializeField] TextMeshProUGUI _reportRepayLoanUI;
    [SerializeField] TextMeshProUGUI _reportPayRentUI;
    [SerializeField] TextMeshProUGUI _reportPaySalaryUI;
    [SerializeField] Button _reportContinue;
    [Space]
    //-------------------------------------------->
    [SerializeField] GameObject _shopParent;                // 店の3Dモデルの親
    [Space]
    //-------------------------------------------->
    [SerializeField] GameObject _openShopUI;                // 店を開く UI
    [Header("SpawnItemsPos")]
    [SerializeField] float _minXPos;
	[SerializeField] float _MaxXPos;
	[SerializeField] float _minYPos;
	[SerializeField] float _MaxYPos;
	[SerializeField] float _minZPos;
	[SerializeField] float _MaxZPos;
    [Space]
    [SerializeField] float _delaySpawnItems;                // アイテム生成の遅延

    [Space]
    List<ItemsData> _newItems = new List<ItemsData>();      // 市場から仕入れ
    List<ItemsData> _shopItems = new List<ItemsData>();     // 店にある品物

    ShopData _shopData = new ShopData();                    // 店の売上データ
    ReportData _reportData = new ReportData();              // 日報データ

    ItemsData _currentItem;                                 // 現在選択中のアイテム

    bool _isShopOpen = false;                               // 店が営業中かどうか
    int _count;                                             // 汎用カウンタ
    Animator _shopUIAnime;                                  // 店 UI のアニメーション       
    string _toDay;                                          // 今日の日付
    bool _onGoAndBack = false;                              // Go/Back UI が開いているか

    [Header("Month")]
	int _february = 2;
	int _march = 3;
	int _may = 5;
	int _june = 6;
	int _august = 8;
	int _september = 9;
	int _november = 11;
	int _december = 12;
	//============================ Awake ============================>
	private void Awake()
    {
        Instance = this;
    }

	//============================ Start ============================>
	private void Start()
    {
        _gotoCarUI.SetActive(false);
        _shopUIAnime=_shopUI.GetComponent<Animator>();
    }

    //============================ Function ============================>

    // 売上 UI を更新する
    void SetSalesUI()
    {
        _salesUI.SetText(_shopData.shopMoney.ToString());
    }

    // 市場から仕入れたアイテムを追加
    public void AddNewItems(ItemsData item)
    {
        _newItems.Add(item);
    }

    // 店にすでにあるアイテムを追加
    public void AddShopItems(ItemsData item)
    {
        _shopItems.Add(item);
    }

    // 店のアイテム一覧 UI を表示する
    public void ShowItemUI()
    {
        ClearContentParent();

        // 重複チェック
        CheckItems();
        if (_shopItems.Count > 0)
        {
            foreach (ItemsData item in _shopItems)
            {
                var obj = Instantiate(_itemPrefab, _contentParent);
                obj.GetComponent<ShopItemUI>().Setup(item);
            }
        }
    }
    // item重複のチェック
    void CheckItems()
    {
        if (_newItems.Count > 0)
        {
            List<ItemsData> items = new();

            foreach (ItemsData newItem in _newItems)
            {
                bool isHave = false;
                if (_shopItems.Count > 0)
                {
                    foreach (ItemsData shopItem in _shopItems)
                    {
                        if (newItem._id == shopItem._id)
                        {
                            isHave = true;
                        }
                    }
                }

                if (!isHave)
                {
                    items.Add(newItem);
                }
            }

            foreach (ItemsData item in items)
            {
                _shopItems.Add(item);
            }
            _newItems.Clear();
        }
    }

    // アイテム一覧 UI をクリアする
    void ClearContentParent()
    {
        if (_contentParent.childCount > 0)
        {
            for (int i = _contentParent.childCount - 1; i >= 0; i--)
            {
                Destroy(_contentParent.GetChild(i).gameObject);
            }
        }
    }
    // 店のUIを表示する
    public void ShowShopUI()
    {
        if (!_shopUI.activeSelf)
        {
            _shopUI.SetActive(true);
        }
        ShowShopUIAnime();
    }
    //-----------------------Button----------------------->

    // 在庫 UI を表示する
    public void ShowInventory()
    {
        if (!_inventoryUI.activeSelf)
        {
            _inventoryUI.SetActive(true);
        }
        ShowItemUI();
    }
    //------------------------------------------------------------------>

    // 品物を棚に置く／戻すための UI を表示する
    public void ShowGoAndBackUI(ItemsData data)
    {
        if (!_goAndBackUI.activeSelf)
        {
            _goAndBackUI.SetActive(true);

            // 現在操作するアイテムを記録
            _currentItem = data;

        }
    }

    // Go & Back UI を閉じる
    public void HideGoAndBackUI()
    {
        if (_goAndBackUI.activeSelf)
        {
            _goAndBackUI.SetActive(false);
        }
    }

    // 品物を棚に置く UI を表示する
    public void ShowGotoCarUI()
    {
        if (!_gotoCarUI.activeSelf)
        {
            // Go/Back UI を閉じる
            HideGoAndBackUI();
            _inputNumberBG.SetActive(false);
            _gotoCarUI.SetActive(true);

            // UI にアイテム情報をセット
            SetGoCarUIInfo();

            // 棚の選択を無効化
            ShopBoxManager.Instance.DisableBoxesSelect();
        }
    }

    // 棚へ置く UI を閉じる
    public void HideGotoCarUI()
    {
        if (_gotoCarUI.activeSelf)
        {
            _gotoCarUI.SetActive(false);
            _priceInputField.text = "";
            _quantityInputField.text = "";

            // アイテム UI を再び点滅状態にする
            ShopItemUI.IsPulse();

            // 棚の選択を再び有効化
            ShopBoxManager.Instance.EnabledBoxesSelect();
        }
    }

    // 品物を棚から戻す UI を表示する
    public void ShowBackUI()
    {
        // 現在選択中の棚に置かれているアイテムだけ戻せる
        if (!_backUI.activeSelf && _currentItem._salePosition == ShopBoxManager.Instance.GetCurrentBox().GetPosition())
        {
            HideGoAndBackUI();
            _backSliderBG.SetActive(false);

            // UI に戻すアイテム情報をセット
            SetBackCarUIInfo();
            _backUI.SetActive(true);

            ShopBoxManager.Instance.DisableBoxesSelect();
        }
    }

    // 戻す UI を閉じる
    public void HideBackUI()
    {
        if (_backUI.activeSelf)
        {
            _backUI.SetActive(false);
            _backQuantityField.text = "";
            ShopItemUI.IsPulse();

            ShopBoxManager.Instance.EnabledBoxesSelect();
        }
    }

    //品出しの内容をセットする
    void SetGoCarUIInfo()
    {
        _goCarName.SetText(_currentItem._name);
        _goCarIcon.sprite = _currentItem._icon;
        _costPrice.SetText(_currentItem._costPrice.ToString() + "$");
        _sellingQuantity.SetText(_currentItem._saleCount.ToString());
        CheckSeason(_currentItem._season);

        // すでに販売価格が設定されている場合は入力欄に表示
        if (_currentItem._salePrice != 0)
        {
            _priceInputField.text = _currentItem._salePrice.ToString();
        }
    }

    //品物を戻す内容をセットする
    void SetBackCarUIInfo()
    {
        _backName.SetText(_currentItem._name);
        _backIMG.sprite = _currentItem._icon;
        _backInSelling.SetText(_currentItem._saleCount.ToString());
    }

    //品物旬のチェック
    void CheckSeason(string season)
    {
        int currentMonth = GameTime.Instance.GetCurrentMonth();
        if (season == "Spring" && currentMonth >= _march && currentMonth <= _may)
        {
            _goCarSeason.enabled = true;
        }
        else if (season == "Summer" && currentMonth >= _june && currentMonth <= _august)
        {
            _goCarSeason.enabled = true; ;
        }
        else if (season == "Autumn" && currentMonth >= _september && currentMonth <= _november)
        {
            _goCarSeason.enabled = true;
        }
        else if (season == "Winter" && (currentMonth >= _december || currentMonth <= _february))
        {
            _goCarSeason.enabled = true;
        }
        else
        {
            _goCarSeason.enabled = false;
        }

    }

    //-----------------------------------------Report------------------------------------------>
    //日次売上報告UIを表示する
    public void ShowReportBG()
    {
        if (!_reportBG.activeSelf)
        {
            // 日報内容をセット
            SetReportUI();
            ShopBoxManager.Instance.DisableBoxesSelect();
            ShopBoxManager.Instance.ResetMaterials();
            _reportBG.SetActive(true);
        }
    }
    //日次売上報告UIを非表示する
    public void HideReportBG()
    {
        if (_reportBG.activeSelf)
        {
            _reportBG.SetActive(false);
        }
    }
    //日次売上報告UIの内容をセットする
    public void SetReportUI()
    {
        _reportDayUI.SetText(_reportData._reportDay);
        _reportWeatherUI.SetText(_reportData._reportWeather);
        _reportAreaUI.SetText(_reportData._reportArea);
        _reportCustomersUI.SetText(_reportData._reportCustomers.ToString());
        _reportSalesUI.SetText(_reportData._reportSales.ToString() + "$");
        _reportRepayLoanUI.SetText(_reportData._reportRepayLoan.ToString() + "$");
        _reportPayRentUI.SetText(_reportData._reportPayRent.ToString() + "$");
        _reportPaySalaryUI.SetText(_reportData._reportPaySalary.ToString() + "$");
    }


    // 日報データのセット
    public void SetReportDay(string day)
    {
        _reportData._reportDay = day;
    }
    public void SetReportWeather(string _weather)
    {
        _reportData._reportWeather = _weather;
    }
    public void SetReportArea(string area)
    {
        _reportData._reportArea = area;
    }
    public void SetReportCustomers(int customers)
    {
        _reportData._reportCustomers = customers;
    }
    public void SetReportSales(int sales)
    {
        _reportData._reportSales = sales;
    }
    public void SetReportRepayLoan(int repayLoan)
    {
        _reportData._reportRepayLoan = repayLoan;
    }
    public void SetReportPayRent(int payRent)
    {
        _reportData._reportPayRent = payRent;
    }
    public void SetReportPaySalary(int paySalary)
    {
        _reportData._reportPayRent = paySalary;
    }

    //次の日を続ける
    public void ContineNextDay()
    {
        // 売上をプレイヤーのお金に反映
        ShopToMyMoney();

        // 日報 UI を閉じる
        HideReportBG();

        // ローン返済処理
        BankData.Instance.RepayLoan(_toDay);

        // プレイヤーのお金が1000もないと店に品物がない時ゲームが終了する
        if (PlayerData.Instance.GetPlayerMoney() < 1000&&_shopItems.Count==0)       
        {
            UIController.Instance.ShowGameOverUI();
        }

        // UI とシステムを再び操作可能にする
        AreaManager.Instance.InteractableAreasBT();
        BrokerageManager.Instance.InteractableBrokerageBT();
        MarketManager.Instance.OpenTheMarket();

        ShopBoxManager.Instance.EnabledBoxesSelect();
    }

    //--------------------------------SallingPrice ===> InputField-------------------------------------->

    //輸入ボードを表す
    public void ShowInputNumber()
    {
        if (!_inputNumberBG.activeSelf)
        {
            _inputNumberBG.SetActive(true);
            HideInputSlider();
        }
    }

    //輸入ボードを隠す
    public void HideInputNumber()
    {
        if (_inputNumberBG.activeSelf)
        {
            _inputNumberBG.SetActive(false);
        }
    }

    // 数字按钮调用
    public void InputNumber(string num)
    {
        if (_priceInputField.text.Length >= _maxInputLength) return;

        _priceInputField.text += num;
    }

    // 删除按钮调用
    public void Delete()
    {
        if (_priceInputField.text.Length == 0) return;

        _priceInputField.text = _priceInputField.text.Substring(0, _priceInputField.text.Length - 1);
    }

    //----------------------------- Quantity ===> InputSlider---------------------------->

    // 数量スライダーを表示する
    public void ShowInputSlider()
    {
        if (!_inputSliderBG.activeSelf)
        {
            _inputSliderBG.SetActive(true);

            // スライダーの最大値を計算
            ResetInputSlider();

            // 数字入力 UI を隠す
            HideInputNumber();
        }
    }

    // 数量スライダーを隠す
    public void HideInputSlider()
    {
        if (_inputSliderBG.activeSelf)
        {
            _inputSliderBG.SetActive(false);
        }
    }

    void ResetInputSlider()
    {
        // 現在棚に置かれているアイテムの総サイズ
        int inBoxSize = _currentItem._saleCount * _currentItem._size;

        // 棚の空き容量
        int freeBoxSize = ShopBoxManager.Instance.GetCurrentBox().GetBoxSize() - inBoxSize;

        // 置けるアイテム数（サイズで割る）
        int canGoItemCount = freeBoxSize / _currentItem._size;

        // 店内在庫より多く置ける場合は在庫数が上限
        if (canGoItemCount <= _currentItem._shopCount)
        {
            _inputSlider.maxValue = canGoItemCount;
        }
        else if (canGoItemCount > _currentItem._shopCount)
        {
            _inputSlider.maxValue = _currentItem._shopCount;
        }

    }
    public void OnChangeSliderValue()
    {
        if (_inputSliderBG.activeSelf)
        {
            _quantityInputField.text = _inputSlider.value.ToString();
        }
    }
    // Submit Go Car
    public void GoCarSubmit()
    {
        if (_priceInputField.text == "")
            return;
        if (_quantityInputField.text == "")
        {
            _quantityInputField.text = 0.ToString();
        }

        // 点滅停止
        ShopItemUI.IsNoPulse();

        // 販売データを更新
        SetGoCarSaleData();

        HideInputNumber();
        HideInputSlider();
        HideGotoCarUI();

        // 3Dアイテム生成
        SpawnItemPrefab(_count, ShopBoxManager.Instance.GetCurrentBox());

        // UI 更新
        ShowItemUI();
    }

    // 棚に置いた後のデータ更新
    void SetGoCarSaleData()
    {
        _count = int.Parse(_quantityInputField.text);
        _currentItem._salePrice = int.Parse(_priceInputField.text);

        // 販売中の数量を増やす
        _currentItem._saleCount += _count;

        // 店内在庫を減らす
        _currentItem._shopCount -= _count;
        _currentItem._salePosition = ShopBoxManager.Instance.GetCurrentBox().GetPosition();
        if (_currentItem._saleCount > 0)
        {
            ShopBoxManager.Instance.GetCurrentBox()._haveItem = true;
        }
        _quantityInputField.text = "";
        _inputSlider.value = 0;
    }
    //spawn item prefab
    void SpawnItemPrefab(int count, ShopBox box)
    {
        StartCoroutine(SpawnWithDelay(count, box));
    }

    IEnumerator SpawnWithDelay(int count, ShopBox box)
    {
        Vector3 pos = box.transform.position;
        for (int i = 0; i < count; i++)
        {
            float _x = UnityEngine.Random.Range(_minXPos,_MaxXPos);
            float _y = UnityEngine.Random.Range(_minYPos, _MaxYPos);        //生成する高さ
            float _z = UnityEngine.Random.Range(_minZPos, _MaxZPos);
            GameObject obj = GameObject.Instantiate(_currentItem._prefab,
                                                    new Vector3(pos.x + _x, pos.y + _y, pos.z + _z),
                                                    UnityEngine.Random.rotation);
            obj.transform.SetParent(box.GetBoxParent().transform);
            yield return new WaitForSeconds(_delaySpawnItems);
        }

    }
    //------------------------------------------------------------>

    // 店内アイテムの取得
    public List<ItemsData> GetShopItems()
    {
        return _shopItems;
    }

    // 販売している商品をゲット
    public List<ItemsData> GetSaleItems()
    {
        List<ItemsData> saleItems = new List<ItemsData>();
        if (_shopItems != null)
            for (int i = 0; i < _shopItems.Count; i++)
            {
                if (_shopItems[i]._saleCount > 0)
                {
                    saleItems.Add(_shopItems[i]);
                }
            }
        return saleItems;
    }

    // 店にある品物の確認,店にない、販売もしてない品物をショップから削除する
    public void ChackItemInShop(ItemsData item)
    {
        if (item._shopCount == 0 && item._saleCount == 0)
        {
            _shopItems.Remove(item);
        }
        ShowItemUI();
    }

    // 販売している品物を全部店に戻す
    public void BackToShopAllItems()
    {
        foreach (ItemsData item in _shopItems)
        {
            if (item._saleCount > 0)
            {
                item._shopCount += item._saleCount;
                item._saleCount = 0;
                item._salePosition = 0;
            }
        }
        ShowItemUI();
    }

    // 店に品物があるかと商売している商品があるかの確認
    public void GetItemsInShopAndSale()
    {
        if (_shopItems.Count == 0)
        {
            UIController.Instance.ShowSoldOutUI();
        }
    }



    //----------------------------- Quantity ===> BackSlider---------------------------->

    // 棚から戻すスライダー
    public void ShowBackSlider()
    {
        if (!_backSliderBG.activeSelf)
        {
            _backSliderBG.SetActive(true);
            ResetbackSlider();
        }
    }
    public void HideBackSlider()
    {
        if (_backSliderBG.activeSelf)
        {
            _backSliderBG.SetActive(false);
        }
    }

    void ResetbackSlider()
    {
        _backSlider.maxValue = _currentItem._saleCount;
    }
    public void OnChangeBackSliderValue()
    {
        if (_backSliderBG.activeSelf)
        {
            _backQuantityField.text = _backSlider.value.ToString();
        }
    }

    // 棚から戻す処理
    public void BackSubmit()
    {
        if (_backQuantityField.text == "") return;
        SetBackSaleData();
        HideBackSlider();
        ShowItemUI();
    }
    void SetBackSaleData()
    {
        int backCount = int.Parse(_backQuantityField.text);
        _currentItem._saleCount -= backCount;
        _currentItem._shopCount += backCount;
        DestroyItem(backCount, ShopBoxManager.Instance.GetCurrentBox());
        if (_currentItem._saleCount == 0)
        {
            ShopBoxManager.Instance.GetCurrentBox()._haveItem = false;
            _currentItem._salePosition = 0;
        }
        _backQuantityField.text = "";
        _backSlider.value = 0;
    }

    //品物を削除する
    public void DestroyItem(int count, ShopBox box)
    {
        if (count > 0)
        {
            for (int i = 0; i < count; i++)
            {
                Destroy(box.GetBoxParent().transform.GetChild(i + 1).gameObject);
            }
        }

    }
    //---------------------------------------------------------->

    //店のプレハブを生成する
    public void SpawnShopPrefab(ShopsData shop)
    {
        if (_shopParent.transform.childCount > 0)
        {
            BackToShopAllItems();
            ShopBoxManager.Instance.ClearItemsInBox();
            Destroy(_shopParent.transform.GetChild(0).gameObject);
        }

        GameObject obj = Instantiate(shop._shopPrefab, _shopParent.transform.position, Quaternion.identity);
        obj.transform.SetParent(_shopParent.transform);

    }


    //----------------------------OpenTheShop----------------------------->

    //開店
    public void OpenTheShop()
    {
        if (_shopItems.Count == 0)
        {
            UIController.Instance.ShowAndSetTip("The store has no items");
            return;
        }
        if (!_isShopOpen && AreaManager.Instance.GetCurrentArea() != null)
        {
            GameTime.Instance.SetCurrentTime(1);
            _isShopOpen = true;
            AreaManager.Instance.UnInteractableAreasBT();
            BrokerageManager.Instance.UnInteractableBrokerageBT();
            MarketManager.Instance.CloseTheMarket();

            _openShopUI.SetActive(false);

            SetReportDay(GameTime.Instance.GetToday());
            SetReportWeather(GameTime.Instance.GetWeather());
            SetReportArea(AreaManager.Instance.GetCurrentArea()._areaType);

            //今日の日付を保存する
            _toDay = GameTime.Instance.GetTime();  
            SetReportRepayLoan(BankData.Instance.ChackRepayLoan(_toDay));

            //家賃の支払う
            SetReportPayRent(0);   
        }
        else
        {
            UIController.Instance.ShowAndSetTip("you must select area");
        }
    }
    //店を閉める
    public void CloseTheShop()
    {
        if (_isShopOpen)
        {
            _isShopOpen = false;
        }
        if (!_openShopUI.activeSelf)
        {
            _openShopUI.SetActive(true);
        }
        SetReportSales(_shopData.shopMoney);

    }

    //店の状態をゲットする
    public bool GetShopStatus()
    {
        return _isShopOpen;
    }


    //========================================>

    //店の売り上げをセットする
    public void SetShopMoney(int money)
    {
        _shopData.shopMoney += money;
        _salesUI.SetText(_shopData.shopMoney.ToString());
    }
    //店の売り上げをプレイヤーのお金へ移動する
    public void ShopToMyMoney()
    {
        if (_shopData.shopMoney == 0) return;
        PlayerData.Instance.AddMoney(_shopData.shopMoney);
        _shopData.shopMoney = 0;
        _salesUI.SetText(_shopData.shopMoney.ToString());
    }
    //------------------------------------------->

    //マウスがGoAndBackボタンにある
    public void OnGoAndBackButton()
    {
        if (!_onGoAndBack)
            _onGoAndBack = true;
    }
    //マウスがGoAndBackボタンにない
    public void NoGoAndBackButton()
    {
        if (_onGoAndBack)
            _onGoAndBack = false;
    }
    public bool GetGoAndBackButton()
    {
        return _onGoAndBack;
    }
    //---------------------------------------------->

    //店のUIを表示するアニメ
    public void ShowShopUIAnime()
    {
        _shopUIAnime.SetBool("IsShow", true);
    }
    //店のUIを隠すアニメ
    public void HidShopUIAnime()
    {
        _shopUIAnime.SetBool("IsShow",false);
    }


    //================================ SaveAndLoadShopData ===================================>

    //店のデータを保存する
    public void SaveShopData()
    {
        GameDataManager.gameData.shopData = _shopData;
        Debug.Log(_shopData.shopMoney);
    }

    //店のデータを読み込む
    public void LoadShopData()
    {
        _shopData = GameDataManager.gameData.shopData;
        Debug.Log("load:"+ _shopData.shopMoney);
        SetSalesUI();
        ShowItemUI();
    }

}
