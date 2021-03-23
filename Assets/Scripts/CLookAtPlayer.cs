using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CLookAtPlayer : MonoBehaviour {
    
    [SerializeField] private GameObject _player;
 
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        transform.LookAt(_player.transform);
	}
}
