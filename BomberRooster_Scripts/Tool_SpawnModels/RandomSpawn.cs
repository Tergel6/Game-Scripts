using System.Collections.Generic;
using UnityEngine;
/*
 * マップを生成するとき使ったコード
 */

[System.Obsolete("dont use build game data!!!!!")]
public class RandomSpawn : MonoBehaviour
{
    [SerializeField] Transform _tr_Parent;                      // 生成したオブジェクトの親ファイル
    public List<GameObject> _objects = new List<GameObject>();  // 生成するオブジェクト
    [Space]
    public int _spawnCount;     // 生成数
    [Space]
    public float _posXMin;      // X軸の最小位置
    public float _posXMax;      // X軸の最大位置
    public float _posZMin;      // Z軸の最小位置
    public float _posZMax;      // Z軸の最大位置

    //=========================== Start ===========================>
    void Start()
    {
        //　生成数によりオブジェクトを生成する
        for (int i = 0; i < _spawnCount; i++)
        {
            for (int j = 0; j < _objects.Count; j++)
            {
                SpawnOBJ(j);
            }
        }
    }

    //=========================== Function ===========================>

    //　オブジェクトを生成する
    void SpawnOBJ(int num)　
    {
        //　位置を設定した間でランダムで指定する
        Vector3 pos = new Vector3(Random.Range(_posXMin, _posXMax), 0, Random.Range(_posZMin, _posZMax));
        //　オブジェクトの回転をランダムで指定する
        Quaternion rot = Quaternion.Euler(0, Random.Range(0, 360), 0);
        //　オブジェクトを位置と回転通り生成する
        GameObject obj = Instantiate(_objects[num], pos, rot);
        //　生成親ファイルを指定する
        obj.transform.SetParent(_tr_Parent);
    } 
}
