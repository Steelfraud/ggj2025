using PlayerController;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerPanel : MonoBehaviour
{
    public TextMeshProUGUI PlayerNameLabel;
    public TextMeshProUGUI PlayerDamageLabel;
    public Image PlayerPortrait;
    public Image PlayerBorder;

    private PlayerAvatar linkedAvatar;

    public void SetPlayerInfo(string playerName, PlayerAvatar playerAvatar, Sprite portrait)
    {
        PlayerNameLabel.text = playerName;
        //PlayerNameLabel.color = playerAvatar.MyColor.PlayerColor;

        PlayerBorder.color = playerAvatar.MyColor.PlayerColor;
        PlayerPortrait.sprite = portrait;

        linkedAvatar = playerAvatar;
    }

    public void Update()
    {
        if (linkedAvatar != null && linkedAvatar.isActiveAndEnabled)
        {
            int numberToShow = Mathf.RoundToInt((linkedAvatar.PushMultiplier - 1) * 100);
            PlayerDamageLabel.text = numberToShow + "%";
        }
        else
        {
            PlayerDamageLabel.text = "DED";
        }
    }

}