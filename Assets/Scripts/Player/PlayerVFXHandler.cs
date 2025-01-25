using System.Collections.Generic;
using UnityEngine;

public class PlayerVFXHandler : MonoBehaviour
{
    public Transform VFXParent;

    private List<GameObject> activeVFXs = new List<GameObject>();

    public void AddNewVFX(GameObject newVFX)
    {
        newVFX.transform.parent = VFXParent;
        newVFX.transform.localPosition = Vector3.zero;
        activeVFXs.Add(newVFX);
    }

    public void RemoveVFX(GameObject newVFX)
    {
        activeVFXs.Remove(newVFX);
        PoolManager.ReturnObjectToPoolOrDestroyIt(newVFX);
    }

}