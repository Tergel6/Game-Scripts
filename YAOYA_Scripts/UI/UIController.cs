using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public static UIController Instance { get; private set; }

    [SerializeField] GameObject _topUI;                     // 画面上部のメインUI
    [SerializeField] GameObject _mapUI;                     // マップUI
    [SerializeField] GameObject _settingUI;                 // 設定UI

    [Space]
    [SerializeField] GameObject _bankUI;                    // 銀行UI
    [SerializeField] GameObject _brokerageUI;               // 仲介所UI
    [SerializeField] GameObject _marketUI;                  // 市場UI
    [SerializeField] GameObject _jobUI;                     // 求人UI

    [Space]
    [SerializeField] GameObject _gameCompletedUI;           // ゲームクリアUI
    [SerializeField] TextMeshProUGUI _gameCompletedMoney;
    [SerializeField] TextMeshProUGUI _gameCompletedDays;

    [SerializeField] GameObject _gameOverUI;                // ゲームオーバーUI
    [SerializeField] TextMeshProUGUI _gameOverMoney;
    [SerializeField] TextMeshProUGUI _gameOverDays;

    [SerializeField] GameObject _soldOutUI;                 // 売り切れUI

    [Header("Setting_Confirm")]
    [SerializeField] GameObject _confirmUI;                 // 設定確認UI
    [SerializeField] Button _yesButton;
    [SerializeField] Button _noButton;

    [Header("TipText")]
    [SerializeField] TextMeshProUGUI _tipText;              // 画面下のTip表示

    Animator _topUIAnime;                                   // 上部UIのアニメーション

    //============================ Awake ============================>
    private void Awake()
    {
        Instance = this;
    }

	//============================ Start ============================>
	private void Start()
    {
        _topUIAnime = _topUI.GetComponent<Animator>();

        // ゲーム開始時はマップUIを閉じる
        HideMapUI();

        // Tipを非表示にする
        HidTip();
    }

    //============================ Function ============================>

    // マップUIの開閉ボタン
    public void ClickMapUI()
    {
        if (_mapUI.activeSelf)
        {
            HideMapUI();
        }
        else
        {
            _mapUI.SetActive(true);

            // 店の棚選択を無効化（マップを開いている間は棚操作できない）
            if (ShopBoxManager.Instance != null)
            {
                ShopBoxManager.Instance.DisableBoxesSelect();
                ShopBoxManager.Instance.ResetMaterials();
            }
            HideSettingUI();
        }
    }

    // マップUIを閉じる
    void HideMapUI()
    {
        if (_mapUI.activeSelf)
        {
            _mapUI.SetActive(false);

            // マップを閉じたら他のUIも閉じる
            HideBankUI();
            HideBrokerageUI();
            HideMarketUI();
            HideJobUI();

            // 棚選択を再び有効化
            if (ShopBoxManager.Instance != null)
            {
                ShopBoxManager.Instance.EnabledBoxesSelect();
            }
        }
    }

    // 設定UIの開閉ボタン
    public void ClickSettingUI()
    {
        if (_settingUI.activeSelf)
        {
            HideSettingUI();
        }
        else
        {
            _settingUI.SetActive(true);
            HideMapUI();
            if (ShopBoxManager.Instance != null)
            {
                ShopBoxManager.Instance.ResetMaterials();
            }

            // 設定画面中はゲームを一時停止
            Time.timeScale = 0;
        }
    }

    // 設定UIを閉じる
    void HideSettingUI()
    {
        _settingUI.SetActive(false);

        // ゲーム再開
        Time.timeScale = 1;
    }
    //---------------------------------------->

    // 各UIの表示・非表示（銀行・仲介所・市場・求人）
    public void ShowBankUI()
    {
        _bankUI.SetActive(true);
    }
    public void HideBankUI()
    {
        if (_bankUI.activeSelf)
        {
            _bankUI.SetActive(false);
        }
    }
    public void ShowBrokerageUI()
    {
        _brokerageUI.SetActive(true);
    }
    public void HideBrokerageUI()
    {
        if (_brokerageUI.activeSelf)
        {
            _brokerageUI.SetActive(false);
        }
    }
    public void ShowMarketUI()
    {
        _marketUI.SetActive(true);
    }
    public void HideMarketUI()
    {
        if (_marketUI.activeSelf)
        {
            _marketUI.SetActive(false);
        }
    }
    public void ShowJobUI()
    {
        _jobUI.SetActive(true);
    }
    public void HideJobUI()
    {
        if (_jobUI.activeSelf)
        {
            _jobUI.SetActive(false);
        }
    }

    //--------------------------------------->

    // ゲームクリアUIを表示する
    public void ShowGameCompletedUI()
    {
        _gameCompletedMoney.SetText(PlayerData.Instance.GetPlayerMoney().ToString() + "$");
        _gameCompletedDays.SetText(GameTime.Instance.GetDaysPassed());
        _gameCompletedUI.SetActive(true);
        Time.timeScale = 0;
    }
    // ゲームクリアUIを非表示する
    public void HideGameCompletedUI()
    {
        _gameCompletedUI.SetActive(false);
        Time.timeScale = 1;
    }
    // ゲーム終了UIを表示する
    public void ShowGameOverUI()
    {
        _gameOverMoney.SetText(PlayerData.Instance.GetPlayerMoney().ToString() + "$");
        _gameOverDays.SetText(GameTime.Instance.GetDaysPassed());
        _gameOverUI.SetActive(true);
        Time.timeScale = 0;
    }
    // ゲーム終了UIを非表示する
    public void HideGameOverUI()
    {
        _gameOverUI.SetActive(false);
        Time.timeScale = 1;
    }

    // 売り切れUI
    public void ShowSoldOutUI()
    {
        _soldOutUI.SetActive(true);
        Time.timeScale = 0;
    }
    public void HideSoldOutUI()
    {
        _soldOutUI.SetActive(false);
        Time.timeScale = 1;
    }
    //------------------------------------------------>

    // 設定確認UI
    public void ShowSettingConfirm()
    {
        _confirmUI.SetActive(true);
    }

    // 上部UIのアニメーション
    public void HideSettingConfirm()
    {
        _confirmUI.SetActive(false);
    }

    //-------------------------------------------------->

    //上のUIが出てくるアニメ
    public void ShowTopUIAnime()
    {
        _topUIAnime.SetBool("IsShow",true);
    }

    //上のUIがなくなるアニメ
    public void HidTopUIAnime()
    {
        _topUIAnime.SetBool("IsShow", false);
    }

    //--------------------------------------------------->

    // Tipを表示（2秒後に自動で消える）
    public void ShowAndSetTip(string tip)
    {
        _tipText.text = tip;
        _tipText.enabled = true;
        Invoke("HidTip", 2f);
    }
    public void HidTip()
    {
        _tipText.enabled=false;
    }

    //-------------------------------------------------->

    // シーンを遅延ロードする
    public void DelayLoadSceneFunc()
    {
        StartCoroutine(DelayLoadScene(1f));
    }

    IEnumerator DelayLoadScene (float seconds)
    {
        yield return new WaitForSeconds(seconds);
        SceneManager.LoadScene("SampleScene");
    }
}
