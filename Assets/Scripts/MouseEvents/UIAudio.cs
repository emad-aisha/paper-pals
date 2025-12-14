using UnityEngine;

public class UIAudio : MonoBehaviour
{
    [Header("Audio Clips")]
    public AudioClip hoverSound;
    public AudioClip clickSound;

    [Header("Audio Sources")]
    public AudioSource hoverSource;
    public AudioSource clickSource;

    float hoverDif = 0.0f;
    float clickDif = 0.3f;

    void Update() {
        float hoverVol = LoadSave.instance.GetSFXVolume();
        if (hoverVol - hoverDif >= 0) hoverVol -= hoverDif;
        else hoverVol = 0;

        float clickVol = LoadSave.instance.GetSFXVolume();
        if (clickVol - clickDif >= 0) clickVol -= clickDif;
        else clickVol = 0;

        hoverSource.volume = hoverVol;
        clickSource.volume = clickVol;
    }

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