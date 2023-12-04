using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
   public static AudioManager instance;
    
    [Header("BGM")]
    public AudioClip bgmClip;
    public float bgmVolume;
    private AudioSource bgmPlayer;
    
    [Header("SFX")]
    public AudioClip[] sfxClips;
    public float sfxVolume;
    public int channels;
    AudioSource[] sfxPlayers;
    private int channelIndex;

    public enum Sfx
    {
        a,
        Dead,
        s,
        QQ,
        Lost
    };

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(this.gameObject);
        }
     DontDestroyOnLoad(this.gameObject);
        
        Init();
        PlayBGM(true);
    }

    private void Init()
    {
        // BGM
        GameObject bgmObj =new GameObject("BGM Player");
        bgmObj.transform.parent= transform;
        bgmPlayer=bgmObj.AddComponent<AudioSource>();
        bgmPlayer.playOnAwake=false;
        bgmPlayer.loop=true;
        bgmPlayer.volume = bgmVolume;
        bgmPlayer.clip = bgmClip;
        
        // SFX
        GameObject sfxObj =new GameObject("SFX Player");
        sfxObj.transform.parent= transform;
        sfxPlayers = new AudioSource[channels];

        for (int i = 0; i < sfxPlayers.Length; i++)
        {
            sfxPlayers[i]=sfxObj.AddComponent<AudioSource>();
            sfxPlayers[i].playOnAwake=false;
            sfxPlayers[i].volume = sfxVolume;
        }
    }
    
    public void PlayBGM(bool isPlay)
    {
        if (isPlay)
        {
            if(!bgmPlayer.isPlaying)
                bgmPlayer.Play();
        }
        else
        {
            bgmPlayer.Stop();
        }
    }
    
    public void SetBgmVolume(float volume)
    {
        bgmVolume = volume;
        bgmPlayer.volume = bgmVolume;
    }

    public float GetBgmVolume()
    {
        return bgmVolume;
    }

    public void SetSfxVolume(float volume)
    {
        sfxVolume = volume;
        foreach (AudioSource sfxPlayer in sfxPlayers)
        {
            sfxPlayer.volume = sfxVolume;
        }
    }

    public void PlaySfx(Sfx sfx)
    {
        for (int i = 0; i < sfxPlayers.Length; i++)
        {
            int loopIndex = (i+channelIndex)%sfxPlayers.Length;

            if (sfxPlayers[loopIndex].isPlaying)
            {
                continue;
            }

            channelIndex = loopIndex;
            sfxPlayers[loopIndex].clip = sfxClips[(int)sfx];
            sfxPlayers[loopIndex].Play();
            break;
        }
    }
}
