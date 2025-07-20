using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SoundBank", menuName = "Scriptable Objects/SoundBank")]
public class SoundBank : ScriptableObject
{
    public List<AudioClip> gameMusics;

    public AudioClip menuMusic;
    
    public AudioClip lostMenuMusic;

    public AudioClip engineSound;

    public AudioClip MatchWon;
    public AudioClip MatchLost;
    public AudioClip GameLost;
    public AudioClip CarBreak;
    

    public List<AudioClip> CarDamageTaken;

}
