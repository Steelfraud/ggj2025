using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Windows;

public class CharacterSelectUI : MonoBehaviour
{
    public GameObject CharacterSelectParent;
    public GameObject EmptySelect;

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

        DrawCharacterSelects(newData.PlayerIndex);
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
        DrawCharacterSelects(player.PlayerIndex);
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
        DrawCharacterSelects(player.PlayerIndex);
    }

    public void SelectCharacter(PlayerData player)
    {
        DataManager.Instance.SelectCharacter(player.DeviceID, player.CharacterIndex);
        SoundEffectManager.instance.PlaySoundEffect("Lock_In");
        CharacterUI ui = characterUIs.Find(x => x.AttachedPlayer == player);
        ui.LockAnimation();
    }

    public void CancelCharacter(PlayerData player)
    {
        DataManager.Instance.UnSelectCharacter(player.DeviceID);
        CharacterUI ui = characterUIs.Find(x => x.AttachedPlayer == player);
        ui.UnLockAnimation();
    }

    public void DrawCharacterSelects(int newPlayerIndex = -1)
    {
        foreach (CharacterUI ui in characterUIs)
        {
            Destroy(ui.gameObject);
        }

        characterUIs.Clear();

        List<PlayerData> list = new List<PlayerData>(DataManager.Instance.activePlayers);
        list = list.OrderByDescending(x => x.PlayerIndex).ToList();

        for (int i = 0; i < 4; i++)
        {
            if (i < list.Count)
            {
                PlayerData data = list[i];
                PlayerVisualInfo visuals = DataManager.Instance.GetCharacterVisuals(data.CharacterIndex);

                CharacterUI ui = Instantiate(visuals.UIPrefab, CharacterSelectParent.transform).GetComponent<CharacterUI>();
                characterUIs.Add(ui);
                ui.PlayerLabel.text = "Player " + data.PlayerIndex;
                ui.AttachedPlayer = data;

                if (data.PlayerIndex == newPlayerIndex)
                {
                    ui.StartAnimation();
                }
            }
            else
            {
                CharacterUI ui = Instantiate(EmptySelect, CharacterSelectParent.transform).GetComponent<CharacterUI>();
                characterUIs.Add(ui);
            }
        }
    }

}