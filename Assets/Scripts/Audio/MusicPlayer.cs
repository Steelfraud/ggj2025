using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

/// <summary>
///     Used to run a playlist of music that can be shuffled.
/// </summary>
public class MusicPlayer : MonoBehaviour
{

    public bool debugMessagesOn;
    public AudioSource musicSource;
    public AudioSource musicSource2;

    //public string preTrackInfoText;
    //public bool showChangeOnUI;

    private AudioSource currentMusicSource;
    private MusicPlaylist currentPlaylist;
    private MusicTrack currentTrack;
    private AudioClip lastTrack;
    private bool stopCurrentOperation;

    private float FadeTime
    {
        get
        {
            if (this.currentPlaylist != null && this.currentPlaylist.dataBeingUsed != null)
            {
                return this.currentPlaylist.dataBeingUsed.trackChangeTime;
            }

            return 0;
        }
    }

    public bool ChangeTrack { get; private set; }

    public bool FadeOut { get; private set; }

    // Use this for initialization
    private void Start()
    {
        if (this.musicSource == null)
        {
            this.musicSource = this.gameObject.AddComponent<AudioSource>();
        }

        if (this.musicSource2 == null)
        {
            this.musicSource2 = this.gameObject.AddComponent<AudioSource>();
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if (this.ChangeTrack == false && this.FadeOut == false && this.currentMusicSource != null)
        {
            if (this.FadeTime > 0.1f && this.currentMusicSource.clip.length - this.currentMusicSource.time < this.FadeTime)
            {
                ChangeMusicTrack();
            }
            else if (this.currentMusicSource.clip.length - this.currentMusicSource.time < 0.1f)
            {
                ChangeMusicTrackInstantly();
            }
        }
    }

    /// <summary>
    ///     Immediately starts playing the given floor clip with no fade in or out.
    /// </summary>
    public void ChangeMusicTrackInstantly()
    {
        if (this.currentPlaylist == null || this.currentPlaylist.dataBeingUsed == null)
        {
            return;
        }

        if (this.currentMusicSource == null)
        {
            this.currentMusicSource = this.musicSource;
        }

        this.currentTrack = this.currentPlaylist.GetNextMusicTrack();

        this.currentMusicSource.clip = this.currentTrack.clipToPlay;
        this.currentMusicSource.volume = this.currentTrack.trackVolume;
        this.currentMusicSource.Play();

        if (this.debugMessagesOn)
        {
            Debug.Log("Instantly changed clip to " + this.currentMusicSource.clip.name + ".");
        }
    }

    /// <summary>
    ///     Starts the fade to new music track in the given time, and if there is any other music playing, they will faded out
    ///     in the same time.
    /// </summary>
    /// <param name="fadeTime">Amount of time for the fadeIn in seconds.</param>
    public void ChangeMusicTrack()
    {
        if (this.currentPlaylist == null || this.currentPlaylist.dataBeingUsed == null)
        {
            return;
        }

        if (this.stopCurrentOperation)
        {
            return;
        }

        if (this.ChangeTrack || this.FadeOut)
        {
            this.stopCurrentOperation = true;
            StartCoroutine(StartChangeAfterLastOneWasQuit());
            return;
        }

        if (this.FadeTime > 0)
        {
            if (this.currentMusicSource == null)
            {
                this.currentMusicSource = this.musicSource;
            }

            if (this.musicSource.isPlaying && this.musicSource2.isPlaying == false)
            {
                this.currentMusicSource = this.musicSource2;
            }
            else
            {
                this.currentMusicSource = this.musicSource;
            }

            this.currentTrack = this.currentPlaylist.GetNextMusicTrack();

            this.currentMusicSource.volume = 0;
            this.currentMusicSource.clip = this.currentTrack.clipToPlay;
            this.currentMusicSource.Play();

            StartCoroutine(ChangeToNewAudioSource(this.FadeTime));

            if (this.debugMessagesOn)
            {
                Debug.Log("Changing clip to " + this.currentMusicSource.clip.name + ".");
            }
        }
        else
        {
            ChangeMusicTrackInstantly();
        }
    }

    public void FadeOutCurrent()
    {
        FadeOutCurrent(this.FadeTime);
    }

    /// <summary>
    ///     Used to just fade out the current track, if there is anything playing.
    /// </summary>
    /// <param name="fadeTime">Fade out time in seconds.</param>
    public void FadeOutCurrent(float fadeTime)
    {
        if (this.currentMusicSource == null || !this.currentMusicSource.isPlaying || !(this.currentMusicSource.volume > 0) || this.FadeOut)
        {
            return;
        }

        if (this.stopCurrentOperation)
        {
            return;
        }

        if (fadeTime > 0)
        {
            this.FadeOut = true;
            StartCoroutine(FadeOutMusic(fadeTime));
        }
        else
        {
            this.currentMusicSource.volume = 0;
            this.currentMusicSource.Stop();
        }
    }

    /// <summary>
    ///     Used to pause the current track.
    /// </summary>
    public void PauseCurrentPlaylist()
    {
        if (this.currentMusicSource != null)
        {
            this.currentMusicSource.Pause();
        }
    }

    /// <summary>
    ///     Used to unpause the currently paused track and fade it back in.
    /// </summary>
    public void UnPauseCurrentPlaylistWithFade()
    {
        if (this.currentMusicSource == null || this.currentPlaylist == null)
        {
            return;
        }

        this.currentMusicSource.UnPause();

        if (this.currentPlaylist.dataBeingUsed.trackChangeTime > 0)
        {
            this.currentMusicSource.volume = 0;
            this.ChangeTrack = true;
        }

        if (this.debugMessagesOn)
        {
            Debug.Log("Starting fade back in.");
        }
    }

    public bool CheckIfSomethingIsPlaying()
    {
        if (this.musicSource.isPlaying || this.musicSource2.isPlaying)
        {
            return true;
        }

        return false;
    }

    public void ChangePlaylist(MusicPlaylistData newPlaylist)
    {
        this.currentPlaylist = new MusicPlaylist {dataBeingUsed = newPlaylist};
        ChangeMusicTrack();
    }

    private IEnumerator StartChangeAfterLastOneWasQuit()
    {
        yield return new WaitWhile(() => this.stopCurrentOperation);
        ChangeMusicTrack();
    }

    private IEnumerator ChangeToNewAudioSource(float fadeTime)
    {
        if (this.currentTrack == null && this.currentPlaylist != null)
        {
            yield break;
        }

        AudioSource previousSource = null;

        if (this.currentMusicSource == this.musicSource)
        {
            previousSource = this.musicSource2;
        }
        else
        {
            previousSource = this.musicSource;
        }

        if (this.currentMusicSource == null || previousSource == null)
        {
            yield break;
        }

        this.stopCurrentOperation = false;
        this.ChangeTrack = true;
        float timeSpent = 0;
        float previousVolume = 0;

        if (this.currentMusicSource == this.musicSource)
        {
            previousVolume = this.musicSource2.volume;
        }
        else
        {
            previousVolume = this.musicSource.volume;
        }

        while (this.stopCurrentOperation == false)
        {
            float lerpValue = Mathf.Clamp01(timeSpent / fadeTime);
            this.currentMusicSource.volume = Mathf.Lerp(0, this.currentTrack.trackVolume, lerpValue);
            previousSource.volume = Mathf.Lerp(previousVolume, 0, lerpValue);

            if (lerpValue >= 1)
            {
                if (this.debugMessagesOn)
                {
                    if (previousSource.clip != null && this.currentMusicSource.clip != null)
                    {
                        Debug.Log("Changed " + this.currentMusicSource.clip.name + " on and faded out " + previousSource.clip.name + ".");
                    }
                    else if (this.currentMusicSource.clip != null)
                    {
                        Debug.Log("Changed to: " + this.currentMusicSource.clip.name + ".");
                    }
                }

                previousSource.Stop();

                break;
            }

            yield return null;
            timeSpent += Time.deltaTime;
        }

        previousSource.volume = 0;

        if (this.stopCurrentOperation == false)
        {
            this.currentMusicSource.volume = this.currentTrack.trackVolume;
        }

        this.stopCurrentOperation = false;
        this.ChangeTrack = false;
    }

    private IEnumerator FadeOutMusic(float fadeTime)
    {
        if (this.currentMusicSource == null || this.currentTrack == null)
        {
            yield break;
        }

        if (this.currentMusicSource == null)
        {
            yield break;
        }

        this.stopCurrentOperation = false;
        this.FadeOut = true;
        float timeSpent = 0;
        float previousVolume = this.currentMusicSource.volume;

        while (this.stopCurrentOperation == false)
        {
            float lerpValue = Mathf.Clamp01(timeSpent / fadeTime);
            this.currentMusicSource.volume = Mathf.Lerp(previousVolume, 0, lerpValue);

            if (lerpValue >= 1)
            {
                break;
            }

            yield return null;
            timeSpent += Time.deltaTime;
        }

        this.currentMusicSource.volume = 0;
        this.currentMusicSource.Stop();

        this.stopCurrentOperation = false;
        this.FadeOut = false;
    }

}

public class MusicPlaylist
{

    internal MusicPlaylistData dataBeingUsed;

    private List<MusicTrack> currentPlaylist;

    private MusicTrack lastTrack;

    internal MusicTrack GetNextMusicTrack()
    {
        if (this.currentPlaylist == null || this.currentPlaylist.Count == 0)
        {
            GenerateNewPlaylist();
        }

        if (this.currentPlaylist != null && this.currentPlaylist.Count > 0)
        {
            this.lastTrack = this.currentPlaylist[0];
            this.currentPlaylist.RemoveAt(0);

            return this.lastTrack;
        }

        return null;
    }

    private void GenerateNewPlaylist()
    {
        if (this.dataBeingUsed == null || this.dataBeingUsed.playList == null)
        {
            return;
        }

        this.currentPlaylist = new List<MusicTrack>();

        if (this.dataBeingUsed.shuffle && this.dataBeingUsed.playList.Count > 2)
        {
            List<int> trackIndexes = new List<int>();

            for (int i = 0; i < this.dataBeingUsed.playList.Count; i++)
            {
                trackIndexes.Add(i);
            }

            int randomIndex = Random.Range(0, trackIndexes.Count);

            while (this.lastTrack != null && this.dataBeingUsed.playList[randomIndex].clipToPlay == this.lastTrack.clipToPlay)
            {
                randomIndex = Random.Range(0, trackIndexes.Count);
            }

            for (int i = 1; i < trackIndexes.Count; i++)
            {
                int temp = trackIndexes[i];
                randomIndex = Random.Range(i, trackIndexes.Count);
                trackIndexes[i] = trackIndexes[randomIndex];
                trackIndexes[randomIndex] = temp;
            }

            foreach (int trackIndex in trackIndexes)
            {
                this.currentPlaylist.Add(this.dataBeingUsed.playList[trackIndex]);
            }
        }
        else if (this.dataBeingUsed.shuffle && this.dataBeingUsed.playList.Count == 2)
        {
            if (Random.Range(0, this.dataBeingUsed.playList.Count) == 0)
            {
                this.currentPlaylist.Add(this.dataBeingUsed.playList[0]);
                this.currentPlaylist.Add(this.dataBeingUsed.playList[1]);
            }
            else
            {
                this.currentPlaylist.Add(this.dataBeingUsed.playList[1]);
                this.currentPlaylist.Add(this.dataBeingUsed.playList[0]);
            }
        }
        else
        {
            foreach (MusicTrack clippy in this.dataBeingUsed.playList)
            {
                this.currentPlaylist.Add(clippy);
            }
        }
    }

}

[Serializable]
public class MusicTrack
{

    public string artistName;
    public AudioClip clipToPlay;

    public string trackName;
    public float trackVolume;

}