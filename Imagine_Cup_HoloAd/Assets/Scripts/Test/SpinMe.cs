using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinMe : MonoBehaviour {
    public float speed;
    public float range;

    private float time = 0f;
	// Update is called once per frame
	void Update () {
        time += Time.deltaTime*(float)System.Math.PI/speed;
        transform.Rotate(0, speed, 0);
        transform.Translate(0, range * (float)System.Math.Sin(time), 0);
        gameObject.transform.Rotate(0, speed, 0);
        transform.Translate(new Vector3(0,0,1*Time.deltaTime));
        //this is awesome hehe
        //new line
	}
}
