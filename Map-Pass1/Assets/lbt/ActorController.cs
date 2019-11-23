using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorController : MonoBehaviour {

    public GameObject model;
    public PlayerInput pi;
    public float walkSpeed = 1.4f;
    public float runMultiplier = 2.0f;

    [SerializeField] // 显示到编辑器中 必须是编辑器资源类型
    private Animator anim;
    private Rigidbody rigid;
    private Vector3 movingVec; // 保存玩家输入

	// Use this for initialization
	void Awake () { // 在Awake中获取所需要的组件
        pi = GetComponent<PlayerInput>();
        anim = model.GetComponent<Animator>();
        rigid = GetComponent<Rigidbody>(); // 不可放在Update中
        // if (rigid == null) { } // 防止未添加组件
	}
	
	// Update is called once per frame
	void Update () {  // Time.deltaTime 1/60
        anim.SetFloat("forward", pi.Dmag * Mathf.Lerp(anim.GetFloat("forward"), ((pi.run) ? 2.0f : 1.0f), 0.5f));
        if (pi.Dmag > 0.1f)
        {
            model.transform.forward = Vector3.Slerp(model.transform.forward, pi.Dvec, 0.3f);
        }
        movingVec = pi.Dmag * model.transform.forward * walkSpeed * ((pi.run) ? runMultiplier : 1.0f);
    }


    // 物理引擎
    void FixedUpdate() // Time.fixedDeltaTime 1/50
    {
        rigid.position += movingVec * Time.fixedDeltaTime;
    }
}
