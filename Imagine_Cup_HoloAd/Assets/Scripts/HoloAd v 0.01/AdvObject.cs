using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdvObject : MonoBehaviour
{
    public GameObject objectWitHoloAdConfigScript;

    public int[] criterionType;
    public Vector4[] criterionData;
    public float[] criterionImportance;

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

    private void Start()
    {
        objectWitHoloAdConfigScript.GetComponent<HoloAd>().ShowCriterionOnGraph(MergeUnityInput());
    }
}
