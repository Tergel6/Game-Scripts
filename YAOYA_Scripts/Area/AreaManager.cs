using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AreaManager : MonoBehaviour
{
    public static AreaManager Instance { get; private set; }
    [Space]
    [SerializeField] AreasData[] areas;                                 // エリアデータ一覧

    [Space]
    [SerializeField] GameObject _areaUI;                                // エリア確認 UI
    [SerializeField] TextMeshProUGUI _areaName;                         // エリア名表示
    [SerializeField] TextMeshProUGUI _areaPopulation;                   // 人口表示

    [Space]
    [SerializeField] List<Button> _areasBT = new List<Button>();        // エリア選択ボタン

    AreasData _currentArea;                                             // 現在選択中のエリア    

    //============================ Awake ============================>
    private void Awake()
    {
        Instance = this;
    }
    //============================ Start ============================>
    private void Start()
    {
        // 初期状態では UI を非表示にする
        _areaUI.SetActive(false);

        // エリア選択ボタンを押せない状態にする
        UnInteractableAreasBT();
    }
    //============================ Function ============================>

    //住宅区を選択する
    public void SelectResidentialArea()
    {
        _areaUI.SetActive(true);
        _currentArea = areas[0];
        SetConfirmAreaUI(_currentArea);
    }
    //学校周辺を選択する
    public void SelectSchoolArea()
    {
        _areaUI.SetActive(true);
        _currentArea = areas[1];
        SetConfirmAreaUI(_currentArea);
    }

    // エリア確認 UI を隠す
    void HideConfirmAreaUI()
    {
        _areaUI.SetActive(false);
    }

    //店を開ける場所を確認する
    public void ConfirmArea()
    {
        // 店をまだ購入していない場合
        if (BrokerageManager.Instance.GetCurrentShop() == null)
        {
            UIController.Instance.ShowAndSetTip("you must buy the shop");
            return;
        }

        // 選択したエリアを各システムに反映する
        CustomerManager.Instance.SetCurrentArea(_currentArea);
        ScaneBackGroundManager.Instance.ShowBackGround(_currentArea._areaType);

        UIController.Instance.ShowAndSetTip("you seleced the "+ _currentArea.name);

        MarketManager.Instance.CloseTheMarket();
        CustomerManager.Instance.SetDayCustomerCount();
        HideConfirmAreaUI();
        ShopManager.Instance.CloseTheShop();
    }

    //店を開ける場所をリセット
    public void CancelArea()
    {
        _currentArea = null;
        HideConfirmAreaUI();
    }

    // UI に選択したエリアの情報を表示する
    void SetConfirmAreaUI(AreasData area)
    {
        _areaName.SetText(area._areaType);
        _areaPopulation.SetText(area._populationCount.ToString());
    }
    //------------------------------------------------------------------>

    // 現在選択中のエリアを取得する
    public AreasData GetCurrentArea()
    {
        return _currentArea;
    }

    //---------------------------------------------------------------->

    // エリア選択ボタンを押せる状態にする
    public void InteractableAreasBT()
    {
        // 店の商品がない場合は選択できない
        if (ShopManager.Instance.GetShopItems().Count ==0) return;

        foreach (Button bt in _areasBT)
        {
            bt.interactable = true;
        }
    }

    // エリア選択ボタンを押せない状態にする
    public void UnInteractableAreasBT()
    {
        foreach (Button bt in _areasBT)
        {
            bt.interactable = false;
        }
    }

}
