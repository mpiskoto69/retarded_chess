using UnityEngine;

public class MenuMusic : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip music;

    void Awake()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        if (audioSource == null || music == null)
            return;

        audioSource.clip = music;
        audioSource.loop = true;
        audioSource.spatialBlend = 0f;
        audioSource.Play();
    }
}