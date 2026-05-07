using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Source")]
    public AudioSource sfxSource;

    [Header("Audio Clips")]
    public AudioClip popSound;

    private void Awake()
    {
        Instance = this;
    }

    public void PlayPopSound()
    {
        if (popSound != null && sfxSource != null)
        {
            sfxSource.pitch = Random.Range(0.85f, 1.15f);

            sfxSource.PlayOneShot(popSound);
        }
    }
}
