using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Sound Effect List", menuName = "Soihtu/General/Sound Effect List", order = 1)]
public class SoundEffectBankData : ScriptableObject
{
    public string clipIdentifier;
    public List<SoundEffect> soundEffectList = new List<SoundEffect>();

    internal SoundEffect GetRandomSoundEffect()
    {
        if (this.soundEffectList.Count == 0)
        {
            return null;
        }

        if (this.soundEffectList.Count == 1)
        {
            return this.soundEffectList[0];
        }

        return this.soundEffectList[Random.Range(0, this.soundEffectList.Count - 1)];
    }

}

[System.Serializable]
public class SoundEffect
{
    public AudioClip clipToPlay;
    [Range(0, 1f)]
    public float effectVolume = 1f;
}