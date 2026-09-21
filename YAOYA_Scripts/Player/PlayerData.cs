
using System;
using TMPro;
using UnityEngine;

//============================ Class ============================>

// プレイヤーのデータをまとめるシリアライズ可能なクラス
[Serializable]
public class Player
{
    public string _playerName;                                  // プレイヤー名
    public int _playerCredit = 60000;                           // 信用度（銀行の借入可能額に使用）
    public int _playerMoney;                                    // 所持金
    public int _playerCapacity;                                 // 倉庫の最大容量
    public int _usedCapacity;                                   // 使用中の倉庫容量
}


//============================ MonoBehaviour ============================>
public class PlayerData : MonoBehaviour
{
    public static PlayerData Instance { get; private set; }

    [SerializeField] TextMeshProUGUI _playerNameUI;             // プレイヤー名の UI
    [SerializeField] TextMeshProUGUI _playerMoneyUI;            // 所持金の UI

    Player _playerData = new();                                 // プレイヤーデータ本体

    //============================ Awake ============================>
    private void Awake()
    {
        Instance = this;
    }

    //============================ Start ============================>
    private void Start()
    {
        // 初期プレイヤー名を設定
        _playerData._playerName = "Josh";

        SetPlayerNameUI(_playerData._playerName);
        SetPlayerMoneyUI(_playerData._playerMoney);
    }

    //============================ Function ============================>

    // プレイヤー名を UI に反映する
    public void SetPlayerNameUI(string name)
    {
        _playerNameUI.text = name;
    }

    // 所持金を UI に反映する
    public void SetPlayerMoneyUI(int money)
    {
        _playerMoneyUI.SetText("{0}", money);
    }

    // 信用度を取得
    public int GetPlayerCredit()
    {
        return _playerData._playerCredit;
    }

    // 信用度を設定
    public void SetPlayerCredit(int credit)
    {
        _playerData._playerCredit = credit;
    }

    //------------------------------------------------>

    // お金を増やす
    public void AddMoney(int money)
    {
        _playerData._playerMoney += money;
        SetPlayerMoneyUI(_playerData._playerMoney);
    }

    // お金を使う
    public bool UseMoney(int money)
    {
        if (_playerData._playerMoney < money)
        {
            // お金が足りない
            return false;
        }

        _playerData._playerMoney -= money;
        SetPlayerMoneyUI(_playerData._playerMoney);
        return true;
    }

    // プレイヤーのお金を取得する
    public int GetPlayerMoney()　　　　　　　　　　　
    {
        return _playerData._playerMoney;
    }

    // プレイヤーのお金を設定する
    public void SetPlayerMoney(int money)　　　　　　　
    {
        _playerData._playerMoney = money;
        SetPlayerMoneyUI(_playerData._playerMoney);
    }

    // プレイヤーの倉庫サイズを取得する
    public int GetPlayerCapacity()　　　　　　　
    {
        return _playerData._playerCapacity;
    }

    // プレイヤーの倉庫を使用したサイズを取得する
    public int GetUsedCapacity()　　　　　　
    {
        return _playerData._usedCapacity;
    }
     
    // プレイヤーの倉庫サイズを設定
    public void SetPlayerCapacity(int capacity)        
    {
        _playerData._playerCapacity = capacity;
    }
    // プレイヤーの倉庫を使用したサイズを設定　
    public void SetUsedCapacity(int capacity)　　　　　
    {
        _playerData._usedCapacity = capacity;
    }

    //============================= SaveAndLoadPlayerData ===============================>

    // プレイヤーのデータを保存する
    public void SavePlayerData()
    {
        GameDataManager.gameData.playerData=_playerData;
    }
    // プレイヤーのデータを読み込む
    public void LoadPlayerData()
    {
        _playerData = GameDataManager.gameData.playerData;

        // UI に反映
        SetPlayerNameUI(_playerData._playerName);
        SetPlayerMoneyUI(_playerData._playerMoney);
    }
}
