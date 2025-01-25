using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(order = 20, menuName = "GGJ2025/Pick up data")]
public class PickUpDataObject : GameData
{

    public float PickupWeight = 1f;
    public GameObject PickupPrefab;
    public PooledPrefabData PooledPrefabData;
    public List<ModifierData> ModifiersToApply;
    public string FloatingTextToShow;
    public Color FloatingTextColor = Color.white;

    public GameObject CreatePickUp()
    {
        if (PooledPrefabData != null && PoolManager.Instance != null)
        {
            return PoolManager.GetPooledObject(PooledPrefabData).gameObject;
        }

        if (PickupPrefab != null)
        {
            return Instantiate(PickupPrefab);
        }

        return null;
    }

}