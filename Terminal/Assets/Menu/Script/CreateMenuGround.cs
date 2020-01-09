using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.IO;
public class CreateMenuGround : MonoBehaviour {

    // Use this for initialization
    public GameObject Ground1;
    public GameObject Ground2_stone;
    public GameObject Ground3_redstong;
    public GameObject Ground4_magma;
    public GameObject Ground5_shawllow_stone;
    public GameObject[,] Cubes = new GameObject[30, 40];
    public string[,] CubesType = new string[30, 40];
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

	}

    public void Create()
    {
        string[] strs = File.ReadAllLines("./Assets/Menu/1.txt", Encoding.UTF8);
        foreach( var str in strs)
        {
            string[] s = str.Split(' ');
            int type = int.Parse(s[0]), row = int.Parse(s[1]), column = int.Parse(s[2]), num = int.Parse(s[3]);
            GameObject Cube = null;
            string stype = "";
            switch(type)
            {
                case 1:
                    Cube = Ground1;
                    stype = "Ground1";
                    break;
                case 21:
                    Cube = Ground2_stone;
                    stype = "Ground2_stone1";
                    break;
                case 22:
                    Cube = Ground2_stone;
                    stype = "Ground2_stone2";
                    break;
                case 23:
                    Cube = Ground2_stone;
                    stype = "Ground2_stone3";
                    break;
                case 24:
                    Cube = Ground2_stone;
                    stype = "Ground2_stone4";
                    break;
                case 3:
                    Cube = Ground3_redstong;
                    stype = "Ground3_redstone";
                    break;
                case 4:
                    Cube = Ground4_magma;
                    stype = "Ground4_magma";
                    break;
                case 5:
                    Cube = Ground5_shawllow_stone;
                    stype = "Ground5_shawllow_stone";
                    break;

            }
            for(int i=0;i<num;i++)
            {
                Cube = Instantiate(Cube, new Vector3(row, 0, column + i), Quaternion.identity);
                Cube.transform.parent = transform;
                Cube.name = "Cube" + stype + "_" + row.ToString() + "_" + (column + i).ToString();
                Cubes[row, column + i] = Cube;
                CubesType[row, column + i] = stype;
            }
        }
        CreateMenu.Cubes = Cubes;
        CreateMenu.CubesType = CubesType;
    }
}
