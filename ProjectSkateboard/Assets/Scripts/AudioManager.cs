using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    [System.Serializable]
    public class GameSound
    {
        public enum SoundType { BGM, SFX }
        public enum Sound
        {
            TitlescreenMusic,
            NightToDayTransition,
            DayMusic,
            PlayerJump,
            TrickSuccess,
            ComboEnd,
            CollectCD,
            MenuButtonOver,
            MenuButtonClick,
            ResultsWin,
            ResultsFail
        }

        [Tooltip("The sound to play.")] public Sound sound; 
        [Tooltip("Which audio source type to use for playing the sound.")] public SoundType soundType;
        [Tooltip("The audio file.")] public AudioClip audioClip;
        [Tooltip("If true, the sound loops when played.")] public bool loop;
    }

    [SerializeField, Tooltip("The master list of all sounds in the game.")] private GameSound[] soundMasterList;

    private AudioSource musicSource, sfxSource;

    private void Awake()
    {
        //Create audio sources to play sounds from
        GameObject musicGameObject = Instantiate(new GameObject("Music Source"), transform);
        musicSource = musicGameObject.AddComponent<AudioSource>();
        GameObject sfxGameObject = Instantiate(new GameObject("SFX Source"), transform);
        sfxSource = sfxGameObject.AddComponent<AudioSource>();

        if(SceneManager.GetActiveScene().buildIndex == 0)
        {
            Debug.Log(SceneManager.GetActiveScene().buildIndex);
            GameManager.Instance?.AudioManager.Play(AudioManager.GameSound.Sound.TitlescreenMusic);
        }
    }

    /// <summary>
    /// Plays a sound at a specific audio source, depending on the sound type.
    /// </summary>
    /// <param name="sound">The game sound to play.</param>
    public void Play(GameSound.Sound sound)
    {
        GameSound currentSound = GetSound(sound);

        if(currentSound.soundType == GameSound.SoundType.BGM)
        {
            musicSource.clip = currentSound.audioClip;
            musicSource.volume = PlayerPrefs.GetFloat("BGMVolume", 0.5f);
            musicSource.Play();
            musicSource.loop = currentSound.loop;
        }

        else if(currentSound.soundType == GameSound.SoundType.SFX)
        {
            sfxSource.clip = currentSound.audioClip;
            sfxSource.volume = PlayerPrefs.GetFloat("SFXVolume", 0.5f);
            sfxSource.Play();
            sfxSource.loop = currentSound.loop;
        }
    }

    /// <summary>
    /// Plays a one-shot on the sound effects source.
    /// </summary>
    /// <param name="sound">The game sound to play a one shot for.</param>
    public void PlayOneShot(GameSound.Sound sound)
    {
        GameSound currentSound = GetSound(sound);
        sfxSource.PlayOneShot(currentSound.audioClip, PlayerPrefs.GetFloat("SFXVolume", 0.5f));
    }

    /// <summary>
    /// Stops a sound if it is already playing.
    /// </summary>
    /// <param name="sound">The sound to stop playing.</param>
    public void Stop(GameSound.Sound sound)
    {
        GameSound currentSound = GetSound(sound);

        if (currentSound.soundType == GameSound.SoundType.BGM)
        {
            if(musicSource.clip == currentSound.audioClip)
                musicSource.Stop();
        }

        else if (currentSound.soundType == GameSound.SoundType.SFX)
        {
            if (sfxSource.clip == currentSound.audioClip)
                sfxSource.Stop();
        }
    }

    /// <summary>
    /// Checks to see if a sound is playing.
    /// </summary>
    /// <param name="sound">The game sound to check.</param>
    /// <returns>If true, the sound is actively playing.</returns>
    public bool IsSoundPlaying(GameSound.Sound sound)
    {
        GameSound currentSound = GetSound(sound);

        if (currentSound.soundType == GameSound.SoundType.BGM)
            return musicSource.isPlaying && musicSource.clip == currentSound.audioClip;

        else if (currentSound.soundType == GameSound.SoundType.SFX)
            return sfxSource.isPlaying && sfxSource.clip == currentSound.audioClip;

        return false;
    }

    /// <summary>
    /// Pauses all sounds.
    /// </summary>
    public void PauseAllSounds()
    {
        musicSource.Pause();
        sfxSource.Pause();
    }

    /// <summary>
    /// Resumes all sounds.
    /// </summary>
    public void ResumeAllSounds()
    {
        musicSource.UnPause();
        sfxSource.UnPause();
    }

    /// <summary>
    /// Stops all sounds.
    /// </summary>
    public void StopAllSounds()
    {
        musicSource.Stop();
        sfxSource.Stop();
    }

    /// <summary>
    /// Returns the sound information based on the sound enum given.
    /// </summary>
    /// <param name="sound">The game sound to get the information of.</param>
    /// <returns>If the game sound is found, returns the sound information.</returns>
    private GameSound GetSound(GameSound.Sound sound)
    {
        foreach(GameSound gameSound in soundMasterList)
        {
            if (gameSound.sound == sound)
                return gameSound;
        }

        Debug.LogError("Sound " + sound + " not found!");
        return null;
    }
}