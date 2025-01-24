using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PooledPrefab", menuName = "Soihtu/Pooling/Pooled Prefab", order = 1)]
public class PooledPrefabData : ScriptableObject
{

    public string identifier => name;

    public GameObject prefab;
    public int objectsToPreSpawn = 0;
    [Tooltip("If set to true, when all the objects are in use, pool manager will instantiate new objects to fill the need. If set to false, it will start reusing old active ones when it runs out.")]
    public bool instantiateNewObjectsWhenNeeded = true;
    [Tooltip("If set to true, these objects won't get cleared when groups are spawned. This is done by parenting their default parent to the PoolManager itself. If the objects are parented to something else, they can still get destroyed.")]
    public bool keepObjectsAliveThroughSceneChanges = false;
    [Tooltip("if keepObjectsAliveThroughSceneChanges is true, then this will automatically place all of them back into the pool. If this is false, they will remain wherever they currently are.")]
    public bool returnObjectsToPoolOnSceneChangeCall = true;
    public List<string> groupIdentifiers = new List<string>();

    [Header("Logging options for warnings (full logs will still show these)")]
    public bool logNotBeingUsed = true;
    public bool logOveruseAndSpawning = true;

}