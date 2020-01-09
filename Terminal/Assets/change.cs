using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class change : MonoBehaviourPunCallbacks
{

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    [PunRPC]
    void a(string name)
    {
        //this.transform.SetParent(parent);
        this.GetComponent<BoxCollider>().enabled = false;
        // 设置父物件为右手
        this.transform.SetParent(GameObject.Find(name).transform.GetComponent<ToolManager>().rightHand.transform);
        // 位置
        this.transform.localPosition = new Vector3(0.0055f, 0.0007f, 0.0034f);
        // 旋转
        this.gameObject.transform.localRotation = Quaternion.Euler(new Vector3(90, 90, 0));
    }
}
