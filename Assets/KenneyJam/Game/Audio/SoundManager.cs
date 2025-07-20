using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    public SoundBank soundBank;
    [SerializeField]
    private AudioSource audioSourceObject;
    [SerializeField]
    private AudioSource musicSourceObject;
    [SerializeField]
    private float musicVolume;

    public AudioMixer audioMixer;

    private AudioSource musicSource;
    private List<int> nextMusicIndices = new();

    private void Start()
    {
        Instance = this;
        StartMusicLoop();

        audioMixer.GetFloat("MusicVolume", out float volume1);
        var g = GameObject.Find("MusicSlider");
        if (g != null)
            g.GetComponent<Slider>().value = ToDB(volume1);
        audioMixer.GetFloat("SfxVolume", out float volume2);
        g = GameObject.Find("SfxSlider");
        if (g != null)
            g.GetComponent<Slider>().value = ToDB(volume2);
    }

    //public float ToDB(float v) { return Mathf.Pow(10, v / 20f); }
    //public float FromDB(float v) { return Mathf.Pow(10, v / 20f); }

    public float ToDB(float v) { return v; }
    public float FromDB(float v) { return v; }

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

    public void SetMusicLevel(float level)
    {
        audioMixer.SetFloat("MusicVolume", FromDB(level));
    }

    public void SetSFXLevel(float level)
    {
        audioMixer.SetFloat("SfxVolume", FromDB(level));
    }

    private void StartMusicLoop()
    {
        if (SceneManager.GetActiveScene().path.Contains("Menu") 
            || SceneManager.GetActiveScene().path.Contains("Garage") 
            || SceneManager.GetActiveScene().path.Contains("Victory"))
        {
            musicSource = Instantiate(musicSourceObject, transform);
            musicSource.clip = soundBank.menuMusic;
            musicSource.volume = musicVolume;
            musicSource.loop = true;
            musicSource.Play();
            return;
        }

        for (int i = 0; i < soundBank.gameMusics.Count; i++)
            nextMusicIndices.Add(i);
        Shuffle(nextMusicIndices);
        musicSource = Instantiate(musicSourceObject, transform);
        musicSource.clip = soundBank.gameMusics[nextMusicIndices[0]];
        musicSource.volume = musicVolume;
        musicSource.Play();
        nextMusicIndices.RemoveAt(0);
    }

    private void Update()
    {
        if (!musicSource.isPlaying && !musicSource.loop && musicSource.time >= musicSource.clip.length)
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

    public AudioSource CreatePermanentAudioSource(AudioClip clip, float volume = 1)
    {
        AudioSource source = Instantiate(audioSourceObject, transform);
        source.clip = clip;
        source.loop = true;
        source.volume = volume;
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
    public AudioSource PlayInstantSound(IList<AudioClip> clipBank, float volume=1, float randomPitchAmout=.1f)
    {
        AudioSource source = Instantiate(audioSourceObject, transform);
        source.clip = clipBank[Random.Range(0, clipBank.Count)];
        source.volume = volume;
        source.pitch += Random.Range(-randomPitchAmout, +randomPitchAmout);
        source.Play();
        Destroy(source, source.clip.length);
        return source;
    }
}
