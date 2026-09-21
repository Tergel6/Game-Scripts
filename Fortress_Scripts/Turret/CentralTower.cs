using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CentralTower : MonoBehaviour
{
    public static CentralTower Instance { get; private set; }

    [SerializeField] GameObject _upgradeDestroyUI;              // アップグレードの UI
    [SerializeField] TextMeshProUGUI _upgradeDestroyUIText;     // UI に表示するテキスト
    [SerializeField] GameObject _hPSlider;                      // HP を表示するスライダー UI
    [SerializeField] int _deadHP;                               // タワーが破壊される HP の閾値
    [SerializeField] int _upHP;                                 // アップグレード時に増加する HP 量
    [Space]
    [SerializeField] GameObject _cube1;                         // タワーの見た目（レベル1）
    [SerializeField] GameObject _cube2;                         // タワーの見た目（レベル2）
    [SerializeField] GameObject _cube3;                         // タワーの見た目（レベル3）
    [Space]
    [SerializeField] float _centralTowerMaxHP;                  // タワーの最大 HP
    [SerializeField] float _centralTowerHP;                     // 現在の HP
    [SerializeField] int _upgradeMoney;                         // アップグレードに必要な金額

    bool _buttonSwitch = false;                                 // UI の ON/OFF 切り替え用フラグ

    //================================ Awake =================================>
    void Awake()
    {
        Instance = this;
    }

    //================================ Start =================================>
    void Start()
    {
        // アップグレード UI を初期状態で非表示にする
        if (_upgradeDestroyUI != null)
        {
            _upgradeDestroyUI.SetActive(false);
        }

        // UI に必要コストを表示する
        _upgradeDestroyUIText.SetText("Upgrade" + _upgradeMoney.ToString());
    }

    //================================ Update =================================>
    void Update()
    {
        // 左クリックしたとき、タワー以外をクリックしたら UI を閉じる
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                // UI とタワー以外をクリックしたら UI を閉じる
                if (_upgradeDestroyUI != null && hit.transform.gameObject != gameObject && hit.transform.gameObject != _upgradeDestroyUI)
                {
                    _upgradeDestroyUI.SetActive(false);
                }
            }
        }

        // HP スライダーの表示更新
        _hPSlider.GetComponent<Slider>().maxValue = _centralTowerMaxHP;
        _hPSlider.GetComponent<Slider>().value = _centralTowerHP;

        //------------------GameOver--------------------->

        // タワーのHPが爆発HPになったら起こる処理
        if (_centralTowerHP <= _deadHP)
        {
            // 爆発エフェクト再生
            BuildManager.Instance.PlayTurretBoomFX(gameObject.transform.position);
            // ゲームオーバー処理
            GameController.Instance.IsGameOver();
            // タワーを削除
            Destroy(this.gameObject);
            // UI にゲームオーバー画面を表示
            UI_Manager.Instance.ShowGameOver();
        }
        //----------------------------------------------->

    }

    //================================ Function =================================>
    // タワーをクリックしたときのメソッド
    public void OnMouseDown()
    {
        // UI の表示/非表示を切り替える
        if (_upgradeDestroyUI != null)
        {
            switch (_buttonSwitch)
            {
                case false:
                    _upgradeDestroyUI.SetActive(true);
                    _buttonSwitch = true;
                    break;
                case true:
                    _upgradeDestroyUI.SetActive(false);
                    _buttonSwitch = false;
                    break;
            }
        }
    }

    // タワーがダメージを受ける処理
    public void CentralTowerTakeDamage(float DanageHP)
    {
        // HP がまだ破壊ラインより上ならダメージを受ける
        if (_centralTowerHP > _deadHP)
        {
            _centralTowerHP -= DanageHP;
        }
    }

    // タワーアップグレード時HPの処理
    public void CentralTowerUPHP()
    {
        // HP と最大 HP を増加させる（アップグレード効果）
        _centralTowerHP += _upHP;
        _centralTowerMaxHP += _upHP;
        // 次のアップグレードコストを倍にする
        _upgradeMoney += _upgradeMoney;
        // UI の表示を更新
        _upgradeDestroyUIText.SetText("Upgrade" + _upgradeMoney.ToString());
    }

    //----------------Upgrade--------------->

    // タワーアップグレード時の処理
    public void CentralUpgrade()
    {
        // 所持金が足りる場合
        if (_upgradeMoney <= UI_Manager.Instance.GetMyMoney())
        {
            // お金を消費
            UI_Manager.Instance.UseMoney(_upgradeMoney);

            // タワーの攻撃範囲を拡大
            CentralLineRender.Instance.RangeUp();

            // HP をアップグレード
            CentralTowerUPHP();

            // タワーの見た目を段階的に変える
            if (_cube1.gameObject.activeSelf && !_cube2.gameObject.activeSelf)
            {
                _cube2.gameObject.SetActive(true);
            }
            else if (_cube2.gameObject.activeSelf && !_cube3.gameObject.activeSelf)
            {
                _cube3.gameObject.SetActive(true);
            }
        }

        // 所持金が足りない場合
        else if (_upgradeMoney > UI_Manager.Instance.GetMyMoney())
        {
            GameObject _myMoney = GameObject.Find("MyMoney");
            TextMeshProUGUI tmp = _myMoney.GetComponent<TextMeshProUGUI>();

            // お金が足りないアニメが演出
            _myMoney.GetComponent<Animator>().SetTrigger("NoMoney");
        }

    }
}
