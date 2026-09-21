using UnityEngine;
using UnityEngine.InputSystem;

public class ShopBox : MonoBehaviour
{
    public bool _haveItem = false;              // この棚に商品があるかどうか

    [SerializeField] Material _originalMat;     // 通常時のマテリアル
    [SerializeField] Material _newMat;          // 選択時のマテリアル
    [Space]
    [SerializeField] GameObject _boxParent;     // 商品を置く親オブジェクト
    [SerializeField] int _boxPosition;          // 棚の位置番号
    [SerializeField] int _boxSize;              // 棚のサイズ（容量）

    Renderer _boxRender;

    bool _isSelect = false;                     // この棚が選択されているか
    bool _isCanSelect = false;                  // 選択可能かどうか   

    Camera _camera;

    //============================ Awake ============================>
    void Awake()
    {
        // メインカメラを取得（Raycast に使用）
        _camera = Camera.main;
    }

    //============================ Start ============================>
    void Start()
    {
        _boxRender = GetComponent<Renderer>();
        _boxRender.material = _originalMat;     // 初期状態は通常マテリアル
    }

    //============================ Update ============================>
    void Update()
    {
        // マウスの左クリックボタン入力の検出
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Ray ray = _camera.ScreenPointToRay(Mouse.current.position.ReadValue());
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                // この棚をクリックした場合
                if (hit.collider.gameObject == this.gameObject)
                {
                    IsOnMouseDown();
                }
                // 別の場所をクリックした場合（選択解除処理）
                else if (_isSelect && _isCanSelect && !ShopItemUI.GetPulse() && !ShopManager.Instance.GetGoAndBackButton())
                {
                    ShopManager.Instance.HideGoAndBackUI();
                    ShopManager.Instance.HideGotoCarUI();
                    ShopBoxManager.Instance.ResetMaterials();
                    ShopBoxManager.Instance.SelectShopBox(null);
                    ShopItemUI.IsNoPulse();
                }
            }
        }
    }

    //============================ Click ============================>
    // マウスの左クリックボタンを押す
    void IsOnMouseDown()
    {
        // 未選択 → 選択状態にする
        if (!_isSelect && _isCanSelect)
        {
            ShopManager.Instance.HideGoAndBackUI();

            ShopBoxManager.Instance.ResetMaterials();
            ShopBoxManager.Instance.SelectShopBox(this);

            // ハイライト表示
            _boxRender.material = _newMat;

            // アイテム UI を点滅状態にする
            ShopItemUI.IsPulse();
            _isSelect = true;
            return;
        }

        // 選択中 → 選択解除
        else if (_isSelect && _isCanSelect)
        {
            ShopManager.Instance.HideGoAndBackUI();

            ShopBoxManager.Instance.ResetMaterials();
            ShopBoxManager.Instance.SelectShopBox(null);
            ShopItemUI.IsNoPulse();
        }

    }
    //============================ Function ============================>

    // マテリアルを元に戻す
    public void ResetMaterial()
    {
        _boxRender.material = _originalMat;
    }

    // 棚の位置番号を取得
    public int GetPosition()
    {
        return _boxPosition;
    }

    // 商品を置く親オブジェクトを取得
    public GameObject GetBoxParent()
    {
        return _boxParent;
    }

    // 選択状態を解除
    public void SetNoSelect()
    {
        _isSelect = false;
    }

    // 棚のサイズ（容量）を取得
    public int GetBoxSize()
    {
        return _boxSize;
    }
    //--------------------------------------------->

    // 選択可能状態にする
    public void SetCanSelect()
    {
        if (!_isCanSelect)
        {
            _isCanSelect = true;
        }
    }

    // 選択不可状態にする
    public void SetCanNotSelect()
    {
        if (_isCanSelect)
        {
            _isCanSelect = false;
        }
    }

    // 棚にある商品をすべて削除する
    public void ClearBoxItems()
    {
        if (_boxParent.transform.childCount>1)
        {
            for (int i = 1; i < _boxParent.transform.childCount; i++)
            {
                Destroy(_boxParent.transform.GetChild(i).gameObject);
            }
        }
    }

}
