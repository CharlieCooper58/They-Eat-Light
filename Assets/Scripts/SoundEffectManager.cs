using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class SoundEffectManager : MonoBehaviour
{
    [SerializeField] AudioClip[] sounds;
    [SerializeField] AudioSource soundEffectAudio;
    [SerializeField] AudioSource musicAudio;

    int musicIndex;
    [SerializeField] AudioClip mainMenuMusic;
    [SerializeField] AudioClip[] gameMusicTracks;

    [SerializeField] AudioClip[] dialogueSounds;
    [SerializeField] AudioClip damageSound;

    [SerializeField] float masterVolume;
    [SerializeField] float musicVolume;
    [SerializeField] float voiceVolume;
    [SerializeField] float soundEffectVolume;

    public string menuBlipSoundName;
    [SerializeField] AudioClip menuSelectSound;


    [SerializeField] float sfxPitchRange;
    [SerializeField] float voicePitchRange;
    [SerializeField] float blipPitchRange;


    [SerializeField] SoundPlayerPool soundPlayerPoolPrefab;

    Dictionary<AudioClip, SoundPlayerPool> soundPlayerPools;
    private void Awake()
    {

        masterVolume = 1;//GameData.GetFloat("Master Volume", 1f);
        musicVolume = 1;//GameData.GetFloat("Music Volume", .8f);
        voiceVolume = 1;//GameData.GetFloat("Voice Volume", .8f);
        soundEffectVolume = 1;//GameData.GetFloat("Sound Volume", .8f);

        soundPlayerPools = new Dictionary<AudioClip, SoundPlayerPool>();
        musicIndex = Random.Range(0, gameMusicTracks.Length);
    }
    public void PlaySoundByName(string name, Vector3 position, float volumeScale = 1, float pitchRange = 0)
    {
        for (int i = 0; i < sounds.Length; i++)
        {
            if (sounds[i].name == name)
            {
                float pitch = Random.Range(1 - pitchRange, 1 + pitchRange);
                SoundPlayerPool pool;
                if (!soundPlayerPools.ContainsKey(sounds[i]))
                {
                    pool = Instantiate(soundPlayerPoolPrefab, transform);
                    pool.SetAudioClip(sounds[i]);
                    soundPlayerPools.Add(sounds[i], pool);
                }
                else
                {
                    pool = soundPlayerPools[sounds[i]];
                }
                pool.PlayAudioClip(position, masterVolume * soundEffectVolume * volumeScale, pitch);
            }
        }
    }

    public void PlayMainMenuMusic()
    {
        musicAudio.clip = mainMenuMusic;
        musicAudio.volume = masterVolume * musicVolume;
        musicAudio.Play();
        StopAllCoroutines();
    }
    public void PlayGameMusic()
    {
        musicIndex = (musicIndex + 1) % gameMusicTracks.Length;
        musicAudio.clip = gameMusicTracks[musicIndex];
        musicAudio.volume = masterVolume * musicVolume;
        musicAudio.Play();
        StartCoroutine(ChangeTrackAfterTrackEnds(musicAudio.clip.length - 0.5f));
    }

    IEnumerator ChangeTrackAfterTrackEnds(float trackLength)
    {
        yield return new WaitForSeconds(trackLength);
        PlayGameMusic();
    }

    public void SetMusicVolume(float value)
    {
        musicVolume = value;
        //GameData.SetFloat("Music Volume", musicVolume);
        musicAudio.volume = masterVolume * musicVolume;
    }

    public void SetMasterVolume(float value)
    {
        masterVolume = value;
        //GameData.SetFloat("Master Volume", masterVolume);
        musicAudio.volume = masterVolume * musicVolume;
    }
    public void SetSoundVolume(float value)
    {
        soundEffectVolume = value;
        //GameData.SetFloat("Sound Volume", soundEffectVolume);
    }
    public void SetVoiceVolume(float value)
    {
        voiceVolume = value;
        //GameData.SetFloat("Voice Volume", voiceVolume);
    }

    public void PlayMenuBlipSound()
    {
        soundEffectAudio.PlayOneShot(menuSelectSound, masterVolume * soundEffectVolume * 1f);
    }
    public void PlayMenuSelectSound()
    {
        soundEffectAudio.pitch = Random.Range(1 - 2 * blipPitchRange, 1 - blipPitchRange);
        soundEffectAudio.PlayOneShot(menuSelectSound, masterVolume * soundEffectVolume * .1f);
    }
}