using UnityEngine;

public class ScaneBackGroundManager : MonoBehaviour
{
    public static ScaneBackGroundManager Instance { get; private set; }

    [SerializeField] GameObject[] _backGrounds;   //”wŒi

    //============================== Awake ==========================>
    private void Awake()
    {
        Instance = this;
    }

    //============================== Start ==========================>
    private void Start()
    {
        //ƒQ[ƒ€‚ªn‚Ü‚é‰Æ‚Ì”wŒi‚ğ•\¦‚·‚é
        ShowBackGround("Home");

    }
    //========================================================>

    // w’è‚³‚ê‚½ƒGƒŠƒA–¼‚Ì”wŒi‚ğ•\¦‚·‚é
    public void ShowBackGround(string areaName)
    {
        switch (areaName) 
        {
            case "Home": GetBackGround(0);
                    break;
            case "SchoolArea":
                GetBackGround(1);
                break;
            case "ResidentialArea":
                GetBackGround(2);
                break;
            default: 
                Debug.LogError("ShowBackGround Is Error");
                break;
        }
    }

    // ”wŒi‚ğØ‚è‘Ö‚¦‚éi‚·‚×‚Ä”ñ•\¦ ¨ w’è”Ô†‚¾‚¯•\¦j
    void GetBackGround(int num)
    {
        // ‚·‚×‚Ä‚Ì”wŒi‚ğ”ñ•\¦‚É‚·‚é
        foreach (var backGround in _backGrounds)
        {
            backGround.gameObject.SetActive(false);
        }

        // w’è‚³‚ê‚½”wŒi‚¾‚¯•\¦‚·‚é
        _backGrounds[num].SetActive(true);
    }
}
