
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="NewCustomer",menuName ="Area/Customer")]
public class CustomerData : ScriptableObject
{
    public List<GameObject> _models = new();    // 顧客の見た目モデル
    public float _inShelfLife;                  // 商品の新鮮度に対する反応
    public float _inQuality;                    // 商品の品質に対する反応
    public float _inSeason;                     // 季節要因への反応
    public float _inSalePrice;                  // 価格に対する反応
    public float _inSalePosition;               // 並ぶ位置への反応
    public float _inSaleCount;                  // 販売数に対する反応
}
