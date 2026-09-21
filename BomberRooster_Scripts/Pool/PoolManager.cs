using UnityEngine;
using UnityEngine.Pool;

public class PoolManager : MonoBehaviour
{
    static public PoolManager instance { get; private set; } 

    [SerializeField] GameObject _eggPre;            //タマゴプレハブ
    [SerializeField] GameObject _boomFXPre;         //タマゴ爆発エフェクトプレハブ
    [SerializeField] GameObject _enemy_01Pre;       //敵_01のプレハブ
    [SerializeField] GameObject _enemy_02Pre;       //敵_02のプレハブ
    [SerializeField] GameObject _enemy_03Pre;       //敵_03のプレハブ

    public ObjectPool<GameObject> _eggPool;         //タマゴプール
    public ObjectPool<GameObject> _boomFXPool;      //タマゴ爆発エフェクトプール
    public ObjectPool<GameObject> _enemy_01Pool;    //敵_01のプール
    public ObjectPool<GameObject> _enemy_02Pool;    //敵_02のプール
    public ObjectPool<GameObject> _enemy_03Pool;    //敵_03のプール

    [Space]
    Transform _transform;   //生成オブジェクトの親

    //======================== Awake =======================>
    void Awake()
    {
        instance = this;

        //　自身のtransformを指定
        _transform = transform;

        /*
各自のプールの表明
 */
        _eggPool = new ObjectPool<GameObject>(createEggFunc, actionOnGet, actionOnRelease, actionOnDestroy, true, 3, 10);
        _boomFXPool = new ObjectPool<GameObject>(createboomEffectFunc, actionOnGet, actionOnRelease, actionOnDestroy, true, 4, 10);
        _enemy_01Pool = new ObjectPool<GameObject>(createEnemy_01Func, actionOnGet, actionOnRelease, actionOnDestroy, true, 25, 50);
        _enemy_02Pool = new ObjectPool<GameObject>(createEnemy_02Func, actionOnGet, actionOnRelease, actionOnDestroy, true, 25, 50);
        _enemy_03Pool = new ObjectPool<GameObject>(createEnemy_03Func, actionOnGet, actionOnRelease, actionOnDestroy, true, 25, 50);
    }

    //======================== Function =======================>
    GameObject createEggFunc()
    {
        return Instantiate(_eggPre, _transform);
    }
    GameObject createboomEffectFunc()
    {
        return Instantiate(_boomFXPre, _transform);
    }
    GameObject createEnemy_01Func()
    {
        return Instantiate(_enemy_01Pre, _transform);
    }
    GameObject createEnemy_02Func()
    {
        return Instantiate(_enemy_02Pre, _transform);
    }
    GameObject createEnemy_03Func()
    {
        return Instantiate(_enemy_03Pre, _transform);
    }

    void actionOnGet(GameObject _object)
    {
        _object.SetActive(true);
    }
    void actionOnRelease(GameObject _object)
    {
        _object.SetActive(false);
    }
    void actionOnDestroy(GameObject _object)
    {
        if (_object != null)
        {
            Destroy(_object);
        }
    }
}
