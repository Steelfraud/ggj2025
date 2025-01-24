using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameData : ScriptableObject
{
    public bool Active = true;

    protected string cachedDataID;

    public virtual string DataID
    {
        get
        {
            if (string.IsNullOrEmpty(this.cachedDataID) == false)
            {
                return this.cachedDataID;
            }

            return name;
        }
    }

    protected virtual void DataLoaded()
    {
        this.cachedDataID = name;
    }

    #region Static data stuff

    private static List<Type> m_derivedTypes;
    private static Dictionary<Type, List<GameData>> m_keyValuePairs;

    private static List<Type> DerivedTypes
    {
        get
        {
            if (m_derivedTypes == null)
            {
                m_derivedTypes = new List<Type>();

                foreach (var domainAssembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    var assemblyTypes = domainAssembly.GetTypes().Where(type => type.IsSubclassOf(typeof(GameData)) && !type.IsAbstract);

                    m_derivedTypes.AddRange(assemblyTypes);
                }
            }

            return m_derivedTypes;
        }
    }

    public static Dictionary<Type, List<GameData>> KeyValuePairs
    {
        get
        {
            if (m_keyValuePairs == null)
            {
                LoadDataFiles();
            }

            return m_keyValuePairs;
        }
    }

    public static void LoadDataFiles()
    {
        m_keyValuePairs = new Dictionary<Type, List<GameData>>();
        List<GameData> all = Resources.LoadAll<GameData>("GameData Assets").ToList();

        foreach (GameData data in all)
        {
            Type type = data.GetType();

            if (m_keyValuePairs.ContainsKey(type) == false)
            {
                m_keyValuePairs.Add(type, new List<GameData>());
            }
            
            m_keyValuePairs[type].Add(data);
            data.DataLoaded();
        }

        m_derivedTypes = new List<Type>();

        foreach (var domainAssembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            var assemblyTypes = domainAssembly.GetTypes().Where(type => type.IsSubclassOf(typeof(GameData)) && !type.IsAbstract);
            m_derivedTypes.AddRange(assemblyTypes);
        }
    }
    
    public static T Get<T>(string ID, bool includeDisabled = false) where T : GameData
    {
        List<T> allRelevant = GetAll<T>(includeDisabled);
        return allRelevant.Find(x => x.DataID == ID);
    }

    public static List<T> GetAll<T>(bool includeDisabled = false) where T : GameData
    {
        List<T> output = new List<T>();
        var derivedTypes = new List<Type>();

        foreach (Type type in DerivedTypes)
        {
            if (type.IsSubclassOf(typeof(T)))
            {
                derivedTypes.Add(type);
            }
        }

        foreach (var derivedType in derivedTypes)
        {
            output.AddRange(GetAll(derivedType).ConvertAll(x => (T)x));
        }

        if (KeyValuePairs.ContainsKey(typeof(T)) == false)
        {
            return output;
        }

        var pairs = KeyValuePairs[typeof(T)];

        if (pairs == null || pairs.Count == 0)
        {
            Debug.LogWarning("No data of type " + typeof(T) + " found in Dictionary");
            return output;
        }

        output.AddRange(pairs.ConvertAll(x => (T)x));

        if (!includeDisabled)
        {
            output.RemoveAll(x => !x.Active);
        }

        return output;
    }

    private static List<object> GetAll(Type t)
    {
        List<object> output = new List<object>();

        if (KeyValuePairs.TryGetValue(t, out List<GameData> value))
        {
            output.AddRange(value);
        }

        return output;
    }

    #endregion

}