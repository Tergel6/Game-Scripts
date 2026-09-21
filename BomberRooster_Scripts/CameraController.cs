using UnityEngine;

public class CameraController : MonoBehaviour
{
    Vector3 _offset;            //  カメラとプレイヤーの位置の差
    Transform _transform;       //  自分自身の「Transform」

    //============================== Start =============================>
    void Start()
    {
        //　自分自身の「Transform」を指定する
        _transform = transform;
        //　カメラとプレイヤーの位置の差を計算する
        _offset = _transform.position - PlayerController.instance.transform.position;
    }

    //============================== LateUpdate =============================>
    void LateUpdate()
    {
        //　プレイヤーが空になって無い場合
        if (PlayerController.instance != null)
        {
            //　カメラがプレイヤーについていく
            _transform.position = PlayerController.instance.transform.position + _offset;
        }
    }
}
