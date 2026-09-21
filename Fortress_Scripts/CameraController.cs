using CartoonFX;
using TMPro;
using UnityEngine;
public class CameraController : MonoBehaviour
{
    [Header("Drag")]
    [SerializeField] float _dragSpeed;      // カメラをドラッグするときの移動速度
    [Header("Zoom")]
    [SerializeField] float _zoomSpeed;      // ズームの速度
    [SerializeField] float _minZoom;        // ズームの最小値
    [SerializeField] float _maxZoom;        // ズームの最大値

    // カメラの移動範囲（X・Y・Zの制限）
    float _minX = -10f, _maxX = 10f;
    float _minY = -10f, _maxY = 10f;
    float _minZ = -12f, _maxZ = 0.7f;
    Vector3 _dragOrigin;                    // ドラッグ開始位置（マウスの位置）

    Transform _transform;                   // オブジェクトの位置・回転・スケールを管理するためのTransform

    //================================ Start =================================>

    private void Start()
    {
        _transform = this.transform;
    }
    //================================ Update =================================>
    void Update()
    {
        // マウスホイールでズーム操作を行う
        if (Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");

            // ズーム値を変更する
            Camera.main.orthographicSize -= scroll * _zoomSpeed;

            // ズーム範囲を制限する
            Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize, _minZoom, _maxZoom);
        }
        //--------------------------->

        // 中ボタンを押した瞬間にドラッグ開始位置を記録する
        if (Input.GetMouseButtonDown(2))
        {
            _dragOrigin = Input.mousePosition;
            return;
        }

        // 中ボタンを押していない場合はドラッグ処理をしない
        if (!Input.GetMouseButton(2))
        {
            return;
        }

        // マウスの移動量を計算する
        Vector3 difference = Input.mousePosition - _dragOrigin;
        _dragOrigin = Input.mousePosition;

        // カメラをドラッグ方向に移動させる
        _transform.position += new Vector3(-difference.x * _dragSpeed * Time.deltaTime, 0, -difference.y * _dragSpeed * Time.deltaTime);

        // カメラの位置を指定範囲内に制限する
        _transform.position = new Vector3(Mathf.Clamp(_transform.position.x, _minX, _maxX), Mathf.Clamp(_transform.position.y, _minY, _maxY), Mathf.Clamp(_transform.position.z, _minZ, _maxZ));

    }
}
