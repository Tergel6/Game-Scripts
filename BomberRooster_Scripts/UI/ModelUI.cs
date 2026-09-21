using UnityEngine;
using UnityEngine.InputSystem;

public class ModelUI : MonoBehaviour
{
    [SerializeField] Transform _modelTransform;             // プレイヤーの3Dモデル

    [SerializeField, Range(0.1f, 1f)] float _rotateScale;   //　回転する幅

    PlayerControl _inputAction;                             //　インプットシステム

    //============================ Awake =============================>
    void Awake()
    {
        //　インプットシステムを声明
        _inputAction=new PlayerControl();
    }

    //============================ InputAction =============================>

    //　インプットを有効に
    void OnEnable()
    {
        _inputAction.Enable();   
    }

    //　インプットを無効に
    void OnDisable()
    {
        _inputAction.Disable();
    }

    //============================ Update =============================>
    void Update()
    {
        //　マウスの右クリックしてないか判断
        if (!_inputAction.ModelUI.LeftClick.IsPressed())
        {
            return;
        }

        //　マウスの横方向（X軸）の移動量を取得
        float mouseDeltaX = Mouse.current.delta.ReadValue().x;
        //　マウスのX移動 = Y軸周りの回転 
        _modelTransform.Rotate(Vector3.down, mouseDeltaX * _rotateScale);
    }
}
