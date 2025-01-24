using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public static class UIUtils
{

    
    
}

public class TextUIPool : GenericUIPool<TextMeshProUGUI> { }

public class ButtonUIPool : GenericUIPool<Button> { }

public class GenericUIPool<T> where T : MonoBehaviour
{

    private T defaultItem;
    private Transform elementParent;
    private List<T> pooledItems = new List<T>();

    public List<T> ElementsInList => new List<T>(this.pooledItems);
    public List<T> ActiveElementsInList => new List<T>(this.pooledItems).FindAll(x => x.gameObject.activeSelf);

    public void SetupPool(T firstItem)
    {
        SetupPool(firstItem, firstItem.transform.parent);
    }

    public void SetupPool(T firstItem, Transform parent)
    {
        this.pooledItems.Add(firstItem);
        defaultItem = firstItem;
        this.elementParent = parent;
        ResetPool();
    }

    public void ResetPool()
    {
        foreach (T item in this.pooledItems) { item.gameObject.SetActive(false); }
    }
    
    public T GetNextItem()
    {
        T elementToGet = this.pooledItems.Find(x => x.gameObject.activeSelf == false);

        if (elementToGet == null)
        {
            elementToGet = GameObject.Instantiate(this.defaultItem.gameObject, this.elementParent).GetComponent<T>();
            this.pooledItems.Add(elementToGet);
        }

        elementToGet.gameObject.SetActive(true);
        return elementToGet;
    }

    public void ReturnToPool(T itemToReturn)
    {
        itemToReturn.gameObject.SetActive(false);
    }

}

public class FloatingTextInfo
{

    public GameObject floatingTextParent;
    public string textToShow;
    public Color colorOfText;

    public FloatingTextInfo(GameObject gObj, string text, Color colour)
    {
        this.floatingTextParent = gObj;
        this.textToShow = text;
        this.colorOfText = colour;
    }

}

public interface IUIDescriptor
{

    string ObjectVisibleName { get; }
    string shortDescription { get; }
    string longDescription { get; }

    Sprite GetSprite();

}