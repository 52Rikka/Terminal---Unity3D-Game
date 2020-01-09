using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
public class CreateMap : MonoBehaviourPunCallbacks
{

    // Variables
    public static GameObject[,] Cubes = new GameObject[30, 40];
    public static string[,] CubesType = new string[30, 40];

	// Use this for initialization
	void Start () {

        transform.Find("Ground").SendMessage("Create");
        transform.Find("Environment").SendMessage("Create");


    }
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKey(KeyCode.Escape))
        {
            SceneManager.LoadScene("Menu");
        }
	}

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        if (!photonView.IsMine)
            return;
        GameObject Player = PhotonNetwork.Instantiate("Player", new Vector3(11, 1, 5), Quaternion.identity, 0);
        Player.name = "Player";
    }
}
