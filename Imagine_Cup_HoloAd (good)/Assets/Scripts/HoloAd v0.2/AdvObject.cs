using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Academy.HoloToolkit.Unity;

public class AdvObject : MonoBehaviour
{ 
    //TODO: сделать более гибкий инструмент плейсинга наподобии критериев
    public Vector2 distToFloor;
    [Tooltip("Surfaces on which object can lay on")]
    public PlaneTypes[] typesOfPlane;

    public bool PlaceIt(Dictionary<PlaneTypes, List<GameObject>> surfaces)
    {
        foreach (PlaneTypes type in typesOfPlane)
        {
            List<GameObject> surfs;
            if (surfaces.TryGetValue(type, out surfs))
            {
                Collider collider = gameObject.GetComponent<Collider>();
                int index = FindNearestPlane(surfs, collider.bounds.size);

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
                            rotation = Quaternion.LookRotation(Camera.main.transform.position);
                            rotation.x = 0f;
                            rotation.z = 0f;
                            break;
                        case PlaneTypes.Floor:
                            rotation = Quaternion.LookRotation(Camera.main.transform.position);
                            rotation.x = 0f;
                            rotation.z = 0f;
                            break;
                        case PlaneTypes.Table:
                            rotation = Quaternion.LookRotation(Camera.main.transform.position);
                            rotation.x = 0f;
                            rotation.z = 0f;
                            break;
                        case PlaneTypes.Wall: rotation = Quaternion.LookRotation(surface.transform.forward, Vector3.up); break;
                    }

                    gameObject.transform.position = position;
                    gameObject.transform.rotation = rotation;
                    Collider gObjColl = gameObject.GetComponent<Collider>();

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

                Debug.Log(gameObject.name + " placed!");
                //Vector3 finalPosition = AdjustPositionWithSpatialMap(position, surfaceType);
                gameObject.transform.parent = HoloAd.advParent.transform;
                return true;
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
    private int FindNearestPlane(List<GameObject> planes, Vector3 minSize)//, List<int> usedPlanes)
    {
        int planeIndex = -1;

        for (int i = 0; i < planes.Count; i++)
        {
            /*
            if (usedPlanes.Contains(i))
            {
                continue;
            }
            */

            Collider collider = planes[i].GetComponent<Collider>();
            if ((collider.bounds.size.x < minSize.x || collider.bounds.size.y < minSize.y))
            {
                // This plane is too small to fit our object.
                continue;
            }

            /*if (distToFloor.x - planes[i].transform.lossyScale.y/2f < planes[i].transform.position.y && planes[i].transform.position.y < distToFloor.y + planes[i].transform.lossyScale.y/2f)
            {
                continue;
            }*/

            return i;
        }

        return planeIndex;
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
