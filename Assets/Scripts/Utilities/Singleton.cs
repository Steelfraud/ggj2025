using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;
    public static T Instance
    {
        protected set { _instance = value; }
        get
        {
            if (applicationIsQuitting)
            {
                //Debug.LogWarning("[Singleton] Instance '" + typeof(T) + "' already destroyed on application quit.");
                return null;
            }

            return _instance;
        }
    }

    protected static bool CreateSingleton(T newInstance, bool setDontDestroy)
    {
        if (_instance != null)
        {
            return false;
        }

        if (newInstance == null)
        {
            return false;
        }

        if (setDontDestroy && newInstance.transform.parent == null)
        {
            DontDestroyOnLoad(newInstance.gameObject);
        }

        _instance = newInstance;
        return true;
    }

    public bool SetDontDestroy = true;

    protected static bool applicationIsQuitting = false;

    protected virtual void OnApplicationQuit()
    {
        applicationIsQuitting = true;
    }

}