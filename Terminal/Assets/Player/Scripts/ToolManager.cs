using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ToolManager : MonoBehaviour {

    // Variable
    public GameObject rightHand; // 右手定位点
    public Transform PlayerTrans; // 角色位置
    public GameObject itemInHand; // 手中的物体

    public enum HANDSTATE
    {
        EMPTY,TOOL,MATERIAL,RAIL
    }
    public HANDSTATE handState;

	// Use this for initialization
	void Start () {
        try
        {
            // 获取角色右手GameObject
            rightHand = transform.DeepFind("RightHand").gameObject;
            // 获取角色的transform
            PlayerTrans = GetParent(transform);
        }
        catch (System.Exception)
        {
            throw;
        }
    }


    /// <summary>
    /// 捡起工具
    /// </summary>
    /// <param name="item"></param>
    public void EquipTool(GameObject item)
    {
        //itInHand.Add(item.tag);
        handState = HANDSTATE.TOOL;
        // 关闭工具的碰撞框，不需要再检测
        item.GetComponent<BoxCollider>().enabled = false; 
        // 设置父物件为右手
        item.transform.SetParent(rightHand.transform);

        // 根据标签或者名称设置相对位置
        if (item.tag == "Axe" || item.name == "Axe")
        {   // 位置
            item.transform.localPosition = new Vector3(0.0055f, 0.0007f, 0.0034f);
            // 旋转
            item.gameObject.transform.localRotation = Quaternion.Euler(new Vector3(90, 90, 0));

        }
        else if(item.tag == "Mattock"|| item.name == "Mattock")
        {   // 位置
            item.transform.localPosition = new Vector3(0.0066f,0.0006f,0.0029f);
            // 旋转
            item.gameObject.transform.localRotation = Quaternion.Euler(new Vector3(90, 90, 0));
        }
        else {
            Debug.Log("Tool's tag error. Can not give tool a local position");
        }
        
        itemInHand = item;
    }


    /// <summary>
    /// 捡起材料
    /// </summary>
    /// <param name="item"></param>
    public void EquipMaterial(GameObject item)
    {
        //itInHand.Add(item.tag);
        handState = HANDSTATE.MATERIAL;
        // 设置父物件为右手
        item.transform.SetParent(rightHand.transform);
        // 关闭材料的碰撞框，不需要再检测
        item.GetComponent<BoxCollider>().enabled = false;

        // 设置相对位置
        if (item.name=="Rail")
        {
            //铁轨的相对位置
            item.transform.localPosition = new Vector3(0.0044f, 0.0069f, 0.0078f);
        }
        else
        {   
            // 材料的相对位置
            item.transform.localPosition = new Vector3(0.0045f, 0.0015f, 0.0043f);
        }
       
        // 旋转为初始值
        item.transform.localRotation = Quaternion.AngleAxis(90, Vector3.right);

        itemInHand = item;
    }

    /// <summary>
    /// 捡起相同的材料
    /// </summary>
    /// <param name="item">材料</param>
    /// <param name="n">手中已经有n个材料</param>
    public void EquipSameMaterial(GameObject itemOnGround, int n)
    {
        // handState = HANDSTATE.MATERIAL;
        // 设置父物件为右手
        itemInHand = rightHand.transform.GetChild(0).gameObject;
        itemOnGround.transform.SetParent(itemInHand.transform);
        Resmaterial res = itemInHand.GetComponent<Resmaterial>();

        // 手里材料的数量
        res.Size += itemOnGround.GetComponent<Resmaterial>().Size;

        // 关闭材料的碰撞框，不需要再检测
        itemOnGround.GetComponent<BoxCollider>().enabled = false;
        // 设置相对位置
        itemOnGround.transform.localPosition = new Vector3(0, 0.2f * n, 0);
        // 旋转为初始值
        itemOnGround.transform.localRotation = Quaternion.identity;
    }


    /// <summary>
    /// 放下工具
    /// </summary>
    public void UnloadTool()
    {
        handState = HANDSTATE.EMPTY;
        Transform itemTrans = rightHand.transform.GetChild(0);
        // 与父物件分离
        itemTrans.SetParent(GameObject.Find("MovableStuff").transform);
        Vector3 vector3 = PlayerTrans.position;
        int x = Mathf.RoundToInt(vector3.x), z = Mathf.RoundToInt(vector3.z);
        // 设置相对位置
        itemTrans.position = new Vector3(x, 1, z);
        // 旋转为
        itemTrans.localRotation = Quaternion.identity;
        // 开启碰撞框
        itemTrans.GetComponent<BoxCollider>().enabled = true;
        itemInHand = null;
    }

    /// <summary>
    /// 放下材料
    /// </summary>
    public void UnloadMaterial()
    {
        handState = HANDSTATE.EMPTY;
        // 与父物件分离
        Transform itemTrans = rightHand.transform.GetChild(0);
        itemTrans.SetParent(GameObject.Find("MovableStuff").transform);
        Vector3 vector3 = PlayerTrans.position + PlayerTrans.forward * 0.5f;
        int x = Mathf.RoundToInt(vector3.x), z = Mathf.RoundToInt(vector3.z);
        // 设置相对位置
        itemTrans.position = new Vector3(x, 1, z);
        itemTrans.localRotation = Quaternion.identity;
        // 开启碰撞框
        itemTrans.GetComponent<BoxCollider>().enabled = true;
        // 解绑itemInHand
        itemInHand = null;
    }

    /// <summary>
    /// 用于把材料装入车厢后，判断手里是否还有材料，更新手的状态
    /// </summary>
    public void UpdateMatrial()
    {
        Resmaterial[] materials = transform.GetComponentsInChildren<Resmaterial>();
        if (materials.Length==0)
        {
            // 修改右手的状态，改为EMPTY
            handState = HANDSTATE.EMPTY;
        }
    }

    /// <summary>
    /// 从手中取一块木头
    /// </summary>
    public Transform GetWood()
    {
        // 与父物件分离
        Transform itemTrans = rightHand.transform.GetChild(0);
        if (itemTrans.Find("Wood") != null)
            return itemTrans.Find("Wood");
        else
            return itemTrans;
    }


    /// <summary>
    /// 从手中取一块铁轨
    /// </summary>
    public Transform GetRail()
    {
        //handState = HANDSTATE.RAIL;
        // 与父物件分离
        Transform itemTrans = rightHand.transform.GetChild(0);
        if (itemTrans.Find("Rail") != null)
            return itemTrans.Find("Rail");
        else
            return itemTrans;
    }

    /// <summary>
    /// 减少手中材料数量
    /// </summary>
    public void LessenMatNum()
    {
        Resmaterial[] materials = rightHand.GetComponentsInChildren<Resmaterial>();
        if (materials.Length != 0)
        {
            itemInHand = rightHand.transform.GetChild(0).gameObject;
            itemInHand.GetComponent<Resmaterial>().Size--;
        }
        else
        {
            handState = HANDSTATE.EMPTY;
        }
    }

    public void PileMaterial(GameObject itemOnGround)
    {
        // 手上物体的size
        Resmaterial rmInHand = itemInHand.GetComponent<Resmaterial>();

        // 获取地上的材料的脚本
        Resmaterial rmOnGround = itemOnGround.GetComponent<Resmaterial>();

        if (rmOnGround.Size + rmInHand.Size <= rmInHand.Capacity) // 如果总数不超过3，则可以堆叠
        {
            handState = HANDSTATE.EMPTY;


            // 获取手里的所有材料
            Resmaterial[] materials = rightHand.GetComponentsInChildren<Resmaterial>();

            for (int i = 0; i < materials.Length; i++)
            {
                // 成为地上材料的子物体
                materials[i].transform.SetParent(itemOnGround.transform);
                // 设置相对位置
                materials[i].transform.localPosition = new Vector3(0, 0.5f * rmOnGround.Size, 0);
                materials[i].transform.localRotation = Quaternion.identity;
                // 地上物体的size增加
                rmOnGround.Size++;

            }

            // 开启碰撞框
            itemInHand.transform.GetComponent<BoxCollider>().enabled = false;
            // 解绑itemInHand
            itemInHand = null;
        }
        else
        {
            UnloadMaterial();
            EquipMaterial(itemOnGround);
        }
    }
 
    /// <summary>
    /// 寻找根结点
    /// </summary>
    /// <param name="child"></param>
    /// <returns></returns>
    private Transform GetParent(Transform child)
    {
        if (child.parent)
        {
            return GetParent(child.parent);
        }
        else
        {
            return child;
        } 
    }
}
