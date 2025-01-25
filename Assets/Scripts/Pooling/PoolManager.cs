using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PoolManager : MonoBehaviour
{

    #region Class definitions

    [Serializable]
    internal class PoolObject
    {

        internal GameObject _object;
        internal PooledObject objectScript;
        internal PoolObjectSettings objectSettings = new PoolObjectSettings();
        internal string type = "";

        internal void ActivatePoolObject()
        {
            if (this._object != null)
            {
                if (this.objectSettings != null)
                {
                    if (string.IsNullOrEmpty(this.objectSettings.objectName) == false)
                    {
                        this._object.name = this.objectSettings.objectName;
                    }

                    if (this.objectSettings.parentToSet != null)
                    {
                        this._object.transform.SetParent(this.objectSettings.parentToSet.transform);
                    }

                    if (this.objectSettings.setLocalPositionAndRotation && this.objectSettings.parentToSet != null)
                    {
                        this._object.transform.localPosition = this.objectSettings.positionToSet;
                        this._object.transform.localRotation = this.objectSettings.rotationToSet;
                    }
                    else if (!this.objectSettings.setLocalPositionAndRotation)
                    {
                        this._object.transform.position = this.objectSettings.positionToSet;
                        this._object.transform.rotation = this.objectSettings.rotationToSet;
                    }
                }

                this._object.SetActive(true);

                if (this.objectSettings != null)
                {
                    //Rigidbody objectBody = this._object.GetComponent<Rigidbody>();

                    //if (this.objectSettings.setRigidBodyForce && objectBody != null)
                    //{
                    //    objectBody.linearVelocity = new Vector3();
                    //    objectBody.angularVelocity = new Vector3();

                    //    objectBody.AddForce(this.objectSettings.rigidbodyForceToSet, this.objectSettings.forceModeToSend);
                    //}

                    if (this.objectSettings.actionToCallWhenActivated != null)
                    {
                        this.objectSettings.actionToCallWhenActivated();
                    }

                    if (this.objectScript != null)
                    {
                        this.objectScript.SetPoolSettings(this.objectSettings);
                    }
                }
            }
        }

        internal void DeActivatePoolObject(GameObject defaultParent)
        {
            if (this._object != null)
            {
                if (this._object.transform.parent != defaultParent)
                {
                    if (defaultParent != null)
                    {
                        this._object.transform.SetParent(defaultParent.transform);
                    }
                    else
                    {
                        this._object.transform.SetParent(null);
                    }
                }

                this._object.SetActive(false);
            }
            else
            {
                Debug.Log("_object was null!");
            }

            this.objectSettings = null;
        }

    }

    internal class TypePoolTracker
    {

        internal int extraInstantiations;
        internal int highestActiveAmount;
        internal int poolClearPerformed;
        internal string poolIdentifier = "";
        internal int preSpawningCount;
        internal int preSpawnObjectsAmount;

    }

    public class TypePoolManager
    {

        private List<PoolObject> _activePool = new List<PoolObject>();
        private List<PoolObject> _idlePool = new List<PoolObject>();
        private List<PoolObject> _waitingForActivationPool = new List<PoolObject>();

        public GameObject defaultParent;
        public PooledPrefabData PooledPrefabDataToUse;

        internal bool setObjectsUnderPoolManager = false;
        internal bool returnAutomaticallyOnSceneChange = false;

        private bool trackingOn;

        public string typeName;
        internal TypePoolTracker typePoolTracker;

        internal bool CheckIfPreSpawnIsEnabled()
        {
            if (this.PooledPrefabDataToUse != null)
            {
                return this.PooledPrefabDataToUse.objectsToPreSpawn > 0;
            }

            return false;
        }

        internal void PreSpawnPoolObjects()
        {
            if (CheckIfPreSpawnIsEnabled())
            {
                ClearAllPoolsFromNulls();

                int objectsToSpawn = this.PooledPrefabDataToUse.objectsToPreSpawn - (this._idlePool.Count + this._waitingForActivationPool.Count + this._activePool.Count);

                for (int i = 0; i < objectsToSpawn; i++)
                {
                    InstantiateNewPooledObject();
                }

                if (this.trackingOn && this.typePoolTracker != null)
                {
                    this.typePoolTracker.preSpawningCount++;
                }
            }
        }

        internal void ClearPool()
        {
            for (int i = 0; i < this._activePool.Count; i++)
            {
                GameObject objectToCheck = this._activePool[i]._object;

                if (objectToCheck != null)
                {
                    Destroy(objectToCheck);
                }
            }

            this._activePool = new List<PoolObject>();

            for (int i = 0; i < this._waitingForActivationPool.Count; i++)
            {
                GameObject objectToCheck = this._waitingForActivationPool[i]._object;

                if (objectToCheck != null)
                {
                    Destroy(objectToCheck);
                }
            }

            this._waitingForActivationPool = new List<PoolObject>();

            for (int i = 0; i < this._idlePool.Count; i++)
            {
                GameObject objectToCheck = this._idlePool[i]._object;

                if (objectToCheck != null)
                {
                    Destroy(objectToCheck);
                }
            }

            this._idlePool = new List<PoolObject>();

            if (this.trackingOn && this.typePoolTracker != null)
            {
                this.typePoolTracker.poolClearPerformed++;
            }
        }

        internal void ReturnAllObjectsToPool()
        {
            for (int i = 0; i < this._activePool.Count; i++)
            {
                PoolObject poolObject = this._activePool[i];

                if (poolObject != null && poolObject._object != null)
                {
                    poolObject.DeActivatePoolObject(this.defaultParent);
                    this._idlePool.Add(poolObject);
                }
            }

            this._activePool = new List<PoolObject>();

            for (int i = 0; i < this._waitingForActivationPool.Count; i++)
            {
                PoolObject poolObject = this._waitingForActivationPool[i];

                if (poolObject != null && poolObject._object != null)
                {
                    poolObject.DeActivatePoolObject(this.defaultParent);
                    this._idlePool.Add(poolObject);
                }
            }

            this._waitingForActivationPool = new List<PoolObject>();
        }

        internal GameObject GetObjectFromPool(PoolObjectSettings settings)
        {
            if (settings != null)
            {
                PoolObject poolObject = TakeObjectFromPool();

                if (poolObject != null)
                {
                    poolObject.objectSettings = settings;
                    settings.startTime = Time.time;

                    if (settings != null && settings.appearanceTime <= 0)
                    {
                        this._idlePool.Remove(poolObject);
                        this._activePool.Add(poolObject);

                        if (this.trackingOn && this.typePoolTracker != null && this._activePool.Count > this.typePoolTracker.highestActiveAmount)
                        {
                            this.typePoolTracker.highestActiveAmount = this._activePool.Count;
                        }

                        poolObject.ActivatePoolObject();
                    }
                    else
                    {
                        this._idlePool.Remove(poolObject);
                        this._waitingForActivationPool.Add(poolObject);
                    }

                    return poolObject._object;
                }
            }

            Debug.LogError("Got null from pool!");

            return null;
        }

        internal PooledObject GetObjectScriptFromPool(PoolObjectSettings settings)
        {
            if (settings == null)
            {
                settings = new PoolObjectSettings();
            }

            PoolObject poolObject = TakeObjectFromPool();

            if (poolObject != null)
            {
                poolObject.objectSettings = settings;
                settings.startTime = Time.time;

                if (settings != null && settings.appearanceTime <= 0)
                {
                    this._idlePool.Remove(poolObject);
                    this._activePool.Add(poolObject);

                    if (this.trackingOn && this.typePoolTracker != null && this._activePool.Count > this.typePoolTracker.highestActiveAmount)
                    {
                        this.typePoolTracker.highestActiveAmount = this._activePool.Count;
                    }

                    poolObject.ActivatePoolObject();
                }
                else
                {
                    this._idlePool.Remove(poolObject);
                    this._waitingForActivationPool.Add(poolObject);
                }

                return poolObject.objectScript;
            }

            Debug.LogError("Got null from pool!");
            return null;
        }

        internal void ReturnObjectBackToPool(GameObject returnedObject)
        {
            if (this._activePool != null && returnedObject != null)
            {
                PoolObject objectToReturnBack = null;

                for (int i = 0; i < this._activePool.Count; i++)
                {
                    if (this._activePool[i]._object == returnedObject)
                    {
                        objectToReturnBack = this._activePool[i];

                        break;
                    }
                }

                if (objectToReturnBack != null)
                {
                    this._idlePool.Add(objectToReturnBack);
                    this._activePool.Remove(objectToReturnBack);
                    objectToReturnBack.DeActivatePoolObject(this.defaultParent);

                    if (this.defaultParent != null && objectToReturnBack._object != null)
                    {
                        objectToReturnBack._object.transform.SetParent(this.defaultParent.transform);
                    }
                }
                else
                {
                    Debug.Log("failed to get objectToReturn!");
                }
            }
        }

        internal void CheckForActivationsInPool()
        {
            if (this._waitingForActivationPool != null)
            {
                List<PoolObject> moveToActiveList = new List<PoolObject>();

                PoolObject objectToCheck = null;
                int waitingForActivationPoolCount = this._waitingForActivationPool.Count;
                for (int i = 0; i < waitingForActivationPoolCount; i++)
                {
                    objectToCheck = this._waitingForActivationPool[i];

                    if (objectToCheck != null)
                    {
                        float timeToAppear = objectToCheck.objectSettings.startTime + objectToCheck.objectSettings.appearanceTime - Time.time;

                        if (timeToAppear < 0)
                        {
                            moveToActiveList.Add(objectToCheck);
                        }
                    }
                    objectToCheck = null;
                }

                PoolObject objectToMove = null;
                int moveToActiveListCount = moveToActiveList.Count;
                for (int i = 0; i < moveToActiveListCount; i++)
                {
                    objectToMove = moveToActiveList[i];

                    this._waitingForActivationPool.Remove(objectToMove);
                    this._activePool.Add(objectToMove);

                    objectToMove.ActivatePoolObject();

                    if (this.trackingOn && this.typePoolTracker != null && this._activePool.Count > this.typePoolTracker.highestActiveAmount)
                    {
                        this.typePoolTracker.highestActiveAmount = this._activePool.Count;
                    }

                    objectToMove = null;
                }
            }
        }

        private void ClearAllPoolsFromNulls()
        {
            for (int i = this._idlePool.Count - 1; i >= 0; i--)
            {
                if (this._idlePool[i] == null || this._idlePool[i]._object == null)
                {
                    this._idlePool.RemoveAt(i);
                }
            }

            for (int i = this._waitingForActivationPool.Count - 1; i >= 0; i--)
            {
                if (this._waitingForActivationPool[i] == null || this._waitingForActivationPool[i]._object == null)
                {
                    this._waitingForActivationPool.RemoveAt(i);
                }
            }

            for (int i = this._activePool.Count - 1; i >= 0; i--)
            {
                if (this._activePool[i] == null || this._activePool[i]._object == null)
                {
                    this._activePool.RemoveAt(i);
                }
            }
        }

        private PoolObject TakeObjectFromPool()
        {
            PoolObject objectToTake = null;

            if (this._idlePool.Count > 0)
            {
                for (int i = this._idlePool.Count - 1; i >= 0; i--)
                {
                    if (this._idlePool[i] == null || this._idlePool[i]._object == null)
                    {
                        this._idlePool.RemoveAt(i);
                    }
                    else
                    {
                        objectToTake = this._idlePool[i];

                        break;
                    }
                }
            }

            if (objectToTake == null && this.PooledPrefabDataToUse.instantiateNewObjectsWhenNeeded)
            {
                objectToTake = InstantiateNewPooledObject();

                if (this.trackingOn && this.typePoolTracker != null)
                {
                    this.typePoolTracker.extraInstantiations++;
                }
            }
            else if (objectToTake == null && this._activePool.Count > 0)
            {
                objectToTake = this._activePool[0];
                objectToTake.DeActivatePoolObject(this.defaultParent);
                this._activePool.RemoveAt(0);
            }
            else if (objectToTake == null && this._waitingForActivationPool.Count > 0)
            {
                objectToTake = this._waitingForActivationPool[0];
                objectToTake.DeActivatePoolObject(this.defaultParent);
                this._waitingForActivationPool.RemoveAt(0);
            }

            return objectToTake;
        }

        private PoolObject InstantiateNewPooledObject()
        {
            PoolObject objectToTake = new PoolObject();
            GameObject gameObjectToCreate = Instantiate(this.PooledPrefabDataToUse.prefab);
            objectToTake._object = gameObjectToCreate;
            objectToTake.type = this.typeName;

            PooledObject pooledObjectScript = gameObjectToCreate.GetComponent<PooledObject>();

            if (pooledObjectScript == null)
            {
                pooledObjectScript = gameObjectToCreate.AddComponent<PooledObject>();
            }

            objectToTake.objectScript = pooledObjectScript;

            if (string.IsNullOrEmpty(this.typeName) == false)
            {
                pooledObjectScript.poolTypeString = this.typeName;
                pooledObjectScript.poolBasePrefab = null;
            }
            else
            {
                pooledObjectScript.poolBasePrefab = this.PooledPrefabDataToUse.prefab;
                pooledObjectScript.poolTypeString = string.Empty;
            }

            if (this.defaultParent == null)
            {
                if (this.setObjectsUnderPoolManager)
                {
                    if (PoolManager.Instance.PutPooledObjectsUnderTypePoolManagerParents)
                    {
                        this.defaultParent = new GameObject();
                        this.defaultParent.transform.SetParent(PoolManager.Instance.transform);

                        if (string.IsNullOrEmpty(this.typeName) == false)
                        {
                            this.defaultParent.name = this.typeName + "_Parent";
                        }
                        else if (this.PooledPrefabDataToUse != null && this.PooledPrefabDataToUse.prefab != null)
                        {
                            this.defaultParent.name = this.PooledPrefabDataToUse.prefab.name + "_Parent";
                        }
                    }
                    else
                    {
                        this.defaultParent = PoolManager.Instance.gameObject;
                    }
                }
                else
                {
                    if (PoolManager.Instance.PutPooledObjectsUnderTypePoolManagerParents)
                    {
                        this.defaultParent = new GameObject();

                        if (PoolManager.Instance.PutAllPooledObjectsUnderSingleParent)
                        {
                            if (PoolManager.Instance.currentLevelPooledObjectParent == null)
                            {
                                PoolManager.Instance.currentLevelPooledObjectParent = new GameObject();
                                PoolManager.Instance.currentLevelPooledObjectParent.name = "LevelPooledObjectsParent";
                            }

                            this.defaultParent.transform.SetParent(PoolManager.Instance.currentLevelPooledObjectParent.transform);
                        }

                        if (string.IsNullOrEmpty(this.typeName) == false)
                        {
                            this.defaultParent.name = this.typeName + "_Parent";
                        }
                        else if (this.PooledPrefabDataToUse != null && this.PooledPrefabDataToUse.prefab != null)
                        {
                            this.defaultParent.name = this.PooledPrefabDataToUse.prefab.name + "_Parent";
                        }
                    }
                    else if (PoolManager.Instance.PutAllPooledObjectsUnderSingleParent)
                    {
                        if (PoolManager.Instance.currentLevelPooledObjectParent == null)
                        {
                            PoolManager.Instance.currentLevelPooledObjectParent = new GameObject();
                            PoolManager.Instance.currentLevelPooledObjectParent.name = "LevelPooledObjectsParent";
                        }

                        this.defaultParent = PoolManager.Instance.currentLevelPooledObjectParent;
                    }
                }
            }

            objectToTake.DeActivatePoolObject(this.defaultParent);
            this._idlePool.Add(objectToTake);

            return objectToTake;
        }

        #region Tracking stuff

        internal void TurnOnPoolTracking()
        {
            this.trackingOn = true;

            this.typePoolTracker = new TypePoolTracker
            {
                poolIdentifier = this.typeName
            };

            if (this.PooledPrefabDataToUse != null)
            {
                this.typePoolTracker.preSpawnObjectsAmount = this.PooledPrefabDataToUse.objectsToPreSpawn;
            }
        }

        internal string GetTrackingResults(bool addLineBreaks = true)
        {
            if (this.trackingOn == false || this.typePoolTracker == null)
            {
                return "No tracking performed on: " + this.typeName;
            }

            string trackingResult = "";

            if (string.IsNullOrEmpty(this.typePoolTracker.poolIdentifier) && this.PooledPrefabDataToUse != null && this.PooledPrefabDataToUse.prefab != null)
            {
                trackingResult += "Tracking for prefab pool: " + this.PooledPrefabDataToUse.prefab.name;
            }
            else
            {
                trackingResult += "Tracking for typename: " + this.typePoolTracker.poolIdentifier;
            }

            if (addLineBreaks)
            {
                trackingResult += "\n";
            }

            trackingResult += "Prespawning object amount was set to: " + this.typePoolTracker.preSpawnObjectsAmount;

            if (addLineBreaks)
            {
                trackingResult += "\n";
            }

            trackingResult += "Prespawning on this pool was performed " + this.typePoolTracker.preSpawningCount + " time(s).";

            if (addLineBreaks)
            {
                trackingResult += "\n";
            }

            trackingResult += "Pool was cleared " + this.typePoolTracker.poolClearPerformed + " time(s).";

            if (addLineBreaks)
            {
                trackingResult += "\n";
            }

            trackingResult += "Highest active amount in the pool at any time was: " + this.typePoolTracker.highestActiveAmount;

            if (addLineBreaks)
            {
                trackingResult += "\n";
            }

            trackingResult += "The Pool perfomed " + this.typePoolTracker.extraInstantiations + " extra instantiations during tracking period";

            return trackingResult;
        }

        internal string GetTrackingWarnings(bool addLineBreaks = true)
        {
            if (this.trackingOn == false || this.typePoolTracker == null)
            {
                return string.Empty;
            }

            if (this.typePoolTracker.preSpawningCount <= 0 && this.typePoolTracker.highestActiveAmount <= 0)
            {
                // if pool was never used, don't give an warning.
                return string.Empty;
            }

            string trackingResult = "";
            string identifier = "";

            if (string.IsNullOrEmpty(this.typePoolTracker.poolIdentifier) && this.PooledPrefabDataToUse != null && this.PooledPrefabDataToUse.prefab != null)
            {
                identifier = this.PooledPrefabDataToUse.prefab.name;
            }
            else
            {
                identifier = this.typePoolTracker.poolIdentifier;
            }

            bool logExtraUse = true;

            if (this.PooledPrefabDataToUse != null)
            {
                logExtraUse = this.PooledPrefabDataToUse.logOveruseAndSpawning;
            }

            if (logExtraUse && this.typePoolTracker.highestActiveAmount > this.typePoolTracker.preSpawnObjectsAmount && this.PooledPrefabDataToUse != null && this.PooledPrefabDataToUse.instantiateNewObjectsWhenNeeded == false)
            {
                trackingResult += "Warning for: " + identifier + ": there were not enough objects in the pool to match needs. Went over by: " + (this.typePoolTracker.highestActiveAmount - this.typePoolTracker.preSpawnObjectsAmount);
            }

            if (logExtraUse && this.typePoolTracker.extraInstantiations > 0)
            {
                if (addLineBreaks && trackingResult.Length > 0)
                {
                    trackingResult += "\n";
                }

                trackingResult += "Warning for: " + identifier + ": there were this many extra instantiations in the pool: " + this.typePoolTracker.extraInstantiations;
            }

            bool logNonUse = true;

            if (this.PooledPrefabDataToUse != null)
            {
                logNonUse = this.PooledPrefabDataToUse.logNotBeingUsed;
            }

            if (logNonUse && this.typePoolTracker.highestActiveAmount <= 0)
            {
                if (addLineBreaks && trackingResult.Length > 0)
                {
                    trackingResult += "\n";
                }

                trackingResult += "Warning for: " + identifier + ": the pool was prespawned but never used.";
            }

            return trackingResult;
        }

        #endregion

    }

    public enum UsageLoggingStyle
    {

        LogAllUsageData = 0,
        LogOnlyWarnings = 1,
        None = 99

    }

    public enum UsageSavingStyle
    {

        UnityDebugLogs,
        SaveToFileInGameFolder,
        None

    }

    #endregion

    #region Variables

    internal static PoolManager Instance { get; private set; }

    [Tooltip("Folder name inside the Resources folder to check for PoolObject scriptable objects.")]
    public string PoolPrefabFolderName = "PooledPrefabs";

    [Tooltip("This makes it so that the results of tracking are only displayed in editor if enabled. Won't do anything if the tracking is not enabled.")]
    public bool onlyShowUsageDataOnEditor = true;

    public UsageLoggingStyle usageLoggingStyle = UsageLoggingStyle.None;
    public UsageSavingStyle usageSavingStyle = UsageSavingStyle.None;

    public bool PutAllPooledObjectsUnderSingleParent = true;
    public bool PutPooledObjectsUnderTypePoolManagerParents = true;

    internal GameObject currentLevelPooledObjectParent;

    private Dictionary<string, TypePoolManager> poolTypeManagerDictionary = new Dictionary<string, TypePoolManager>();

    private Dictionary<string, List<TypePoolManager>> groupPoolManagerDictionary = new Dictionary<string, List<TypePoolManager>>();

    private readonly Dictionary<GameObject, TypePoolManager> poolPrefabManagerDictionary = new Dictionary<GameObject, TypePoolManager>();

    private Dictionary<string, PooledPrefabData> poolPrefabDataDict;

    private readonly List<TypePoolManager> activePoolManagers = new List<TypePoolManager>();
    private List<TypePoolManager> dontDestroyOnSceneChangeList = new List<TypePoolManager>();

    private bool IsTrackingEnabled
    {
        get
        {
            bool canTrack = true;

            if (this.onlyShowUsageDataOnEditor)
            {
#if !UNITY_EDITOR
                    canTrack = false;
#endif
            }

            return canTrack && this.usageLoggingStyle != UsageLoggingStyle.None;
        }
    }

    #endregion

    #region Unity base functions and initialization

    private void Awake()
    {
        if (CreateSingleton())
        {
            Initialize();
        }
    }
    
    private void Update()
    {
        TypePoolManager manager = null;

        if (this.activePoolManagers != null)
        {
            for (int i = 0; i < this.activePoolManagers.Count; i++)
            {
                manager = this.activePoolManagers[i];

                if (manager != null)
                {
                    manager.CheckForActivationsInPool();
                }
            }
        }
    }

    private void OnDestroy()
    {
        if (this.IsTrackingEnabled && this.activePoolManagers != null && this.activePoolManagers.Count > 0)
        {
            switch (this.usageSavingStyle)
            {
                case UsageSavingStyle.UnityDebugLogs:
                    PrintTrackingData();
                    break;

                case UsageSavingStyle.SaveToFileInGameFolder:
                    SaveTrackingDataToFile();
                    break;
            }
        }
    }

    private bool CreateSingleton()
    {
        if (Instance != null)
        {
            Destroy(this.gameObject);
            return false;
        }

        if (this.transform.parent == null)
        {
            DontDestroyOnLoad(this.gameObject);
        }

        Instance = this;

        return true;
    }

    private void Initialize()
    {
        if (this.poolTypeManagerDictionary == null)
        {
            this.poolTypeManagerDictionary = new Dictionary<string, TypePoolManager>();
        }

        if (this.groupPoolManagerDictionary == null)
        {
            this.groupPoolManagerDictionary = new Dictionary<string, List<TypePoolManager>>();
        }

        if (this.dontDestroyOnSceneChangeList == null)
        {
            this.dontDestroyOnSceneChangeList = new List<TypePoolManager>();
        }

        PooledPrefabData[] data = Resources.LoadAll<PooledPrefabData>(this.PoolPrefabFolderName);
        this.poolPrefabDataDict = new Dictionary<string, PooledPrefabData>();

        if (data != null && data.Length > 0)
        {
            foreach (PooledPrefabData obj in data)
            {
                if (obj == null)
                {
                    continue;
                }

                if (obj.prefab == null)
                {
                    continue;
                }

                if (string.IsNullOrEmpty(obj.identifier) == false && this.poolPrefabDataDict.ContainsKey(obj.identifier) == false)
                {
                    this.poolPrefabDataDict.Add(obj.identifier, obj);

                    if (this.poolTypeManagerDictionary.ContainsKey(obj.identifier) == false)
                    {
                        bool keepThisAlive = obj.keepObjectsAliveThroughSceneChanges;

                        TypePoolManager poolManager = new TypePoolManager
                        {
                            PooledPrefabDataToUse = obj,
                            typeName = obj.identifier,
                            setObjectsUnderPoolManager = keepThisAlive,
                            returnAutomaticallyOnSceneChange = obj.returnObjectsToPoolOnSceneChangeCall
                        };

                        this.poolTypeManagerDictionary.Add(obj.identifier, poolManager);
                        this.activePoolManagers.Add(poolManager);

                        if (keepThisAlive)
                        {
                            this.dontDestroyOnSceneChangeList.Add(poolManager);
                        }

                        if (this.IsTrackingEnabled)
                        {
                            poolManager.TurnOnPoolTracking();
                        }

                        foreach (string groupIdentifier in obj.groupIdentifiers)
                        {
                            if (this.groupPoolManagerDictionary.ContainsKey(groupIdentifier))
                            {
                                this.groupPoolManagerDictionary[groupIdentifier].Add(poolManager);
                            }
                            else
                            {
                                this.groupPoolManagerDictionary.Add(groupIdentifier, new List<TypePoolManager> {poolManager});
                            }
                        }
                    }
                }
            }
        }
    }

    private void PrintTrackingData()
    {
        bool canPrint = true;

        if (this.onlyShowUsageDataOnEditor)
        {
#if !UNITY_EDITOR
            canPrint = false;
#endif
        }

        if (canPrint == false)
        {
            return;
        }

        bool startMessageShown = false;

        switch (this.usageLoggingStyle)
        {
            case UsageLoggingStyle.LogAllUsageData:

                foreach (TypePoolManager poolManager in this.activePoolManagers)
                {
                    if (poolManager == null)
                    {
                        continue;
                    }

                    string poolUsageLog = poolManager.GetTrackingResults();

                    if (string.IsNullOrEmpty(poolUsageLog) == false)
                    {
                        if (startMessageShown == false)
                        {
                            Debug.Log("Printing full poolmanager usage stats below.");
                            startMessageShown = true;
                        }

                        Debug.Log(poolUsageLog);
                    }
                }

                break;

            case UsageLoggingStyle.LogOnlyWarnings:
                foreach (TypePoolManager poolManager in this.activePoolManagers)
                {
                    if (poolManager == null)
                    {
                        continue;
                    }

                    string poolUsageLog = poolManager.GetTrackingWarnings();

                    if (string.IsNullOrEmpty(poolUsageLog) == false)
                    {
                        if (startMessageShown == false)
                        {
                            Debug.Log("Printing warnings from poolmanager usage stats below.");
                            startMessageShown = true;
                        }

                        Debug.LogWarning(poolUsageLog);
                    }
                }

                break;
        }
    }

    private void SaveTrackingDataToFile()
    {
        bool canPrint = true;

        if (this.onlyShowUsageDataOnEditor)
        {
#if !UNITY_EDITOR
            canPrint = false;
#endif
        }

        if (canPrint == false)
        {
            return;
        }

        bool startMessageShown = false;
        string stringToSave = "";

        switch (this.usageLoggingStyle)
        {
            case UsageLoggingStyle.LogAllUsageData:

                foreach (TypePoolManager poolManager in this.activePoolManagers)
                {
                    if (poolManager == null)
                    {
                        continue;
                    }

                    string poolUsageLog = poolManager.GetTrackingResults();

                    if (string.IsNullOrEmpty(poolUsageLog) == false)
                    {
                        if (startMessageShown == false)
                        {
                            stringToSave += "Printing full poolmanager usage stats below.\n\n";
                            startMessageShown = true;
                        }

                        stringToSave += poolUsageLog + "\n\n";
                    }
                }

                break;

            case UsageLoggingStyle.LogOnlyWarnings:
                foreach (TypePoolManager poolManager in this.activePoolManagers)
                {
                    if (poolManager == null)
                    {
                        continue;
                    }

                    string poolUsageLog = poolManager.GetTrackingWarnings();

                    if (string.IsNullOrEmpty(poolUsageLog) == false)
                    {
                        if (startMessageShown == false)
                        {
                            stringToSave += "Printing warnings from poolmanager usage stats below.\n\n";
                            startMessageShown = true;
                        }

                        stringToSave += poolUsageLog + "\n\n";
                    }
                }

                break;
        }

        if (stringToSave.Length > 0)
        {
            string fileName = "PoolUsageStats.txt";

#if UNITY_EDITOR
            try
            {
                if (File.Exists(fileName))
                {
                    File.Delete(fileName);
                }

                using (FileStream stream = File.Open(fileName, FileMode.OpenOrCreate))
                {
                    StreamWriter streamWriter = new StreamWriter(stream);
                    streamWriter.Write(stringToSave);
                    streamWriter.Close();
                }

                Debug.Log("Saved pool manager stats to: " + fileName);
            }
            catch
            {
                Debug.LogError("Poolmanager failed to save report!");
            }
#else
            try
            {
                string wholeFilePath = Application.persistentDataPath + "/" + fileName;

                if (File.Exists(wholeFilePath))
                {
                    File.Delete(wholeFilePath);
                }

                using (FileStream stream = File.Open(wholeFilePath, FileMode.OpenOrCreate))
                {
                    StreamWriter streamWriter = new StreamWriter(stream);
                    streamWriter.Write(stringToSave);
                    streamWriter.Close();
                }

                Debug.Log("Saved pool manager stats to: " + wholeFilePath);
            }
            catch
            {
                Debug.LogError("Poolmanager failed to save report!");
            }
#endif
        }
    }

    #endregion

    #region Handling of pooled objects

    public bool CheckIfPoolExists(string poolIdentifier)
    {
        if (string.IsNullOrEmpty(poolIdentifier) == false && this.poolTypeManagerDictionary != null)
        {
            return this.poolTypeManagerDictionary.ContainsKey(poolIdentifier);
        }

        return false;
    }

    public bool CheckIfPoolExists(GameObject prefabToUse)
    {
        if (prefabToUse != null && this.poolPrefabManagerDictionary != null)
        {
            return this.poolPrefabManagerDictionary.ContainsKey(prefabToUse);
        }

        return false;
    }

    /// <summary>
    ///     Call this function on scene load with the groups of pooled objects to prespawn. All the rest of the pools will be
    ///     cleared.
    /// </summary>
    /// <param name="groupIdentifiers">List of group identifiers to prespawn</param>
    /// <param name="clearOtherPools">should be true if all other pools should be cleared at the same time, false if they should be left alone.</param>
    public void PreSpawnPooledObjectsInGroups(List<string> groupIdentifiers, bool clearOtherPools = true)
    {
        if (groupIdentifiers == null)
        {
            return;
        }

        List<TypePoolManager> nonPreSpawnManagers = new List<TypePoolManager>(this.activePoolManagers);

        foreach (string identifier in groupIdentifiers)
        {
            if (string.IsNullOrEmpty(identifier) == false && this.groupPoolManagerDictionary.ContainsKey(identifier))
            {
                foreach (TypePoolManager poolManager in this.groupPoolManagerDictionary[identifier])
                {
                    if (poolManager == null)
                    {
                        continue;
                    }

                    if (nonPreSpawnManagers.Contains(poolManager))
                    {
                        poolManager.ClearPool();
                        poolManager.PreSpawnPoolObjects();
                        nonPreSpawnManagers.Remove(poolManager);
                    }
                }
            }
            else
            {
                Debug.LogWarning("Call to nonexistant group when prespawing pooled objects! identifier was: " + identifier);
            }
        }

        if (clearOtherPools == false)
        {
            return;
        }

        foreach (TypePoolManager poolManager in nonPreSpawnManagers)
        {
            if (poolManager != null && poolManager.setObjectsUnderPoolManager == false)
            {
                poolManager.ClearPool();
            }
        }
    }

    /// <summary>
    /// Prespawn objects in a poolmanagers group without clearing anything else.
    /// </summary>
    /// <param name="groupIdentifier">Identifier of the pool manager group to prespawn.</param>
    public void PreSpawnPooledObjectsInAGroup(string groupIdentifier)
    {
        if (this.groupPoolManagerDictionary != null && (string.IsNullOrEmpty(groupIdentifier) == false && this.groupPoolManagerDictionary.ContainsKey(groupIdentifier)))
        {
            foreach (TypePoolManager poolManager in this.groupPoolManagerDictionary[groupIdentifier])
            {
                if (poolManager == null)
                {
                    continue;
                }

                poolManager.ClearPool();
                poolManager.PreSpawnPoolObjects();
            }
        }
        else
        {
            Debug.LogWarning("Call to nonexistant group when prespawing pooled objects!");
        }
    }

    /// <summary>
    /// PreSpawns objects in a specific TypePoolManager with the given identifier.
    /// </summary>
    /// <param name="poolIdentifier">Identifier for the TypePoolManager to prespawn.</param>
    public void PreSpawnPooledObjectsInAManager(string poolIdentifier)
    {
        if (this.poolTypeManagerDictionary != null && (string.IsNullOrEmpty(poolIdentifier) == false && this.poolTypeManagerDictionary.ContainsKey(poolIdentifier)))
        {
            TypePoolManager manager = this.poolTypeManagerDictionary[poolIdentifier];
            
            if (manager != null)
            {
                manager.PreSpawnPoolObjects();
            }
        }
        else
        {
            Debug.LogWarning("Call to nonexistent typepoolmanager when prespawning pooled objects! Identifier was: " + poolIdentifier);
        }
    }

    /// <summary>
    ///     Get pooled object from pool with string identifier.
    /// </summary>
    /// <param name="type">String identifier for the pooled object</param>
    /// <returns>Pooled object if available, null if no pool manager exists.</returns>
    public GameObject GetPooledObject(string type)
    {
        return GetPooledObject(type, new PoolObjectSettings());
    }

    /// <summary>
    ///     Get pooled object from pool with string identifier.
    /// </summary>
    /// <param name="type">String identifier for the pooled object</param>
    /// <param name="settings">Pooled object settings to use</param>
    /// <returns>Pooled object if available, null if no pool manager exists.</returns>
    public GameObject GetPooledObject(string type, PoolObjectSettings settings)
    {
        if (string.IsNullOrEmpty(type))
        {
            Debug.Log("Trying to get pooled object with null or empty string, propably typo somewhere!?!?");
            return null;
        }
        
        if (this.poolTypeManagerDictionary.ContainsKey(type))
        {
            return this.poolTypeManagerDictionary[type].GetObjectFromPool(settings);
        }

        Debug.Log("Call to non existant type of Pool Manager with string: " + type);

        return null;
    }

    /// <summary>
    ///     Get pooled object from pool with prefab identifier.
    /// </summary>
    /// <param name="prefabToUse">Prefab used in the poolmanager.</param>
    /// <param name="settings">Pooled object settings to use</param>
    /// <returns>Pooled object if available, null if the prefab given was null.</returns>
    public GameObject GetPooledObject(GameObject prefabToUse, PoolObjectSettings settings)
    {
        if (prefabToUse != null)
        {
            if (this.poolPrefabManagerDictionary.ContainsKey(prefabToUse))
            {
                return this.poolPrefabManagerDictionary[prefabToUse].GetObjectFromPool(settings);
            }

            TypePoolManager typePoolManager = new TypePoolManager();

            typePoolManager.PooledPrefabDataToUse = ScriptableObject.CreateInstance<PooledPrefabData>();
            typePoolManager.PooledPrefabDataToUse.prefab = prefabToUse;

            this.activePoolManagers.Add(typePoolManager);
            this.poolPrefabManagerDictionary.Add(prefabToUse, typePoolManager);

            if (IsTrackingEnabled)
            {
                typePoolManager.TurnOnPoolTracking();
            }

            return typePoolManager.GetObjectFromPool(settings);
        }

        Debug.Log("Call to PoolManager with null prefab!");

        return null;
    }

    public PooledObject GetPooledObjectScript(string type, PoolObjectSettings settings)
    {
        if (string.IsNullOrEmpty(type))
        {
            Debug.Log("Trying to get pooled object with null or empty string, propably typo somewhere!?!?");
            return null;
        }

        if (this.poolTypeManagerDictionary.ContainsKey(type))
        {
            return this.poolTypeManagerDictionary[type].GetObjectScriptFromPool(settings);
        }

        Debug.Log("Call to non existant type of Pool Manager with string: " + type);

        return null;
    }

    public PooledObject GetPooledObjectScript(GameObject prefabToUse, PoolObjectSettings settings)
    {
        if (prefabToUse != null)
        {
            if (this.poolPrefabManagerDictionary.ContainsKey(prefabToUse))
            {
                return this.poolPrefabManagerDictionary[prefabToUse].GetObjectScriptFromPool(settings);
            }

            TypePoolManager typePoolManager = new TypePoolManager();

            typePoolManager.PooledPrefabDataToUse = new PooledPrefabData
            {
                prefab = prefabToUse
            };

            this.activePoolManagers.Add(typePoolManager);
            this.poolPrefabManagerDictionary.Add(prefabToUse, typePoolManager);

            if (IsTrackingEnabled)
            {
                typePoolManager.TurnOnPoolTracking();
            }

            return typePoolManager.GetObjectScriptFromPool(settings);
        }

        Debug.Log("Call to PoolManager with null prefab!");

        return null;
    }
    
    /// <summary>
    ///     Returns an object back to the pool with string identifier.
    /// </summary>
    /// <param name="type">String identifier for the pooled object</param>
    /// <param name="objectToReturn">Pooled objects parent gameobject.</param>
    /// <returns>true if return was succesful, false if it was not.</returns>
    public bool ReturnObjectToPool(string type, GameObject objectToReturn)
    {
        if (this.poolTypeManagerDictionary.ContainsKey(type))
        {
            this.poolTypeManagerDictionary[type].ReturnObjectBackToPool(objectToReturn);

            return true;
        }

        Debug.Log("Call to non existant type of Pool Manager!");

        return false;
    }

    /// <summary>
    ///     Returns an object back to the pool with prefab identifier.
    /// </summary>
    /// <param name="prefabToUse">prefab identifier for the pooled object</param>
    /// <param name="objectToReturn">Pooled objects parent gameobject.</param>
    /// <returns>true if return was succesful, false if it was not.</returns>
    public bool ReturnObjectToPool(GameObject prefabToUse, GameObject objectToReturn)
    {
        if (this.poolPrefabManagerDictionary.ContainsKey(prefabToUse))
        {
            this.poolPrefabManagerDictionary[prefabToUse].ReturnObjectBackToPool(objectToReturn);

            return true;
        }

        Debug.Log("Call to non existant type of prefab Pool Manager!");

        return false;
    }

    /// <summary>
    ///     Deactivates and sets all active objects back into their pools.
    /// </summary>
    public void ReturnAllObjectsToPools()
    {
        if (this.activePoolManagers == null)
        {
            return;
        }

        foreach (TypePoolManager poolManager in this.activePoolManagers)
        {
            if (poolManager != null)
            {
                poolManager.ReturnAllObjectsToPool();
            }
        }
    }

    /// <summary>
    ///  Deactivates and sets all pooled objects that are marked as dont destroy on scene change back into their pools.
    /// </summary>
    public void ReturnAllDontDestroyOnSceneChangeObjectsToPools()
    {
        if (this.dontDestroyOnSceneChangeList == null)
        {
            return;
        }

        foreach (TypePoolManager poolManager in this.dontDestroyOnSceneChangeList)
        {
            if (poolManager != null && poolManager.returnAutomaticallyOnSceneChange)
            {
                poolManager.ReturnAllObjectsToPool();
            }
        }
    }

    /// <summary>
    ///     Clears all the active poolmanagers out of pooled objects.
    /// </summary>
    public void ClearAllPools()
    {
        if (this.activePoolManagers != null)
        {
            for (int i = 0; i < this.activePoolManagers.Count; i++)
            {
                TypePoolManager poolManager = this.activePoolManagers[i];

                if (poolManager != null)
                {
                    poolManager.ClearPool();
                }
            }
        }
    }

    public static PooledObject GetPooledObject(PooledPrefabData data, PoolObjectSettings settings = null)
    {
        if (Instance == null)
            return null;

        return Instance.GetPooledObjectScript(data.identifier, settings);
    }

    /// <summary>
    ///     Tries to return the object to their pool, if they are part of a pool. Otherwise, destroys the gameobject.
    /// </summary>
    /// <param name="gameObject">Object that we want to try and return to the pool.</param>
    public static void ReturnObjectToPoolOrDestroyIt(GameObject gameObject)
    {
        if (gameObject == null)
        {
            return;
        }

        if (Instance == null)
        {
            Destroy(gameObject);
            return;
        }

        PooledObject pooledObjectScript = gameObject.GetComponent<PooledObject>();

        if (pooledObjectScript != null)
        {
            pooledObjectScript.ReturnPooledObjectBackToPool();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static void ReturnObjectsToPool(List<PooledObject> pooledObjects)
    {
        foreach (PooledObject poolable in pooledObjects)
        {
            if (poolable == null) continue;
            poolable.ReturnPooledObjectBackToPool();
        }
    }

    #endregion

}

[Serializable]
public class PoolObjectSettings
{

    internal Action actionToCallWhenActivated = null;
    internal float appearanceTime = 0f;
    internal ForceMode forceModeToSend = ForceMode.Impulse;
    internal string objectName = string.Empty;
    internal GameObject parentToSet = null;
    internal Transform transformToFollow = null;
    internal Vector3 positionToSet = new Vector3();
    internal Vector3 rigidbodyForceToSet = new Vector3();
    internal Quaternion rotationToSet = new Quaternion();
    internal bool setLocalPositionAndRotation = false;
    internal bool setRigidBodyForce = false;
    internal float startTime;

    internal float timeBeforeReturningToPool = 0; // If this is zero or lower, it is never automatically returned to the pool, but has to be manually done.

}