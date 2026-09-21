
using System;
using UnityEngine;

[Serializable]
public struct PopulationComposition
{
    public float elderlyPeople;     // ‚—îÒ‚ÌŠ„‡
    public float officeWorkers;     // ‰ïĞˆõ‚ÌŠ„‡
    public float houseWives;        // å•w‚ÌŠ„‡
    public float eliteClass;        // •x—T‘w‚ÌŠ„‡
}
[Serializable]
public struct Weather
{
    public float sunny;             // °‚ê‚Ì“ú‚ÌŠ„‡
    public float rain;              // ‰J‚Ì“ú‚ÌŠ„‡
    public float typhoon;           // ‘ä•—‚Ì“ú‚ÌŠ„‡
    public float snow;              // á‚Ì“ú‚ÌŠ„‡
    public float highTemperature;   // ‚‰·‚Ì“ú‚ÌŠ„‡
    public float windy;             // ‹­•—‚Ì“ú‚ÌŠ„‡
}
[Serializable]
public struct Week
{
    public float monday;            // Œ—j“ú‚Ì—ˆ‹qŒXŒü
    public float tuesday;           // ‰Î—j“ú‚Ì—ˆ‹qŒXŒü
    public float wednesday;         // …—j“ú‚Ì—ˆ‹qŒXŒü
    public float thursday;          // –Ø—j“ú‚Ì—ˆ‹qŒXŒü
    public float friday;            // ‹à—j“ú‚Ì—ˆ‹qŒXŒü
    public float saturday;          // “y—j“ú‚Ì—ˆ‹qŒXŒü
    public float sunday;            // “ú—j“ú‚Ì—ˆ‹qŒXŒü
}
[Serializable]
public struct TimePeriod
{
    public float from10to12;        // 10?12‚Ì—ˆ‹qŒXŒü
    public float from12to14;        // 12?14‚Ì—ˆ‹qŒXŒü
    public float from14to16;        // 14?16‚Ì—ˆ‹qŒXŒü
    public float from16to18;        // 16?18‚Ì—ˆ‹qŒXŒü
    public float from18to20;        // 18?20‚Ì—ˆ‹qŒXŒü
}
[Serializable]
public struct StaffSkill
{
    public float _aaa;              // ƒXƒ^ƒbƒt”\—ÍA
    public float _bbb;              // ƒXƒ^ƒbƒt”\—ÍB
    public float _ccc;              // ƒXƒ^ƒbƒt”\—ÍC
}

[CreateAssetMenu(fileName ="NewArea",menuName ="Map/Area")] 
public class AreasData : ScriptableObject
{
    public string _areaType;                                // ƒGƒŠƒA‚Ìí—Ş
    public int _populationCount;                            // ƒGƒŠƒA‚ÌlŒû”

    public PopulationComposition _populationComposition;    // lŒû\¬ƒf[ƒ^
    public Weather _weather;                                // “V‹C‚É‚æ‚é—ˆ‹qŒXŒü
    public Week _week;                                      // —j“ú‚É‚æ‚é—ˆ‹qŒXŒü
    public TimePeriod _timePeriod;                          // ŠÔ‘Ñ‚É‚æ‚é—ˆ‹qŒXŒü
    public StaffSkill _staffskill;                          // ƒXƒ^ƒbƒt‚Ì”\—Íƒf[ƒ^

    [Space]
    public float _payDay;                                   // ‹‹—¿“ú‚É‚æ‚é‰e‹¿’l
    public float _festival;                                 // Õ‚èEƒCƒxƒ“ƒg‚Ì‰e‹¿’l

}
