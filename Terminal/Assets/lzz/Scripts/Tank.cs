using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tank : MonoBehaviour {

    // 车厢容量
    public int WoodCapacity;
    public int IronCapacity;


    // 车厢存储资源块队列
    public List<Resmaterial> WoodStorage = new List<Resmaterial>();
    public List<Resmaterial> IronStorage = new List<Resmaterial>();

 //   // Use this for initialization
 //   void Start()
 //   {

 //   }

    // 储藏车厢存储材料
    public void StoreMat(Resmaterial material)
    {
        string MatName = material.name;

        if(MatName=="Wood")
        {   // 车厢已满则返回
            if (WoodStorage.Count == WoodCapacity)
                return;
            else if(WoodStorage.Count == 0)
            {   // 将当前资源块放到车厢底
                material.transform.parent = gameObject.transform;
                material.transform.localPosition = new Vector3(0, 0.7f, -0.25f);
                material.transform.localEulerAngles = Vector3.zero;
                material.transform.localScale = new Vector3(1, 1, 0.5f);
                WoodStorage.Add(material);
            }
            else
            {   // 将当前资源块摞上去
                material.transform.parent = gameObject.transform;
                material.transform.localPosition = WoodStorage[WoodStorage.Count - 1].transform.localPosition + new Vector3(0, 0.2f, 0);
                material.transform.localEulerAngles = Vector3.zero;
                material.transform.localScale = new Vector3(1, 1, 0.5f);
                WoodStorage.Add(material);
            }
        }
        else
        {   // 车厢已满则返回
            if (IronStorage.Count == IronCapacity)
                return;
            else if (IronStorage.Count == 0)
            {   // 将当前资源块放到车厢底
                material.transform.parent = gameObject.transform;
                material.transform.localPosition = new Vector3(0, 0.7f, 0.05f);
                material.transform.localEulerAngles = new Vector3(0, 0, 0);
                material.transform.localScale = new Vector3(1, 1, 0.5f);
                IronStorage.Add(material);
            }
            else
            {   // 将当前资源块摞上去
                material.transform.parent = gameObject.transform;
                material.transform.localPosition = IronStorage[IronStorage.Count - 1].transform.localPosition + new Vector3(0, 0.2f, 0);
                material.transform.localEulerAngles = new Vector3(0, 0, 0);
                material.transform.localScale = new Vector3(1, 1, 0.5f);
                IronStorage.Add(material);
            }
        }
    }

    // 生成铁轨消耗资源各一份
    public void RemoveMat(int label)
    {
        if (label != 7)
        {
            Destroy(WoodStorage[WoodStorage.Count - 1].gameObject);
            Destroy(IronStorage[IronStorage.Count - 1].gameObject);
        }
        else
        {
            WoodStorage.RemoveAt(WoodStorage.Count - 1);
            IronStorage.RemoveAt(IronStorage.Count - 1);
        }
    }
}
