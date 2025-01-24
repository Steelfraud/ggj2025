using UnityEngine;
using UnityEngine.Audio;

/// <summary>
///     Used to change volume of the different audio groups.
/// </summary>
public class AudioMixerManager : MonoBehaviour
{

    public static AudioMixerManager instance;

    public AudioMixer usedMixer;
    public string MasterVolumeName = "MasterVolume";
    public string EffectsVolumeName = "EffectsVolume";
    public string MusicVolumeName = "MusicVolume";

    public AnimationCurve volumeCurve;

    private float currentMasterVolumeLerp = 1f;
    public float MasterVolume
    {
        get
        {
            return this.currentMasterVolumeLerp;
        }
        set
        {
            this.currentMasterVolumeLerp = Mathf.Clamp01(value);

            AudioMixer audioMixer = this.usedMixer;

            if (audioMixer != null && this.volumeCurve != null)
            {
                float valueToSet = this.volumeCurve.Evaluate(this.currentMasterVolumeLerp);
                audioMixer.SetFloat(this.MasterVolumeName, valueToSet);
            }
        }
    }

    private float currentEffectsVolumeLerp = 1f;
    public float EffectsVolume
    {
        get
        {
            return this.currentEffectsVolumeLerp;
        }
        set
        {
            this.currentEffectsVolumeLerp = Mathf.Clamp01(value);

            AudioMixer audioMixer = this.usedMixer;

            if (audioMixer != null && this.volumeCurve != null)
            {
                float valueToSet = this.volumeCurve.Evaluate(this.currentEffectsVolumeLerp);
                audioMixer.SetFloat(this.EffectsVolumeName, valueToSet);
            }
        }
    }

    private float currentMusicVolumeLerp = 1f;
    public float MusicVolume
    {
        get
        {
            return this.currentMusicVolumeLerp;
        }
        set
        {
            this.currentMusicVolumeLerp = Mathf.Clamp01(value);

            AudioMixer audioMixer = this.usedMixer;

            if (audioMixer != null && this.volumeCurve != null)
            {
                float valueToSet = this.volumeCurve.Evaluate(this.currentMusicVolumeLerp);
                audioMixer.SetFloat(this.MusicVolumeName, valueToSet);
            }
        }
    }

    private void Start()
    {
        if (instance == null)
        {
            instance = this;
            Initialize();
        }
    }

    private void Initialize()
    {
        if (PlayerPrefs.HasKey(this.MasterVolumeName))
        {
            MasterVolume = PlayerPrefs.GetFloat(this.MasterVolumeName);
        }

        if (PlayerPrefs.HasKey(this.MusicVolumeName))
        {
            MusicVolume = PlayerPrefs.GetFloat(this.MusicVolumeName);
        }

        if (PlayerPrefs.HasKey(this.EffectsVolumeName))
        {
            EffectsVolume = PlayerPrefs.GetFloat(this.EffectsVolumeName);
        }
    }

    internal void SaveCurrentValues()
    {
        PlayerPrefs.SetFloat(this.MasterVolumeName, MasterVolume);
        PlayerPrefs.SetFloat(this.MusicVolumeName, MusicVolume);
        PlayerPrefs.SetFloat(this.EffectsVolumeName, EffectsVolume);
        PlayerPrefs.Save();
    }

}