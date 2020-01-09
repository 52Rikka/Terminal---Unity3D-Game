using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Forge : MonoBehaviourPunCallbacks
{   // 车厢铁轨容量
    public int RailCapacity;

    // 铁轨生成时间
    public float EachTime;
    public float time;
    public int Label;
    // 
    public Rail Rail;
    public Tank tank;

    public GameObject[] Primitive = new GameObject[4];

    // 保存生成了的铁轨
    public List<GameObject> RailStorage = new List<GameObject>();


    // Use this for initialization
    void Start()
    {
        time = EachTime;
    }

    // Update is called once per frame
    void Update()
    {
        GenerateRail();
    }

    // 生成铁轨
    private void GenerateRail()
    {
        if (RailStorage.Count < RailCapacity)
        {   // 当前车厢未满
            if (tank.WoodStorage.Count != 0 && tank.IronStorage.Count != 0)
            {   // 储物车厢内有两种材料则可以生成铁轨
                if (time > 0.0f)
                {
                    time -= Time.deltaTime;
                }
                else
                {
                    switch (Label)
                    {
                        case 0:     // 枕木
                            tank.RemoveMat(Label);
                            GenePrimitive(Primitive[0], new Vector3(0, 0.125f + 0.15f * RailStorage.Count, 0.08675f));
                            break;
                        case 1:     // 枕木
                            GenePrimitive(Primitive[0], new Vector3(0, 0.125f + 0.15f * RailStorage.Count, -0.08f));
                            break;
                        case 2:     // 枕木
                            GenePrimitive(Primitive[0], new Vector3(0, 0.125f + 0.15f * RailStorage.Count, -0.24675f));
                            break;
                        case 3:     // 轨道
                            GenePrimitive(Primitive[1], new Vector3(-0.25f, 0.175f + 0.15f * RailStorage.Count, -0.08f));
                            break;
                        case 4:     // 轨道
                            GenePrimitive(Primitive[1], new Vector3(0.25f, 0.175f + 0.15f * RailStorage.Count, -0.08f));
                            break;
                        case 5:     // 管道
                            GenePrimitive(Primitive[2], new Vector3(0, 0.15f + 0.15f * RailStorage.Count, -0.08f));
                            break;
                        case 6:     // 节点
                            GenePrimitive(Primitive[3], new Vector3(0, 0.15f + 0.15f * RailStorage.Count, -0.08f));
                            break;
                        case 7:     // 将车厢中基本组件删除，生成完整铁轨
                            tank.RemoveMat(Label);
                            DeletePrimitive();
                            break;
                        default: break;

                    }
                }
            }
        }
    }


    /// <summary>
    /// ActorController::Train所调用的取铁轨，需要根据手中状态进行一系列的判断
    /// </summary>
    /// <param name="hand">ToolManager中的rightHand</param>
    /// <returns>是否捡起了铁轨</returns>
    public bool FetchRail(GameObject hand)
    {   
        if (RailStorage.Count != 0)
        {   // 当前车厢里有铁轨

            // 拿起铁轨
            return TakeRail(hand);
        }
        else
        {
            return false;
        }
    }

    // 拿起铁轨
    private bool TakeRail(GameObject hand)
    {
        bool takeRail = false; // 是否成功捡起了铁轨

        int oldrails = RailStorage.Count;
        // 人物拿走的铁轨数
        int takens =0;

        // 获取手中已拿的铁轨数
        int holding = hand.gameObject.GetComponentsInChildren<Rail>().Length;

        
        Transform itemTrans = null; // 手中铁轨的Transform
        if (holding != 0)
        {   // 如果手中有铁轨，则获取铁轨Transform，作为捡起铁轨的父物体
            itemTrans = hand.transform.GetChild(0);
        }

        if (holding < 3)
        {   // 当前手中铁轨不足三个

            // 有拿起铁轨
            takeRail = true;

            if (holding == 0) // 如果手里没有铁轨
            {
                // 拿起一个铁轨
                RailStorage[0].transform.SetParent(hand.transform); // 父物体为手
                RailStorage[0].transform.localPosition = new Vector3(0.0044f, 0.0069f, 0.0078f);
                RailStorage[0].transform.localRotation = Quaternion.AngleAxis(90, Vector3.right);
                RailStorage[0].GetComponent<BoxCollider>().enabled = false;
                RailStorage[0].layer = LayerMask.NameToLayer("Material");
                itemTrans = hand.transform.GetChild(0);
                takens++;
            }
            else
            {
                // 拿起一个铁轨
                RailStorage[0].transform.SetParent(itemTrans); // 父物体为手中的铁轨
                RailStorage[0].transform.localPosition = new Vector3(0, 0.2f * holding, 0);
                RailStorage[0].transform.localRotation = Quaternion.identity;
                RailStorage[0].GetComponent<BoxCollider>().enabled = false;
                RailStorage[0].layer = LayerMask.NameToLayer("Material");
                itemTrans.GetComponent<Resmaterial>().Size++;
                takens++;
            }
            holding++;

            // 拿起第二个和第三个铁轨
            for (int i = 1; i < RailStorage.Count; i++)
            {
                // 将车厢中的铁轨依次放入手中
                RailStorage[i].transform.SetParent(itemTrans); // 父物体为手中的铁轨
                RailStorage[i].transform.localPosition = new Vector3(0, 0.2f * holding, 0);
                RailStorage[i].transform.localRotation = Quaternion.identity;
                RailStorage[i].GetComponent<BoxCollider>().enabled = false;
                RailStorage[i].layer = LayerMask.NameToLayer("Material");
                itemTrans.GetComponent<Resmaterial>().Size++;
                holding++;
                takens++;
            }

            
            // 将人物拿走的铁轨从List中删除
            RailStorage.RemoveRange(0, takens);

            // 剩余铁轨的位置下降
            Transform[] remains = gameObject.GetComponentsInChildren<Transform>();
            for (int i = 2; i < remains.Length; i++)
            {
                remains[i].localPosition += new Vector3(0, -0.15f * takens, 0);
            }
        }
        return takeRail;
    }

    // 生成铁轨的一个组件
    private void GenePrimitive(GameObject gobj, Vector3 vec)
    {   // 根据gobj和vec设置新生成的组件的位置
        GameObject newPrimitive = GameObject.Instantiate(gobj);
        newPrimitive.transform.parent = gameObject.transform;
        newPrimitive.transform.localPosition = vec;
        newPrimitive.transform.localEulerAngles = Vector3.zero;
        newPrimitive.transform.localScale = new Vector3(newPrimitive.transform.lossyScale.x, newPrimitive.transform.lossyScale.y, 0.5f * newPrimitive.transform.lossyScale.z);
        if (Label == 5)
        {
            newPrimitive.transform.localEulerAngles += new Vector3(90, 0, 0);
            newPrimitive.transform.localScale = new Vector3(newPrimitive.transform.lossyScale.x * 0.5f, newPrimitive.transform.lossyScale.y, newPrimitive.transform.lossyScale.z);
        }
        if (Label == 6)
            newPrimitive.transform.localEulerAngles += new Vector3(0, 0, 90);
        // 时间重置，准备生成下一组件
        time = EachTime;
        Label++;
    }

    private void DeletePrimitive()
    {   // 将构成一个铁轨的基本组件删除
        Transform[] primitives = gameObject.GetComponentsInChildren<Transform>();
        for (int i = 0; i < 7; i++)
        {
            Destroy(primitives[primitives.Length - 1 - i].gameObject);
        }

        // 生成一个完整的铁轨
        GameObject newrail = PhotonNetwork.Instantiate(Rail.name, Vector3.zero, Quaternion.identity);
        newrail.transform.SetParent(gameObject.transform,true);
        newrail.transform.localPosition = new Vector3(0, 0.6f + 0.15f * RailStorage.Count, -0.08f);
        newrail.transform.localEulerAngles = Vector3.zero;
        newrail.transform.localScale = new Vector3(1, 1, 0.5f);
        newrail.name = "Rail";

        // 将生成的铁轨放入队列中
        RailStorage.Add(newrail);
        time = EachTime;
        Label = 0;
    }
}

