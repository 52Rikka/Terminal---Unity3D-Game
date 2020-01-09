using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
public class PlayerInput : MonoBehaviourPunCallbacks
{
    // Variable
    [Header("======  Key Settings  ======")]
    public string keyA = "left shift"; // 加速键
    public string keyB = "space"; // pick or drop

    [Header("======  Output Signals  ======")]
    public float ver;
    public float hor;
    public float Dmag; // magnitude 大小
    public Vector3 Dvec; // 方向

    public int faceDirect;

    // 1.pressing signal
    public bool run;
    // 2.trigger once signal
    public bool pick; // 捡起或者放下

    [Header("======  Others  ======")]
    public bool inputEnabled = true;

    bool[] border = new bool[4];

    // Use this for initialization
    void Start()
    {
        for (int i = 0; i < 4; ++i)
            border[i] = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (PhotonNetwork.InRoom)
        {
            if (!photonView.IsMine && PhotonNetwork.IsConnected)
                return;
        }


        ver = Input.GetAxis("Horizontal");
        hor = -Input.GetAxis("Vertical");


        // 边界设置
        if ((border[0] && ver > 0) || (border[2] && ver < 0)) ver = 0;
        if ((border[1] && hor < 0) || (border[3] && hor > 0)) hor = 0;

        if (ver != 0 && Mathf.Abs(ver) >= Mathf.Abs(hor))
        {
            faceDirect = (ver > 0 ? 0 : 2);
        }
        else if (hor != 0 && Mathf.Abs(ver) < Mathf.Abs(hor))
        {
            faceDirect = (hor < 0 ? 1 : 3);
        }


        if (inputEnabled == false)
        {
            hor = 0;
            ver = 0;
        }
        Vector2 tempDAxis = SquareToCircle(new Vector2(hor, ver));
        float horCircle = tempDAxis.x;
        float verCircle = tempDAxis.y;
        Dmag = Mathf.Sqrt(horCircle * horCircle + verCircle * verCircle);
        //Dvec = DrightCircle * transform.right + DupCircle * transform.forward;
        run = Input.GetKey(keyA);
        pick = Input.GetKeyUp(keyB);
    }


    // 矩形坐标转圆形坐标
    private Vector2 SquareToCircle(Vector2 input)
    {
        Vector2 output = Vector2.zero;
        output.x = input.x * Mathf.Sqrt(1 - (input.y * input.y) / 2.0f);
        output.y = input.y * Mathf.Sqrt(1 - (input.x * input.x) / 2.0f);

        return output;
    }


    public void OnBorder(int direct)
    {
        border[direct] = true;
    }

    public void NotOnBorder(int direct)
    {
        border[direct] = false;
    }
}
