using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    public SoundBank soundBank;
    [SerializeField]
    private AudioSource audioSourceObject;
    [SerializeField]
    private float musicVolume;

    private AudioSource musicSource;
    private List<int> nextMusicIndices = new();

    private void Start()
    {
        Instance = this;
        StartMusicLoop();
    }

    public static void Shuffle<T>(IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    private void StartMusicLoop()
    {
        for (int i = 0; i < soundBank.gameMusics.Count; i++)
            nextMusicIndices.Add(i);
        Shuffle(nextMusicIndices);
        musicSource = Instantiate(audioSourceObject, transform);
        musicSource.clip = soundBank.gameMusics[nextMusicIndices[0]];
        musicSource.volume = musicVolume;
        musicSource.Play();
        nextMusicIndices.RemoveAt(0);
    }

    private void Update()
    {
        if (!musicSource.isPlaying && musicSource.time >= musicSource.clip.length)
        {
            if (nextMusicIndices.Count == 0)
            {
                for (int i = 0; i < soundBank.gameMusics.Count; i++)
                    nextMusicIndices.Add(i);
                Shuffle(nextMusicIndices);
            }
            musicSource.Stop();
            musicSource.clip = soundBank.gameMusics[nextMusicIndices[0]];
            musicSource.Play();
            nextMusicIndices.RemoveAt(0);
        }
    }

    public AudioSource CreatePermanentAudioSource(AudioClip clip)
    {
        AudioSource source = Instantiate(audioSourceObject, transform);
        source.clip = clip;
        source.loop = true;
        source.volume = 1;
        source.Play();
        return source;
    }

    public void FadeOutPermanentAudioSource(AudioSource source)
    {
        Destroy(source);
    }

    public AudioSource PlayInstantSound(AudioClip clip, float volume=1)
    {
        AudioSource source = Instantiate(audioSourceObject, transform);
        source.clip = clip;
        source.volume = volume;
        source.Play();
        Destroy(source, clip.length);
        return source;
    }
}
