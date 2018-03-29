using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Academy.HoloToolkit.Unity;
using HoloToolkit.Unity.SpatialMapping;

public class AdvObject : MonoBehaviour
{
    //TODO: сделать более гибкий инструмент плейсинга наподобии критериев
    [Tooltip("Distation to floor diap")]
    public Vector2 distToFloor;

    [Tooltip("Min distation to another object")]
    public float minDistToAnotherObject;

    [Tooltip("Surfaces on which object can lay on")]
    public PlaneTypes[] typesOfPlane;

    
    static private float RaycastDistanceError = .03f;
    static private float RaycastVisiabilitySat = .6f;

    private Dictionary<PlaneTypes, List<int>> notSatisfiedSurfaces;

    private void CheckUsedSurfsDictInitiated()
    {
        //initializing
        if (notSatisfiedSurfaces == null)
        {
            notSatisfiedSurfaces = new Dictionary<PlaneTypes, List<int>>();
            foreach (PlaneTypes type in System.Enum.GetValues(typeof(PlaneTypes)))
            {
                notSatisfiedSurfaces.Add(type, new List<int>());
            }
        }
    }

    public bool PlaceIt(Dictionary<PlaneTypes, List<GameObject>> surfaces)
    {
        CheckUsedSurfsDictInitiated();
        
        foreach (PlaneTypes type in typesOfPlane)
        {
            List<GameObject> surfs;
            List<int> notSatSurfs;
            if (surfaces.TryGetValue(type, out surfs) && notSatisfiedSurfaces.TryGetValue(type, out notSatSurfs))
            {
                Collider collider = gameObject.GetComponent<Collider>();
                int index = -1;

                bool planeIsFinded = false;
                //finding visible nearest plane
                while(true)
                {
                    index = FindNearestPlane(type, surfs, collider.bounds.size);
                    Debug.Log(index);
                    if (index == -1)
                    {
                        break;
                    }

                    float planeThick = SurfaceMeshesToPlanes.Instance.SurfacePlanePrefab.GetComponent<SurfacePlane>().PlaneThickness;
                    //8 dots of collider bounds and 1 dot of the center
                    Vector3[] rayDirections =
                    {
                        new Vector3(0,0,0),
                        new Vector3(collider.bounds.size.x/2, collider.bounds.size.y, collider.bounds.size.z/2),
                        new Vector3(-collider.bounds.size.x/2, collider.bounds.size.y, collider.bounds.size.z/2),
                        new Vector3(collider.bounds.size.x/2, collider.bounds.size.y, -collider.bounds.size.z/2),
                        new Vector3(-collider.bounds.size.x/2, collider.bounds.size.y, -collider.bounds.size.z/2),
                        new Vector3(collider.bounds.size.x/2, planeThick, collider.bounds.size.z/2),
                        new Vector3(-collider.bounds.size.x/2, planeThick, collider.bounds.size.z/2),
                        new Vector3(collider.bounds.size.x/2, planeThick, -collider.bounds.size.z/2),
                        new Vector3(-collider.bounds.size.x/2, planeThick, -collider.bounds.size.z/2)
                    };

                    int visibleDots = 0;
                    foreach (Vector3 dot in rayDirections)
                    {
                        Vector3 dir = surfs[index].transform.position + dot - Camera.main.transform.position;
                        Ray ray = new Ray(Camera.main.transform.position, dir);

                        if (!Physics.Raycast(ray, dir.magnitude - RaycastDistanceError, 1 << surfs[index].layer | SpatialMappingManager.Instance.LayerMask))
                        {
                            visibleDots += 1;
                        }

                        if (visibleDots / (float)rayDirections.Length > RaycastVisiabilitySat)
                        {
                            planeIsFinded = true;
                            break;
                        }
                    }
                    
                    var height = surfs[index].transform.position.y - SurfaceMeshesToPlanes.Instance.FloorYPosition;
                    if (!(distToFloor.x < height && height < distToFloor.y) && planeIsFinded)
                    {
                        planeIsFinded = false;
                    }
                    
                    if(planeIsFinded)
                    {
                        foreach (Transform other in HoloAd.advParent.GetComponentsInChildren<Transform>())
                        {
                            if ((other.transform.position - surfs[index].transform.position).magnitude < minDistToAnotherObject)
                            {
                                planeIsFinded = false;
                                break;
                            }
                        }
                    }
                    
                    if (!planeIsFinded)
                    {
                        notSatSurfs.Add(index);
                    }
                    else break;
                }
                
                // If we can't find a good plane we will put the object floating in space.
                Vector3 position = Camera.main.transform.position + Camera.main.transform.forward * 2.0f + Camera.main.transform.right * (Random.value - 1.0f) * 2.0f;
                Quaternion rotation = Quaternion.identity;

                // If we do find a good plane we can do something smarter.
                if (index >= 0)
                {
                    GameObject surface = surfs[index];
                    SurfacePlane plane = surface.GetComponent<SurfacePlane>();
                    position = surface.transform.position + (plane.PlaneThickness * plane.SurfaceNormal);
                    position = AdjustPositionWithSpatialMap(position, plane.SurfaceNormal);
                    rotation = Camera.main.transform.localRotation;

                    switch (type)
                    {
                        case PlaneTypes.Ceiling:
                            rotation = Quaternion.LookRotation(Camera.main.transform.position - position);
                            rotation.x = 0f;
                            rotation.z = 0f;
                            break;
                        case PlaneTypes.Floor:
                            rotation = Quaternion.LookRotation(Camera.main.transform.position - position);
                            rotation.x = 0f;
                            rotation.z = 0f;
                            break;
                        case PlaneTypes.Table:
                            rotation = Quaternion.LookRotation(Camera.main.transform.position - position);
                            rotation.x = 0f;
                            rotation.z = 0f;
                            break;
                        case PlaneTypes.Wall: rotation = Quaternion.LookRotation(surface.transform.forward, Vector3.up); break;
                    }

                    gameObject.transform.position = position;
                    gameObject.transform.rotation = rotation;
                    Collider gObjColl = gameObject.GetComponent<Collider>();
                    Debug.Log(gameObject.name + " is placed!");
                    //Vector3 finalPosition = AdjustPositionWithSpatialMap(position, surfaceType);
                    gameObject.transform.parent = HoloAd.advParent.transform;
                    return true;

                    /*
                    foreach (Collider otherObjCollider in HoloAd.advParent.transform.GetComponentsInChildren<Collider>())
                    {
                        if (gObjColl.)
                    }
                    */
                } else
                {
                    continue;
                }
            }
        }
        return false;
    }

    /// <summary>
    /// Attempts to find a the closest plane to the user which is large enough to fit the object.
    /// </summary>
    /// <param name="planes">List of planes to consider for object placement.</param>
    /// <param name="minSize">Minimum size that the plane is required to be.</param>
    /// <param name="startIndex">Index in the planes collection that we want to start at (to help avoid double-placement of objects).</param>
    /// <param name="isVertical">True, if we are currently evaluating vertical surfaces.</param>
    /// <returns></returns>
    private int FindNearestPlane(PlaneTypes key, List<GameObject> planes, Vector3 minSize)//, List<int> usedPlanes)
    {
        for (int i = 0; i < planes.Count; i++)
        {
            List<int> notSatSurfs;

            if (notSatisfiedSurfaces.TryGetValue(key, out notSatSurfs))
            {
                if (notSatSurfs.Contains(i))
                {
                    continue;
                }
                else
                {
                    Collider collider = planes[i].GetComponent<Collider>();
                    
                    /*if ((collider.bounds.size.x < minSize.x || collider.bounds.size.y < minSize.y))
                    {
                        // This plane is too small to fit our object.
                        notSatSurfs.Add(i);
                        continue;
                    }*/

                    /*if (distToFloor.x - planes[i].transform.lossyScale.y/2f < planes[i].transform.position.y && planes[i].transform.position.y < distToFloor.y + planes[i].transform.lossyScale.y/2f)
                    {
                        continue;
                    }*/

                    return i;
                }
            }
            else throw new System.Exception("Something went wrong...");
        }

        return -1;
    }

    /// <summary>
    /// Adjusts the initial position of the object if it is being occluded by the spatial map.
    /// </summary>
    /// <param name="position">Position of object to adjust.</param>
    /// <param name="surfaceNormal">Normal of surface that the object is positioned against.</param>
    /// <returns></returns>
    private Vector3 AdjustPositionWithSpatialMap(Vector3 position, Vector3 surfaceNormal)
    {
        Vector3 newPosition = position;
        RaycastHit hitInfo;
        float distance = 0.5f;

        // Check to see if there is a SpatialMapping mesh occluding the object at its current position.
        if (Physics.Raycast(position, surfaceNormal, out hitInfo, distance, SpatialMappingManager.Instance.LayerMask))
        {
            // If the object is occluded, reset its position.
            newPosition = hitInfo.point;
        }

        return newPosition;
    }
}
