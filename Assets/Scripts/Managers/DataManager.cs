using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Windows;

public class DataManager : Singleton<DataManager>
{

    public List<PlayerVisualInfo> AvailableColors;

    private Dictionary<int, PlayerVisualInfo> playerColors = new Dictionary<int, PlayerVisualInfo>();

    private void Awake()
    {
        if (CreateSingleton(this, SetDontDestroy))
        {
            GameData.LoadDataFiles();
        }
    }

    public PlayerVisualInfo GetPlayerColor(int controllerID)
    {
        PlayerVisualInfo colorToSet = null;

        if (playerColors.ContainsKey(controllerID))
        {
            colorToSet = playerColors[controllerID];
        }
        else
        {
            List<PlayerVisualInfo> availableColors = new List<PlayerVisualInfo>(AvailableColors);
            availableColors.RemoveAll(x => playerColors.ContainsValue(x));
            colorToSet = availableColors.GetRandomElementFromList();
            playerColors.Add(controllerID, colorToSet);
            colorToSet.PlayerIndex = playerColors.Count;
        }

        return colorToSet;
    }

}

[System.Serializable]
public class PlayerVisualInfo
{
    public Material PlayerMaterial;
    public Color PlayerColor;
    public GameObject PlayerModel;
    internal int PlayerIndex;
}