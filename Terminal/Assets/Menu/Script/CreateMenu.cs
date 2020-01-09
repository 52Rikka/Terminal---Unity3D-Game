using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateMenu : MonoBehaviour {

    // Use this for initialization
    public GameObject Menu;
    public static GameObject[,] Cubes = new GameObject[30, 40];
    public static string[,] CubesType = new string[30, 40];
	void Start () {
        Menu = Instantiate(Menu);
        Menu.name = "menu";
        GameObject.Find("Ground").SendMessage("Create");
        GameObject.Find("Environment").SendMessage("Create");
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
