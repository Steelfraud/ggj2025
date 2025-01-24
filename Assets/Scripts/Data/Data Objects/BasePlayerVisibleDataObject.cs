using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(order = 20, menuName = "Soihtu/General Resources/Base Visual Object")]
public class BasePlayerVisibleDataObject : GameData, IUIDescriptor
{
    public string VisibleName;
    [TextAreaAttribute]
    public string ShortDescription;
    [TextAreaAttribute]
    public string LongDescription;
    public TextToSpriteDictionary SpriteDictionary = new TextToSpriteDictionary() { {"Default", null} };
    public PlayerVisibilitySetting ItemVisibilitySettings = PlayerVisibilitySetting.VisibleAlways;

    public virtual Sprite GetRelevantSprite(string identifier)
    {
        if (this.SpriteDictionary.ContainsKey(identifier))
        {
            return this.SpriteDictionary[identifier];
        }

        //if (IconManager.instance != null)
        //{
        //    return IconManager.instance.DefaultPlaceHolderSprite;
        //}

        return null;
    }

    public string ObjectVisibleName 
    {
        get 
        {
            return VisibleName;
        }
    }

    public string shortDescription { get { return ShortDescription; } }
    public string longDescription { get { return LongDescription; } }

    public string NameAndDescriptionTooltip => ObjectVisibleName + ": " + ShortDescription;
    public string LongOrShortDescription => string.IsNullOrEmpty(this.LongDescription) ? ShortDescription : LongDescription;

    public Sprite GetSprite()
    {
        return GetRelevantSprite("Default");
    }

    public virtual bool IsUnlocked()
    {
        return true;
    }

    public virtual bool IsVisibleToPlayer()
    {
        switch (this.ItemVisibilitySettings)
        {
            case PlayerVisibilitySetting.HiddenAlways:
                return false;
            case PlayerVisibilitySetting.HiddenUntilUnlocked:
                return IsUnlocked();
            default:
                return true;
        }
    }

    public virtual bool ShowAsMysteryItem()
    {
        if (this.ItemVisibilitySettings == PlayerVisibilitySetting.MysteryUntilUnlocked)
        {
            return IsUnlocked() == false;
        }

        return false;
    }

    public static List<T> GetAllVisibleItems<T>() where T : BasePlayerVisibleDataObject
    {
        List<T> allItems = GetAll<T>();
        allItems.RemoveAll(x => x.IsVisibleToPlayer() == false);

        List<T> mysteryItems = allItems.FindAll(x => x.ShowAsMysteryItem());
        allItems.RemoveAll(x => mysteryItems.Contains(x));
        allItems.AddRange(mysteryItems);

        return allItems;
    }

}

public enum PlayerVisibilitySetting
{

    VisibleAlways,
    HiddenAlways,
    HiddenUntilUnlocked,
    MysteryUntilUnlocked

}