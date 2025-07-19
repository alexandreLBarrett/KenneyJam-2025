using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SoundBank", menuName = "Scriptable Objects/SoundBank")]
public class SoundBank : ScriptableObject
{
    public List<AudioClip> gameMusics;

    public AudioClip menuMusic;

    public AudioClip engineSound;

}
