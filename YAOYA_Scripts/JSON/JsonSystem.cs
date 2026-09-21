using System.IO;
using UnityEngine;
public class JsonSystem : MonoBehaviour
{

    //JSONファイルを作成する
    public static void SaveByJson(string saveFileName, object data)   
    {
        // オブジェクトを JSON 形式に変換
        var jsonData = JsonUtility.ToJson(data,true);

        // 保存先のパスを作成する
        var path = Path.Combine(Application.persistentDataPath, saveFileName);

        try
        {
            // JSONデータを書き込む
            File.WriteAllText(path, jsonData);

            Debug.Log("save to:" + path);
        }
        catch (System.Exception e)
        {
            Debug.Log($"is:{path}.\n{e}");
        }
    }


    //JSONファイルを読み込む
    public static T LoadFromJson<T>(string saveFileName)    
    {
        var path = Path.Combine(Application.persistentDataPath, saveFileName);

        try
        {
            // JSONファイルを読み込む
            var jsonData = File.ReadAllText(path);

            // JSON → オブジェクトに変換
            var data = JsonUtility.FromJson<T>(jsonData);
            return data;
        }
        catch (System.Exception e)
        {
            Debug.Log($"read{path}.\n{e}");
            // 読み込み失敗時はデフォルト値を返す
            return default;
        }

    }

    //JSONファイルを削除する
    public static void DeleteSaveFile(string saveFileName)   
    {
        var path = Path.Combine(Application.persistentDataPath, saveFileName);
        try
        {
            File.Delete(path);
        }
        catch (System.Exception e)
        {
            Debug.Log($"file{path}.\n{e}");
        }

    }
}

