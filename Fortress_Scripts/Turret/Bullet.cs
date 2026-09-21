
using Unity.VisualScripting;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] float _range;          // 敵を追尾し続けるための距離（追尾更新範囲）
    [SerializeField] float _deleteRange;    // 弾を削除する距離（敵に到達したと判断する範囲）

    [SerializeField] float _bulletSpeed;    // 弾の移動速度
    [SerializeField] float _bulletDamage;   // 弾が与えるダメージ量

    GameObject _targetEnemy;                // 目標敵オブジェクト
    Vector3 _targetEnemyPos;                // 敵の位置を記録しておくための変数

    AudioSource _fireSound;                 // 発射音を再生するための AudioSource
    Vector3 _thisPos;

    //================================ Awake =================================>
    void Awake()
    {
        // 弾オブジェクトに付いている AudioSource を取得する
        _fireSound = GetComponent<AudioSource>();
    }

    //================================ Start =================================>
    void Start()
    {
        _thisPos=transform.position;
        //// 他の弾との衝突を無視する設定
        //// 弾同士がぶつかって物理挙動が乱れないようにする
        //GameObject[] bullets = GameObject.FindGameObjectsWithTag("Bullet");
        //foreach (GameObject bullet in bullets)
        //{
        //    Physics.IgnoreCollision(GetComponent<Collider>(), bullet.GetComponent<Collider>());
        //}
        // 発射時点での敵の位置を記録する
        // 敵が動いても、一定距離まではこの位置に向かって進む
        _targetEnemyPos = _targetEnemy.transform.position;

        
    }

    // 弾が当たった場合の処理
    void OnTriggerEnter(Collider other)
    {
        // 弾が敵に当たった場合、敵にダメージを与える
        if (other.gameObject.transform.CompareTag("Enemy"))
        {
            other.gameObject.GetComponent<EnemyController>().TakeDamage(_bulletDamage);
            BuildManager.Instance.ReleaseBullet(this.gameObject);
        }
    }

    //================================ Update =================================>
    void Update()
    {
        _thisPos = transform.position;
        _targetEnemyPos = _targetEnemy.transform.position;
        // 敵が存在し、また一定距離以内なら追尾位置を更新する
        // sqrMagnitude は高速な距離判定（距離の二乗）
        if ( ((_thisPos - _targetEnemyPos).sqrMagnitude) < _range)
        {
            _targetEnemyPos = _targetEnemy.transform.position;
            // 目標敵の位置に向けて弾の向く
            transform.LookAt(_targetEnemyPos);
            // 弾を前方向へ移動させる（LookAt の方向に進む）
            transform.Translate(Vector3.forward * _bulletSpeed * Time.deltaTime);
        }
        if(_targetEnemy.gameObject.activeSelf==false ||((_thisPos - _targetEnemy.transform.position).sqrMagnitude) >_range)
        {
            BuildManager.Instance.ReleaseBullet(this.gameObject);
        }
    }

    //================================ Function =================================>

    // 発射元から指定された敵をターゲットとして設定するメソッド
    public void TakeTarget(GameObject targetEnemyFor)
    {
        // 発射元から指定された敵をターゲットとして設定する
        _targetEnemy = targetEnemyFor;
    }

    // 弾の発射音を再生するメソッド
    public void PlayFireSound()
    {
        // 弾の発射音を再生する
        _fireSound.PlayOneShot(_fireSound.clip);
    }
}
