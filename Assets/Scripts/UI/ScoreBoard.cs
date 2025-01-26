using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class ScoreBoard : MonoBehaviour
{
    [SerializeField]
    private TMP_Text scoreText;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnEnable()
    {
        if (GameManager.Instance == null) return;

        int playerCount = GameManager.Instance.joinedPlayers.Count;
        List<PlayerVisualInfo> playerDataList = new List<PlayerVisualInfo>();

        scoreText.text = "Wins:\n";

        for (int i = 1; i < playerCount+1; i++)
        {
            playerDataList.Add(DataManager.Instance.GetPlayerColor(i));
        }

        playerDataList.OrderByDescending(p => p.PlayerWins);

        foreach (PlayerVisualInfo playerData in playerDataList)
        {
            scoreText.text += "Player " + playerData.PlayerIndex + ": " + playerData.PlayerWins + "\n";
        }
    }
}
