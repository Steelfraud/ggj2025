using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;

public static class LogicUtils
{

    private const string hexchars = "0123456789abcdef";

    public static bool NullCheck(Object obj, Component owner)
    {
        if (ReferenceEquals(owner, null) || owner == null)
        {
            string variable = "empty component";

            if (ReferenceEquals(obj, null) == false)
            {
                variable = obj.GetType().ToString();
            }

            System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace();
            Debug.LogError("tried to nullcheck " + variable + " on an empty owner while executing " /* +trace*/);

            return false;
        }

        if (ReferenceEquals(obj, null) || obj == null)
        {
            System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace();
            Debug.LogError("some variable missing on " + owner.name + " while executing " /* +trace*/);

            return false;
        }

        return true;
    }

    public static bool MultiNullCheck(Component owner, params Object[] args)
    {
        foreach (Object obj in args)
        {
            if (NullCheck(obj, owner) == false)
            {
                return false;
            }
        }

        return true;
    }

    public static bool IsPlayer(Component obj)
    {
        if (obj == null)
        {
            return false;
        }

        return obj.CompareTag("Player");
    }

    public static bool IsEnemy(Component obj)
    {
        if (obj == null)
        {
            return false;
        }

        return obj.CompareTag("Enemy");
    }

    public static float GetAnimationLength(Animator anim, string animationName)
    {
        float time = -1;

        RuntimeAnimatorController ac = anim.runtimeAnimatorController;

        foreach (AnimationClip clip in ac.animationClips)
        {
            if (clip.name == animationName)
            {
                time = clip.length;
            }
        }

        return time;
    }

    public static float GetCurrentAnimationLength(this Animator anim)
    {
        float length = anim.GetCurrentAnimatorStateInfo(0).length / anim.GetCurrentAnimatorStateInfo(0).speed;

        return HasValue(length) ? length : 0;
    }

    public static YieldInstruction WaitTillAnimationEnds(Animator anim)
    {
        if (anim == null)
        {
            Debug.LogError("Animator null when waiting for animaton to end");

            return new WaitForEndOfFrame();
        }

        float waitfor;
        waitfor = GetCurrentAnimationLength(anim);

        return new WaitForSeconds(waitfor);
    }

    public static bool HasValue(float value)
    {
        return !float.IsNaN(value) && !float.IsInfinity(value);
    }

    public static Dictionary<T, T2> FillDictionary<T, T2>(string dataPath, Func<T2, T> keyFunc) where T2 : Object
    {
        T2[] data = Resources.LoadAll<T2>(dataPath);
        Dictionary<T, T2> dictionary = new Dictionary<T, T2>();

        if (data != null && data.Length > 0)
        {
            foreach (T2 obj in data)
            {
                if (obj == null)
                {
                    continue;
                }

                if (!dictionary.ContainsKey(keyFunc(obj)))
                {
                    dictionary.Add(keyFunc(obj), obj);
                }
            }
        }

        return dictionary;
    }

    public static Dictionary<T, T2> FillDictionaryWithList<T, T2>(List<T2> objects, Func<T2, T> keyFunc)
    {
        Dictionary<T, T2> dictionary = new Dictionary<T, T2>();

        if (objects != null)
        {
            foreach (T2 obj in objects)
            {
                if (obj == null)
                {
                    continue;
                }

                if (!dictionary.ContainsKey(keyFunc(obj)))
                {
                    dictionary.Add(keyFunc(obj), obj);
                }
            }
        }

        return dictionary;
    }

    public static List<GameObject> GetAllChildrenOf(Transform parent)
    {
        List<GameObject> children = new List<GameObject>();

        foreach (Transform child in parent)
        {
            children.Add(child.gameObject);
            children.AddRange(GetAllChildrenOf(child));
        }

        return children;
    }

    public static string RandomString(int length)
    {
        string path = Path.GetRandomFileName();
        path = path.Replace(".", ""); // Remove period.

        return path;
    }

    public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component
    {
        T component = gameObject.GetComponent<T>();

        if (component == null)
        {
            component = gameObject.AddComponent<T>();
        }

        return component;
    }

    public static bool IsInRange<T>(this T value, T min, T max) where T : struct, IComparable
    {
        if (value.CompareTo(min) > 0 && value.CompareTo(max) < 0)
        {
            return true;
        }

        return false;
    }

    public static T RandomEnumValue<T>()
    {
        Array v = Enum.GetValues(typeof(T));

        return (T) v.GetValue(new System.Random().Next(v.Length));
    }

    public static T GetWeighedRandom<T>(List<T> objectList, Func<T, float> keyFunc)
    {
        if (objectList == null || keyFunc == null)
        {
            return default;
        }

        if (objectList.Count == 0)
        {
            return default;
        }

        List<float> weights = new List<float>();

        foreach (T obj in objectList)
        {
            if (obj == null)
            {
                continue;
            }

            weights.Add(keyFunc(obj));
        }

        return GetWeighedRandom(objectList, weights);
    }

    public static T GetWeighedRandom<T>(List<T> objectList, List<float> weights)
    {
        if (objectList == null || weights == null)
        {
            return default;
        }

        if (objectList.Count != weights.Count)
        {
            return default;
        }

        if (objectList.Count == 0)
        {
            return default;
        }

        int indexChosen = ChooseWeighedRandomIndexFromList(weights);

        return objectList[indexChosen];
    }

    public static List<T> GetIndividualEnums<T>(T enumObj) where T : Enum
    {
        List<T> eventTypes = Enum.GetValues(typeof(T)).Cast<T>().Where(m => enumObj.HasFlag(m)).ToList();
        return eventTypes;
    }

    public static int ChooseWeighedRandomIndexFromList(List<float> weights)
    {
        if (weights == null)
        {
            return 0;
        }

        if (weights.Count <= 1)
        {
            return 0;
        }

        float totalWeights = 0;

        foreach (float weight in weights)
        {
            totalWeights += weight;
        }

        float currentWeight = 0;
        float randomizedValue = UnityEngine.Random.Range(0f, totalWeights);

        for (int i = 0; i < weights.Count; i++)
        {
            float selectedWeight = weights[i];

            if (randomizedValue >= currentWeight && randomizedValue < currentWeight + selectedWeight)
            {
                return i;
            }

            currentWeight += selectedWeight;
        }

        Debug.LogError("Logic failure happened somewhere! ChooseWeighedRandomIndexFromList should never come here!");

        return 0;
    }

    public static Color FromU32(this Color c, uint v)
    {
        c.r = (float) ((v >> 16) & 0xff) / 255.0f;
        c.g = (float) ((v >> 8) & 0xff) / 255.0f;
        c.b = (float) (v & 0xff) / 255.0f;
        c.a = 1.0f;

        return c;
    }

    public static string ToHexString(this Color c)
    {
        Color32 tmp = c;

        return tmp.r.ToString("X2") + tmp.g.ToString("X2") + tmp.b.ToString("X2") + tmp.a.ToString("X2");
    }

    public static string ToHexString(this byte[] data)
    {
        return data.ToHexString(0, data.Length);
    }

    public static string ToHexString(this byte[] data, int offset, int length)
    {
        char[] hex = new char[length * 2];
        int i = 0;

        for (int j = offset; j < offset + length; ++j)
        {
            byte b = data[j];
            hex[i * 2] = hexchars[(b >> 4) & 0xF];
            hex[i * 2 + 1] = hexchars[b & 0xF];
            ++i;
        }

        return new string(hex);
    }

    public static string GetSystemLanguage()
    {
        SystemLanguage loc = Application.systemLanguage;

        switch (loc)
        {
            case SystemLanguage.Afrikaans:
                return "af";
            case SystemLanguage.Arabic:
                return "ar";
            case SystemLanguage.Basque:
                return "eu";
            case SystemLanguage.Belarusian:
                return "be";
            case SystemLanguage.Bulgarian:
                return "bg";
            case SystemLanguage.Catalan:
                return "ca";
            case SystemLanguage.Chinese:
                return "zh-CN";
            case SystemLanguage.Czech:
                return "cs";
            case SystemLanguage.Danish:
                return "da";
            case SystemLanguage.Dutch:
                return "nl";
            case SystemLanguage.English:
                return "en";
            case SystemLanguage.Estonian:
                return "et";
            case SystemLanguage.Faroese:
                return "fo";
            case SystemLanguage.Finnish:
                return "fi";
            case SystemLanguage.French:
                return "fr";
            case SystemLanguage.German:
                return "de";
            case SystemLanguage.Greek:
                return "el";
            case SystemLanguage.Hebrew:
                return "iw";
            case SystemLanguage.Icelandic:
                return "is";
            case SystemLanguage.Indonesian:
                return "id";
            case SystemLanguage.Italian:
                return "it";
            case SystemLanguage.Japanese:
                return "ja";
            case SystemLanguage.Korean:
                return "ko";
            case SystemLanguage.Latvian:
                return "lv";
            case SystemLanguage.Lithuanian:
                return "lt";
            case SystemLanguage.Norwegian:
                return "no";
            case SystemLanguage.Polish:
                return "pl";
            case SystemLanguage.Portuguese:
                return "pt";
            case SystemLanguage.Romanian:
                return "ro";
            case SystemLanguage.Russian:
                return "ru";
            case SystemLanguage.SerboCroatian:
                return "sh";
            case SystemLanguage.Slovak:
                return "sk";
            case SystemLanguage.Slovenian:
                return "sl";
            case SystemLanguage.Spanish:
                return "es";
            case SystemLanguage.Swedish:
                return "sv";
            case SystemLanguage.Thai:
                return "th";
            case SystemLanguage.Turkish:
                return "tr";
            case SystemLanguage.Ukrainian:
                return "uk";
            case SystemLanguage.Vietnamese:
                return "vi";
            case SystemLanguage.ChineseSimplified:
                return "zh-CN";
            case SystemLanguage.ChineseTraditional:
                return "zh-TW";
            case SystemLanguage.Unknown:
                break;
            case SystemLanguage.Hungarian:
                return "hu";
            default:
                break;
        }

        return null;
    }

    public static DateTime UnixTimestampToDate(long timestamp)
    {
        DateTime unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        return unixEpoch.AddSeconds(timestamp).ToLocalTime();
    }

    public static long DateToUnixTimestamp(DateTime dt)
    {
        DateTime dtUtc = dt.ToUniversalTime();
        DateTime unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        return (long) dtUtc.Subtract(unixEpoch).TotalSeconds;
    }

    public static object GetObjectFromString(string classAsString)
    {
        Type objectType = Type.GetType(classAsString);

        if (objectType == null)
        {
            Debug.Log("object type was null???");
            return default;
        }

        object newObject = Activator.CreateInstance(objectType);

        if (newObject == null)
        {
            return default;
        }

        return newObject;
    }

    public static List<string> GetSubClassesAsStrings(Type typeWanted, List<string> ignoredClassNames = null, bool useAssemblyQualifiedName = false, bool includeBaseClass = false)
    {
        List<Type> availableTypes = null; 

        if (typeWanted.IsInterface)
        {
            availableTypes = Assembly.GetAssembly(typeWanted).GetTypes().Where(t => typeWanted.IsAssignableFrom(t) && t != typeWanted).ToList();
        }
        else
        {
            availableTypes = Assembly.GetAssembly(typeWanted).GetTypes().Where(t => t.IsSubclassOf(typeWanted)).ToList();
        }

        List<string> typesAsStrings = new List<string>();

        if (includeBaseClass)
        {
            string baseTypeName = useAssemblyQualifiedName ? typeWanted.FullName : typeWanted.Name;
            typesAsStrings.Add(baseTypeName);
        }

        foreach (Type activeType in availableTypes)
        {
            if (ignoredClassNames != null && ignoredClassNames.Contains(activeType.Name))
            {
                continue;
            }

            string typeName = useAssemblyQualifiedName ? activeType.FullName : activeType.Name;
            typesAsStrings.Add(typeName);
        }

        return typesAsStrings;
    }

}