
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    static Camera _mainCamera;					// メインカメラを保存する

    public NewActions inputAciton;				// Input System のアクション

    [SerializeField] GameObject[] _objs;		// すべての操作可能オブジェクト

    Ray _ray;									// レイキャスト用のレイ
    RaycastHit _hit;							// レイが当たった情報

    GameObject _targetOBJ;						// 選択中のオブジェクト

    //================================== Awake ==================================>
    void Awake()
	{
        // Input System を初期化する
        inputAciton = new NewActions();
	}

    // アクションを有効にする
    void OnEnable()
	{
		inputAciton.Enable();
	}

    // アクションを無効にする
    void OnDisable()
	{
		inputAciton.Disable();
	}

	//================================== Start ==================================>
	void Start()
	{

        // 重力を設定
        Physics.gravity = new Vector3(0, -10, 0);
        // メインカメラ取得
        _mainCamera = Camera.main;

	}

	//================================== Update ==================================>
	void Update()
    {
		// オブジェクト選択をチェックする
        if (inputAciton.Object.Click.triggered)
		{
			_ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
			if (Physics.Raycast(_ray, out _hit))
			{
                // UIの上ではなく、矢印でもない場所をクリックした時
                if (!EventSystem.current.IsPointerOverGameObject() && !_hit.transform.CompareTag("Arrow"))
				{
					foreach (GameObject obj in _objs)
					{
						obj.transform.parent.parent.parent.GetComponent<ObjectController>().SetOriginalMat();
						obj.transform.parent.parent.parent.GetComponent<ObjectController>().DisableRot();
						obj.transform.parent.parent.parent.GetComponent<ObjectController>().DisablePos();
						_targetOBJ = null;
					}
				}
                // GObjects をクリックした時 → 選択状態にする
                if (_hit.transform.CompareTag("GObjects"))
				{
					_hit.transform.parent.parent.parent.parent.GetComponent<ObjectController>().SetSelectedMat();
					_targetOBJ = _hit.transform.parent.parent.parent.parent.gameObject;
				}

			}
		}
	}

    //================================== Function ==================================>

    // 選んだオブジェクトを移動モードにする
    public void OnPosition()
	{
		if (_targetOBJ != null)
		{
			_targetOBJ.GetComponent<ObjectController>().OnPosition();
		}

	}
    // 選んだオブジェクトを回転モードにする
    public void OnRotation()
	{
		if (_targetOBJ != null)
		{
			_targetOBJ.GetComponent<ObjectController>().OnRotation();
		}
	}

    // 全オブジェクトの矢印と選択状態をリセットする
    public void HideArrow_SetMat()
	{
		foreach (GameObject obj in _objs)
		{
			obj.transform.parent.parent.parent.GetComponent<ObjectController>().SetOriginalMat();
			obj.transform.parent.parent.parent.GetComponent<ObjectController>().DisablePos();
			obj.transform.parent.parent.parent.GetComponent<ObjectController>().DisableRot();
		}
	}

    //------------------------------------------------>
    // ゲームを一時停止する
    public static void PauseGame()
	{
		Time.timeScale = 0;
		_mainCamera.GetComponent<CameraController>().OnDisable();
	}

    // ゲームを再開する
    public static void ContinueGame()
	{
		Time.timeScale = 1;
		_mainCamera.GetComponent<CameraController>().OnEnable();
	}

    // ゲームをリスタートする
    public static void RestartGame()
	{
		AudioController.instance.StopBGM();
		SceneManager.LoadScene("GamePlay");
		ContinueGame();
	}

    // メインメニューへ戻る
    public static void ToMainMenu()
	{
		AudioController.instance.StopBGM();
		SceneManager.LoadScene("MainMenu");
	}
}
