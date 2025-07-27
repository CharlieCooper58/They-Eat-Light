using System.Collections.Generic;
using UnityEngine;

public class SoundPlayerPool : MonoBehaviour
{
    public int maxAudioSources;

    AudioClip masterClip;
    float audioVol;
    List<AudioSource> audioSources;

    int audioSourceIndex;

    private void Awake()
    {
        audioSources = new List<AudioSource>();
    }
    public void SetAudioClip(AudioClip clip)
    {
        masterClip = clip;
    }
    public void PlayAudioClip(Vector3 position, float vol, float pitch)
    {
        AudioSource source;
        if (audioSources.Count > audioSourceIndex + 1)
        {
            source = audioSources[audioSourceIndex];
        }
        else
        {
            GameObject newSource = new GameObject();
            newSource.transform.parent = transform;
            source = newSource.AddComponent<AudioSource>();
            source.clip = masterClip;
            source.loop = false;
            audioSources.Add(source);
        }

        source.transform.position = position;
        source.volume = vol;
        source.pitch = pitch;
        source.Play();

        audioSourceIndex = (1 + audioSourceIndex) % maxAudioSources;
    }
}