using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateMap : MonoBehaviour {

    // Variables
    public static GameObject[,] Cubes = new GameObject[30, 40];
    public static string[,] CubesType = new string[30, 40];

	// Use this for initialization
	void Start () {
        GameObject.Find("Ground").SendMessageUpwards("Create");
        GameObject.Find("Environment").SendMessageUpwards("Create");

    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
