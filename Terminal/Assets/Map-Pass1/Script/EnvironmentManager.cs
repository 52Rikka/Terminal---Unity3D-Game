using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;


public class EnvironmentManager : MonoBehaviourPunCallbacks
{

    public Transform alltrees;
    public Transform allmines;
    Queue<Vector2> tq, mq;

	// Use this for initialization
	void Start () {
        tq = new Queue<Vector2>();
        mq = new Queue<Vector2>();
        int tn = 0, mn = 0;

        Transform Ground = GameObject.Find("Ground").transform;
        foreach (Transform cube in Ground)
        {
            int x = (int)cube.position.x, z = (int)cube.position.z;
            string[] cname = cube.name.Split('_');
            switch (cname[0])
            {
                case "CubeTree": tq.Enqueue(new Vector2(x, z)); break;
                case "CubeMine": mq.Enqueue(new Vector2(x, z)); break;
            }
            cube.name = cname[0] + "_" + x + "_" + z;
        }

        System.Random random = new System.Random();


        Transform trees = transform.Find("Trees");
        GameObject[] tgo = new GameObject[30];
        foreach (Transform tree in alltrees)
        {
            tgo[tn] = tree.gameObject;
            tn++;
        }
        print("tn: " + tn);

        while (tq.Count > 0)
        {
            Vector2 vector2 = tq.Dequeue();
            int temp = random.Next(0, tn);

            GameObject newtree = Instantiate(tgo[temp]);
            newtree.transform.parent = trees;
            newtree.transform.position = new Vector3(vector2.x, 1, vector2.y);
            newtree.name = "Tree_" + (int)vector2.x + "_" + (int)vector2.y;
            newtree.AddComponent<PhotonTransformView>();
        }

        Transform mines = transform.Find("Mines");
        GameObject[] mgo = new GameObject[20];
        foreach (Transform mine in allmines)
        {
            mgo[mn] = mine.gameObject;
            mn++;
        }
        print("mn: " + mn);

        while (mq.Count > 0)
        {
            Vector2 vector2 = mq.Dequeue();
            int temp = random.Next(0, mn);
            
            GameObject newmine = Instantiate(mgo[temp]);
            newmine.transform.parent = mines;
            newmine.transform.position = new Vector3(vector2.x, 1, vector2.y);
            newmine.name = "Mine_" + (int)vector2.x + "_" + (int)vector2.y;
            newmine.AddComponent<PhotonTransformView>();
        }

    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
