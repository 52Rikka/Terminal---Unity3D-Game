using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class Resource : MonoBehaviourPunCallbacks
{

    public int HP;              // 树木或矿石的生命值
    public float Actiontime;    // 预先定义一次动作的时间间隔（如：砍一下的时间

    public float time;          // 监视时间间隔

    public Resmaterial Resmaterial;// 当前资源块所对应材料

    // Use this for initialization
    void Start()
    {
        time = Actiontime;
    }

    [PunRPC]
    void Part_destory()
    {
        HP--;
        time = Actiontime;
        Transform[] Subtransforms = GetComponentsInChildren<Transform>();
        Destroy(Subtransforms[1].gameObject);

    }

    [PunRPC]
    void destory()
    {
        Destroy(gameObject);
    }

    public void Action()
    {
        if (HP > 0)
        {   // 当前资源块生命值>0，则受到伤害
            if (time > 0.0f)
                time -= Time.deltaTime;
            else
            {   // 砍完一下血量减1，time重置

                // 获取当前物体子物件，删除第一个子物件

                PhotonView p = PhotonView.Get(this);
                p.RPC("Part_destory", RpcTarget.All);

            }
            Debug.Log(HP);
        }
        else
        {

             Debug.Log("HP == 0");
             // 当前资源块生命值<0，生成资源并销毁原物件
             if(PhotonNetwork.IsMasterClient)
                Resmaterial.Generate().transform.position = transform.position;
             else
            {

            }

            /*同步摧毁物体*/
            PhotonView p = PhotonView.Get(this);
            p.RPC("destory", RpcTarget.All);
        }
    }

    // 用于实现当任务离开物体时，若当前动作执行到一半，即当前time！=Actiontime
    // 不加也可以，但就无法实现一次动作的完整性
    //private void OnTriggerExit(Collider other)
    //{
    //    time = Actiontime;
    //}

    private void OnCollisionExit(Collision collisionInfo)
    {
        time = Actiontime;
    }
}
