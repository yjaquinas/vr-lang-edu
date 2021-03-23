using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CExclaimRotation : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        transform.Rotate(0, 30f * Time.deltaTime, 0);
    }
}
