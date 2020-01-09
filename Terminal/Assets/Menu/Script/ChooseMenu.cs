using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
public class ChooseMenu : MonoBehaviourPunCallbacks
{

    // Variable
    Material inmat;
    Material exmat;
    string choice;
    private bool ex = false;
	// Use this for initialization
	void Start () {

        inmat = transform.Find("internal").GetComponent<Renderer>().material;

    }

    // Update is called once per frame
    void Update () {

	}

    private void OnTriggerEnter(Collider other)
    {
        exmat = transform.Find("external").GetComponent<Renderer>().material;
        transform.Find("internal").GetComponent<Renderer>().material = exmat;
        transform.Find("Text").GetComponent<Renderer>().material.color = inmat.color;
        choice = name;
    }

    private void OnTriggerStay(Collider other)
    {
        if (Input.GetKey(KeyCode.Space))
        {
            switch (choice)
            {
                case "Start_area":
                    {
                        if (!ex&&PhotonNetwork.IsConnected)
                        {
                            ex = true;
                            PhotonNetwork.OfflineMode = false;
                            PhotonNetwork.ConnectUsingSettings();
                            Debug.Log("Welcome");
                        }
                    }
                    break;
                case "Exit_area" : Application.Quit();  break; 
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        transform.Find("internal").GetComponent<Renderer>().material = inmat;
        transform.Find("Text").GetComponent<Renderer>().material.color = exmat.color;
    }

    //public override void OnJoinedRoom()
    //{
    //    Debug.Log("加入房间成功");
    //    base.OnJoinedRoom();
    //    if (!PhotonNetwork.IsMasterClient)
    //      return;
    //    // PhotonNetwork.Instantiate("Worker", new Vector3(11, 1, 5), Quaternion.identity, 0);

    //    // PhotonNetwork.LoadLevel(1);
    //    SceneManager.LoadScene(1);
    //}
}
