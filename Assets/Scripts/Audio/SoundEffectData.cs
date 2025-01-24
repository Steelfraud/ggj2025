using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Sound Effect", menuName = "Soihtu/General/Sound effect", order = 1)]
public class SoundEffectData : ScriptableObject
{
    public AudioClip clipToPlay;
    public string clipIdentifier;
    [Range(0, 1f)]
    public float effectVolume = 1f;
}
