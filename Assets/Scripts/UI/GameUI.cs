using PlayerController;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    public TextMeshProUGUI TimerLabel;
    public GameObject GameEndPanelParent;
    public TextMeshProUGUI WinnerText;
    public FloatingTextHandler TextHandler;
    public Image portraitImage;
    public Image playerBackground;

    public List<PlayerPanel> AvailablePlayerPanels;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        foreach (PlayerPanel panel in AvailablePlayerPanels)
        {
            panel.gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (TimerLabel != null)
        {
            if (GameManager.Instance != null)
            {
                TimerLabel.text = Mathf.RoundToInt(GameManager.Instance.RoundTimer).ToString();
            }
            else
            {
                TimerLabel.text = "";
            }
        }
    }

    public void ShowGameEnd(string winnerText, Sprite portrait, Color color)
    {
        ToggleGameEndPanel(true);
        WinnerText.text = winnerText;

        if (portrait != null)
        {
            portraitImage.sprite = portrait;
        }
        
        playerBackground.color = color;
    }

    public void ToggleGameEndPanel(bool setTo)
    {
        GameEndPanelParent.SetActive(setTo);
    }

    public void AddNewFloatingText(string text, Color textColor, Vector3 position)
    {
        TextHandler.AddNewTextToQueue(new FloatingTextData()
        {
            positionToSet = position,
            textColor = textColor,
            textToSet = text,
            timeToStayActiveFor = 3f,
            textMovementPerSecond = new Vector3(0, 1, 0)
        });
        Debug.Log("added new text: " + text);
    }

    public void RestartLevel()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.buildIndex);
    }

    public void AddNewPlayerUI(string playerName, PlayerAvatar avatar, Sprite portrait)
    {
        PlayerPanel panel = AvailablePlayerPanels.Find(x => x.isActiveAndEnabled == false);
        panel.gameObject.SetActive(true);
        panel.SetPlayerInfo(playerName, avatar, portrait);
    }

}