using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Visualization : MonoBehaviour
{
    private GameObject prefabMatrixCell;
    private float matrixScale = 1f;
    private float minVisibleSatisfiability = .5f;

    private GameObject matrix;
    private GameObject[] cells = new GameObject[0];

    private int maxX, maxY, maxZ;

    public void SetConfig(HoloAd config)
    {
        this.matrixScale = config.matrixScale;
        this.prefabMatrixCell = config.prefabMatrixCell;
        this.minVisibleSatisfiability = config.minVisibleSatisfiability;
        cells = new GameObject[0];
    }

    public void DestroyVisuals()
    {
        foreach (GameObject cell in cells)
        {
            GameObject.Destroy(cell);
        }
        cells = null;

        GameObject.Destroy(matrix);
        matrix = null;
    }

    public void ReconstructVisuals(HoloAd config)
    {
        DestroyVisuals();

        matrixScale = config.matrixScale;
        this.prefabMatrixCell = config.prefabMatrixCell;

        Vector3 matSize = config.room.transform.transform.lossyScale / config.matrixScale;
        matrix = new GameObject();
        matrix.name = "matrix";

        maxX = (int)System.Math.Ceiling(matSize.x);
        maxY = (int)System.Math.Ceiling(matSize.y);
        maxZ = (int)System.Math.Ceiling(matSize.z);

        matrix.transform.localScale = new Vector3(config.room.transform.lossyScale.x / matSize.x, config.room.transform.lossyScale.y / matSize.y, config.room.transform.lossyScale.z / matSize.z);
        matrix.transform.rotation = config.room.transform.rotation;

        cells = new GameObject[maxX * maxY * maxZ];
        for (int x = 0; x < maxX; x++)
            for (int y = 0; y < maxY; y++)
                for (int z = 0; z < maxZ; z++)
                {
                    GameObject cell = GameObject.Instantiate(prefabMatrixCell);
                    
                    cell.name = "cell: " + x + " " + y + " " + z;
                    cell.transform.position = config.room.transform.position;
                    cell.transform.rotation = config.room.transform.rotation;
                    cell.transform.Translate(new Vector3(
                         -matSize.x / 2f * matrixScale + x * matrixScale + matrixScale / 2f,
                         -matSize.y / 2f * matrixScale + y * matrixScale + matrixScale / 2f,
                         -matSize.z / 2f * matrixScale + z * matrixScale + matrixScale / 2f
                        ));
                    cell.transform.localScale = new Vector3(matrixScale, matrixScale, matrixScale);
                    cell.transform.SetParent(matrix.transform);
                    cells[x + maxX * y + maxX * maxY * z] = cell;
                }
    }

    public void ShowSatisfiabilityGraph(Satisfiability satisfiability, HoloAd config)
    {
        if (satisfiability.GetLength() == maxX * maxY * maxZ)
        {
            for (int index = 0; index < maxX * maxY * maxZ; index++)
            {
                float sat= satisfiability.cells[index];
                if (float.IsNaN(sat))
                {
                    sat = 0f;
                }
                Renderer renderer = cells[index].GetComponent<Renderer>();
                if (sat < config.minVisibleSatisfiability)
                {
                    renderer.enabled = false;
                }
                else
                {
                    renderer.enabled = true;
                    renderer.material.color = new Color(sat, sat, sat, sat);
                }
            }
        } else
        {
            ReconstructVisuals(config);
            ShowSatisfiabilityGraph(satisfiability, config);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}