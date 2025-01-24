using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class ExtensionUtils
{
    private static System.Random rng = new System.Random();

    public static bool IsValid(this object obj)
    {
        return obj != null && !obj.Equals(null);
    }

    public static bool IsEmpty<T>(this List<T> list)
	{
		return list == null || list.Count == 0;
	}

	public static bool IsEmpty<T>(this T[] array)
	{
		return array == null || array.Length == 0;
	}

    public static int FloorToNearestTen(this int number)
    {
        int numOfDivisions = 0;
        float divisibleNumber = number;

        while (true)
        {
            divisibleNumber /= 10;
            if (divisibleNumber < 1)
            {
                break;
            }
            numOfDivisions++;
        }

        divisibleNumber *= 10;
        divisibleNumber = Mathf.Floor(divisibleNumber);

        for (int i = 0; i < numOfDivisions; i++)
        {
            divisibleNumber *= 10;
        }

        return (int)divisibleNumber;
    }

    public static void Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    public static T GetRandomElementFromList<T>(this List<T> list)
    {
        if (list.IsEmpty())
            return default(T);

        int amountOfItemsInList = list.Count;
        int randomIndexInList = UnityEngine.Random.Range(0, amountOfItemsInList);
        T randomElement = list[randomIndexInList];
        return randomElement;
    }

}