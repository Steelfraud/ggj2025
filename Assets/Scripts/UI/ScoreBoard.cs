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

        List<PlayerData> playerDataList = new List<PlayerData>(DataManager.Instance.activePlayers);
        playerDataList = playerDataList.OrderByDescending(x => x.PlayerWins).ToList();

        scoreText.text = "Wins:\n";

        foreach (PlayerData playerData in playerDataList)
        {
            scoreText.text += "Player " + playerData.PlayerIndex + ": " + playerData.PlayerWins + "\n";
        }
    }
}
