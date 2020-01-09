using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class RailsManager : MonoBehaviourPunCallbacks
{

    // Variables
    public enum FaceDirect     //朝向
    {
        East,
        North,
        West,
        South,
        Unknown
    }

    public enum TurnDirect     //转向
    {
        Straight,
        Left,
        Right,
        Unknown
    }

    public bool Finish;     //全部铁轨
    GameObject[,] RailsArray;      //保存铁轨对象

    public struct RailData
    {
        public Vector3 pos;
        public FaceDirect faceDirect;
        public TurnDirect turnDirect;
    }
    public RailData LastRail;
    public RailData[] EndRail;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    // 初始化铁轨
    public void InitRails()
    {
        int world_x = (int)transform.position.x, world_z = (int)transform.position.z;
        RailsArray = new GameObject[30, 40];

        int i = 0;
        foreach (Transform rail in transform)
        {
            rail.position = new Vector3(world_x, 1, world_z + i);
            rail.name = "Rail_" + world_x + "_" + (world_z + i);
            RailsArray[world_x, world_z + i] = rail.gameObject;
            ++i;
        }

        Transform StationRails = GameObject.Find("Terminal").transform.Find("Rails");
        EndRail = new RailData[2];
        i = 0;
        foreach (Transform rail in StationRails)
        {
            EndRail[i] = new RailData();
            EndRail[i].pos = rail.position;
            EndRail[i].faceDirect = FaceDirect.East;
            EndRail[i].turnDirect = TurnDirect.Straight;
            int sx = (int)rail.position.x, sz = (int)rail.position.z;
            RailsArray[sx, sz] = rail.gameObject;
            ++i;
        }

        LastRail = new RailData();
        LastRail.pos = new Vector3(world_x, 1, world_z + 9);
        LastRail.faceDirect = FaceDirect.East;
        LastRail.turnDirect = TurnDirect.Straight;

        Finish = false;
    }

    // 邻近终点铁轨
    public int AdjacentToEnd(Vector3 new_pos)
    {
        if (NewFaceDirection(EndRail[0].pos, new_pos) != FaceDirect.Unknown)
            return 0;
        else if (NewFaceDirection(EndRail[1].pos, new_pos) != FaceDirect.Unknown)
            return 1;
        return -1;
    }

    // 接上终点铁轨
    //public void ConnectToEnd(int end, out TurnDirect turnDirect)
    //{
    //    turnDirect = TurnDirect.Unknown;
    //    int ex = (int)EndRail[end].pos.x, ez = (int)EndRail[end].pos.z;
    //    Destroy(RailsArray[ex, ez]);
    //    Put(EndRail[end].pos, out turnDirect);
    //    Finish = true;
    //}

    // 接上终点铁轨
    public void ConnectToEnd(int end, out TurnDirect[] turnDirects)
    {
        turnDirects = new TurnDirect[2];
        int ex = (int)EndRail[end].pos.x;
        int ez = (int)EndRail[end].pos.z;
        //Destroy(RailsArray[ex, ez]);
        Put(RailsArray[ex, ez], EndRail[end].pos, out turnDirects[0]);

        end = (end + 1) % 2;
        ex = (int)EndRail[end].pos.x;
        ez = (int)EndRail[end].pos.z;
        //Destroy(RailsArray[ex, ez]);
        Put(RailsArray[ex, ez], EndRail[end].pos, out turnDirects[1]);

        Finish = true;
    }

    // 铺设铁轨模型
    void PutRailModel(Vector3 vector3, int rotation, GameObject rail)
    {
        int x = (int)vector3.x, z = (int)vector3.z;
        //GameObject newrail = PhotonNetwork.Instantiate(rail.name, vector3, Quaternion.identity);
        rail.transform.parent = transform;
        rail.transform.position = vector3;
        rail.transform.rotation = Quaternion.Euler(0, rotation, 0);
        rail.name = "Rail_" + x + "_" + z;
        RailsArray[x, z] = rail;
    }

    // 新的朝向
    FaceDirect NewFaceDirection(Vector3 new_pos, Vector3 old_pos)
    {
        int dx = (int)(new_pos.x - old_pos.x), dz = (int)(new_pos.z - old_pos.z);
        if (dx == 0 && dz == 1) return FaceDirect.East;
        else if (dx == -1 && dz == 0) return FaceDirect.North;
        else if (dx == 0 && dz == -1) return FaceDirect.West;
        else if (dx == 1 && dz == 0) return FaceDirect.South;
        else return FaceDirect.Unknown;
    }

    // 转向
    TurnDirect TurnDirection(FaceDirect new_faceDirect, FaceDirect old_faceDirect)
    {
        int nf = (int)new_faceDirect, lf = (int)old_faceDirect;
        if (lf == nf) return TurnDirect.Straight;
        else if ((lf + 1) % 4 == nf) return TurnDirect.Left;
        else if ((lf + 3) % 4 == nf) return TurnDirect.Right;
        else return TurnDirect.Unknown;
    }

    // 判断是否允许铺设铁轨
    public bool PutEnable(Vector3 new_pos)
    {
        int lx = (int)LastRail.pos.x, lz = (int)LastRail.pos.z;
        int x = (int)new_pos.x, z = (int)new_pos.z;
        if (RailsArray[x, z] != null || Mathf.Abs(x - lx) + Mathf.Abs(z - lz) >= 2)
            return false;
        return true;
    }

    // 判断是否允许铺设铁轨（重载1）
    public bool PutEnable(Vector3 new_pos, out FaceDirect new_faceDirect)
    {
        new_faceDirect = FaceDirect.Unknown;
        int lx = (int)LastRail.pos.x, lz = (int)LastRail.pos.z;
        int x = (int)new_pos.x, z = (int)new_pos.z;
        if (RailsArray[x, z] != null || Mathf.Abs(x - lx) + Mathf.Abs(z - lz) >= 2)
            return false;
        new_faceDirect = NewFaceDirection(new_pos, LastRail.pos);
        return true;
    }

    // 判断是否允许铺设铁轨（重载2）
    public bool PutEnable(Vector3 new_pos, out TurnDirect turnDirect)
    {
        turnDirect = TurnDirect.Unknown;
        int lx = (int)LastRail.pos.x, lz = (int)LastRail.pos.z;
        int x = (int)new_pos.x, z = (int)new_pos.z;
        if (RailsArray[x, z] != null || Mathf.Abs(x - lx) + Mathf.Abs(z - lz) >= 2)
            return false;
        FaceDirect new_faceDirect = NewFaceDirection(new_pos, LastRail.pos);
        turnDirect = TurnDirection(new_faceDirect, LastRail.faceDirect);
        return true;
    }

    // 判断是否允许铺设铁轨（重载3）
    public bool PutEnable(Vector3 new_pos, out FaceDirect new_faceDirect, out TurnDirect turnDirect)
    {
        new_faceDirect = FaceDirect.Unknown;
        turnDirect = TurnDirect.Unknown;
        int lx = (int)LastRail.pos.x, lz = (int)LastRail.pos.z;
        int x = (int)new_pos.x, z = (int)new_pos.z;
        if (RailsArray[x, z] != null || Mathf.Abs(x - lx) + Mathf.Abs(z - lz) >= 2)
            return false;
        new_faceDirect = NewFaceDirection(new_pos, LastRail.pos);
        turnDirect = TurnDirection(new_faceDirect, LastRail.faceDirect);
        return true;
    }

    // 将上一段铁轨变弯曲
    void CurveLastRail(TurnDirect turnDirect)
    {
        int lx = (int)LastRail.pos.x, lz = (int)LastRail.pos.z, lf = (int)LastRail.faceDirect;
        int rotation = 0;
        switch (turnDirect)
        {
            case TurnDirect.Left: rotation = (lf + 3) % 4 * -90; break;
            case TurnDirect.Right: rotation = lf * -90; break;
        }
        PhotonNetwork.Destroy(RailsArray[lx, lz]);
        //Destroy(RailsArray[lx, lz]);
        GameObject curveRail = PhotonNetwork.Instantiate("CurveRail", LastRail.pos, Quaternion.identity);
        PutRailModel(LastRail.pos, rotation, curveRail);
    }

    // 在该位置铺设铁轨（重载1）
    void Put(GameObject rail, Vector3 new_pos, TurnDirect turnDirect)
    {
        FaceDirect new_faceDirect = NewFaceDirection(new_pos, LastRail.pos);
        if (turnDirect != TurnDirect.Straight)
            CurveLastRail(turnDirect);
        int rotation = (int)new_faceDirect * -90;
        PutRailModel(new_pos, rotation, rail);  //放置的铁轨必定是直行铁轨
        LastRail.pos = new_pos; LastRail.faceDirect = new_faceDirect;
    }

    // 在该位置铺设铁轨（重载2）
    void Put(GameObject rail, Vector3 new_pos, out TurnDirect turnDirect)
    {
        FaceDirect new_faceDirect = NewFaceDirection(new_pos, LastRail.pos);
        turnDirect = TurnDirection(new_faceDirect, LastRail.faceDirect);
        if (turnDirect != TurnDirect.Straight)
            CurveLastRail(turnDirect);
        int rotation = (int)new_faceDirect * -90;
        PutRailModel(new_pos, rotation, rail);  //放置的铁轨必定是直行铁轨
        LastRail.pos = new_pos; LastRail.faceDirect = new_faceDirect;
    }

    // 在该位置尝试铺设铁轨
    public bool TryPut(GameObject rail, Vector3 new_pos, bool enableToCurveLastRail, out TurnDirect turnDirect)
    {
        turnDirect = TurnDirect.Unknown;
        if (Finish)
            return false;

        if (!PutEnable(new_pos, out turnDirect))
            return false;

        if (!enableToCurveLastRail && turnDirect != TurnDirect.Straight)
            return false;

        Put(rail, new_pos, turnDirect);

        return true;
    }

}
