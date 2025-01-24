using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundEffectManager : MonoBehaviour
{
    public static SoundEffectManager instance;
    public AudioSource effectAudioSource;
    public string SoundEffectFolderName = "SoundEffects";
    public string SoundEffectBankFoldername = "SoundEffects";

    private Dictionary<string, SoundEffectData> soundEffectDictionary;
    private Dictionary<string, SoundEffectBankData> soundEffectBankDictionary;

    // Use this for initialization
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            if (this.effectAudioSource == null)
            {
                this.effectAudioSource = gameObject.AddComponent<AudioSource>();
            }

            InitializeData();
        }
    }

    private void InitializeData()
    {
        this.soundEffectDictionary = LogicUtils.FillDictionary(this.SoundEffectFolderName, (SoundEffectData data) => data.clipIdentifier);
        this.soundEffectBankDictionary = LogicUtils.FillDictionary(this.SoundEffectBankFoldername, (SoundEffectBankData data) => data.clipIdentifier);
    }

    internal void PlaySoundEffect(string effectToPlay)
    {
        if (this.effectAudioSource && this.soundEffectDictionary != null && this.soundEffectDictionary.ContainsKey(effectToPlay))
        {
            SoundEffectData data = this.soundEffectDictionary[effectToPlay];

            if (data != null && data.clipToPlay)
            {
                this.effectAudioSource.PlayOneShot(data.clipToPlay, data.effectVolume);
            }
        }
    }

    internal void PlaySoundEffectBank(string effectToPlay)
    {
        if (this.effectAudioSource && this.soundEffectBankDictionary != null && this.soundEffectBankDictionary.ContainsKey(effectToPlay))
        {
            SoundEffectBankData data = this.soundEffectBankDictionary[effectToPlay];
            SoundEffect sound = data.GetRandomSoundEffect();

            if (sound != null && sound.clipToPlay)
            {
                this.effectAudioSource.PlayOneShot(sound.clipToPlay, sound.effectVolume);
            }
        }
    }
    
}
