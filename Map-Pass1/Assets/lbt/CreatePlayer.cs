using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatePlayer : MonoBehaviour {

    // Variables
    public GameObject Player;

	// Use this for initialization
	void Start () {
        Player = Instantiate(Player, new Vector3(13, 1, 3), Quaternion.identity);
        Player.transform.parent = transform;
        Player.name = "player_1";
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
