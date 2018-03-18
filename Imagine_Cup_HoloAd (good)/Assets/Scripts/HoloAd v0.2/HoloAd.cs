using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoloAd : MonoBehaviour {
    [Tooltip("Prefabs of objects will be placed")]
    public GameObject[] advObjectsPrefabs;

    static public GameObject advParent;//parent for adv objects

    private List<GameObject> advObjects;

    public List<GameObject> GetAdvObjects()
    {
        return advObjects;
    }

    // Use this for initialization
    void Start () {
        advParent = new GameObject("AdvObjects");
        advObjects = new List<GameObject>();
        foreach(GameObject prefab in advObjectsPrefabs)
        {
            GameObject adObj = Instantiate(prefab);
            adObj.SetActive(false);
            adObj.transform.parent = gameObject.transform;
            advObjects.Add(adObj);
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
