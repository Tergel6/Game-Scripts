using System;
using UnityEngine;

[Serializable]
public struct ShopReview
{
    public int _aaaaa;      // •]‰¿€–ÚA
    public int _bbbbb;      // •]‰¿€–ÚB
    public int _ccccc;      // •]‰¿€–ÚC
    public int _ddddd;      // •]‰¿€–ÚD
    public int _eeeee;      // •]‰¿€–ÚE
}

//============================ ScriptableObject ============================>

// ’‡‰îŠ‚Åw“ü‚Å‚«‚éu“X•Üv‚Ìƒf[ƒ^
[CreateAssetMenu(fileName ="NewShop",menuName ="Brokerage/Shop")]
public class ShopsData : ScriptableObject
{
    public string _name;                // “X•Ü–¼
    public Sprite _icon;                // UI ‚É•\¦‚·‚éƒAƒCƒRƒ“
    public Sprite _image;               // “X•Ü‚Ì‰æ‘œ
    public GameObject _shopPrefab;      // “X•Ü‚Ì3Dƒ‚ƒfƒ‹
    public int _price;                  // “X•Ü‚Ìw“ü‰¿Ši
    public int _stallTypes;             // •i–Ú”
    public int _stallSize;              // “X“à‚Ì’I‚ÌƒTƒCƒY
    public int _wareHouseSize;          // “X•Ü‚Ì‘qŒÉ—e—Ê
    public int _staff;                  // “X•Ü‚ÉŒÙ‚¦‚éƒXƒ^ƒbƒt”
    public ShopReview _shopReview;      // “X‚Ì•]‰¿
}
