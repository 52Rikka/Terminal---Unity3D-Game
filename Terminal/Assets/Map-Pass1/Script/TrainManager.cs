using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class TrainManager : MonoBehaviourPunCallbacks
{

    // Variables
    int MaxCount = 100;         //通过一段铁轨所需时间（次数）

    public bool Pass;           //是否通关成功
    public bool ReachTerminal;  //是否到达终点
    public bool[] Damage;       //火车段是否损坏
    public bool MoveToLast;     //是否运行到最后一个铁轨

    Vector2[] left;             //左转弯计算圆心的辅助数组
    Vector2[] right;            //右转弯计算圆心的辅助数组

    Transform[] TrainSection;   //保存各节火车段
    Transform Explode;
    ParticleSystem explode;

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

    struct UnrailedData     //待通过铁轨队列数据结构
    {
        public Vector3 pos;
        public TurnDirect turnDirect;
        public UnrailedData(Vector3 _pos, TurnDirect _turnDirect)
        {
            pos = _pos;
            turnDirect = _turnDirect;
        }
    }

    class MoveData      //火车段的运动参数类
    {
        public bool exist;      //火车是否存在
        public FaceDirect faceDirect_now, faceDirect_after;     //火车朝向
        public TurnDirect turnDirect_now, turnDirect_after;     //火车转向
        public Vector2 circlePos_now, circlePos_after;          //火车转弯圆周的圆心坐标
        public double distance, angle;     //火车在当前铁轨段已行进的路程和已转动的角度
        public Queue<UnrailedData> unrailed = new Queue<UnrailedData>();    //用队列保存待通过的铁轨信息
        public MoveData()
        {
            exist = true;
            faceDirect_now = faceDirect_after = FaceDirect.East;
            turnDirect_now = turnDirect_after = TurnDirect.Straight;
            distance = 1;
            angle = 0;
        }
    }
    MoveData[] movedatas = new MoveData[4];     //4个火车段的运动参数数组


    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    // 初始化火车
    public void InitTrain()
    {
        TrainSection = new Transform[4];
        TrainSection[0] = transform.Find("Locomotive");
        TrainSection[1] = transform.Find("Tank");
        TrainSection[2] = transform.Find("Forge");
        TrainSection[3] = transform.Find("WaterBox");

        int world_x = (int)transform.position.x, world_z = (int)transform.position.z;
        for (int i = 3; i >= 0; --i)
        {
            movedatas[i] = new MoveData();
            for (int j = 3; j >= i; --j)
            {
                UnrailedData data = new UnrailedData(new Vector3(world_x, 1, world_z + 2 * (4 - i)), TurnDirect.Straight);
                movedatas[j].unrailed.Enqueue(data);
                data = new UnrailedData(new Vector3(world_x, 1, world_z + 2 * (4 - i) + 1), TurnDirect.Straight);
                movedatas[j].unrailed.Enqueue(data);
            }
        }

        left = new Vector2[4];
        left[0] = new Vector2(-.5f, -.5f);
        left[1] = new Vector2(.5f, -.5f);
        left[2] = new Vector2(.5f, .5f);
        left[3] = new Vector2(-.5f, .5f);

        right = new Vector2[4];
        right[0] = new Vector2(.5f, -.5f);
        right[1] = new Vector2(.5f, .5f);
        right[2] = new Vector2(-.5f, .5f);
        right[3] = new Vector2(-.5f, -.5f);


        ReachTerminal = false;
        MoveToLast = false;
        Damage = new bool[4];
        Damage[0] = Damage[1] = Damage[2] = Damage[3] = false;

        Explode = transform.Find("Explode");
        explode = Explode.GetComponent<ParticleSystem>();
        explode.Pause(true);
    }

    // 火车运动
    public void Move()
    {
        if (ReachTerminal || Damage[3]) return;
        if (Input.GetKey(KeyCode.RightShift)) MaxCount = 10;

        for (int i = 0; i < 4; ++i)
        {
            if (!movedatas[i].exist) continue;

            if (movedatas[i].distance == 1 || movedatas[i].angle == 90)
            {
                MoveToNextRail(i);
            }
            else
            {
                MoveOnCurrentRail(i);
            }
        }
    }

    // 运动到下一个铁轨段
    void MoveToNextRail(int i)
    {
        Adjust(i);  //调整坐标


        // 设置下一段运动参数
        movedatas[i].faceDirect_now = movedatas[i].faceDirect_after;
        movedatas[i].turnDirect_now = movedatas[i].turnDirect_after;
        movedatas[i].circlePos_now = movedatas[i].circlePos_after;
        movedatas[i].distance = 0;
        movedatas[i].angle = 0;

        //if (i == 0) print("face: " + movedatas[i].faceDirect_now);
        if (movedatas[i].unrailed.Count == 0)   //待通过铁轨队列为空
        {
            if (!Pass)    //未通关（铁轨未铺设完毕）
            {
                print(TrainSection[i].name + " boom!");     //爆炸
                Explode.position = TrainSection[i].position;
                explode.Play();
                Destroy(TrainSection[i].gameObject);
                movedatas[i].exist = false;
                Damage[i] = true;   //第i段火车爆炸
            }
            else if (i == 0)    //火车头已到达终点
            {
                print("Train has reached the station.");
                ReachTerminal = true;
            }
            return;
        }

        if (i == 0)
        {
            MoveToLast = (movedatas[0].unrailed.Count == 1);
        }

        UnrailedData rail = movedatas[i].unrailed.Dequeue();    //出队
        if (rail.turnDirect == TurnDirect.Left)
        {
            //print("dequeue: " + rail.pos);
            float lx = rail.pos.x + left[(int)movedatas[i].faceDirect_after].x;
            float ly = rail.pos.z + left[(int)movedatas[i].faceDirect_after].y;
            movedatas[i].circlePos_after = new Vector2(lx, ly);
            //print("center: " + movedatas[i].circlePos_after);
        }
        else if (rail.turnDirect == TurnDirect.Right)
        {
            float rx = rail.pos.x + right[(int)movedatas[i].faceDirect_after].x;
            float ry = rail.pos.z + right[(int)movedatas[i].faceDirect_after].y;
            movedatas[i].circlePos_after = new Vector2(rx, ry);
        }
        movedatas[i].faceDirect_after = NewFaceDirection(movedatas[i].faceDirect_after, rail.turnDirect);
        movedatas[i].turnDirect_after = rail.turnDirect;
    }

    // 在当前铁轨段运动
    void MoveOnCurrentRail(int i)
    {
        double move_v = (1.0 / MaxCount), rotate_v = (90.0 / MaxCount); //直行线速度和旋转角速度

        if (movedatas[i].turnDirect_now == TurnDirect.Straight)   //直行
        {
            double remain_distance = 1.0 - movedatas[i].distance;
            if (remain_distance >= move_v)  //剩余路程不小于线速度
            {
                movedatas[i].distance += move_v;
                Vector3 vector3 = new Vector3(0, 0, (float)move_v);
                TrainSection[i].Translate(vector3);
            }
            else
            {
                movedatas[i].distance = 1;
                TrainSection[i].Translate(new Vector3(0, 0, (float)remain_distance));
            }
        }
        else                //左转或右转
        {
            double remain_angle = 90 - movedatas[i].angle;
            double k = System.Math.PI / 180;
            int fdir = (int)movedatas[i].faceDirect_now, tdir = (int)movedatas[i].turnDirect_now;
            int cw = (tdir == 1 ? 1 : -1);
            float cx = movedatas[i].circlePos_now.x, cy = movedatas[i].circlePos_now.y;
            if (remain_angle >= rotate_v)   //剩余角度不小于角速度
            {
                movedatas[i].angle += rotate_v;
                double radians = k * ((fdir + 3) % 4 * 90 + cw * movedatas[i].angle);
                float rx = cx + (float)System.Math.Cos(radians) * .5f;
                float rz = cy + (float)System.Math.Sin(radians) * .5f;
                TrainSection[i].position = new Vector3(rx, 1, rz);
                TrainSection[i].Rotate(new Vector3(0, (float)(-cw * rotate_v), 0));
            }
            else
            {
                movedatas[i].angle = 90;
                double radians = k * (((fdir + 3) % 4 + cw) * 90);
                float rx = cx + (float)System.Math.Cos(radians) * .5f;
                float rz = cy + (float)System.Math.Sin(radians) * .5f;

                TrainSection[i].position = new Vector3(rx, 1, rz);
                TrainSection[i].Rotate(new Vector3(0, (float)(-cw * remain_angle), 0));
            }
        }
    }

    // 新的朝向
    FaceDirect NewFaceDirection(FaceDirect faceDirect, TurnDirect turnDirect)
    {
        switch (turnDirect)
        {
            case TurnDirect.Straight: return faceDirect;
            case TurnDirect.Left: return (FaceDirect)(((int)faceDirect + 1) % 4);
            case TurnDirect.Right: return (FaceDirect)(((int)faceDirect + 3) % 4);
        }
        return FaceDirect.Unknown;
    }

    // 调整火车坐标
    void Adjust(int section)
    {
        Vector3 vector3 = TrainSection[section].position;
        float px = (float)System.Math.Round(vector3.x, 1);
        float py = (float)System.Math.Round(vector3.y, 1);
        float pz = (float)System.Math.Round(vector3.z, 1);
        if (Mathf.Abs(px - (int)px) != 0.5) px = Mathf.Round(px);
        if (Mathf.Abs(py - (int)py) != 0.5) py = Mathf.Round(py);
        if (Mathf.Abs(pz - (int)pz) != 0.5) pz = Mathf.Round(pz);
        TrainSection[section].position = new Vector3(px, py, pz);

        Vector3 rvector3 = TrainSection[section].eulerAngles;
        int rx = Mathf.RoundToInt(rvector3.x);
        int ry = Mathf.RoundToInt(rvector3.y);
        int rz = Mathf.RoundToInt(rvector3.z);
        TrainSection[section].rotation = Quaternion.Euler(rx, ry, rz);
    }

    // 添加铁轨
    public void AddRail(Vector3 pos, TurnDirect turnDirect)
    {
        for (int i = 0; i < 4; i++)
            movedatas[i].unrailed.Enqueue(new UnrailedData(pos, turnDirect));
    }

    // 修改最后一个铁轨的类型（直行0、左转1、右转2）
    public void UpdateLastRail(TurnDirect turnDirect)
    {
        for (int i = 0; i < 4; ++i)
        {
            if (movedatas[i].unrailed.Count == 0) return;
            for (int j = 0; j < movedatas[i].unrailed.Count - 1; ++j)
                movedatas[i].unrailed.Enqueue(movedatas[i].unrailed.Dequeue());
            UnrailedData lastRail = movedatas[i].unrailed.Dequeue();
            lastRail.turnDirect = turnDirect;
            movedatas[i].unrailed.Enqueue(lastRail);
        }
    }

    // 更新单位运行时间
    public void UpdateMaxCount(int _MaxCount)
    {
        MaxCount = _MaxCount;
    }

    // 火车加速
    public void Accelerate()
    {
        MaxCount = 10;
    }
}
