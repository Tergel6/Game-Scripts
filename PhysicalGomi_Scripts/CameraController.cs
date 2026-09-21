using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    public Transform gamePoint;          // カメラが戻る基準位置
    public Transform tr_Target;          // カメラが見るターゲット

    public NewActions newAction;         // 新しいInput Systemのアクション

    // カメラの角度チェック用
    public bool IsCameraX_0_90 => _isCameraX_0_90;
    public bool IsCameraX_90_360 => _isCameraX_90_360;
    public bool IsCameraY_0_90 => _isCameraY_0_90;
    public bool IsCameraY_90_180 => _isCameraY_90_180;
    public bool IsCameraY_180_270 => _isCameraY_180_270;
    public bool IsCameraY_270_360 => _isCameraY_270_360;

    [SerializeField] float cameraRotateSpeed;     // カメラ回転スピード
    [SerializeField] float cameraScalingSpeed;    // ズームスピード
    [SerializeField] float cameraMinDistance;     // 最小距離
    [SerializeField] float cameraMaxDistance;     // 最大距離
    [SerializeField] float cameraMoveSpeed;       // カメラ移動スピード
    [SerializeField] float cameraRotateDuration;  // カメラ戻り時間

    Transform _transform;     // カメラ自身のTransform
    GameObject _obj;          // ダブルクリックで選んだオブジェクト

    Vector3 _cameraPos;       // カメラの初期位置
    Vector3 _newCameraPos;    // カメラが戻る前の位置

    float _t;                 // 補間用の値

    bool _isCameraBack = false;   // カメラが戻る状態かどうか

    Vector2 _xAndz;           // マウス移動値
    float _angleX;            // カメラの水平角度
    float _angleY;            // カメラの垂直角度

    float _offset;            // ターゲットとの距離
    Vector3 _dir;             // ターゲットからカメラへの方向

    float _x, _y, _dy, _z;    // カメラ位置計算用

    float _mouseWheelValue;   // ホイール値

    bool _isCameraMove;       // カメラ移動中かどうか
    Ray _ray;                 // レイキャスト用
    RaycastHit _hit;

    float _lastClickTime = 0;         // 前回クリック時間
    float _doubleClickThreshold = 0.3f; // ダブルクリック判定時間
    bool _isMoveToTarget = false;     // 選んだオブジェクトへ移動中か

    // カメラ角度の状態
    bool _isCameraX_0_90 = false;
	bool _isCameraX_90_360 = false;
	bool _isCameraY_0_90 = false;
	bool _isCameraY_90_180 = false;
	bool _isCameraY_180_270 = false;
	bool _isCameraY_270_360 = false;

	//================================== Awake =====================================>
	void Awake()
	{
		newAction = new NewActions();   // Input Systemの初期化
    }

    // アクションを有効化
    public void OnEnable()
	{
		newAction.Enable();           
    }

    // Input Systemの初期化
    public void OnDisable()
	{
		newAction.Disable();
	}
	//================================== Start =====================================>
	void Start()
	{
		_transform = gameObject.transform;
		_cameraPos = _transform.position; // 初期位置を保存
        _t = 0;
		InitAngleXY();           // カメラ角度を計算
        GetCameraYAngles();      // カメラの向きを判定
    }

    //================================== LateUpdate =====================================>
    void LateUpdate()
    {
        cameraRotate();     // カメラ回転
        CameraScaling();    // ズーム
        CameraMove();       // カメラ移動
        CameraBack();       // カメラを元の位置へ戻す
        SelectTarget();     // ダブルクリックでターゲット選択
        MoveToTarget();     // 選んだターゲットへ移動
        GetCameraYAngles(); // カメラ角度の更新
    }

    //================================== Function =====================================>

    // カメラを元の位置へ戻す
    void CameraBack()
	{
		if (_isCameraBack)
		{
			_t += Time.deltaTime * cameraRotateDuration;

            // カメラとターゲットを元の位置へ戻す
            _transform.position = Vector3.Lerp(_newCameraPos, _cameraPos, _t);
			tr_Target.position = Vector3.Lerp(tr_Target.position, gamePoint.position, _t);
			_transform.LookAt(tr_Target.position);
			if (_t >= 1f)
			{
				_isCameraBack = false;
				_t = 0;
				InitAngleXY();  // 角度を再計算
            }
		}
	}

    // カメラを戻すモードに切り替える
    public void IsCameraBack()
	{ 
		_newCameraPos = _transform.position;   // 戻る前の位置を保存
        _isCameraBack = true;
	}

    // マウスでカメラを回転させる
    void cameraRotate()
	{
		if (newAction.Camera.Click.ReadValue<float>() != 0)
		{
			_xAndz = -newAction.Camera.Look.ReadValue<Vector2>() * cameraRotateSpeed;
			_angleX = (_xAndz.x + _angleX) % 360;
			_angleY = Mathf.Clamp(_xAndz.y + _angleY, 5, 80);

			_y = Mathf.Sin(_angleY * Mathf.Deg2Rad) * _offset;
			_dy = Mathf.Cos(_angleY * Mathf.Deg2Rad) * _offset;
			_x = Mathf.Cos(_angleX * Mathf.Deg2Rad) * _dy;
			_z = Mathf.Sin(_angleX * Mathf.Deg2Rad) * _dy;

			_transform.position = tr_Target.position + new Vector3(_x, _y, _z);

			_transform.LookAt(tr_Target.position);

			GetCameraYAngles();
		}
	}

    // ホイールでズームする
    void CameraScaling()
	{

		if (newAction.Camera.Wheel.ReadValue<float>() != 0)
		{
			if (_offset > cameraMinDistance && _offset < cameraMaxDistance)
			{
				Scaling();
			}
			else if (_offset < cameraMinDistance && newAction.Camera.Wheel.ReadValue<float>() < 0)
			{
				Scaling();
			}
			else if (_offset > cameraMaxDistance && newAction.Camera.Wheel.ReadValue<float>() > 0)
			{
				Scaling();
			}
		}

	}

    // ズーム処理を行う
    void Scaling()
	{
		_mouseWheelValue = newAction.Camera.Wheel.ReadValue<float>();
		_transform.position = _transform.position + (_transform.forward * cameraScalingSpeed * _mouseWheelValue * Time.deltaTime);

		InitAngleXY();
	}

    // カメラの角度と距離を計算する
    void InitAngleXY()
	{
		_transform.LookAt(tr_Target.position);

		_dir = _transform.position - tr_Target.position;
		_offset = _dir.magnitude;

		_angleY = Mathf.Asin(_dir.y / _offset) * Mathf.Rad2Deg;
		_angleX = Mathf.Atan2(_dir.z, _dir.x) * Mathf.Rad2Deg;
	}

    // マウスドラッグでカメラを移動する
    void CameraMove()
	{
		if (newAction.Camera.Move.IsPressed())
		{
			_isCameraMove = true;
		}
		else
		{
			_isCameraMove = false;
		}
		if (_isCameraMove)
		{

			Vector2 delta = Mouse.current.delta.ReadValue();
			Vector3 moveCamera = new Vector3(-delta.x, -delta.y, 0) * cameraMoveSpeed;
			Vector3 moveTarget = new Vector3(delta.x, -delta.y, 0) * cameraMoveSpeed;

			_transform.Translate(moveCamera, Space.Self);
			tr_Target.LookAt(_transform);
			tr_Target.Translate(moveTarget, Space.Self);
		}
	}

    // ダブルクリックでターゲットを選ぶ
    void SelectTarget()
	{
		if (Mouse.current.leftButton.wasPressedThisFrame)
		{
			float now = Time.time;
			if (now - _lastClickTime < _doubleClickThreshold)
			{
				_ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
				if (Physics.Raycast(_ray, out _hit))
				{
					if (_hit.transform.tag == "GObjects")
					{
						_obj = _hit.transform.gameObject;
						_isMoveToTarget = true;
					}
				}
			}
			_lastClickTime = now;
		}
	}

    // 選んだオブジェクトへカメラを移動する
    void MoveToTarget()
	{
		if (_isMoveToTarget)
		{
			tr_Target.position = Vector3.Lerp(tr_Target.position, _obj.transform.position, Time.deltaTime * 2f);
			_transform.LookAt(tr_Target);
			if (Vector3.Distance(tr_Target.position, _obj.transform.position) < 0.1f)
			{
				_isMoveToTarget = false;
			}
		}
	}

    // カメラの向きを4つの範囲で判定する
    public void GetCameraYAngles()
	{
		if (_transform.eulerAngles.y < 90f)
		{
			_isCameraY_0_90 = true;
			_isCameraY_90_180 = false;
			_isCameraY_180_270 = false;
			_isCameraY_270_360 = false;
		}
		else if (_transform.eulerAngles.y >= 90f && _transform.eulerAngles.y < 180f)
		{
			_isCameraY_90_180 = true;
			_isCameraY_0_90 = false;
			_isCameraY_180_270 = false;
			_isCameraY_270_360 = false;
		}
		else if (_transform.eulerAngles.y >= 180f && _transform.eulerAngles.y < 270f)
		{
			_isCameraY_180_270 = true;
			_isCameraY_0_90 = false;
			_isCameraY_90_180 = false;
			_isCameraY_270_360 = false;
		}
		else if (_transform.eulerAngles.y >= 270f && _transform.eulerAngles.y < 360f)
		{
			_isCameraY_270_360 = true;
			_isCameraY_0_90 = false;
			_isCameraY_90_180 = false;
			_isCameraY_180_270 = false;
		}
		if (_transform.eulerAngles.x < 90f)
		{
			_isCameraX_0_90 = true;
			_isCameraX_90_360 = false;
		}
		else if (_transform.eulerAngles.x >= 90f && _transform.eulerAngles.x < 360f)
		{
			_isCameraX_90_360 = true;
			_isCameraX_0_90 = false;
		}
	}
}
