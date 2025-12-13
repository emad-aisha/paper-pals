using UnityEngine;

public class BossMusicTrigger : MonoBehaviour
{
    private bool wasTriggered = false;
    private AudioSource source;
    private void Awake()
    {
       source = GetComponent<AudioSource>();
    }
    void OnTriggerEnter(Collider other)
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

      if (source != null)
        {
            source.Play();
        }
    }

}
