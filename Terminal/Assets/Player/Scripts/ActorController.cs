using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
public class ActorController : MonoBehaviourPunCallbacks
{

    // Variables

    [SerializeField] // 显示到编辑器中 必须是编辑器资源类型
    private Animator anim;
    public PlayerInput pi;
    public ToolManager tm;

    public float turnSpeed = 10;        // 转身速度
    public float walkspeed = 3.0f;      // 行走速度
    public float runMultiplier = 1.5f;  // 奔跑速度

    // signal
    public bool hack; // 砍树、挖矿的动画信号

    public enum STATE // 动画状态
    {
        IDLE, // 普通摆臂
        LIFT  // 抬起双手 
    }
    public STATE state;


    Collider[] hitcollidersForward;                         // 角色前方的物体
    Vector3 BoxExtents = new Vector3(0.45f, 0.45f, 0.45f);  // 所探测的体积的半径 0.45 
    public float MaxDis = 0.6f;                             // 射线检测碰撞的最长距离（人物中心作为原点                                
    Ray faceRay;                                            // 用于检测碰撞的 角色正面的射线
    Color lastColor, currentColor, newColor;
    GameObject closestObject = null;


    GameManager gameManager;

    // Use this for initialization
    void Awake()
    { // 在Awake中获取所需要的组件
        pi = GetComponent<PlayerInput>();
        anim = GetComponent<Animator>();
        if (SceneManager.GetActiveScene().name != "Menu")
            gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        if (pi == null)
        {
            pi = transform.gameObject.AddComponent<PlayerInput>();
            Debug.Log("Creat PlayerInput");
        }
        tm = GetComponent<ToolManager>();
        if (tm == null)
        {
            tm = transform.gameObject.AddComponent<ToolManager>();
        }
    }

    void Start()
    {

    }
    void Update()
    {
        // 更新人物状态
        state = (tm.handState == ToolManager.HANDSTATE.MATERIAL || tm.handState == ToolManager.HANDSTATE.RAIL) ? STATE.LIFT : STATE.IDLE;
        // 当接受到拾起或放下信号
        if (pi.pick)
        {
            Pick();
        }

        // 当角色手里有工具时，进行砍树或采矿
        if (tm.handState == ToolManager.HANDSTATE.TOOL)
        {
            HackOrMine();
        }
        else
        {
            hack = false;
        }


    }

    void FixedUpdate()
    {
        if (pi.hor != 0 || pi.ver != 0)
        {
            //转身            
            Rotate(pi.hor, pi.ver);
        }
        //移动
        transform.Translate(new Vector3(pi.hor, 0, pi.ver) * Time.fixedDeltaTime * walkspeed * (pi.run ? runMultiplier : 1.0f), Space.World);

        // 动画
        anim.SetFloat("forward", pi.Dmag * Mathf.Lerp(anim.GetFloat("forward"), ((pi.run) ? 2.0f : 1.0f), 0.5f));
        anim.SetBool("lift", state == STATE.LIFT);
        anim.SetBool("hack", hack);
    }

    void Rotate(float hor, float ver)
    {
        //获取方向        
        Vector3 dir = new Vector3(hor, 0, ver);
        //将方向转换为四元数        
        Quaternion quaDir = Quaternion.LookRotation(dir, Vector3.up);
        //缓慢转动到目标点        
        transform.rotation = Quaternion.Lerp(transform.rotation, quaDir, Time.fixedDeltaTime * turnSpeed);
    }

    public void DrawBoxForDebug(Vector3 box, Vector3 origin)
    {
        Debug.DrawRay(origin + new Vector3(-0.5f, -0.5f, -0.5f), Vector3.up, new Color(1, 1, 1));
        Debug.DrawRay(origin + new Vector3(0.5f, -0.5f, -0.5f), Vector3.up, new Color(0, 1, 1));
        Debug.DrawRay(origin + new Vector3(-0.5f, -0.5f, 0.5f), Vector3.up, new Color(1, 0, 0));
        Debug.DrawRay(origin + new Vector3(0.5f, -0.5f, 0.5f), Vector3.up, new Color(0, 1, 0));
    }

    /// <summary>
    /// 当角色手里有工具时，进行砍树或采矿
    /// </summary>
    private void HackOrMine()
    {
        // 创建射线碰撞目标
        RaycastHit hitObject;
        // 更新射线
        faceRay = new Ray(transform.position + transform.up * 0.1f, transform.forward);
        Debug.DrawRay(transform.position + transform.up * 0.1f, transform.forward);
        // 射线碰撞检测
        bool isHitResourc = Physics.Raycast(faceRay, out hitObject, MaxDis, LayerMask.GetMask("Resource"));
        if (isHitResourc)
        {
            Resource Resblock = hitObject.collider.gameObject.GetComponent<Resource>();
            // 获取人物所持工具
            GameObject tool = tm.itemInHand;

            if (Match(Resblock, tool))
            {   // 若人物手中道具与资源块匹配则进行操作
                Resblock.Action();
                hack = true;
            }
            else
            {
                hack = false;
            }
        }
        else
        {
            hack = false;
        }
    }

    private void Pick()
    {
        // 创建射线碰撞目标
        RaycastHit HitObject;
        // 更新射线
        faceRay = new Ray(transform.position + transform.up * 0.1f, transform.forward);
        Debug.DrawRay(transform.position + transform.up * 0.1f, transform.forward);
        // 射线碰撞检测
        bool isHitTrain = Physics.Raycast(faceRay, out HitObject, MaxDis, LayerMask.GetMask("Train"));



        if (isHitTrain)
        {   // 如果检测到火车 与火车进行交互，捡铁轨或者放材料
            Debug.Log("Find Train");
            string Hitname = HitObject.collider.gameObject.name;
            Train(Hitname, HitObject);
        }
        else
        {   // 没检测到火车 捡起放下工具或材料
            Debug.Log("No Train");
            PickOrLoad();
        }
    }

    /// <summary>
    /// 当人物手里没东西，或者有材料的时候与火车交互
    /// </summary>
    private void Train(string hitname, RaycastHit hitObject)
    {
   
        if (hitname == "Tank")
        {
            // 获取碰撞到的火车
            Tank tank = hitObject.collider.gameObject.GetComponent<Tank>();
            Resmaterial[] materials = transform.GetComponentsInChildren<Resmaterial>();

            // 将人物手中资源块放入车厢
            for (int i = materials.Length; i > 0; i--)
            {   // 逆序，先把手中材料的子物体放进车厢
                tank.StoreMat(materials[i-1]);
            }

            tm.UpdateMatrial();
        }
        else if (hitname == "Forge")
        {

            // 获取所碰撞到的车厢
            Forge forge = hitObject.collider.gameObject.GetComponent<Forge>();
            // 人物hero取出铁轨
            //forge.FetchRail(transform.gameObject);
            bool takeRail = forge.FetchRail(tm.rightHand);

            if (takeRail)// 如果捡起了铁轨，修改右手的状态，改为Rail
            {
                tm.handState = ToolManager.HANDSTATE.RAIL;
            }
        }

    }

    /// <summary>
    /// 当按下space时，捡起或放下工具或者材料
    /// </summary>
    void PickOrLoad()
    {
        // 拾起的区域为角色面前的0.5格
        Vector3 vecPickOrLoad = transform.position + transform.forward * 0.5f;
        // 画出单位方格，即可以碰撞的区域
        DrawBoxForDebug(BoxExtents, new Vector3(vecPickOrLoad.x, 0.5f, vecPickOrLoad.z));
        // 单位方格中的物体，只检测材料和工具
        hitcollidersForward = Physics.OverlapBox(new Vector3(vecPickOrLoad.x, 0.5f, vecPickOrLoad.z), BoxExtents, Quaternion.identity, LayerMask.GetMask("Material", "Tool"));

        if (hitcollidersForward.Length >= 1) // 地上有东西
        {
            // 查找最近的物体
            closestObject = hitcollidersForward[0].gameObject;

            int i = 1;
            
            while (i < hitcollidersForward.Length)
            {
                // 比较距离，如果距离更近，则替换closestObject
                if (Vector3.Distance(transform.position, hitcollidersForward[i].transform.position) < Vector3.Distance(transform.position, closestObject.transform.position))
                {
                    closestObject = hitcollidersForward[i].gameObject;
                }
                i++;
            }

            // 根据角色手的状态捡起或放下工具或者材料
            GameObject itemOnGround = closestObject.gameObject; // 地上的工具或材料
            bool isTool = (itemOnGround.tag == "Tool");
            switch (tm.handState)// 角色的手的状态
            {
                case ToolManager.HANDSTATE.EMPTY: // 手里没有东西，可以拿工具或者材料
                    if (isTool)
                    {
                        tm.EquipTool(itemOnGround);
                    }
                    else
                    {
                        tm.EquipMaterial(itemOnGround);

                        if (itemOnGround.name == "Rail")
                            tm.handState = ToolManager.HANDSTATE.RAIL;
                    }

                    break;
                case ToolManager.HANDSTATE.TOOL: // 手里有工具，先放下工具，再拿起工具或者材料
                    if (isTool)
                    {
                        tm.UnloadTool();
                        tm.EquipTool(itemOnGround);
                    }
                    else
                    {
                        tm.UnloadTool();
                        tm.EquipMaterial(itemOnGround);
                    }
                    break;
                case ToolManager.HANDSTATE.MATERIAL: // 手里有材料，先放下材料，再拿起工具或者材料
                    if (isTool) // Todo : 判断手里材料类型
                    {
                        tm.UnloadMaterial();
                        tm.EquipTool(itemOnGround);
                    }
                    else
                    {
                        if (tm.itemInHand.name == itemOnGround.name) // 如果是同种材料则堆叠
                        {
                            tm.PileMaterial(itemOnGround);
                        }
                        else // 如果是不同种材料，则先放下材料，再拿起材料
                        {
                            tm.UnloadMaterial();
                            tm.EquipMaterial(itemOnGround);
                        }
                    }
                    break;
                default:
                    break;
            }

        }
        else // 方格内的无工具或材料，即空地，可以放东西
        {
            switch (tm.handState)// 角色的手的状态
            {
                case ToolManager.HANDSTATE.EMPTY: // 手里没有东西，什么事也不做
                    Debug.Log("Nothing to pick up");
                    break;
                case ToolManager.HANDSTATE.TOOL: // 放下工具
                    tm.UnloadTool();
                    break;
                case ToolManager.HANDSTATE.MATERIAL: // 放下材料
                    Transform mat = tm.rightHand.transform.GetChild(0);
                    if (mat.name == "Wood")
                    {
                        Transform wood = tm.GetWood();
                        if (!gameManager.BuildBridge(wood))
                        {
                            tm.UnloadMaterial();
                        }
                        else
                        {
                            tm.LessenMatNum();
                        }
                    }
                    else
                    {
                        tm.UnloadMaterial();
                    }
                    break;
                case ToolManager.HANDSTATE.RAIL:
                    Transform rail = tm.GetRail();
                    if (!gameManager.PutRail(rail.gameObject))
                    {
                        tm.UnloadMaterial();
                    }
                    else
                    {
                        tm.LessenMatNum();
                    }
                    break;

                default:
                    break;
            }
        }

    }


    // 检测资源块与工具匹配与否
    bool Match(Resource resblock, GameObject tool)
    {
        // 获取当前资源块的标签
        string Restag = resblock.gameObject.tag;
        // 若资源块标签与手中道具匹配则返回真
        if ((Restag == "Tree" && tool.name == "Axe")
            || Restag == "Mine" && tool.name == "Mattock")
            return true;
        else
            return false;
    }
}
