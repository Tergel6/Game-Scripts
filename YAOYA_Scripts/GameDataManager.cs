
using System;
using System.Collections.Generic;
using UnityEngine;

//============================ Class ============================>
[Serializable]
public class GameData
{
    public Player playerData;               // プレイヤーに関するデータ
    public WorldData worldData;             // ワールドの状態データ
    public Bank bankData;                   // 銀行システムのデータ
    public BrokerageData brokerageData;     // 証券取引に関するデータ
    public ShopData shopData;               // ショップのデータ
    public List<ItemsDataDTO> itemsData;    // アイテム一覧データ

}

//============================ MonoBehaviour ============================>
public class GameDataManager : MonoBehaviour
{
    // Json に保存する時のキー名
    string GAME_DATA = "gameData";

    // ゲーム全体のデータをまとめるオブジェクト
    public static GameData gameData = new GameData();

    //============================ SaveGameDataByJson ============================>

    // ゲームのデータをJsonに書き込むメソッド
    public void SaveGameDataByJson()
    {
        // 各システムのデータを GameData に反映する
        GameTime.Instance.SaveWorldData();
        PlayerData.Instance.SavePlayerData();
        BankData.Instance.SaveBankData();
        BrokerageManager.Instance.SaveBrokerageData();
        MarketManager.Instance.SaveItemsData();
        ShopManager.Instance.SaveShopData();

        // GameData を Json ファイルとして保存する
        JsonSystem.SaveByJson(GAME_DATA, gameData);
    }

    // ゲームのデータをJsonから読み取りメソッド
    public void LoadGameDataByJson()
    {
        // Json から GameData を読み込む
        gameData = JsonSystem.LoadFromJson<GameData>(GAME_DATA);

        // 読み込んだデータを各システムへ反映する
        GameTime.Instance.LoadWorldData();
        PlayerData.Instance.LoadPlayerData();
        BankData.Instance.LoadBankData();
        BrokerageManager.Instance.LoadBrokerageData();
        MarketManager.Instance.LoadItemsData();
        ShopManager.Instance.LoadShopData();
    }

}
