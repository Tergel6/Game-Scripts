using System.Collections;
using UnityEngine;

public class Drone_MainMenu : MonoBehaviour
{
    [SerializeField] Transform[] _funTransforms = new Transform[4];     // ドロンのファン
    [SerializeField] float _funRotSpeed;                                //　ファンが回転スピード
    [SerializeField] float _moveSpeed;                                  // 横移動スピード

    [SerializeField][Range(0, 10)] float _flySpeed;                     // 上下移動スピード
    [SerializeField][Range(0, 50)] float _amplitude;                    // 上下移動の幅

    [SerializeField] Transform _handATransform;                         // ハンドルAの Transform
	[SerializeField] Transform _handBTransform;                         // ハンドルBの Transform
	[SerializeField] float _handRotVolume;                              // ハンドルの回転量                                                  
	[SerializeField] float _handRotSpeed;                               // ハンドルの回転速度

	Transform _transform;
    float _y;                                                           //　ドロンの位置のｙ軸の数値
    Vector3 _startPos;                                                  //　スタートの位置
    Vector3 _exitPos;                                                   //　終わりの位置

    Quaternion _oldHandAVolume;                                         // ハンドルAの前フレームの回転値
	Quaternion _newHandAVolume;                                         // ハンドルAの新しい回転値
	Quaternion _oldHandBVolume;                                         // ハンドルBの前フレームの回転値
	Quaternion _newHandBVolume;                                         // ハンドルBの新しい回転値

	bool _onPlaceCan=false;                                             // 缶が位置に置かれているか

	//==================================== Start =====================================>
	void Start()
    {
		// ゲームの時間を通常に戻す
		Time.timeScale = 1;
		// 自分のTransformを取得
		_transform = GetComponent<Transform>();
		// ドローンの初期Y位置を保存
		_y = _transform.position.y;
		// スタート位置を保存
		_startPos = transform.position;
        _exitPos = new Vector3(transform.position.x + 350, transform.position.y, transform.position.z);
		// 最初は退場位置に置く
		_transform.position = _exitPos;

		// ハンドルAの回転の開始角度と終了角度を設定
		_oldHandAVolume = Quaternion.Euler(_handATransform.rotation.x, _handATransform.rotation.y+90, _handATransform.rotation.z);
        _newHandAVolume = Quaternion.Euler(_handATransform.rotation.x+ _handRotVolume, _handATransform.rotation.y + 90, _handATransform.rotation.z);
		// ハンドルBの回転の開始角度と終了角度を設定
		_oldHandBVolume = Quaternion.Euler(_handBTransform.rotation.x, _handBTransform.rotation.y+90, _handBTransform.rotation.z);
        _newHandBVolume = Quaternion.Euler(_handBTransform.rotation.x- _handRotVolume, _handBTransform.rotation.y + 90, _handBTransform.rotation.z);
    }

	//==================================== Update =====================================>
	void Update()
    {
        FunRotate();
        DroneFly();

        if (UIController_Main._pointerEnterStart) {
            MoveStartPos();
        }
        else
        {
            MoveExitPos();
        }

        if (_onPlaceCan)
        {
            StartCoroutine(PlaceCanOnce());
        } 
    }

	//==================================== Function =====================================>

	// ドローンを上下飛ぶ処理
	void DroneFly()
    {
        float newY = _y + Mathf.Sin(Time.time * _flySpeed) * _amplitude;
        _transform.position = new Vector3(_transform.position.x, newY, 0);
    }

	// ファンを回転させる処理
	void FunRotate()
    {
        for (int i = 0; i < _funTransforms.Length; i++)
        {
            _funTransforms[i].Rotate(0, _funRotSpeed, 0);
        }
    }


    //　スタートの位置に移動
    void MoveStartPos()
    {
        if (_transform.position != _startPos)
        {
            _transform.position = Vector3.Lerp(_transform.position, _startPos, _moveSpeed*Time.deltaTime);
        }
    }

    //　終わりの位置に移動
    void MoveExitPos()
    {
        if (_transform.position != _exitPos)
        {
            _transform.position = Vector3.Lerp(_transform.position, _exitPos, _moveSpeed*Time.deltaTime);
        }
    }

	// 缶が置かれたときの処理
	public void OnPlaceCan()
    {
        _onPlaceCan = true;
        AudioController.instance.PlayClickButton();
    }

	// ハンドルを一度だけ回転させるアニメーション処理
	IEnumerator PlaceCanOnce()
    {
        float t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime / _handRotSpeed;
            _handATransform.rotation = Quaternion.Slerp(_oldHandAVolume, _newHandAVolume, t);
            _handBTransform.rotation = Quaternion.Slerp(_oldHandBVolume, _newHandBVolume, t);
            yield return null;
        }
    }
}
