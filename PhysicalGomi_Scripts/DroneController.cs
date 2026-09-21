using System;
using System.Collections;
using UnityEngine;

public class DroneController : MonoBehaviour
{
    static Boolean isReleaseHand;										// 手を開く指示が出たか
    static Boolean isClampHand;											// 手を閉じる指示が出たか

    [Range(0f, 40f)][SerializeField] float _startValue;					// 手の初期角度
    [Range(0f, 40f)][SerializeField] float _targetValue;                // 手を開く角度

	[Space]
    [SerializeField] float _funRotateSpeed;								// ファンの回転スピード
    [SerializeField] float _amplitude;									// 上下の揺れ幅
    [SerializeField] float _flySpeed;									// 上下の揺れスピード
	[Header("Hand")]
    [SerializeField] Transform _hand01Transform;						// 左手のTransform
    [SerializeField] Transform _hand02Transform;						// 右手のTransform
	[Header("Funs")]
    [SerializeField] Transform[] _funsTransform = new Transform[4];		// 4つのファンのTransform

    [SerializeField] float _rotSpeed;									// 手の回転スピード
    [SerializeField] float _tt;											// 補間用の時間


    Quaternion _target01;												// 左手の開いた角度
    Quaternion _target02;												// 右手の開いた角度
    Quaternion _original01;												// 左手の元の角度
    Quaternion _original02;												// 右手の元の角度

    Boolean _isDroneFlying;												// ドローンが飛行中かどうか

    Vector3 _droneStartPos;												// ドローンの初期位置

    //====================================== Start ===========================================>
    void Start()
	{
        // 左手の開いた角度をゲット
        _target01 = Quaternion.Euler(_targetValue, _hand01Transform.rotation.y, _hand01Transform.rotation.z);
        // 右手の開いた角度をゲット
        _target02 = Quaternion.Euler(-_targetValue, _hand02Transform.rotation.y, _hand02Transform.rotation.z);
        // 左手の元の角度をゲット
        _original01 = Quaternion.Euler(_startValue, _hand01Transform.rotation.y, _hand01Transform.rotation.z);
        // 右手の元の角度をゲット
        _original02 = Quaternion.Euler(-_startValue, _hand02Transform.rotation.y, _hand02Transform.rotation.z);
        // 左手のTransformを指定
        _hand01Transform.rotation = _original01;
        // 右手のTransformを指定
        _hand02Transform.rotation = _original02;

        // 手を開く指示を出さない
        isReleaseHand = false;
        // 手を閉じる指示を出さない
        isClampHand = false;

        //--------------------------------------------->
        // ドローンが飛行中にする
        _isDroneFlying = true;

        // ドローンの初期位置を設定
        _droneStartPos = transform.position;
	}

	//====================================== Update ===========================================>
	void Update()
	{
        // 毎フレーム、飛行と手の動きを更新する
        if (_isDroneFlying)
		{
			DroneFly();
			FunRotate();
		}

		if (isReleaseHand)
		{
			isReleaseHand = false;
			StartCoroutine(ReleaseHandOnce());
		}

		if (isClampHand)
		{
			isClampHand = false;
			StartCoroutine(ClampHandOnce());
		}
	}
    //====================================== Fuction ===========================================>

    // 手を開くアニメを1回だけ再生する
    IEnumerator ReleaseHandOnce()
	{
		_tt = 0;
		while (_tt < 1f)
		{
			_tt += Time.deltaTime / _rotSpeed;
			_hand01Transform.rotation = Quaternion.Slerp(_original01, _target01, _tt);
			_hand02Transform.rotation = Quaternion.Slerp(_original02, _target02, _tt);
			yield return null;
		}
	}
    // 手を閉じるアニメを1回だけ再生する

    IEnumerator ClampHandOnce()
	{
		_tt = 0;
		while (_tt < 1f)
		{
			_tt += Time.deltaTime / _rotSpeed;
			_hand01Transform.rotation = Quaternion.Slerp(_target01, _original01, _tt);
			_hand02Transform.rotation = Quaternion.Slerp(_target02, _original02, _tt);
			yield return null;
		}
	}

    // 手を開くフラグをオンにする
    public static void OnReleaseHand()
	{
		if (!isReleaseHand)
		{
			isReleaseHand = true;
		}
	}

    // 手を閉じるフラグをオンにする
    public static void OnClampHand()
	{
		if (!isClampHand)
		{
			isClampHand = true;
		}
	}

    // ファンを回転させる
    void FunRotate()
	{
		for (int i = 0; i < _funsTransform.Length; i++)
		{
			_funsTransform[i].Rotate(Vector3.up * _funRotateSpeed * Time.deltaTime);
		}
	}

    // ドローンを上下にふわっと動かす
    void DroneFly()
	{
		float newY = Mathf.Sin(Time.deltaTime * _flySpeed) * _amplitude;
		transform.position = _droneStartPos + new Vector3(0, newY, 0);
	}
}
