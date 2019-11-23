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
    public static readonly GameObject[,] Cubes = new GameObject[30, 40];
    public static string[,] CubesType = new string[30, 40];

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

        string[] strs = File.ReadAllLines("./Assets/Map/Ground/Ground.txt", Encoding.UTF8);

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

        print("grass : " + grass);
        print("stone : " + stone);
        print("tree : " + tree);
        print("soil : " + mine);
        print("river : " + river);

        CreateMap.Cubes = Cubes;
        CreateMap.CubesType = CubesType;
    }
}
