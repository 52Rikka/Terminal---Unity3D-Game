using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;

public class CreateMenuEnvironment : MonoBehaviour {

    // Use this for initialization
    public GameObject[] Stones1;
    public GameObject[] Stones2;
    public GameObject[] Stones3;
    public GameObject[] Stones4;
    public GameObject[] Mines;
    public GameObject[] Face_cubes;
    private GameObject Stone;
    private string[,] CubesType = CreateMenu.CubesType;
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Create()
    {
        CubesType = CreateMenu.CubesType;

        System.Random random = new System.Random();

        for (int row = 0; row < 30; row++)
            for (int column = 0; column < 40; column++)
            {
                if (CubesType[row, column] == null) continue;

                if (CubesType[row, column].Equals("Ground2_stone1"))
                {

                    int i = random.Next(1, 13);
                    Stone = Instantiate(Stones1[i - 1], new Vector3(row, 1, column), Quaternion.identity);
                    Stone.transform.parent = transform;
                    Stone.name = "Tree_" + row + "_" + column;
                }
                else if (CubesType[row, column].Equals("Ground2_stone2"))
                {

                    int i = random.Next(1, 3);
                    Stone = Instantiate(Stones2[i - 1], new Vector3(row, 1, column), Quaternion.identity);
                    Stone.transform.parent = transform;
                    Stone.name = "Tree_" + row + "_" + column;
                }
                else if (CubesType[row, column].Equals("Ground2_stone3"))
                {

                    int i = random.Next(1, 3);
                    Stone = Instantiate(Stones3[i - 1], new Vector3(row, 1, column), Quaternion.identity);
                    Stone.transform.parent = transform;
                    Stone.name = "Tree_" + row + "_" + column;
                }
                else if (CubesType[row, column].Equals("Ground2_stone4"))
                {

                    int i = random.Next(1, 1);
                    Stone = Instantiate(Stones4[i - 1], new Vector3(row, 1, column), Quaternion.identity);
                    Stone.transform.parent = transform;
                    Stone.name = "Tree_" + row + "_" + column;
                }
                else if (CubesType[row, column].Equals("Ground5_shawllow_stone"))
                {

                    int i = random.Next(1, 15);
                    Stone = Instantiate(Mines[i - 1], new Vector3(row, 1, column), Quaternion.identity);
                    Stone.transform.parent = transform;
                    Stone.name = "Tree_" + row + "_" + column;
                }
                else if (CubesType[row, column].Equals("Ground3_redstone"))
                {

                    int i = random.Next(1, 7);
                    Stone = Instantiate(Face_cubes[i - 1], new Vector3(row, 1, column), Quaternion.identity);
                    Stone.transform.parent = transform;
                    Stone.name = "Tree_" + row + "_" + column;
                }
            }
    }
}
