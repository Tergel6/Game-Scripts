using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

//============================ Class ============================>

// 銀行のデータをまとめるクラス
[Serializable]
public class Bank
{
    public int _loanLimit;                                          // 借入可能額
    public List<LoanData> _loanData = new List<LoanData>();         // 借入額の箱
    public List<RepayData> _repayData = new List<RepayData>();      // 返済データ
}

// 新しい借入額データ
[Serializable]
public class LoanData　　　　　　　　　                              
{
    public int _newLoanAmount;                                      // 借入額
    public int _newLoanterm;                                        // 借入期間（月）
    public float _newInterest;                                      // 金利
    public String _newLoanDate;                                     // 借入日
    public LoanData(int newLoanAmount, int newLoanterm, float newInterest, string newLoanDate)
    {
        _newLoanAmount = newLoanAmount;
        _newLoanterm = newLoanterm;
        _newInterest = newInterest;
        _newLoanDate = newLoanDate;
    }
}

// 返済データ
[Serializable]
public class RepayData　　　                                       
{
    public String _repayDate;                               // 返済日
    public int _repayAmount;                                // 返済額
    public RepayData(String newDate, int newAmount)
    {
        _repayDate = newDate;
        _repayAmount = newAmount;
    }
}

//============================ MonoBehaviour ============================>
public class BankData : MonoBehaviour
{
    public static BankData Instance { get; private set; }

    
    [SerializeField] Button _bankButton;
    [SerializeField] TextMeshProUGUI _loanLimitUI;          // 借入可能額の表示
    [SerializeField] TextMeshProUGUI _loanAmountUI;         // 選択中の借入額
    [SerializeField] Slider _loanAmountSlider;              // 借入額スライダー
    [SerializeField] TextMeshProUGUI _maxAmount;            // 最大借入額の表示
    [SerializeField] int _startLoanAmount;                  // スライダー1目盛りの金額

    [Header("LoanConfirmUI")]
    [SerializeField] GameObject _loanInfoUI;                // 借入確認 UI
    [SerializeField] TextMeshProUGUI _newLoanAmount;        // 借入額表示
    [SerializeField] TextMeshProUGUI _newLoanTerm;          // 借入期間表示
    [SerializeField] TextMeshProUGUI _newMonthlyPayment;    // 月々の返済額表示

    [Space]
    [SerializeField] Toggle _month3;                        //借入期間（3月）
    [SerializeField] Toggle _month6;　　　　　               //借入期間（6月）
    [SerializeField] Toggle _month8;　　　　　               //借入期間（8月）
    [SerializeField] float _interest3Month;                 //借入期間（3月）の金利
    [SerializeField] float _interest6Month;                 //借入期間（6月）の金利
    [SerializeField] float _interest8Month;                 //借入期間（8月）の金利
    

    Bank _bankData = new();                                 // 銀行データ本体

    int _threeMonths = 3;
    int _sixMonths = 6;
    int _eightMonths = 8;
    int _twelveMonths = 12;

    int _loanAmount;                                        // 借入額
    int _loanTerm;                                          // 借入期間
    float _loanInterest;                                    // 金利
    string _loanDate;                                       // 借入日

    //============================ Awake ============================>
    private void Awake()
    {
        Instance = this;
    }

    //============================ Start ============================>
    private void Start()
    {
        // 初期の借入可能額を設定
        SetLoanLimit();             
    }

    //============================ Function ============================>

    // スライダーの値から借入額を計算する
    public void SetLoanAmount()
    {
        _loanAmount = (int)_loanAmountSlider.value * _startLoanAmount;
        _loanAmountUI.SetText(_loanAmount.ToString());
    }

    //借入データ（借入確認UI）を表示する
    public void ShowLoanInfo()
    {
        if (_loanAmount == 0) return;

        SetLoanTerm();
        SetLoanInfo();
        _loanInfoUI.SetActive(true);
    }
    //借入データ（借入確認UI）を非表示する
    public void HideLoanInfo()
    {
        _loanInfoUI.SetActive(false);
    }
    //借入データを設定する
    void SetLoanInfo()
    {
        _newLoanAmount.SetText(_loanAmount.ToString() + "$");
        _newLoanTerm.SetText(_loanTerm.ToString() + " month");
        _newMonthlyPayment.SetText(((int)((_loanAmount + _loanAmount * 0.01f * _loanInterest * _loanTerm) / _loanTerm)).ToString() + "$");
    }

    //---------------------------------------------------------------->
    //借入を確認する
    public void ConfirmTheLoan()
    {
        _bankData._loanLimit -= _loanAmount;
        UIController.Instance.ShowAndSetTip("You borrowed:" + _loanAmount + " yen");

        PlayerData.Instance.AddMoney(_loanAmount);
        PlayerData.Instance.SetPlayerCredit(_bankData._loanLimit);

        AddNewLoan();
        SetLoanLimit();
        HideLoanInfo();
        InitLoanData();
    }

    // 借入可能額を更新する
    void SetLoanLimit()
    {
        int creadit = PlayerData.Instance.GetPlayerCredit();
        _bankData._loanLimit = creadit;
        _loanLimitUI.SetText(_bankData._loanLimit.ToString() + "$");
        _maxAmount.SetText(_bankData._loanLimit.ToString() + "$");
        SetSliderMaxAmount();
    }

    // スライダーの最大値を設定する
    void SetSliderMaxAmount()
    {
        _loanAmountSlider.maxValue = _bankData._loanLimit / _startLoanAmount;
    }
    //----------------------Slider-------------------------->

    // 借入データを初期化する
    void InitLoanData()
    {
        _loanAmount = 0;
        _loanAmountSlider.value = _loanAmount;
        _loanAmountUI.SetText(_loanAmount.ToString());
    }

    // 借入期間と金利を設定する
    void SetLoanTerm()
    {
        if (_month3.isOn)
        {
            _loanTerm = _threeMonths;
            _loanInterest = _interest3Month;
        }
        else if (_month6.isOn)
        {
            _loanTerm = _sixMonths;
            _loanInterest = _interest6Month;
        }
        else if (_month8.isOn)
        {
            _loanTerm = _eightMonths;
            _loanInterest = _interest8Month;
        }
    }

    //新しい借入を追加する
    void AddNewLoan()　　　　　　　　　　
    {
        _loanDate = GameTime.Instance.GetTime();
        _bankData._loanData.Add(new LoanData(_loanAmount, _loanTerm, _loanInterest, _loanDate));
        AddRepayData();
    }


    // 返済データを追加する
    void AddRepayData()             
    {
        int year = int.Parse(_loanDate.Substring(0, 4));
        int month = int.Parse(_loanDate.Substring(4, 2));
        int day = int.Parse(_loanDate.Substring(6, 2));

        int repayAmount = (int)((_loanAmount + _loanAmount * 0.01f * _loanInterest * _loanTerm) / _loanTerm);

        // 返済時間
        for (int i = 0; i < _loanTerm; i++)  
        {
            month++;
            if (month > _twelveMonths)
            {
                year++;
            }
            String newDate = $"{year}{month:00}{day:00}";

            _bankData._repayData.Add(new RepayData(newDate, repayAmount));
        }
    }

    // 返済の日と金額の確認
    public int ChackRepayLoan(String date)
    {
        int repayLoan = 0;
        if (_bankData._repayData.Count <= 0)
        {
            repayLoan = 0;
        }
        else
        {
            for (int i = _bankData._repayData.Count - 1; i >= 0; i--)
            {
                if (_bankData._repayData[i]._repayDate == date)
                {
                    repayLoan += _bankData._repayData[i]._repayAmount;
                }
            }
        }
        
        return repayLoan;

    }
    // 実際に返済する
    public void RepayLoan(String date)　　　　　　　
    {
        if (_bankData._repayData.Count <= 0) return;

        for (int i = _bankData._repayData.Count - 1; i >= 0; i--)
        {
            if (_bankData._repayData[i]._repayDate == date)
            {
                if (PlayerData.Instance.GetPlayerMoney() > _bankData._repayData[i]._repayAmount)
                {
                    PlayerData.Instance.UseMoney(_bankData._repayData[i]._repayAmount);
                    _bankData._repayData.RemoveAt(i);
                }
                else
                {
                    Debug.LogError("no Money!!!");
                }
            }
        }
    }

    // 銀行 UI を押せる状態にする
    public void InteractableBankUI()
    {
        _bankButton.interactable = true;
    }

    // 銀行 UI を押せない状態にする
    public void UnInteractableBankUI()
    {
        _bankButton.interactable = false;
    }

    //============================== SaveAndLoadBankData =============================>

    //　銀行のデータを保存する
    public void SaveBankData()
    {
        GameDataManager.gameData.bankData = _bankData;
    }

    //　銀行のデータを読み込む
    public void LoadBankData()
    {
        _bankData = GameDataManager.gameData.bankData;
    }

}
