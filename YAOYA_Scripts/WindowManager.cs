using UnityEngine;

public class StartupResolution : MonoBehaviour
{
    void Start()
    {
        // 現在の画面サイズを取得（Windows の実際の解像度）
        int w = Screen.currentResolution.width;
        int h = Screen.currentResolution.height;

        // 竖屏基准分辨率
        float baseW = 1080f;
        float baseH = 1920f;

        // 画面に収まるように縮小率を計算
        float scale = Mathf.Min(w / baseW, h / baseH);

        // 最終的なウィンドウサイズを設定（縦画面）
        Screen.SetResolution(
            (int)(baseW * scale),
            (int)(baseH * scale),
            false // ウィンドウモード
        );
    }
}