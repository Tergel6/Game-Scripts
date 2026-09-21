
using System;
using TMPro;
using UnityEngine;

//============================== Class ==========================>

// ゲーム内の時間・日付・天気などを保存するデータクラス
[Serializable]
public class WorldData
{
    public float _currentTime;  // ゲーム内で経過した時間
    public int _minute;         // 現在の分
    public int hour;            // 現在の時
    public int _startHour;      // 仕事開始の時間
    public int _workHour;       // 1日の労働時間
    public int _day;            // 現在の日付
    public int _week;           // 現在の曜日
    public int month;           // 現在の月
    public int _year;           // 現在の年
    public int _weather;        // 天気の種類
}

//============================ MonoBehaviour ============================>
public class GameTime : MonoBehaviour
{
    public static GameTime Instance { get; private set; }

    [SerializeField] TextMeshProUGUI _weatherUI;    // 天気表示
    [SerializeField] TextMeshProUGUI _dateUI;       // 日付表示
    [SerializeField] TextMeshProUGUI _TimeUI;       // 時間表示
    [SerializeField] int _startHour;                // 仕事開始時間
    [SerializeField] int _workHour;                 // 仕事時間（1日の長さ）


    [SerializeField][Range(1, 10)] int _timeSpeed;  // 時間の進む速さ（ゲーム内）

    [SerializeField] int _payDay;                   // 給料日（毎月）

    // 天気の種類
    string[] weatherArray =
{
        "Sunny",
        "Rainy",
        "Typhoon",
        "Snowy",
        "High Temperature",
        "Strong Wind"
    };

    // 曜日の種類
    string[] weekArray =
    {
        "Mon",
        "Tue",
        "Wed",
        "Thu",
        "Fri",
        "Sat",
        "Sun"
    };

    WorldData _worldData = new WorldData();
    int _workTime;                              // 1日の総時間（分）      

    bool _isPassHour = false;                   // 1時間経過したか
    bool _isPassDay = false;                    // 1日経過したか

    bool _isPayday;                             // 給料日かどうか

    //============================== Awake ==========================>
    private void Awake()
    {
        Instance = this;                        // シングルトンの設定

    }

    //============================== Start ==========================>
    void Start()
    {
        // 初期設定
        _worldData._startHour = _startHour;
        _worldData._workHour = _workHour;

        // 1日の総分数
        _workTime = _workHour * 60;

        // 天気を決める
        SetWeather();

        // 時間を初期化
        SetTime();
    }

    //============================== Update ==========================>
    void Update()
    {
        // ショップが閉まっている時は時間を進めない
        if (ShopManager.Instance.GetShopStatus()==false) 
            return;

        // 時間を進める
        SetTime();

        // 日付の変化チェック
        IsPassDay();

        // 時間の変化チェック
        IsPassHour();
    }

    //------------------------------------------------------------->

    // 1日が経過したかどうか判定する
    void IsPassDay()
    {
        // 日付が変わる瞬間（開始時間 + 分が0）
        if (!_isPassDay && _worldData.hour == _startHour&&_worldData._minute==0)
        {
            if (_worldData.month == 4)
            {
                BankData.Instance.UnInteractableBankUI();
            }
            Debug.Log("Next Day");
            _isPassDay = true;

            // 次の日の処理
            ShopManager.Instance.CloseTheShop();
            ShopManager.Instance.ShowReportBG();

            // 新しい天気を設定
            SetWeather();
            CustomerManager.Instance.SetDayCustomerCount();
            MarketManager.Instance.OpenTheMarket();
        }
        else if (_isPassDay && _worldData.hour == _startHour + 1)
        {
            // 日付変化フラグをリセット
            _isPassDay = false;
        }
    }

    // 1時間が経過したかどうか判定する
    void IsPassHour()
    {
        if (!_isPassHour && _worldData._minute == 1)
        {
            _isPassHour = true;
            CustomerManager.Instance.GetTimeCustomerCount(_worldData.hour);
        }
        else if (_isPassHour && _worldData._minute == 2)
        {
            _isPassHour = false;
        }
    }

    //------------------------------------------------------------------------>

    // 天気をランダムで設定する
    void SetWeather()
    {
        int r = UnityEngine.Random.Range(0, 101);
        if (r <= 48)
        {
            _weatherUI.SetText(weatherArray[0]);
            _worldData._weather = 0;
        }
        else if (r > 48&&r<=69)
        {
            _weatherUI.SetText(weatherArray[1]);
            _worldData._weather = 1;
        }
        else if (r > 69 && r <= 70)
        {
            _weatherUI.SetText(weatherArray[2]);
            _worldData._weather = 2;
        }
        else if (r > 70 && r <= 72)
        {
            _weatherUI.SetText(weatherArray[3]);
            _worldData._weather = 3;
        }
        else if (r > 72 && r <= 84)
        {
            _weatherUI.SetText(weatherArray[4]);
            _worldData._weather = 4;
        }
        else if (r > 84 && r <= 100)
        {
            _weatherUI.SetText(weatherArray[5]);
            _worldData._weather = 5;
        }

    }

    // 時間を進めて UI に反映する
    void SetTime()
    {
        // 時間を進める（timeSpeed によって速さが変わる）
        _worldData._currentTime +=Time.deltaTime * _timeSpeed;

        // 分の計算
        _worldData._minute = (int)_worldData._currentTime % 60;

        // 時間の計算（開始時間からの経過）
        _worldData.hour =(int) (_worldData._startHour + (_worldData._currentTime / 60) % _worldData._workHour);

        // 時間の表示
        _TimeUI.SetText("{0:00}:{1:00}", _worldData.hour, _worldData._minute);

        // 日付・曜日・月・年の計算
        _worldData._day = 1 + (int)(_worldData._currentTime / (_worldData._workHour * 60)) % 31;
        _worldData._week = (int)(_worldData._currentTime / (_worldData._workHour * 60 * 7)) % 7;
        _worldData.month = 1 + (int)(_worldData._currentTime / (_worldData._workHour * 60 * 30)) % 12;
        _worldData._year = 2001 + (int)(_worldData._currentTime / (_worldData._workHour * 60 * 30 * 12));

        // UI に反映
        _dateUI.SetText($"{_worldData._year}/{_worldData.month:00}/{_worldData._day:00} {weekArray[_worldData._week]}");
        _weatherUI.SetText(weatherArray[_worldData._weather]);

        // 他のシステムへ現在の時間データを渡す
        CustomerManager.Instance.SetCurrentworldData(_worldData);

        // 給料日の判定
        SetPayday();
    }

    // 現在の日付を文字列で返す
    public String GetTime()
    {
        String newDate = $"{_worldData._year}{_worldData.month:00}{_worldData._day:00}";

        return newDate;
    }

    //給料日をセット
    void SetPayday()
    {
        if (_worldData._day == _payDay && _isPayday == false)
        {
            _isPayday = true;
        }
        else if (_worldData._day != _payDay && _isPayday == true)
        {
            _isPayday = false;
        }
    }

    //給料日をゲット
    public bool GetPayday()
    {
        return _isPayday;
    }

    //今の月をゲットする
    public int GetCurrentMonth()
    {
        return _worldData.month;
    }

    // 現在の時間を強制的に進める
    public void SetCurrentTime(int num)
    {
        _worldData._currentTime += num;
    }

    // 今日の日付を文字列で返す
    public String GetToday()
    {
        return $"{_worldData._year}{_worldData.month:00}{_worldData._day:00}{weekArray[_worldData._week]}";
    }

    // 現在の天気を返す
    public String GetWeather()
    {
        return weatherArray[_worldData._weather];
    }

    // 経過した日数を返す
    public String GetDaysPassed()
    {
        return $"{(int)_worldData._currentTime / _workTime}"+"Days";
    }

    //販売途中で次の日に行く
    public void GoToNextDay()
    {
        _worldData._currentTime = _worldData._currentTime + (_workTime - (_worldData._currentTime % _workTime));
    }


    //=============================== SaveAndLoadWorldData ==================================>

    //　世界のデータを保存する
    public void SaveWorldData()
    {
        GameDataManager.gameData.worldData = _worldData;
    }

    //　世界のデータを読み込む
    public void LoadWorldData()
    {
        _worldData = GameDataManager.gameData.worldData;
    }

}
