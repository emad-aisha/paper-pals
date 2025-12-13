using UnityEngine;

public class FadeTrigger : MonoBehaviour
{
    public float fadeTimer = 3;
    private bool wasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (wasTriggered)
        {
            return;
        }

        if (!other.CompareTag("Player"))
        {
            return;
        }

        wasTriggered = true;

        FadeMusic fadeMusic = Object.FindAnyObjectByType<FadeMusic>();

        if (fadeMusic != null)
        {
            fadeMusic.FadeOut(fadeTimer);
        }
    }
}


