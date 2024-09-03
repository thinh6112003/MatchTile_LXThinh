using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    [Header("--- Audio Source ----------")]
    public AudioSource musicSource;
    public AudioSource SoundSource;

    [Header("--- Audio Clip -----")]
    public AudioClip backgroundMusic;    // v
    public AudioClip buttonClick;        // V
    public AudioClip gameLose;           // v
    public AudioClip gameWin;            // v
    public AudioClip tapTile;            // v
    public AudioClip matchTile;          // 
    public AudioClip fly;                // v

    void Start()
    {
        Instance = this;
        Debug.Log("play sound");
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
