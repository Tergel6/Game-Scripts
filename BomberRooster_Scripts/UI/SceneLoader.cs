using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    static readonly int isFadeInHash = Animator.StringToHash("isFadeIn");       // 暗いから明るくなるアニメの名前をIDに変える
    static readonly int isFadeOutHash = Animator.StringToHash("isFadeOut");     // 明るいから暗くなるアニメの名前をIDに変える
    public static SceneLoader instance { get; private set; }

    [SerializeField] Animator _loadSceneAnime;                                  // シーンを読み込むに使う動画

    //========================== Start ===========================>
    void Start()
    {
        instance = this;
    }

    //========================== Function ===========================>

    //　シーン名通りシーンを読み込む
    public void LoadScene(string sceneName)
    {
        //　シーンを読み込む
        StartCoroutine(LoadSceneIE(sceneName));
    }

    //　シーンを読み込むアニメを実行する
    IEnumerator LoadSceneIE(string sceneName)
    {
        // シーンが暗いから明るくなるアニメを禁止
        _loadSceneAnime.SetBool(isFadeInHash, false);
        // シーンが明るいから暗くなるアニメを実行
        _loadSceneAnime.SetBool(isFadeOutHash, true);

        //　1秒待つ
        yield return new WaitForSeconds(1);
        //　シーンを読み込む
        SceneManager.LoadScene(sceneName);

        // シーンが暗いから明るくなるアニメを実行
        _loadSceneAnime.SetBool(isFadeInHash, true);
        // シーンが明るいから暗くなるアニメを禁止
        _loadSceneAnime.SetBool(isFadeOutHash, false);
    }
}
