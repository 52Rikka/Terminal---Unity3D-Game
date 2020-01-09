using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AsyncLoadScene : MonoBehaviour {


    public Text LoadText;
    public Slider LoadSlider;
    private AsyncOperation async = null;

    void Start()
    {
        if (SceneManager.GetActiveScene().name == "LoadMap")
        {
            //启动协程
            StartCoroutine(LoadScene());
        }

    }

    //异步加载
    IEnumerator LoadScene()
    {
        async = SceneManager.LoadSceneAsync("Map-Pass1");
        async.allowSceneActivation = false;

        
        yield return async;
    }


    // Use this for initialization
    //void Start () {

    //}

    // Update is called once per frame
    void Update () {

        if (async != null && !async.isDone)
        {
            if (LoadSlider.value < 0.9f)
            {
                if (LoadSlider.value < async.progress)
                {
                    LoadSlider.value += Time.deltaTime;
                    LoadText.text = (int)(LoadSlider.value * 100) + "%";
                }
            }
            else
            {
                if (LoadSlider.value < 1.0f)
                {
                    LoadSlider.value += Time.deltaTime;
                    LoadText.text = (int)(LoadSlider.value * 100) + "%";
                }
                else //if (Input.GetKey(KeyCode.Space))
                {
                    async.allowSceneActivation = true;
                }
            }
        
        
        }
    }
}
