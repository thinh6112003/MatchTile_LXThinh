using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    [Header("--- Audio Source ----------")]
    public AudioSource musicSource;
    public AudioSource SoundSource;

    [Header("--- Audio Clip -----")]
    public AudioClip backgroundMusic;
    public AudioClip buttonClick;
    public AudioClip gameDefeat;
    public AudioClip gameVictory;
    public AudioClip tapTile;
    public AudioClip matchTile;
    public AudioClip fly;

    void Start()
    {
        Instance = this;
        musicSource.clip = backgroundMusic;
        musicSource.Play();
    }
    public void PlaySFX(AudioClip clip)
    {
        SoundSource.PlayOneShot(clip);
    }
    public void PlayButtonClickSound()
    {
        SoundSource.PlayOneShot(buttonClick);
    }
    public void activeMusic()
    {
        musicSource.enabled = !musicSource.enabled;
    }
    public void activeSound()
    {
        SoundSource.enabled = !SoundSource.enabled;
    }

}
