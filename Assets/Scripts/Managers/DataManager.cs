using System.Collections.Generic;
using UnityEngine;

public class DataManager : Singleton<DataManager>
{

    public List<PlayerVisualInfo> AvailableColors;

    public List<PlayerData> activePlayers = new List<PlayerData>();

    private Dictionary<int, PlayerVisualInfo> playerColors = new Dictionary<int, PlayerVisualInfo>();

    private void Awake()
    {
        if (CreateSingleton(this, SetDontDestroy))
        {
            GameData.LoadDataFiles();
        }
    }

    public List<int> GetAvailableCharacterIndexes()
    {
        List<int> indexes = new List<int>();

        for (int i = 0; i < AvailableColors.Count; i++)
        {
            if (playerColors.ContainsValue(AvailableColors[i]) == false)
            {
                int indexToSave = i;
                indexes.Add(indexToSave);
            }
        }

        return indexes;
    }

    public PlayerData AddNewPlayer(int deviceID)
    {
        PlayerData playerData = new PlayerData() { DeviceID = deviceID, PlayerIndex = activePlayers.Count + 1 };
        activePlayers.Add(playerData);

        List<int> ints = GetAvailableCharacterIndexes();

        playerData.CharacterIndex = ints.GetRandomElementFromList();

        return playerData;
    }

    public void RemoveNewPlayer(int deviceID)
    {
        activePlayers.RemoveAll(x => x.DeviceID == deviceID);

        if (playerColors.ContainsKey(deviceID))
        {
            playerColors.Remove(deviceID);
        }
    }

    public void SelectCharacter(int controllerID, int characterIndex)
    {
        if (playerColors.ContainsKey(controllerID))
        {
            playerColors[controllerID] = AvailableColors[characterIndex];
        }
        else
        {
            playerColors.Add(controllerID, AvailableColors[characterIndex]);
        }
    }

    public void UnSelectCharacter(int controllerID)
    {
        playerColors.Remove(controllerID);
    }

    public int GetCharacterIndex(PlayerVisualInfo character)
    {
        return AvailableColors.IndexOf(character);
    }

    public bool HasSelectedCharacter(int controllerID)
    {
        return playerColors.ContainsKey(controllerID);
    }

    public PlayerVisualInfo GetPlayerColor(int controllerID)
    {
        if (playerColors.ContainsKey(controllerID))
        {
            return playerColors[controllerID];
        }

        return GetRandomCharacter(controllerID);
    }

    public PlayerVisualInfo GetCharacterVisuals(int characterIndex)
    {
        return AvailableColors[characterIndex];
    }

    public PlayerVisualInfo GetPlayerCharacter(PlayerData data)
    {        
        return GetPlayerColor(data.DeviceID);
    }

    public PlayerVisualInfo GetRandomCharacter(int controllerID)
    {
        PlayerVisualInfo colorToSet = null;

        List<PlayerVisualInfo> availableColors = new List<PlayerVisualInfo>(AvailableColors);
        availableColors.RemoveAll(x => playerColors.ContainsValue(x));

        if (availableColors.Count == 0)
        {
            availableColors = new List<PlayerVisualInfo>(AvailableColors);
        }

        colorToSet = availableColors.GetRandomElementFromList();
        playerColors.Add(controllerID, colorToSet);
        colorToSet.PlayerIndex = playerColors.Count;

        return colorToSet;
    }

}

public class PlayerData
{
    internal int PlayerIndex;
    internal int DeviceID;
    internal int PlayerWins = 0;
    internal int CharacterIndex;
}

[System.Serializable]
public class PlayerVisualInfo
{

    public Material PlayerMaterial;
    public Color PlayerColor;
    public GameObject PlayerModel;
    public Sprite PlayerPortrait;
    public string AudioID;
    public GameObject UIPrefab;
    internal int PlayerIndex;
    internal int DeviceID;
    internal int PlayerWins = 0;
    internal int CharacterIndex;
}