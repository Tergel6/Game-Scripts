using System;
using UnityEngine;

// タレットの種類を区別するための列挙型
public enum TurretType
{
    GoldGenerator,          // お金を生成する銀行タワー
    GunTurret               // 敵を攻撃する砲台タワー
}   

[Serializable]
// タワーの基本データをまとめたクラス
public class TurretData 
{
    public TurretType type;                     //タワーの種類
    public GameObject turretPre;                //タワーのプレハブ
    public int cost;                            //設置にかかる金額
    public GameObject turretUpgradedPre;        //アップグレードタワープレハブ
    public int _costUpGraded;                   //アップグレードにかかる金額
}
