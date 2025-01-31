using PlayerController;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMenuInputController : MonoBehaviour
{
    public PlayerInput MyInput;

    internal PlayerData PlayerData;

    private float startTime = 0;
    private CharacterSelectUI characterSelectUI;

    public void SetupController(PlayerData myData, CharacterSelectUI selectUI)
    {
        characterSelectUI = selectUI;
        PlayerData = myData;
        startTime = Time.timeSinceLevelLoad;
    }

    void OnMove(InputValue inputValue)
    {
        if (Time.timeSinceLevelLoad - startTime <= 0.25f)
            return;

        if (DataManager.Instance.HasSelectedCharacter(PlayerData.DeviceID))
        {
            return;
        }
        
        Vector2 inputVector = inputValue.Get<Vector2>();

        if (inputVector.x > 0.5f)
        {
            characterSelectUI.NextCharacter(PlayerData);
            //Debug.Log("next");
        }
        else if (inputVector.x < -0.5f)
        {
            characterSelectUI.PreviousCharacter(PlayerData);
            //Debug.Log("previous");
        }
    }

    void OnAccept(InputValue inputValue)
    {
        if (inputValue.isPressed == false)
            return;

        if (Time.timeSinceLevelLoad - startTime <= 0.25f)
            return;

        float inputPressed = inputValue.Get<float>();

        //Debug.Log("accept");

        if (DataManager.Instance.HasSelectedCharacter(PlayerData.DeviceID) == false)
        {
            characterSelectUI.SelectCharacter(PlayerData);
            //Debug.Log("lock in");
        }
        else
        {
            characterSelectUI.TryToStartGame();
            //Debug.Log("try start");
        }
    }


    void OnCancel(InputValue inputValue)
    {
        if (inputValue.isPressed == false)
            return;

        if (Time.timeSinceLevelLoad - startTime <= 0.25f)
            return;

        float inputPressed = inputValue.Get<float>();

        //Debug.Log("cancel");

        if (DataManager.Instance.HasSelectedCharacter(PlayerData.DeviceID))
        {
            characterSelectUI.CancelCharacter(PlayerData);
            //Debug.Log("unlock in");
        }
        else
        {
            characterSelectUI.RemovePlayer(PlayerData.DeviceID);
            Destroy(gameObject);
            //Debug.Log("im out");
        }        
    }

}