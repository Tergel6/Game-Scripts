using UnityEngine;

public class BoomEffectController : MonoBehaviour
{
    [SerializeField] float _damage;         // 爆弾が与えるダメージ

    //======================== Start =======================>
    void Start()
    {
        // 爆発スタート
        gameObject.SetActive(true);

        // 0.1秒あと爆発が終わる
        Invoke(nameof(NoActive), 0.1f);
    }

    //======================== Collider =======================>

    //　当たった時の処理
    void OnTriggerEnter(Collider other)
    {
        //　当たったモノをタグで判断する
        switch (other.tag)
        {
            //　「Egg」だったらEggが爆発する
            case "Egg":
                other.GetComponent<EggController>().BombBoom();
                break;
            //　「Player」だったらプレイヤーが死ぬ
            case "Player":
                other.GetComponent<PlayerController>(). PlayerDie();
                break;
            //　「Enemy」だったら敵が傷つく
            case "Enemy":
                other.GetComponent<EnemyController>().InJured(_damage);
                break;
        }
    }

    //======================== Function =======================>

    // 爆発が終わる
    void NoActive()
    {
        // 爆発を禁止
        gameObject.SetActive(false);
    }
}
