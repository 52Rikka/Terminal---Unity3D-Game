using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class CreateGround : MonoBehaviour {

    // Variables
    public GameObject CubeGrass;
    public GameObject CubeStone;
    public GameObject CubeTree;
    public GameObject CubeMine;
    public GameObject CubeRiver;
    public GameObject[,] Cubes = new GameObject[30, 40];
    public string[,] CubesType = new string[30, 40];



    public class Border
    {
        public bool exist;
        public bool east, north, west, south;
        public bool[] direct;
        public Border()
        {
            exist = false;
            direct = new bool[4];
            direct[0] = east = false;
            direct[1] = north = false;
            direct[2] = west = false;
            direct[3] = south = false;
        }
    };
    public Border[,] cubesBorder = new Border[30,40];



    // Use this for initialization
    void Start () {
		
       

	}

	// Update is called once per frame
	void Update () {
		
	}

    public void Create ()
    {
        int grass = 0;
        int stone = 0;
        int tree = 0;
        int mine = 0;
        int river = 0;

        string[] strs = File.ReadAllLines("./Assets/Map-Pass1/Ground/Ground.txt", Encoding.UTF8);

        foreach (var str in strs)
        {
            string[] s = str.Split(' ');
            int type = int.Parse(s[0]), row = int.Parse(s[1]), column = int.Parse(s[2]), num = int.Parse(s[3]);
            GameObject Cube = null;
            string stype = "";
            switch (type)
            {
                case 1:
                    Cube = CubeGrass;
                    grass += num;
                    stype = "Grass";
                    break;
                case 2:
                    Cube = CubeStone;
                    stone += num;
                    stype = "Stone";
                    break;
                case 3:
                    Cube = CubeTree;
                    tree += num;
                    stype = "Tree";
                    break;
                case 4:
                    Cube = CubeMine;
                    mine += num;
                    stype = "Mine";
                    break;
                case 5:
                    Cube = CubeRiver;
                    river += num;
                    stype = "River";
                    break;
            }

            for (int i = 0; i < num; i++)
            {
                Cube = Instantiate(Cube, new Vector3(row, 0, column + i), Quaternion.identity);
                Cube.transform.parent = transform;
                Cube.name = "Cube" + stype + "_" + row.ToString() + "_" + (column + i).ToString();
                Cubes[row, column + i] = Cube;
                CubesType[row, column + i] = stype;
            }
        }

        //print("grass : " + grass);
        //print("stone : " + stone);
        //print("tree : " + tree);
        //print("soil : " + mine);
        //print("river : " + river);

        CreateMap.Cubes = Cubes;
        CreateMap.CubesType = CubesType;



        SetBorder();
    }

    void SetBorder()
    {
        for (int ii = 0; ii < 30; ii++)
            for (int j = 0; j < 40; j++)
                cubesBorder[ii, j] = new Border();

        string[] strs = File.ReadAllLines("./Assets/Map-Pass1/Ground/Border.txt", Encoding.UTF8);
        int i = 0;
        while (i < strs.Length)
        {
            for (int j = 1; j <= 25; j++)
            {
                string[] s = strs[i + j].Split(' ');
                int count = s.Length, x = int.Parse(s[0]);
                for (int k = 1; k < count; k++)
                {
                    int y = int.Parse(s[k]);
                    cubesBorder[x, y].exist = true;
                    cubesBorder[x, y].direct[i/26] = true;

                }          
            }
            i += 26;
        }

        //GameManager.border = cubesBorder;
        //GameManager.start = true;
    }
}
