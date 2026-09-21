using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemUI : MonoBehaviour
{
    public static bool _isUIPulse = false;                          // UI が点滅アニメーション中かどうか

    [SerializeField] Image _iconUI;                                 // アイテムのアイコン
    [SerializeField] Image _seasonUI;                               // 季節アイコン（旬の時期に表示）
    [SerializeField] List<GameObject> _qualityUIStars = new();      // 品質を示す星アイコン
    [SerializeField] int _qualityStarCount;                         // 星の最大数
    [SerializeField] TextMeshProUGUI _countUI;                      // 店内在庫数

    [Header("Anime data")]
    [SerializeField] float _scaleAmount = 0.2f;                     // 点滅時の拡大率
    [SerializeField] float _scaleSpeed = 2f;                        // 点滅速度

    RectTransform _uiRect;
    Vector3 _originalScale;

    ItemsData _data;                                                // この UI が表示するアイテムデータ         
    int _count;                                                     // 店内在庫数

    [Header("Month")]
	int _february = 2;
	int _march = 3;
	int _may = 5;
	int _june = 6;
	int _august = 8;
	int _september = 9;
	int _november = 11;
	int _december = 12;

	//============================ Start ============================>
	private void Start()
    {
        _uiRect = GetComponent<RectTransform>();

        // アニメーション用に初期スケールを保存
        _originalScale = _uiRect.localScale;                    
    }

	//============================ Update ============================>
	private void Update()
    {
        // 点滅アニメーション処理
        UIPulse();
    }

    //============================ Function ============================>

    // アイテム情報を UI にセットする
    public void Setup(ItemsData item)
    {
        _data = item;
        _iconUI.sprite = item._icon;
        _count = item._shopCount;
        _countUI.SetText(_count.ToString());

        // 品質星の表示
        SetQualityUIStar(item._quantily);
        // 季節アイコンの表示
        CheckSeason(item._season);
    }

    //---------------------------------------------->

    // 品質星を設定する（品質値の星だけを点灯）
    void SetQualityUIStar(int i)
    {
        if (0 <= i && i <= _qualityStarCount)
        {
            foreach (GameObject star in _qualityUIStars)
            {
                star.SetActive(false);
            }
            _qualityUIStars[i].SetActive(true);
        }

    }

    // 季節アイコンの表示（旬の時期なら表示）
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

    //--------------------------------------------->

    //UIの動画
    void UIPulse()    
    {
        if (!_isUIPulse) return;
        float scale = 1 + Mathf.Sin(Time.time * _scaleSpeed) * _scaleAmount;
        _uiRect.localScale = _originalScale * scale;
    }

    //------------------------------------------->

    // 点滅開始
    public static void IsPulse()
    {
        _isUIPulse = true;
    }

    // 点滅停止
    public static void IsNoPulse()
    {
        _isUIPulse = false;
    }

    // 点滅中かどうか取得
    public static bool GetPulse()
    {
        return _isUIPulse;
    }
    //------------------------------------------->

    // このアイテム UI をクリックした時の処理
    public void OnClickThis()
    {
        // 点滅中でなければ選択できない
        if (!_isUIPulse) return;

        // ① すでにこの棚に置かれているアイテム  
        // ② まだ棚が空で、アイテムが未配置  
        // このどちらかなら棚に配置できる
        if (_data._salePosition == ShopBoxManager.Instance.GetCurrentBox().GetPosition() ||
               _data._salePosition == 0 && !ShopBoxManager.Instance.GetCurrentBox()._haveItem)
        {
            // 点滅停止
            IsNoPulse();

            // 配置 UI を表示
            ShopManager.Instance.ShowGoAndBackUI(_data);
        }
    }
}
