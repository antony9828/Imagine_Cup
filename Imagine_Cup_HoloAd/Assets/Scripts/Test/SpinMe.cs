using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinMe : MonoBehaviour {
    public float speed;
    public float range;

    private float time = 0f;
	// Update is called once per frame
	void Update () {
<<<<<<< HEAD
        time += (float)System.Math.PI/speed;
        transform.Rotate(0, speed, 0);
        transform.Translate(0, range * (float)System.Math.Sin(time), 0);
	}
}
