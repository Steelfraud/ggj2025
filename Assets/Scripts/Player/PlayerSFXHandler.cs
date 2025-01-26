using System.Collections.Generic;
using PlayerController;
using UnityEngine;

public class PlayerSFXHandler : MonoBehaviour
{
    public PlayerAvatar MyAvatar;

    private Dictionary<string, float> effectLastPlayed = new Dictionary<string, float>();

    public void PlaySoundEffect(string effectID)
    {
        string playerEffectID = this.MyAvatar.MyColor.AudioID + effectID;
        SoundEffectManager.instance.PlaySoundEffect(playerEffectID);

        if (effectLastPlayed.ContainsKey(playerEffectID))
        {
            this.effectLastPlayed[playerEffectID] = Time.timeSinceLevelLoad;
        }
        else
        {
            this.effectLastPlayed.Add(playerEffectID, Time.timeSinceLevelLoad);
        }
    }

}