using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdvObject : MonoBehaviour
{
    public GameObject objectWitHoloAdConfigScript;

    public bool visualizeMatrix = false;

    public int[] criterionType;
    public Vector4[] criterionData;
    public float[] criterionImportance;

    public Visualization visualization;

    private HoloAd config;

    void Start()
    {
        config = objectWitHoloAdConfigScript.GetComponent<HoloAd>();

        if (visualizeMatrix)
            gameObject.AddComponent<Visualization>();

        visualization = gameObject.GetComponent<Visualization>();
        visualization.SetConfig(config);

        config.AddAdvObject(gameObject);
    }

    private HoloAd.Criterion[] MergeUnityInput()
    {
        int len = criterionData.Length;
        HoloAd.Criterion[] result = new HoloAd.Criterion[len];
        for (int i = 0; i < len; i++)
        {
            result[i] = new HoloAd.Criterion(criterionType[i], criterionData[i], criterionImportance[i]);
        }
        return (result);
    }

    public void ShowCriterionOnGraph()
    {
        config.RecalculateRoom();
        if (visualizeMatrix)
        {
            Satisfiability satMap = new Satisfiability(config);
            satMap.TestCriterion( MergeUnityInput(), config);
            visualization.ShowSatisfiabilityGraph(satMap, config);
        }
    }

    public Vector3 FindAPlace()
    {
        Satisfiability satMap = new Satisfiability(config);
        satMap.TestCriterion(MergeUnityInput(), config);

        for (int x = 0; x < satMap.maxX; x++)
            for (int y = 0; y < satMap.maxY; y++)
                for (int z = 0; z < satMap.maxZ; z++)
                {
                    if (satMap.cells[satMap.CalcIndex(x,y,z)] > config.minVisibleSatisfiability)
                    {
                        return config.room.transform.TransformPoint(new Vector3(
                            2f * (x - .5f * satMap.maxX) * config.matrixScale / satMap.maxX,
                            2f * (y - .5f * satMap.maxY) * config.matrixScale / satMap.maxY,
                            2f * (z - .5f * satMap.maxZ) * config.matrixScale / satMap.maxZ
                            ));
                    }
                }

        return new Vector3(0, 0, 0);
    }
}


