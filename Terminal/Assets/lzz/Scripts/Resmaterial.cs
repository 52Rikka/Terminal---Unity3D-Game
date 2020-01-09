using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Resmaterial : MonoBehaviourPunCallbacks
{

    public GameObject MaterialPrefab;   // 存储当前材料的Prefab对象
    public int Capacity;                // 人物可同时拿起的资源块数
    public int Size = 1;                // 存储当前材料的堆叠块数

    void Start()
    {

    }

    [PunRPC]
    void destory()
    {
        Destroy(gameObject);
    }

    // 生成材料块
    [PunRPC]
    public GameObject Generate()
    {
        GameObject newwood = PhotonNetwork.InstantiateSceneObject(MaterialPrefab.name, Vector3.zero, Quaternion.identity);
        Debug.Log(MaterialPrefab.name);
        newwood.name = MaterialPrefab.name;
        newwood.transform.SetParent(null);
        return newwood;
    }

    [PunRPC]
    void a()
    {

    }

    private void OnTriggerEnter(Collider player)
    {
        Resmaterial[] materials = player.gameObject.GetComponentsInChildren<Resmaterial>();
        if (materials.Length > 0 && MaterialPrefab.name != "StraightRail")
        { // 手中有材料，且不能是铁轨

            Debug.Log("pick second material");

            if (materials[0].name == MaterialPrefab.name)
            {   // 若相同则判断容量是否已满
                print("enter"+ (materials.Length + Size));
                if (materials.Length + Size <= Capacity)// 如果角色手中的材料数量 + 该地上的材料的堆叠数量 <= 容量
                {   // 表示人物还能继续捡材料
                    ToolManager tm = player.gameObject.GetComponent<ToolManager>();
                    tm.EquipSameMaterial(MaterialPrefab, materials.Length);
                    print("ok");
                }
                else
                {   // 容量已满则退出
                    return;
                }
            }
        }
    }
}
