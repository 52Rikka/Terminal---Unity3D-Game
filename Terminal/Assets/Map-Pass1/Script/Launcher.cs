using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
public class Launcher:MonoBehaviourPunCallbacks
{
    
    
	// Use this for initialization
	void Awake () {
        PhotonNetwork.OfflineMode = true;       //刚开始是离线模式
        GameObject Player = PhotonNetwork.Instantiate("Player", new Vector3(11, 1, 5), Quaternion.identity, 0);     //初始化游戏玩家
        Player.name = "Player";
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        
        base.OnDisconnected(cause);
        Debug.Log("网络连接失败！");
    }
    /*如果网络连接成功就加入或者创建房间*/
    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        Debug.Log("加入房间");

        PhotonNetwork.JoinOrCreateRoom("room", new Photon.Realtime.RoomOptions { MaxPlayers = 4 , CleanupCacheOnLeave = false }, TypedLobby.Default);
    }
    public override void OnJoinedRoom()
    {
        Debug.Log("加入房间成功");
        base.OnJoinedRoom();
        if (!PhotonNetwork.IsMasterClient)
          return;
        PhotonNetwork.LoadLevel(2);
    }

}
