using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinMe : MonoBehaviour {
    public float speed;

	// Update is called once per frame
	void Update () {
        gameObject.transform.Rotate(0, speed, 0);
        transform.Translate(new Vector3(0,0,1*Time.deltaTime));
        //this is awesome hehe
	}
}
