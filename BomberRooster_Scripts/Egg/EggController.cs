using UnityEngine;

public class EggController : MonoBehaviour
{
    [Header("Change Egg Materials Condition")]
	[SerializeField] int _countCondition;           //タマゴのテクスチャをチェンジするタマゴ数の条件
    [SerializeField] int _rangeCondition;           //タマゴのテクスチャをチェンジする爆発距離の条件

    [Header("Egg Materials")]
	[SerializeField] Texture2D _color_Original;     // タマゴ元のテクスチャ
	[SerializeField] Texture2D _color_Count;        // プレイヤータマゴ数のスキルが一定の条件になる時変わるタマゴのテクスチャ
	[SerializeField] Texture2D _color_Range;        // プレイヤータマゴ爆発距離のスキルが一定の条件になる時変わるタマゴのテクスチャ
	[SerializeField] Texture2D _color_CR;           // プレイヤータマゴ数と爆発距離のスキルが一定の条件になる時変わるタマゴのテクスチャ

	[SerializeField] Renderer _renderer;			// 自身の「Renderer」
    [SerializeField] float _boomTime;               // タマゴの爆発時間

    GameObject _bombFX;                             // 爆発エフェクト

	Transform _transform;                           // 自身の「Transform」

    bool _isSpawnBomb = true;                       // タマゴを置いたのか確認

	//======================== Start =======================>
	void Start()
	{
        // 自身の「transform」を表明
        _transform = transform;
	}

	//======================== Update =======================>
	void Update()
	{
        //　タマゴを置いたら
        if (_isSpawnBomb)
		{
            //　タマゴを置いてないにする
            _isSpawnBomb = false;
            //　タマゴの爆発時間になったら爆発始まる
            Invoke(nameof(BombBoom), _boomTime);
		}
	}

	//======================== Function =======================>

	// タマゴが爆発する
	public void BombBoom()
	{
        //　タマゴがアクティブかどうかと爆発エフェクトがないかの確認
        if (gameObject.activeSelf && _bombFX == null)
		{
			//　タマゴの数を増やす
			PlayerController.instance.EggCountUp();
			//　タマゴをプールに戻す
			PoolManager.instance._eggPool.Release(gameObject);
			//　爆発し始まる
			StartBoom();
            // 「タマゴが爆発音」をプレー
            AudioController.instance.PlayBombBoomSound();
            //　「タマゴを置いた」と設定
            _isSpawnBomb = true;
		}
	}

	// タマゴが爆発スタート
	void StartBoom()
	{
		// 中心の爆発
		_bombFX = PoolManager.instance._boomFXPool.Get();
		_bombFX.transform.position = _transform.position;

		// 四方向の爆発
		BoomEffect(Vector3.forward);    //上
		BoomEffect(Vector3.back);       //下
		BoomEffect(Vector3.left);       //左
		BoomEffect(Vector3.right);      //右
	}

	// ４っつの方向に爆発する
	void BoomEffect(Vector3 dir)
	{
		//　爆弾エフェクトのプールを表明
		var boomFXPool = PoolManager.instance._boomFXPool;

        //　タマゴの中心を基点に、距離に応じて爆発する
        for (int i = 1; i <= PlayerController.instance._skill.bombRange; i++)
		{
			//　プールから爆発エフェクトを読み出す
			GameObject newBombFX = boomFXPool.Get();
            //　爆発エフェクトの位置を設定
            newBombFX.transform.position = _transform.position + 0.4f * i * dir;
		}
	}

	// タマゴのテクスチャをチェンジする
	public void ChangeEggColor()
	{
		//	新しいてマテリアルの色を表明する
		Texture2D newTexture= _color_Original;

		//　プレイヤーのタマゴの数を表明する
		int newEggCount = GamePlayUIController.instance.uiEggCount;
        //　プレイヤーの爆発距離を表明する
        float newBombRange = PlayerController.instance._skill.bombRange;

        /*
		 条件に合わせち新しいマテリアルの色を設定する
		 */
        if (newEggCount < _countCondition && newBombRange < _rangeCondition)
		{
			newTexture = _color_Original;
        }
		else if (newEggCount >= _countCondition && newBombRange < _rangeCondition)
		{
            newTexture = _color_Range;
		}
		else if (newEggCount < _countCondition && newBombRange >= _rangeCondition)
		{
            newTexture = _color_Count;
		}
		else if (newEggCount >= _countCondition && newBombRange >= _rangeCondition)
		{
            newTexture = _color_CR;
		}

        // タマゴのテクスチャを新しいてマテリアルの色チェンジする
        _renderer.material.SetTexture("_BaseMap", newTexture);
    }
}
