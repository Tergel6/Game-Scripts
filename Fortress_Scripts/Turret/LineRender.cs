using UnityEngine;

public class LineRender : MonoBehaviour
{
	[SerializeField] int _segments;		// 円を描くための分割数（多いほど滑らかになる）
    [SerializeField] float _radius;     // 円の半径
    [SerializeField] float _lineWidth;  // 線の太さ
    LineRenderer _line;                 // 円を描画するための LineRenderer

    //================================ Start =================================>
    void Start()
	{
        // LineRenderer を動的に追加する
        _line = gameObject.AddComponent<LineRenderer>();

        // 円を描くための頂点数
        _line.positionCount = _segments + 1;

        // 円を閉じる設定
        _line.loop = true;

        // 線の太さを設定
        _line.widthMultiplier = _lineWidth;

        // マテリアルを作成して白色に設定（URP 用）
        Material mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
		mat.color = Color.white;
		_line.material = mat;
	}

	//================================ Update =================================>
	void Update()
	{
        // 毎フレーム円を描き直す
        DrawCircle();
	}

	// Update is called once per frame
	void DrawCircle()
	{
        // 円周上の各点を計算して LineRenderer に設定する
        for (int i = 0; i <= _segments; i++)
		{
            // 円周角度
            float angle = i * (2 * Mathf.PI / _segments);

            // x,z 座標を計算（円の方程式）
            float x = Mathf.Cos(angle) * _radius;
			float z = Mathf.Sin(angle) * _radius;

            // 円の中心位置（transform.position）を加算してワールド座標にする
            _line.SetPosition(i, new Vector3(x, 0, z) + transform.position);

		}
	}


}
