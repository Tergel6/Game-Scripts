using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Market/Item")]
public class ItemsData : ScriptableObject
{
    public string _id;              // アイテムのID
    public string _name;            // アイテム名
    public GameObject _prefab;      // アイテムのプレハブ
    public Sprite _icon;            // UI に表示するアイコン
    public string _category;        // カテゴリー（野菜・果物など）
    public int _marketPrice;        // 市場価格
    public int _minQty;             // 最小仕入れ数
    public string _origin;          // 原産地
    public string _color;           // 色
    public string _season;          // 季節（旬の時期）
    public int _size;               // アイテムのサイズ
    [Space]
    //------------------Market---------------->
    public int _quantily;           // 品質
    public int _costPrice;          // 仕入価格
    public int _shelfLife;          // 賞味期限
    [Space]
    public int _buyCount;           // 仕入れ数
   
    [Space]
    //-----------------Shop------------------>
    public int _shopCount;          // 店内の在庫数
    public int _salePosition;       // 並ぶ位置
    public int _salePrice;          // 店での販売価格
    public int _saleCount;          // 販売中の数量

}
