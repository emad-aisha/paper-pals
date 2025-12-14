using UnityEngine;

public class UIAudio : MonoBehaviour
{
    [Header("Audio Clips")]
    public AudioClip hoverSound;
    public AudioClip clickSound;

    [Header("Audio Sources")]
    public AudioSource hoverSource;
    public AudioSource clickSource;

    // Play hover sound
    public void PlayHoverSound()
    {
        if (hoverSound != null && hoverSource != null)
        {
            hoverSource.PlayOneShot(hoverSound);
        }
    }
         

    // Play click sound
    public void PlayClickSound()
    {
        if (clickSound != null && clickSource != null)
        {
            clickSource.PlayOneShot(clickSound);
        }
            
    }
}