using UnityEngine;

public class ShopBoxManager : MonoBehaviour
{
    public static ShopBoxManager Instance {  get; private set; }

    [SerializeField] GameObject[] _boxes;       // 店内の棚（ShopBox）の一覧

    ShopBox _currentBox;                        // 現在選択中の棚

    //============================ Awake ============================>
    private void Awake()
    {
        Instance = this;
    }

    //============================ Function ============================>

    // 全ての棚のマテリアルと選択状態をリセットする
    public void ResetMaterials()
    {
            for (int i = 0; i < _boxes.Length; i++)
            {
            // マテリアルを元に戻す
            _boxes[i].GetComponent<ShopBox>().ResetMaterial();

            // 選択状態を解除
            _boxes[i].GetComponent<ShopBox>().SetNoSelect();

            // アイテム UI の点滅を停止
            ShopItemUI.IsNoPulse();
        }
    }

    // 選択中の棚を設定する（null で選択解除）
    public void SelectShopBox(ShopBox box)
    {
        _currentBox = box;
    }

    //---------------------------------------------->

    // 現在選択中の棚を取得する
    public ShopBox GetCurrentBox()
    {
        return _currentBox;
    }

    // 指定位置番号の棚を取得する
    public ShopBox GetBoxInPosition(int num)
    {
        return _boxes[num-1].GetComponent<ShopBox>();
    }

    // 全ての棚を「選択不可」にする
    public void DisableBoxesSelect()
    {
        foreach (GameObject box in _boxes)
        {
            box.GetComponent<ShopBox>().SetCanNotSelect();
        }
    }

    // 全ての棚を「選択可能」にする
    public void EnabledBoxesSelect()
    {
        foreach (GameObject box in _boxes)
        {
            box.GetComponent<ShopBox>().SetCanSelect();
        }
    }

    // 全ての棚に置かれた商品を削除する
    public void ClearItemsInBox()
    {
        foreach(GameObject box in _boxes)
        {
            box.GetComponent<ShopBox>().ClearBoxItems();
        }
    }
}
