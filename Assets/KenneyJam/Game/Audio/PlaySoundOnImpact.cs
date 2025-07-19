using System.Collections.Generic;
using UnityEngine;

public class PlaySoundOnImpact : MonoBehaviour
{
    public float impactMagnitudeThreshold;
    public float volume = 1;
    public float randomPitchRange = .1f;

    public List<AudioClip> clips;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.relativeVelocity.magnitude > impactMagnitudeThreshold)
        {
            AudioSource src = SoundManager.Instance.PlayInstantSound(clips[Random.Range(0, clips.Count)], volume);
            src.pitch += Random.Range(-randomPitchRange, +randomPitchRange);
        }
    }
}
