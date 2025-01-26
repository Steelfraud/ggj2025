using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Windows;

public class CharacterSelectUI : MonoBehaviour
{
    public GameObject CharacterSelectParent;

    private List<CharacterUI> characterUIs = new List<CharacterUI>();
    
    private void OnEnable()
    {
        SoundEffectManager.instance.PlaySoundEffect("Choose_Bubbler");
        DrawCharacterSelects();
    }

    public void OnPlayerJoined(PlayerInput input)
    {
        PlayerData newData = DataManager.Instance.AddNewPlayer(input.devices[0].deviceId);
        input.gameObject.GetComponent<PlayerMenuInputController>().SetupController(newData, this);

        DrawCharacterSelects();
    }

    public void PlayerLeft(PlayerInput input)
    {
        if (DataManager.Instance == null || input.devices.Count == 0)
            return;

        RemovePlayer(input.devices[0].deviceId);
    }

    public void RemovePlayer(int deviceID)
    {
        DataManager.Instance.RemoveNewPlayer(deviceID);
        DrawCharacterSelects();
    }

    public void TryToStartGame()
    {
        //if (false)
        {
            SceneManager.LoadScene(1);
        }
    }

    public void NextCharacter(PlayerData player)
    {
        int currentIndex = player.CharacterIndex;
        List<int> characters = DataManager.Instance.GetAvailableCharacterIndexes();

        int indexOf = characters.IndexOf(currentIndex);

        if (indexOf >= 0)
        {
            if (indexOf + 1 >= characters.Count) 
            {
                currentIndex = 0;
            }
            else
            {
                currentIndex = characters[indexOf + 1];
            }
        }
        else
        {
            currentIndex = 0;
        }

        player.CharacterIndex = currentIndex;
        DrawCharacterSelects();
    }

    public void PreviousCharacter(PlayerData player)
    {
        int currentIndex = player.CharacterIndex;
        List<int> characters = DataManager.Instance.GetAvailableCharacterIndexes();

        int indexOf = characters.IndexOf(currentIndex);

        if (indexOf >= 0)
        {
            if (indexOf - 1 < 0)
            {
                currentIndex = characters.Last();
            }
            else
            {
                currentIndex = characters[indexOf - 1];
            }
        }
        else
        {
            currentIndex = 0;
        }

        player.CharacterIndex = currentIndex;
        DrawCharacterSelects();
    }

    public void SelectCharacter(PlayerData player)
    {
        DataManager.Instance.SelectCharacter(player.DeviceID, player.CharacterIndex);
    }

    public void CancelCharacter(PlayerData player)
    {
        DataManager.Instance.UnSelectCharacter(player.DeviceID);
    }

    public void DrawCharacterSelects()
    {
        foreach (CharacterUI ui in characterUIs)
        {
            Destroy(ui.gameObject);
        }

        characterUIs.Clear();

        List<PlayerData> list = new List<PlayerData>(DataManager.Instance.activePlayers);
        list = list.OrderByDescending(x => x.PlayerIndex).ToList();

        foreach (PlayerData data in list)
        {
            PlayerVisualInfo visuals = DataManager.Instance.GetCharacterVisuals(data.CharacterIndex);

            CharacterUI ui = Instantiate(visuals.UIPrefab, CharacterSelectParent.transform).GetComponent<CharacterUI>();
            characterUIs.Add(ui);
            ui.PlayerLabel.text = "Player " + data.PlayerIndex;
        }
    }

}