using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;

public class GameManager : MonoBehaviourPunCallbacks
{

    // Variables
    Transform Environment;
    Transform Player;
    Transform Rails;
    Transform Train;
    ToolManager toolManager;
    RailsManager railsManager;
    TrainManager trainManager;
    Panel PanelManager;

    Transform Rail_preview;
    Transform Bridge_preview;

    public Transform BridgeModel;       //木桥模型
    public Transform BorderModel;       //边界模型

    bool Pass;          //是否通关成功
    bool ReachTerminal; //是否到达终点
    bool TrainDamage;   //火车是否爆炸
    bool BridgeBuild;   //是否允许搭建木桥
    bool RailPut;       //是否允许放置铁轨
    bool TrainMove;     //是否允许火车移动
    bool EnActive;      //面板是否激活

    bool testflag;
    enum GorundCube     //地面方块类型
    {
        empty,
        grass,
        stone,
        tree,
        mine,
        river
    }
    GorundCube[,] groundcube = new GorundCube[30, 40];  //地面方块数组

    enum Obstacle   //障碍物类型
    {
        empty,
        grass,
        rail,
        bridge,
        building,
        other
    }
    Obstacle[,] obstacles = new Obstacle[30, 40];   //障碍物数组


    bool static_border;     //静态边界是否已构建

    // Use this for initialization
    void Start()
    {
       
        //  DontDestroyOnLoad(transform.gameObject);
        //PhotonNetwork.OfflineMode = true;
        if(PhotonNetwork.IsMasterClient)
        {
            GameObject item = PhotonNetwork.Instantiate("Axe", new Vector3(15, 1f, 6), Quaternion.identity);
            item.name = "Axe";
            item.transform.SetParent(GameObject.Find("MovableStuff").transform);

        }


        Environment = GameObject.Find("Environment").transform;

        Rail_preview = GameObject.Find("Environment").transform.Find("Rail_preview");
        Bridge_preview = GameObject.Find("Environment").transform.Find("Bridge_preview");

        GameObject player = PhotonNetwork.Instantiate("Player3", new Vector3(14, 2.2f, 14), Quaternion.identity);
        player.name = "Player";
        Player = player.transform;

        toolManager = Player.GetComponent<ToolManager>();

        //GameObject train = PhotonNetwork.Instantiate("Train", new Vector3(13, 1, 4), Quaternion.identity);
        //train.name = "Train";
        //Train = train.transform;
        Train = GameObject.Find("Train").transform;
        trainManager = Train.GetComponent<TrainManager>();

        Rails = GameObject.Find("Rails").transform;
        railsManager = Rails.GetComponent<RailsManager>();

        PanelManager = GameObject.Find("Panel").transform.Find("Canvas").GetComponent<Panel>();

        SetObstacleArray();
        SetBorderArray();
        railsManager.InitRails();
        trainManager.InitTrain();

        Pass = false;
        ReachTerminal = false;
        BridgeBuild = false;
        RailPut = false;
        TrainMove = false;

        static_border = false;
        testflag = false;

        // 倒计时3秒出发
        //Invoke("TrainSetOut", 3.0f);
    }

    // Update is called once per frame
    void Update()
    {

        UpdateController();
        if (EnActive && !Input.GetKeyDown(KeyCode.Escape))
            return;


        if (ReachTerminal)
            return;

        if (Pass)
        {
            trainManager.Pass = Pass;
            trainManager.Accelerate();
            trainManager.Move();
            return;
        }


        GetKeyInput();
        Preview();

        if (TrainMove)
            trainManager.Move();


    }

    // 更新控制变量
    void UpdateController()
    {
        Pass = railsManager.Finish;
        ReachTerminal = trainManager.ReachTerminal;
        TrainDamage = trainManager.Damage[0];
        EnActive = PanelManager.EnActive;
    }

    // 设置障碍物数组
    void SetObstacleArray()
    {
        Transform Ground = GameObject.Find("Ground").transform;
        foreach (Transform cube in Ground)
        {
            int x = (int)cube.position.x, z = (int)cube.position.z;
            string[] cname = cube.name.Split('_');
            switch (cname[0])
            {
                case "CubeGrass": groundcube[x, z] = GorundCube.grass; obstacles[x, z] = Obstacle.empty; break;
                case "CubeStone": groundcube[x, z] = GorundCube.stone; obstacles[x, z] = Obstacle.other; break;
                case "CubeTree": groundcube[x, z] = GorundCube.tree; obstacles[x, z] = Obstacle.other; break;
                case "CubeMine": groundcube[x, z] = GorundCube.mine; obstacles[x, z] = Obstacle.other; break;
                case "CubeRiver": groundcube[x, z] = GorundCube.river; obstacles[x, z] = Obstacle.empty; break;
            }
            cube.name = cname[0] + "_" + x + "_" + z;
        }

        Transform Grasses = GameObject.Find("Environment").transform.Find("Grasses").transform;
        foreach (Transform grass in Grasses)
        {
            int x = (int)grass.position.x, z = (int)grass.position.z;
            obstacles[x, z] = Obstacle.grass;
            grass.name = "Grass_" + x + "_" + z;
        }


        foreach (Transform rail in Rails)
        {
            int x = (int)rail.position.x, z = (int)rail.position.z;
            obstacles[x, z] = Obstacle.rail;
        }

        Transform EndRails = GameObject.Find("Terminal").transform.Find("Rails").transform;
        foreach (Transform rail in EndRails)
        {
            int x = (int)rail.position.x, z = (int)rail.position.z;
            obstacles[x, z] = Obstacle.rail;
        }

        Vector3 terminal_pos = GameObject.Find("Terminal").transform.Find("Building").transform.position;
        int tx = (int)terminal_pos.x, tz = (int)terminal_pos.z;
        int mi = 3, mj = 4;
        if (terminal_pos.x - tx == .5f)
        {
            mi = 4; mj = 3;
        }
        for (int i = 0; i < mi; ++i)
            for (int j = 0; j < mj; ++j)
            {
                obstacles[i + tx - 1, j + tz - 1] = Obstacle.building;
            }


    }

    // 设置边界数组
    void SetBorderArray()
    {
        int[] dx = new int[4], dy = new int[4];
        dx[0] = 0; dy[0] = 1;
        dx[1] = -1; dy[1] = 0;
        dx[2] = 0; dy[2] = -1;
        dx[3] = 1; dy[3] = 0;
        RemoveAllDynamicBorders();

        for (int i = 0; i < 30; ++i)
            for (int j = 0; j < 40; ++j)
            {
                if (groundcube[i, j] == GorundCube.empty) continue;

                for (int k = 0; k < 4; ++k)
                {
                    int x = i + dx[k], y = j + dy[k];

                    if (x < 0 || y < 0 || x >= 30 || y >= 40
                        || groundcube[x, y] == GorundCube.empty
                        || groundcube[i, j] != GorundCube.stone && groundcube[x, y] == GorundCube.stone)
                    {
                        if (!static_border)
                        {
                            BuildStaticBorder(new Vector3((i + x) / 2.0f, 1, (j + y) / 2.0f), k * -90);
                        }
                    }
                    else if ((groundcube[i, j] != GorundCube.river || obstacles[i, j] == Obstacle.bridge)
                            && groundcube[x, y] == GorundCube.river && obstacles[x, y] == Obstacle.empty)
                    {
                        BuildDynamicBorder(new Vector3((i + x) / 2.0f, 1, (j + y) / 2.0f), k * -90);
                    }
                }
            }
        static_border = true;
    }

    // 构建静态边界
    void BuildStaticBorder(Vector3 pos, int rotation)
    {
        GameObject bd = Instantiate(BorderModel.gameObject);
        bd.transform.parent = Environment.Find("Borders").Find("static");
        bd.name = pos.x.ToString("#0.0") + "_" + pos.z.ToString("#0.0");

        bd.transform.position = pos;
        bd.transform.rotation = Quaternion.Euler(new Vector3(0, rotation, 0));
    }

    // 构建动态边界
    void BuildDynamicBorder(Vector3 pos, int rotation)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            GameObject bd = PhotonNetwork.Instantiate(BorderModel.gameObject.name, pos, Quaternion.identity);
            bd.transform.parent = Environment.Find("Borders").Find("dynamic");
            bd.name = pos.x.ToString("#0.0") + "_" + pos.z.ToString("#0.0");

            bd.transform.position = pos;
            bd.transform.rotation = Quaternion.Euler(new Vector3(0, rotation, 0));
        }
    }

    // 移除所有动态边界
    void RemoveAllDynamicBorders()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Transform bds = Environment.Find("Borders").Find("dynamic");
            foreach (Transform bd in bds)
            {
                PhotonNetwork.Destroy(bd.gameObject);
            }
        }
    }

    // 获取键盘输入
    void GetKeyInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PanelManager.OpenOrClosePanel();
        }
        else if (Input.GetKeyDown(KeyCode.G))
        {
            TrainMove = true;
        }
        else if (Input.GetKeyDown(KeyCode.H))
        {
            TrainMove = false;
        }
        else if (Input.GetKeyDown(KeyCode.P))
        {
            testflag = true;
        }
        else if (Input.GetKeyUp(KeyCode.P))
        {
            GameObject rail = PhotonNetwork.Instantiate("StraightRail", Vector3.zero, Quaternion.identity);
            if (!PutRail(rail))
                PhotonNetwork.Destroy(rail);
            testflag = false;
        }
    }

    // 造木桥
    public bool BuildBridge(Transform wood)
    {
        if (TrainDamage) return false;

        if (!BridgeBuild) return false;

        PhotonNetwork.Destroy(wood.gameObject);

        int x = (int)Bridge_preview.position.x, z = (int)Bridge_preview.position.z;

        GameObject bridge = PhotonNetwork.Instantiate(BridgeModel.gameObject.name, Vector3.zero, Quaternion.identity);
        bridge.name = "BridgeModel";
        bridge.transform.position = Bridge_preview.position;
        obstacles[x, z] = Obstacle.bridge;

        SetBorderArray();

        return true;
    }

    // 预置物件
    void Preview()
    {
        Rail_preview.position = new Vector3(15, -1, 20);
        Bridge_preview.position = new Vector3(15, -1, 20);
        if (TrainDamage) return;

        PlayerInput playerInput = Player.GetComponent<PlayerInput>();
        int faceDirect = playerInput.faceDirect;
        Vector3 ppos = Player.position;
        int x = Mathf.RoundToInt(ppos.x), z = Mathf.RoundToInt(ppos.z);

        int tx = x, tz = z;
        switch (faceDirect)
        {
            case 0: ++tz; break;
            case 1: --tx; break;
            case 2: --tz; break;
            case 3: ++tx; break;
        }
        Vector3 preview_pos = new Vector3(tx, 1, tz);
        if (toolManager.handState == ToolManager.HANDSTATE.RAIL)
        {
            if (!PreviewRail(new Vector3(x, 1, z)))
                PreviewRail(preview_pos);
        }
        else if (toolManager.handState == ToolManager.HANDSTATE.MATERIAL
                && toolManager.rightHand.transform.GetChild(0).name == "Wood")
        {
            PreviewBridge(preview_pos);
        }
        else if (testflag)
        {
            if (!PreviewRail(new Vector3(x, 1, z)))
                PreviewRail(preview_pos);
        }
    }

    // 预置木桥
    bool PreviewBridge(Vector3 preview_pos)
    {
        int x = (int)preview_pos.x, z = (int)preview_pos.z;
        if (groundcube[x, z] != GorundCube.river || obstacles[x, z] != Obstacle.empty)
            return BridgeBuild = false;

        Bridge_preview.position = preview_pos;
        return BridgeBuild = true;
    }

    // 预置铁轨
    bool PreviewRail(Vector3 preview_pos)
    {
        int x = (int)preview_pos.x, z = (int)preview_pos.z;
        if (groundcube[x, z] == GorundCube.empty
            || groundcube[x, z] == GorundCube.river && obstacles[x, z] != Obstacle.bridge
            || groundcube[x, z] == GorundCube.stone)
            return RailPut = false;

        if (obstacles[x, z] != Obstacle.empty
            && obstacles[x, z] != Obstacle.grass
            && obstacles[x, z] != Obstacle.bridge)
            return RailPut = false;

        RailsManager.FaceDirect pfaced;
        RailsManager.TurnDirect pturnd;
        if (!railsManager.PutEnable(preview_pos, out pfaced, out pturnd))
            return RailPut = false;

        if (trainManager.MoveToLast && pturnd != RailsManager.TurnDirect.Straight)
            return false;


        Rail_preview.position = preview_pos;
        Rail_preview.rotation = Quaternion.Euler(0, (int)pfaced * -90, 0);

        return RailPut = true;
    }

    // 添加铁轨物件，给火车待行驶的铁轨队列入队
    public bool PutRail(GameObject rail)
    {
        if (TrainDamage) return false;

        if (!RailPut) return false;

        Vector3 new_pos = Rail_preview.position;
        int x = (int)new_pos.x, z = (int)new_pos.z;
        RailsManager.TurnDirect turnDirect;
        if (railsManager.TryPut(rail, new_pos, !trainManager.MoveToLast, out turnDirect)) //尝试铺设铁轨
        {
            rail.layer = 0;
            Rail_preview.position = new Vector3(x, -1, z);

            // 根据转向判断是否需要更新队尾元素
            if (turnDirect == RailsManager.TurnDirect.Left || turnDirect == RailsManager.TurnDirect.Right)
                trainManager.UpdateLastRail((TrainManager.TurnDirect)turnDirect);
            // 入队新的铁轨
            trainManager.AddRail(new_pos, TrainManager.TurnDirect.Straight);

            // 除草
            if (obstacles[x, z] == Obstacle.grass)
            {
                Transform grass = GameObject.Find("Environment").transform.Find("Grasses").Find("Grass_" + x + "_" + z);
                Destroy(grass.gameObject);
                obstacles[x, z] = Obstacle.empty;
            }
            obstacles[x, z] = Obstacle.rail;

            int end = railsManager.AdjacentToEnd(new_pos);  //判断是否邻近终点铁轨
            if (end != -1)  // end为0代表邻近下标为0的终点铁轨，为1代表邻近下标为1的终点铁轨，为-1代表不邻近终点铁轨
            {
                RailsManager.TurnDirect[] turnDirects;
                railsManager.ConnectToEnd(end, out turnDirects);

                Vector3 ep0 = railsManager.EndRail[end].pos, ep1 = railsManager.EndRail[(end + 1) % 2].pos;

                if (turnDirects[0] == RailsManager.TurnDirect.Left || turnDirects[0] == RailsManager.TurnDirect.Right)
                    trainManager.UpdateLastRail((TrainManager.TurnDirect)turnDirects[0]);

                trainManager.AddRail(ep0, (TrainManager.TurnDirect)turnDirects[1]);
                trainManager.AddRail(ep1, TrainManager.TurnDirect.Straight);

                print("All rails have been laid.");
            }

            return true;
        }
        else return false;

    }

    // 火车出发
    void TrainSetOut()
    {
        print("Train start to run!");
        TrainMove = true;
    }

    //private void 

    //public override void OnJoinedRoom()
    //{
    //    base.OnJoinedRoom();
    //    if (!photonView.IsMine)
    //        return;
    //    GameObject Player = PhotonNetwork.Instantiate("Player", new Vector3(11, 1, 5), Quaternion.identity, 0);
    //    Player.name = "Player";
    //}
}
