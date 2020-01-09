using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundMusic : MonoBehaviour {

    public AudioClip music;
    private AudioSource back;
    // Use this for initialization
    void Start()
    {
        back = this.GetComponent<AudioSource>();
        back.loop = true; //设置循环播放  
        back.volume = 0.5f;//设置音量最大，区间在0-1之间
        back.clip = music;
        back.Play(); //播放背景音乐
    }

    // Update is called once per frame
    void Update()
    {
    }

}
