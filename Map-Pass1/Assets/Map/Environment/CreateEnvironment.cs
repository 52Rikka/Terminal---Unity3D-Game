using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class CreateEnvironment : MonoBehaviour {

    // Variables
    private string[,] CubesType = CreateMap.CubesType;
    public GameObject Train;
    public GameObject Station;
    public GameObject StoneHeap;
    public GameObject Grass;
    public GameObject[] Trees;
    public GameObject[] Mines;
    private GameObject Tree;
    private GameObject Mine;

    // Use this for initialization
    void Start () {

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Create ()
    {
        CubesType = CreateMap.CubesType;

        Train = Instantiate(Train);
        Train.name = "Train";

        Station = Instantiate(Station);
        Station.name = "Station";

        StoneHeap = Instantiate(StoneHeap);
        StoneHeap.transform.parent = transform;
        StoneHeap.name = "StoneHeap";

        string[] strs = File.ReadAllLines("./Assets/Map/Environment/Grass.txt", Encoding.UTF8);
        foreach (var str in strs)
        {
            string[] s = str.Split(' ');
            int row = int.Parse(s[0]), column = int.Parse(s[1]);
            Grass = Instantiate(Grass, new Vector3(row, 1.0001f, column), Quaternion.identity);
            Grass.transform.parent = transform.Find("Grass");
            Grass.name = "Grass_" + s[0] + "_" + s[1];
        }

        System.Random random = new System.Random();

        for (int row = 0; row < 30; row++)
            for (int column = 0; column < 40; column++)
            {
                if (CubesType[row, column] == null) continue;

                if (CubesType[row, column].Equals("Tree"))
                {

                    int i = random.Next(1, 26);
                    Tree = Instantiate(Trees[i - 1], new Vector3(row, 1.0001f, column), Quaternion.identity);
                    Tree.transform.parent = transform.Find("Trees");
                    Tree.name = "Tree_" + row + "_" + column;
                }
                else if (CubesType[row, column].Equals("Mine"))
                {
                    int i = random.Next(1, 15);
                    Mine = Instantiate(Mines[i - 1], new Vector3(row, 1, column), Quaternion.identity);
                    Mine.transform.parent = transform.Find("Mines");
                    Mine.name = "Mine_" + row + "_" + column;
                }
            }
    }
}
