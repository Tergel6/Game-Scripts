
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ObjectController : MonoBehaviour
{
    [SerializeField][Range(1f, 20f)] float _rotateSpeed;   // 回転のスピード

    [SerializeField] Material selectedMat;                 // 選択中に使うマテリアル

    [SerializeField] Material mat_Arrow01;                 // 移動矢印のマテリアル
    [SerializeField] Material mat_Arrow02;                 // 回転矢印のマテリアル

    List<Transform> tr_GsonsOBJ = new List<Transform>();   // 子オブジェクトのTransformリスト
    List<Material> originalMats = new List<Material>();    // 元のマテリアルを保存するリスト

    Camera _mainCamera;                                    // メインカメラ
    CameraController _cameraController;                    // カメラコントローラー

    Transform _parentOBJTransform;                         // 親オブジェクトのTransform
    Transform _sonOBJTransform;                            // 子オブジェクトのTransform
    Transform _objectTransform;                            // 実際に動かすオブジェクトのTransform

    GameObject _rotationArrow;                             // 回転用の矢印
    GameObject _positionArrow;                             // 移動用の矢印

    Transform _transform;                                  // このオブジェクト自身のTransform
    GameObject _xPos;                                      // X方向移動の矢印
    GameObject _yPos;                                      // Y方向移動の矢印
    GameObject _zPos;                                      // Z方向移動の矢印
    GameObject _xRot;                                      // X方向回転の矢印
    GameObject _yRot;                                      // Y方向回転の矢印
    GameObject _zRot;                                      // Z方向回転の矢印

    Ray _ray;                                              // レイキャスト用のレイ
    RaycastHit _hit;                                       // レイが当たった情報

    float _zOffset;                                        // Z方向のオフセット
    Vector3 _dragOffset;                                   // ドラッグ開始時の位置差

    bool _isXPosDragging = false;                          // X移動をドラッグ中か
    bool _isYPosDragging = false;                          // Y移動をドラッグ中か
    bool _isZPosDragging = false;                          // Z移動をドラッグ中か
    bool _isXRotDragging = false;                          // X回転をドラッグ中か
    bool _isYRotDragging = false;                          // Y回転をドラッグ中か
    bool _isZRotDragging = false;                          // Z回転をドラッグ中か

    Vector2 _lastMousePos;                                 // 前フレームのマウス位置

    NewActions _clickAction;                               // クリック用のInput Action

    //=============================== Awake ===============================>
    void Awake()
    {
        _clickAction = new NewActions();
    }

    void OnEnable()
    {
        _clickAction.Enable();
        _clickAction.Object.Click.started += _OnMouseDown;
        _clickAction.Object.Click.canceled += _OnMouseUp;
    }
    void OnDisable()
    {
        _clickAction.Object.Click.started -= _OnMouseDown;
        _clickAction.Object.Click.canceled -= _OnMouseUp;
        _clickAction.Disable();
    }

    //=============================== Start ===============================>
    void Start()
    {

        _transform = gameObject.transform;
        _parentOBJTransform = _transform.GetChild(0);       // 親
        _sonOBJTransform = _parentOBJTransform.GetChild(0); // 子
        _objectTransform = _sonOBJTransform.GetChild(0);

        // 子オブジェクトのTransformと元のマテリアルを保存
        foreach (Transform tr_GsonOBJ in _objectTransform)
        {
            tr_GsonsOBJ.Add(tr_GsonOBJ);
            originalMats.Add(tr_GsonOBJ.GetComponent<MeshRenderer>().material);
        }

        _mainCamera = Camera.main;
        _cameraController = Camera.main.GetComponent<CameraController>();
        _positionArrow = _parentOBJTransform.GetChild(1).gameObject;
        _rotationArrow = _parentOBJTransform.GetChild(2).gameObject;

        // 最初は矢印を非表示
        _positionArrow.SetActive(false);
        _rotationArrow.SetActive(false);

        // 移動矢印のXYZ
        _xPos = _positionArrow.transform.GetChild(0).gameObject;
        _yPos = _positionArrow.transform.GetChild(1).gameObject;
        _zPos = _positionArrow.transform.GetChild(2).gameObject;

        // 回転矢印のXYZ
        _xRot = _rotationArrow.transform.GetChild(0).gameObject;
        _yRot = _rotationArrow.transform.GetChild(1).gameObject;
        _zRot = _rotationArrow.transform.GetChild(2).gameObject;

    }

    //=============================== Update ===============================>
    void Update()
    {
        ShowPosAndRot();        // カメラ角度に合わせて矢印を表示
        if (_isXPosDragging || _isYPosDragging || _isZPosDragging)
        {
            MoveObject();       // オブジェクトを移動

        }
        else if (_isXRotDragging || _isYRotDragging || _isZRotDragging)
        {
            RotateObject();     // オブジェクトを回転
        }

    }

    //=============================== Function ===============================>

    // 回転モードのオン・オフを切り替える
    public void OnRotation()
    {
        if (!_rotationArrow.activeSelf)
        {
            EnableRot();
            DisablePos();

        }
        else if (_rotationArrow.activeSelf)
        {
            DisableRot();
        }

    }

    // 移動モードのオン・オフを切り替える
    public void OnPosition()
    {
        if (!_positionArrow.activeSelf)
        {
            EnablePos();
            DisableRot();
        }
        else if (_positionArrow.activeSelf)
        {
            DisablePos();
        }

    }

    // 移動矢印を表示する
    public void EnablePos()
    {
        _positionArrow.SetActive(true);
    }

    // 移動矢印を非表示にする
    public void DisablePos()
    {
        if (_positionArrow.activeSelf)
        {
            _positionArrow.SetActive(false);
        }
    }

    // 回転矢印を表示する
    public void EnableRot()
    {
        _rotationArrow.SetActive(true);
    }

    // 回転矢印を非表示にする
    public void DisableRot()
    {
        if (_rotationArrow.activeSelf)
        {
            _rotationArrow.SetActive(false);
        }

    }

    // カメラ角度に合わせて矢印の向きを調整する
    public void ShowPosAndRot()
    {

        //------------------------------Pos_X--------------------------------->
        if (_cameraController.IsCameraY_0_90)
        {
            _xPos.transform.localPosition = new Vector3(-1.4f, _xPos.transform.localPosition.y, _xPos.transform.localPosition.z);
            _xPos.transform.localRotation = Quaternion.Euler(_xPos.transform.localRotation.x, _xPos.transform.localRotation.y, 90);
            _zPos.transform.localPosition = new Vector3(_zPos.transform.localPosition.x, _zPos.transform.localPosition.y, -1.4f);
            _zPos.transform.localRotation = Quaternion.Euler(-90, _zPos.transform.localRotation.y, _zPos.transform.localRotation.z);

            _xRot.transform.localRotation = Quaternion.Euler(_xRot.transform.localRotation.x, 90f, _xRot.transform.localRotation.z);
            _yRot.transform.localRotation = Quaternion.Euler(90, 180, _yRot.transform.localRotation.z);
            _zRot.transform.localRotation = Quaternion.Euler(_zRot.transform.localRotation.x, 180, _zRot.transform.localRotation.z);
        }
        else if (_cameraController.IsCameraY_90_180)
        {
            _xPos.transform.localPosition = new Vector3(-1.4f, _xPos.transform.localPosition.y, _xPos.transform.localPosition.z);
            _xPos.transform.localRotation = Quaternion.Euler(_xPos.transform.localRotation.x, _xPos.transform.localRotation.y, 90);
            _zPos.transform.localPosition = new Vector3(_zPos.transform.localPosition.x, _zPos.transform.localPosition.y, 1.4f);
            _zPos.transform.localRotation = Quaternion.Euler(90, _zPos.transform.localRotation.y, _zPos.transform.localRotation.z);

            _xRot.transform.localRotation = Quaternion.Euler(_xRot.transform.localRotation.x, -90f, _xRot.transform.localRotation.z);
            _yRot.transform.localRotation = Quaternion.Euler(90, 270f, _yRot.transform.localRotation.z);
            _zRot.transform.localRotation = Quaternion.Euler(_zRot.transform.localRotation.x, 180, _zRot.transform.localRotation.z);
        }
        else if (_cameraController.IsCameraY_180_270)
        {
            _xPos.transform.localPosition = new Vector3(1.4f, _xPos.transform.localPosition.y, _xPos.transform.localPosition.z);
            _xPos.transform.localRotation = Quaternion.Euler(_xPos.transform.localRotation.x, _xPos.transform.localRotation.y, -90);
            _zPos.transform.localPosition = new Vector3(_zPos.transform.localPosition.x, _zPos.transform.localPosition.y, 1.4f);
            _zPos.transform.localRotation = Quaternion.Euler(90, _zPos.transform.localRotation.y, _zPos.transform.localRotation.z);

            _xRot.transform.localRotation = Quaternion.Euler(_xRot.transform.localRotation.x, -90f, _xRot.transform.localRotation.z);
            _yRot.transform.localRotation = Quaternion.Euler(90, 0, _yRot.transform.localRotation.z);
            _zRot.transform.localRotation = Quaternion.Euler(_zRot.transform.localRotation.x, 0, _zRot.transform.localRotation.z);
        }
        else if (_cameraController.IsCameraY_270_360)
        {
            _xPos.transform.localPosition = new Vector3(1.4f, _xPos.transform.localPosition.y, _xPos.transform.localPosition.z);
            _xPos.transform.localRotation = Quaternion.Euler(_xPos.transform.localRotation.x, _xPos.transform.localRotation.y, -90);
            _zPos.transform.localPosition = new Vector3(_zPos.transform.localPosition.x, _zPos.transform.localPosition.y, -1.4f);
            _zPos.transform.localRotation = Quaternion.Euler(-90, _zPos.transform.localRotation.y, _zPos.transform.localRotation.z);

            _xRot.transform.localRotation = Quaternion.Euler(_xRot.transform.localRotation.x, 90f, _xRot.transform.localRotation.z);
            _yRot.transform.localRotation = Quaternion.Euler(90, 90f, _yRot.transform.localRotation.z);
            _zRot.transform.localRotation = Quaternion.Euler(_zRot.transform.localRotation.x, 0, _zRot.transform.localRotation.z);
        }
        //------------------------------Pos_Y--------------------------------->
        if (_cameraController.IsCameraX_0_90)
        {
            _yPos.transform.localPosition = new Vector3(_yPos.transform.localPosition.x, 1.4f, _yPos.transform.localPosition.z);
        }
        else if (_cameraController.IsCameraX_90_360)
        {
            _yPos.transform.localPosition = new Vector3(_yPos.transform.localPosition.x, -1.4f, _yPos.transform.localPosition.z);
        }

        //------------------------------------------------------------------------>
    }


    //----------------------------------------------------------------->

    // 矢印をクリックした時の処理
    void _OnMouseDown(InputAction.CallbackContext context)
    {
        _ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(_ray, out _hit))
        {
            if (!_hit.transform.IsChildOf(_parentOBJTransform))
                return;

            switch (_hit.collider.name)
            {
                case "X_Pos":
                    SelectPos_X();
                    break;
                case "Y_Pos":
                    SelectPos_Y();
                    break;
                case "Z_Pos":
                    SelectPos_Z();
                    break;
                case "X_Rot":
                    SelectRot_X();
                    break;
                case "Y_Rot":
                    SelectRot_Y();
                    break;
                case "Z_Rot":
                    SelectRot_Z();
                    break;
            }
        }
    }

    // マウスを離した時、ドラッグ状態を解除する
    void _OnMouseUp(InputAction.CallbackContext context)
    {
        _isXPosDragging = false;
        _isYPosDragging = false;
        _isZPosDragging = false;
        _isXRotDragging = false;
        _isYRotDragging = false;
        _isZRotDragging = false;

    }
    //---------------------------------------------------------->

    // ドラッグ中の軸に合わせてオブジェクトを移動する
    public void MoveObject()
    {

        if (_isXPosDragging)
        {
            Vector3 screenPos = new Vector3(Mouse.current.position.x.ReadValue(), Mouse.current.position.y.ReadValue(), _zOffset);
            Vector3 worldPos = _mainCamera.ScreenToWorldPoint(screenPos) + _dragOffset;

            _parentOBJTransform.position = new Vector3(worldPos.x, _parentOBJTransform.position.y, _parentOBJTransform.position.z);
        }
        else if (_isYPosDragging)
        {
            Vector3 screenPos = new Vector3(Mouse.current.position.x.ReadValue(), Mouse.current.position.y.ReadValue(), _zOffset);
            Vector3 worldPos = _mainCamera.ScreenToWorldPoint(screenPos) + _dragOffset;

            _parentOBJTransform.position = new Vector3(_parentOBJTransform.position.x, worldPos.y, _parentOBJTransform.position.z);
        }
        else if (_isZPosDragging)
        {
            Vector3 screenPos = new Vector3(Mouse.current.position.x.ReadValue(), Mouse.current.position.y.ReadValue(), _zOffset);
            Vector3 worldPos = _mainCamera.ScreenToWorldPoint(screenPos) + _dragOffset;

            _parentOBJTransform.position = new Vector3(_parentOBJTransform.position.x, _parentOBJTransform.position.y, worldPos.z);
        }
    }

    // ドラッグ中の軸に合わせてオブジェクトを回転する
    public void RotateObject()
    {
        if (_isXRotDragging && _cameraController.IsCameraY_0_90)
        {
            Vector2 currentMousePos = Mouse.current.position.ReadValue();
            float deltaX = currentMousePos.x - _lastMousePos.x;
            float deltaY = currentMousePos.y - _lastMousePos.y;

            float rotationAmount = (-deltaX + deltaY) * _rotateSpeed * Time.deltaTime;
            _sonOBJTransform.Rotate(rotationAmount, 0, 0, Space.World);

            _lastMousePos = currentMousePos;
        }
        else if (_isXRotDragging && _cameraController.IsCameraY_90_180)
        {
            Vector2 currentMousePos = Mouse.current.position.ReadValue();
            float deltaX = currentMousePos.x - _lastMousePos.x;
            float deltaY = currentMousePos.y - _lastMousePos.y;

            float rotationAmount = (-deltaX - deltaY) * _rotateSpeed * Time.deltaTime;
            _sonOBJTransform.Rotate(rotationAmount, 0, 0, Space.World);

            _lastMousePos = currentMousePos;
        }
        else if (_isXRotDragging && _cameraController.IsCameraY_180_270)
        {
            Vector2 currentMousePos = Mouse.current.position.ReadValue();
            float deltaX = currentMousePos.x - _lastMousePos.x;
            float deltaY = currentMousePos.y - _lastMousePos.y;

            float rotationAmount = (deltaX - deltaY) * _rotateSpeed * Time.deltaTime;
            _sonOBJTransform.Rotate(rotationAmount, 0, 0, Space.World);

            _lastMousePos = currentMousePos;
        }
        else if (_isXRotDragging && _cameraController.IsCameraY_270_360)
        {
            Vector2 currentMousePos = Mouse.current.position.ReadValue();
            float deltaX = currentMousePos.x - _lastMousePos.x;
            float deltaY = currentMousePos.y - _lastMousePos.y;

            float rotationAmount = (deltaX + deltaY) * _rotateSpeed * Time.deltaTime;
            _sonOBJTransform.Rotate(rotationAmount, 0, 0, Space.World);

            _lastMousePos = currentMousePos;
        }
        //------------------------------------------Y----------------------------------------------->
        if (_isYRotDragging)
        {
            Vector2 currentMousePos = Mouse.current.position.ReadValue();
            float deltaX = currentMousePos.x - _lastMousePos.x;
            float deltaY = currentMousePos.y - _lastMousePos.y;

            float rotationAmount = -(deltaX - deltaY) * _rotateSpeed * Time.deltaTime;
            _sonOBJTransform.Rotate(0, rotationAmount, 0, Space.World);

            _lastMousePos = currentMousePos;
        }
        //------------------------------------------------------------------------------------------>
        if (_isZRotDragging && _cameraController.IsCameraY_0_90)
        {
            Vector2 currentMousePos = Mouse.current.position.ReadValue();
            float deltaX = currentMousePos.x - _lastMousePos.x;
            float deltaY = currentMousePos.y - _lastMousePos.y;

            float rotationAmount = (-deltaX - deltaY) * _rotateSpeed * Time.deltaTime;
            _sonOBJTransform.Rotate(0, 0, rotationAmount, Space.World);

            _lastMousePos = currentMousePos;
        }
        else if (_isZRotDragging && _cameraController.IsCameraY_90_180)
        {
            Vector2 currentMousePos = Mouse.current.position.ReadValue();
            float deltaX = currentMousePos.x - _lastMousePos.x;
            float deltaY = currentMousePos.y - _lastMousePos.y;

            float rotationAmount = (deltaX - deltaY) * _rotateSpeed * Time.deltaTime;
            _sonOBJTransform.Rotate(0, 0, rotationAmount, Space.World);

            _lastMousePos = currentMousePos;
        }
        else if (_isZRotDragging && _cameraController.IsCameraY_180_270)
        {
            Vector2 currentMousePos = Mouse.current.position.ReadValue();
            float deltaX = currentMousePos.x - _lastMousePos.x;
            float deltaY = currentMousePos.y - _lastMousePos.y;

            float rotationAmount = (deltaX + deltaY) * _rotateSpeed * Time.deltaTime;
            _sonOBJTransform.Rotate(0, 0, rotationAmount, Space.World);

            _lastMousePos = currentMousePos;
        }
        else if (_isZRotDragging && _cameraController.IsCameraY_270_360)
        {
            Vector2 currentMousePos = Mouse.current.position.ReadValue();
            float deltaX = currentMousePos.x - _lastMousePos.x;
            float deltaY = currentMousePos.y - _lastMousePos.y;

            float rotationAmount = (-deltaX + deltaY) * _rotateSpeed * Time.deltaTime;
            _sonOBJTransform.Rotate(0, 0, rotationAmount, Space.World);

            _lastMousePos = currentMousePos;
        }
    }

    // X方向の移動を選択する
    void SelectPos_X()
    {
        GetOffset();
        _isXPosDragging = true;
        _xPos.GetComponent<MeshRenderer>().material = mat_Arrow02;
        _yPos.GetComponent<MeshRenderer>().material = mat_Arrow01;
        _zPos.GetComponent<MeshRenderer>().material = mat_Arrow01;
    }

    // Y方向の移動を選択する
    void SelectPos_Y()
    {
        GetOffset();
        _isYPosDragging = true;
        _yPos.GetComponent<MeshRenderer>().material = mat_Arrow02;
        _xPos.GetComponent<MeshRenderer>().material = mat_Arrow01;
        _zPos.GetComponent<MeshRenderer>().material = mat_Arrow01;
    }

    // Z方向の移動を選択する
    void SelectPos_Z()
    {
        GetOffset();
        _isZPosDragging = true;
        _zPos.GetComponent<MeshRenderer>().material = mat_Arrow02;
        _yPos.GetComponent<MeshRenderer>().material = mat_Arrow01;
        _xPos.GetComponent<MeshRenderer>().material = mat_Arrow01;
    }

    // X軸回転を選択する
    void SelectRot_X()
    {
        _lastMousePos = Mouse.current.position.ReadValue();
        _isXRotDragging = true;
        _xRot.GetComponent<MeshRenderer>().material = mat_Arrow02;
        _yRot.GetComponent<MeshRenderer>().material = mat_Arrow01;
        _zRot.GetComponent<MeshRenderer>().material = mat_Arrow01;
    }

    // Y軸回転を選択する
    void SelectRot_Y()
    {
        _lastMousePos = Mouse.current.position.ReadValue();
        _isYRotDragging = true;
        _yRot.GetComponent<MeshRenderer>().material = mat_Arrow02;
        _xRot.GetComponent<MeshRenderer>().material = mat_Arrow01;
        _zRot.GetComponent<MeshRenderer>().material = mat_Arrow01;
    }

    // Z軸回転を選択する
    void SelectRot_Z()
    {
        _lastMousePos = Mouse.current.position.ReadValue();
        _isZRotDragging = true;
        _zRot.GetComponent<MeshRenderer>().material = mat_Arrow02;
        _yRot.GetComponent<MeshRenderer>().material = mat_Arrow01;
        _xRot.GetComponent<MeshRenderer>().material = mat_Arrow01;
    }

    // ドラッグ開始時の位置差を計算する
    void GetOffset()
    {
        _zOffset = _mainCamera.WorldToScreenPoint(_parentOBJTransform.position).z;
        _dragOffset = _parentOBJTransform.position - _mainCamera.ScreenToWorldPoint(new Vector3(Mouse.current.position.x.ReadValue(), Mouse.current.position.y.ReadValue(), _zOffset));
    }

    //--------------------------------------------------------->

    // 元のマテリアルに戻す
    public void SetOriginalMat()
    {
        for (int i = 0; i < tr_GsonsOBJ.Count; i++)
        {
            tr_GsonsOBJ[i].GetComponent<MeshRenderer>().material = originalMats[i];

            if (!tr_GsonsOBJ[i].GetComponent<Collider>().enabled)
            {
                tr_GsonsOBJ[i].GetComponent<Collider>().enabled = true;
            }
        }
        UIController.Instance.HidePosRotBT();
    }

    // 選択中のマテリアルに変更する
    public void SetSelectedMat()
    {
        for (int i = 0; i < tr_GsonsOBJ.Count; i++)
        {
            tr_GsonsOBJ[i].GetComponent<MeshRenderer>().material = selectedMat;
            tr_GsonsOBJ[i].GetComponent <Collider>().enabled = false;
        }
        UIController.Instance.ShowPosRotBT();
    }

}
