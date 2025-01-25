using PlayerController;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameUI : MonoBehaviour
{
    public class PlayerPanelUIPool : GenericUIPool<PlayerPanel> { }

    public TextMeshProUGUI TimerLabel;
    public GameObject GameEndPanelParent;

    public List<PlayerPanel> AvailablePlayerPanels;
    public PlayerPanel FirstPlayerPanel;

    private PlayerPanelUIPool panelPool;

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

    public void ToggleGameEndPanel(bool setTo)
    {
        GameEndPanelParent.SetActive(setTo);
    }

    public void RestartLevel()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.buildIndex);
    }

    public void AddNewPlayerUI(string playerName, PlayerAvatar avatar)
    {
        PlayerPanel panel = AvailablePlayerPanels.Find(x => x.isActiveAndEnabled == false);
        panel.gameObject.SetActive(true);
        panel.SetPlayerInfo(playerName, avatar);
    }

}