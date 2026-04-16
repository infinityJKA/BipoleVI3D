using System.Collections;
using UnityEngine;
using System;

public class AudioManager : MonoBehaviour
{
    public SoundObj[] sfx, music;
    [SerializeField] AudioSource musicSource, sfxSource, sfxSource2;
    public void PlayMusic(string name)
    {
        Debug.Log("Trying to play music " + name);
        SoundObj s = Array.Find(music, x=>x.id == name);

        if (s == null) Debug.Log("Music not found");
        else
        {
            musicSource.clip = s.audio;
            musicSource.Play();
        }
    }

    public void PlaySfx(string name)
    {
        SoundObj s = Array.Find(sfx, x => x.id == name);

        if (s == null) Debug.Log("Sfx not found");
        else
        {
            sfxSource.clip = s.audio;
            sfxSource.pitch = 1;
            sfxSource.volume = 1;
            sfxSource.Play();
        }
    }

    public void PlaySfx(string name, float pitch, float volume)
    {
        SoundObj s = Array.Find(sfx, x => x.id == name);

        if (s == null) Debug.Log("Sfx not found");
        else
        {
            sfxSource.clip = s.audio;
            sfxSource.pitch = pitch;
            sfxSource.volume = volume;
            sfxSource.Play();
        }
    }

    public void PlaySfxTrack2(string name)
    {
        SoundObj s = Array.Find(sfx, x => x.id == name);

        if (s == null) Debug.Log("Sfx not found");
        else
        {
            sfxSource2.clip = s.audio;
            sfxSource2.pitch = 1;
            sfxSource2.Play();
        }
    }

    public void PlaySfxRandomPitch(string name)
    {
        SoundObj s = Array.Find(sfx, x => x.id == name);

        if (s == null) Debug.Log("Sfx not found");
        else
        {
            sfxSource.clip = s.audio;
            sfxSource.volume = 1;
            sfxSource.pitch = UnityEngine.Random.Range(1f,2f);
            sfxSource.Play();
        }
    }


}

[System.Serializable]
public class SoundObj
{
    public string id;
    public AudioClip audio;
}