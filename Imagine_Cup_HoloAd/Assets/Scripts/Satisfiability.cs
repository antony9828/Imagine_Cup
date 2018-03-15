using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Satisfiability : MonoBehaviour
{
    public readonly int maxX, maxY, maxZ;
    public float[] cells;

    public Satisfiability(HoloAd config)
    {
        maxX = config.maxX;
        maxY = config.maxY;
        maxZ = config.maxZ;
        ClearSatData();
    }

    public void ClearSatData()
    {
        int len = GetLength();
        cells = new float[len];
        for (int i = 0; i < len; i++)
            cells[i] = 0f;
    }

    public float GetCellSat(int x, int y, int z)
    {
        return cells[CalcIndex(x, y, z)];
    }

    public float GetCellSat(Vector3Int cord)
    {
        return GetCellSat(cord.x, cord.y, cord.z);
    }

    public float GetCellSat(int index)
    {
        return cells[index];
    }

    public void SetCellSat(float sat, int x, int y, int z)
    {
        cells[CalcIndex(x, y, z)] = sat;
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

    public void TestCriterion(HoloAd.Criterion[] testCriterion, HoloAd config)
    {
        ClearSatData();

        Transform room = config.room.transform;

        List<HoloAd.Criterion> comCriterion = new List<HoloAd.Criterion>(), criterion = new List<HoloAd.Criterion>();
        float maxSat = 0f, maxComSat = 0f;
        foreach (HoloAd.Criterion crit in testCriterion)
        {
            if (crit.type == 7)
            {
                maxComSat += crit.importance;
                comCriterion.Add(crit);
            }
            else
            {
                maxSat += crit.importance;
                criterion.Add(crit);
            }
        }

        for (int x = 0; x < maxX; x++)
            for (int y = 0; y < maxY; y++)
                for (int z = 0; z < maxZ; z++)
                {
                    List<Vector3[]> triangles = config.meshAnalyzer.trigsInCells[CalcIndex(x, y, z)];
                    float cellSat = 0f;
                    foreach (HoloAd.Criterion crit in criterion)
                    {
                        float result = 0f;
                        switch (crit.type)
                        {
                            case 0: result = 1; break;
                            case 1:
                                {
                                    foreach (Vector3[] trig in triangles)
                                    {
                                        float trigSat = (trig[0].y + trig[1].y + trig[2].y) / 3f + room.lossyScale.y / 2f;
                                        result += trigSat > (crit.data.x) ? (trigSat < (crit.data.y) ? 1f : 0f) : 0f;
                                    }
                                    result = result / triangles.Count;
                                    break;
                                }
                            case 2:
                                {
                                    foreach (Vector3[] trig in triangles)
                                    {
                                        float trigSat = 0f;
                                        float ceilCord = room.lossyScale.y;
                                        for (int i = 0; i < 3; i++) trigSat += ceilCord - trig[i].y > crit.data.x && ceilCord - trig[i].y < crit.data.y ? 1f : 0f;
                                        result += trigSat / 3f;
                                    }
                                    result = result / triangles.Count;
                                    break;
                                }
                            case 3:
                                {
                                    
                                    break;
                                }
                            case 4: break;
                            case 5: //будет реализовываться движком
                                {
                                    result = 1;
                                    break;
                                }
                            case 6:
                                {
                                    float allTrigsSurf = 0f;
                                    foreach (Vector3[] trig in triangles)
                                    {
                                        float trigS = Vector3.Cross(trig[0] - trig[2], trig[0] - trig[1]).magnitude / 2f;
                                        allTrigsSurf += trigS;
                                        Vector3 normal = new Vector3(crit.data.x, crit.data.y, crit.data.z).normalized;//т.к. вбивается из юнити, чтобы не париться с длинами
                                        if (trig[3].x - normal.x < crit.data.w &&
                                            trig[3].y - normal.y < crit.data.w &&
                                            trig[3].z - normal.z < crit.data.w)
                                        {
                                            result += trigS;
                                        }
                                    }
                                    result = result / allTrigsSurf;
                                    break;
                                }
                        }
                        cellSat += crit.importance * result;
                    }
                    SetCellSat(cellSat/maxSat, x, y, z);
                }

        if (comCriterion.Count > 0)
        {
            int len = GetLength();
            float[] comCellsSat = new float[len];

            for (int x = 0; x < maxX; x++)
                for (int y = 0; y < maxY; y++)
                    for (int z = 0; z < maxZ; z++)
                    {
                        float result = 0f;
                        foreach (HoloAd.Criterion crit in comCriterion)
                        {
                            Vector3Int shift = new Vector3Int(
                                (int)System.Math.Round(x + crit.data.x),
                                (int)System.Math.Round(y + crit.data.y),
                                (int)System.Math.Round(z + crit.data.z)
                                );

                            if (IsCellInMatrix(shift))
                            {
                                result += GetCellSat(shift) * crit.importance;
                            }
                        }
                        comCellsSat[CalcIndex(x, y, z)] = result;
                    }

            for (int i = 0; i < len; i++)
            {
                cells[i] = (comCellsSat[i] + cells[i] * maxSat) / (maxSat + maxComSat);
            }
        }
        
    }

}
