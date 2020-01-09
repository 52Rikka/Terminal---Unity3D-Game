using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fire : MonoBehaviour
{
    ParticleSystem particleSystem;
    ParticleSystem.ForceOverLifetimeModule forceMode;
    float fireX,fireZ;
    // Use this for initialization
    public float MoveSpeed;
    void Start()
    {
        MoveSpeed = 2.0f;
        particleSystem = GetComponent<ParticleSystem>();
        forceMode = particleSystem.forceOverLifetime;
        fireX = transform.position.x;//获取火焰的场景X坐标
        fireZ = transform.position.z;//获取火焰的场景Z坐标
    }

    // Update is called once per frame
    void Update()
    {

        ParticleSystem.MinMaxCurve temp = forceMode.x;//获取火焰的物体X坐标
        ParticleSystem.MinMaxCurve temp1 = forceMode.y; //获取火焰的物体Y坐标

        //火焰摆动效果
        if (transform.position.x < fireX)//火焰在场景中发生位移
        {
            if (temp.constantMax < 6.0f)
                temp.constantMax = temp.constantMax + 0.5f;
            forceMode.x = temp;
            fireX = transform.position.x;
        }
        if (transform.position.x > fireX)
        {
            if (temp.constantMax < 6.0f)
                temp.constantMax = temp.constantMax + 0.5f;
            forceMode.x = temp;
            fireX = transform.position.x;
        }

        if (transform.position.z < fireZ)
        {
            if (temp.constantMax < 6.0f)
                temp.constantMax = temp.constantMax + 0.5f;
            forceMode.x = temp;
            fireX = transform.position.x;
        }
        if (transform.position.z > fireZ)
        {
            if (temp.constantMax < 6.0f)
                temp.constantMax = temp.constantMax + 0.5f;
            forceMode.x = temp;
            fireX = transform.position.x;
            fireX = transform.position.x;
        }

        //火焰恢复效果
        if (temp.constantMax > 0)
        {
            temp.constantMax = temp.constantMax - 0.48f;
            forceMode.x = temp;
        }
        else
        {
            temp.constantMax = temp.constantMax + 0.48f;
            forceMode.x = temp;
        }

        if (temp1.constantMax > 0)
        {
            temp1.constantMax = temp1.constantMax - 0.48f;
            forceMode.y = temp1;
        }
        else
        {
            temp1.constantMax = temp1.constantMax + 0.48f;
            forceMode.y = temp1;
        }

    }

    //    void OnGUI()
    //{
    //    if (GUI.Button(new Rect(10, 30, 50, 30), "left"))
    //    {
    //        ParticleSystem.MinMaxCurve temp = forceMode.x;
    //        temp.constantMax = temp.constantMax - 0.5f;
    //        forceMode.x = temp;
    //    }

    //    if (GUI.Button(new Rect(10, 70, 50, 30), "right"))
    //    {
    //        ParticleSystem.MinMaxCurve temp = forceMode.x;
    //        temp.constantMax = temp.constantMax + 0.5f;
    //        forceMode.x = temp;
    //    }

    //    if (GUI.Button(new Rect(10, 110, 50, 30), "big"))
    //    {

    //        particleSystem.startSize = particleSystem.startSize * 11.11f;
    //        particleSystem.startLifetime = particleSystem.startLifetime * 11.11f;
    //    }

    //    if (GUI.Button(new Rect(10, 150, 50, 30), "small"))
    //    {
    //        particleSystem.startSize = particleSystem.startSize * 0.9f;
    //        particleSystem.startLifetime = particleSystem.startLifetime * 0.9f;
    //    }
    //}

}

