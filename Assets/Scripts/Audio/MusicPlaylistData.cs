using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MusicPlaylist", menuName = "Soihtu/General/Playlist", order = 1)]
public class MusicPlaylistData : ScriptableObject
{

    public string identifier;
    public List<MusicTrack> playList;
    public bool shuffle;
    public float trackChangeTime;

}