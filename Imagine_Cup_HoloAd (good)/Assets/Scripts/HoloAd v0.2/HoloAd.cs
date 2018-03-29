using HoloToolkit.Unity.InputModule;
using HoloToolkit.Unity.InputModule.Tests;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoloAd : MonoBehaviour {
    [Tooltip("Popup menu prefab")]
    public GameObject popupMenu;

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
        advParent.transform.parent = transform;

        advObjects = new List<GameObject>();
        foreach(GameObject prefab in advObjectsPrefabs)
        {
            GameObject adObj = Instantiate(prefab);
            adObj.SetActive(false);
            adObj.AddComponent<TestButton>();
            adObj.AddComponent<CursorModifier>();
            adObj.AddComponent<TogglePopupMenu>();

            GameObject adObjMenu = Instantiate(popupMenu);
            //adObjMenu.SetActive(true);
            TogglePopupMenu component = adObj.GetComponent<TogglePopupMenu>();
            component.popupMenu = adObjMenu.GetComponent<PopupMenu>();
            component.button = adObj.GetComponent<TestButton>();
            adObjMenu.transform.parent = adObj.transform;
            //adObjMenu.transform.position = new Vector3();

            adObj.transform.parent = gameObject.transform;
            advObjects.Add(adObj);
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
