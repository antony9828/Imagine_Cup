using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoloAd : MonoBehaviour
{
    public GameObject room;
    public GameObject spatialMapping;

    public GameObject prefabMatrixCell;

    public float matrixScale = 1f;

    public float minVisibleSatisfiability = .5f;

    public int maxX = 0, maxY = 0, maxZ = 0;


    public MeshAnalyzer meshAnalyzer;

    private List<GameObject> advObjects = new List<GameObject>();

    void Start()
    {
        
        gameObject.AddComponent<MeshAnalyzer>();

        
        meshAnalyzer = gameObject.GetComponent<MeshAnalyzer>();

    }

    public void RecalculateRoom()
    {
        Vector3 matSize = room.transform.localScale / matrixScale;

        maxX = (int)System.Math.Ceiling(matSize.x);
        maxY = (int)System.Math.Ceiling(matSize.y);
        maxZ = (int)System.Math.Ceiling(matSize.z);

        meshAnalyzer.RecalculateMesh(this);
    }

    public void AddAdvObject(GameObject obj)
    {
        advObjects.Add(obj);
    }



    public int CalcIndex(int x, int y, int z)
    {
        return x + maxX * y + maxX * maxY * z;
    }

    public int CalcIndex(Vector3Int cord)
    {
        return CalcIndex(cord.x, cord.y, cord.z); ;
    }

    public int GetLength()
    {
        return maxX * maxY * maxZ;
    }

    public bool IsCellInMatrix(int x, int y, int z)
    {
        return (x < maxX && x >= 0 && y < maxY && y >= 0 && z < maxZ && z >= 0);
    }

    public bool IsCellInMatrix(Vector3Int cord)
    {
        return IsCellInMatrix(cord.x, cord.y, cord.z);
    }

    public struct Criterion
    {
        //0 - пустой критерий. ничего не делает.
        //1 - Расстояние до пола (dMin, dMax)
        //2 - Расстояние до потолка (dMin, dMax)
        //3 - Количество и толщина препятствий под объектом (kMin, kMax, thMin, thMax)
        //4 - Количество и толщина препятствий над объектом (kMin, kMax, thMin, thMax)
        //5 - Занимаемый объектом объем (x, y, z)
        //6 - Нормаль поверхности, к которой прилегает объект (x, y, z, er)
        public Criterion(int type, Vector4 data, float importance)
        {
            this.type = type;
            this.data = data;
            this.importance = importance;
        }
        public int type;
        public Vector4 data;
        public float importance;
    }

    // Update is called once per frame
    void Update ()
    {
	    if (Input.GetKeyDown("space"))
        {
            Debug.Log("kek");
            foreach (GameObject adv in advObjects)
            {
                adv.GetComponent<AdvObject>().ShowCriterionOnGraph();
                adv.transform.position = adv.GetComponent<AdvObject>().FindAPlace();
            }
        }
	}
}
