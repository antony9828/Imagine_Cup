using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshAnalyzer : MonoBehaviour
{
    public List<Vector3[]>[] trigsInCells; // v1, v2, v3, normal

    public void RecalculateMesh(HoloAd config)
    {

        trigsInCells = new List<Vector3[]>[config.maxX * config.maxY * config.maxZ];


        for (int i = 0; i < config.maxX * config.maxY * config.maxZ; i++)
        {
            trigsInCells[i] = new List<Vector3[]>();
        }

        int childCount = config.spatialMapping.transform.childCount;
        for (int i = 0; i < childCount; i++)
        {
            GameObject child = config.spatialMapping.transform.GetChild(i).gameObject;
            Mesh mesh = child.GetComponent<MeshFilter>().mesh;
            Vector3[] meshVerts = mesh.vertices;
            int[] meshTrigs = mesh.triangles;
            int trigsCount = meshTrigs.Length;

            for (int trigIndex = 0; trigIndex < trigsCount; trigIndex += 3)
            {
                int[] verts = { meshTrigs[trigIndex], meshTrigs[trigIndex + 1], meshTrigs[trigIndex + 2] };
                foreach (int v in verts)
                {
                    Vector3 vec = config.room.transform.InverseTransformPoint(child.transform.TransformPoint(meshVerts[v]));

                    float cubeSide = config.matrixScale;
                    int[] matCords = {
                        (int) System.Math.Round( config.room.transform.localScale.x * (vec.x + .5f) / cubeSide ),
                        (int) System.Math.Round( config.room.transform.localScale.y * (vec.y + .5f) / cubeSide ),
                        (int) System.Math.Round( config.room.transform.localScale.z * (vec.z + .5f) / cubeSide)
                    };


                    if (config.IsCellInMatrix(matCords[0], matCords[1], matCords[2]))
                    {
                        Vector3[] trig = new Vector3[] {
                                config.room.transform.InverseTransformPoint( child.transform.TransformPoint( meshVerts[verts[0]] ) ),
                                config.room.transform.InverseTransformPoint( child.transform.TransformPoint( meshVerts[verts[1]] ) ),
                                config.room.transform.InverseTransformPoint( child.transform.TransformPoint( meshVerts[verts[2]] ) ),
                                Vector3.Cross(meshVerts[verts[0]] - meshVerts[verts[2]], meshVerts[verts[0]] - meshVerts[verts[1]]).normalized
                            };

                        trigsInCells[matCords[0] + config.maxX * matCords[1] + config.maxX * config.maxY * matCords[2]].Add(trig);
                        break;
                    }
                }
            }
        }
    }
}
