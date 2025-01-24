using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class MusicPlaylistManager : MonoBehaviour
{

    public static MusicPlaylistManager Instance;

    [FormerlySerializedAs("musicPlayer")] public MusicPlayer MusicPlayer;
    public string startingPlaylistIdentifier;

    private MusicPlaylistData currentlyRunningPlaylist;
    private Dictionary<string, MusicPlaylistData> playlistDictionary;

    // Use this for initialization
    private void Start()
    {
        if (Instance == null)
        {
            Instance = this;
            this.playlistDictionary = LogicUtils.FillDictionary("Playlists", (MusicPlaylistData data) => data.identifier);
            ChangePlaylist(this.startingPlaylistIdentifier);
        }
    }

    public void ChangePlaylist(string playlistName)
    {
        if (this.MusicPlayer == null)
        {
            return;
        }

        if (this.playlistDictionary == null || this.playlistDictionary.ContainsKey(playlistName) == false)
        {
            return;
        }

        MusicPlaylistData data = this.playlistDictionary[playlistName];

        if (this.currentlyRunningPlaylist != data)
        {
            this.currentlyRunningPlaylist = data;
            this.MusicPlayer.ChangePlaylist(data);
        }
    }

    public void PauseCurrentPlaylist()
    {
        if (this.MusicPlayer != null)
        {
            this.MusicPlayer.PauseCurrentPlaylist();
        }
    }

    public void UnPauseCurrentPlaylist()
    {
        if (this.MusicPlayer != null)
        {
            this.MusicPlayer.UnPauseCurrentPlaylistWithFade();
        }
    }

}