using System.Collections;
using UnityEngine;

public class Can_Main : MonoBehaviour
{
    [SerializeField] GameObject _canGrandParent;    // 缶親オブジェクト

    [SerializeField] Transform _canTransform;       // 缶の Transform
    [SerializeField] Rigidbody _canRigibody;        // 缶の Rigidbody
    [SerializeField] float _canDownTime;            // 缶が落ちる時間
    [SerializeField] Vector3 _gracity;              // 缶が落ちる重力
    //==================================== Start =====================================>
    void Start()
    {
        // 重力を強めに設定
        Physics.gravity = _gracity;
        // 物理を無効化
        _canRigibody.isKinematic = true;
    }

    //==================================== Function =====================================>
    public void isCanDown()
    {
        // 物理を有効化
        _canRigibody.isKinematic = false;
        // 少し遅れて親を付け直す処理を実行
        StartCoroutine(LateCanDown());
    }

    IEnumerator LateCanDown()
    {
        // 0.7秒待ってから実行
        yield return new WaitForSeconds(_canDownTime);
        // 缶を指定した親オブジェクトに戻す
        transform.SetParent(_canGrandParent.transform);
    }
}
