using UnityEngine;

public class AtmosphereSound : MonoBehaviour
{
    public Collider Area;
    public GameObject Player;
    private AudioSource Source;

    bool Played;

    float offset = 0.2f;

    // Start is called before the first frame update
    void Start()
    {
        Source = GetComponent<AudioSource>();

        if (LoadSave.instance.GetSFXVolume() - offset >= 0) Source.volume = LoadSave.instance.GetSFXVolume() - offset;
        else Source.volume = 0;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == Player)
        {
            if (!Source.isPlaying) { 
                Debug.Log("Ambeince: " + this.gameObject.name);
                Source.Play();
                Played = true;
            }
        }
    }

    void OnTriggerExit(Collider other) {
        if (other.gameObject == Player) {
            if (Source.isPlaying && Source.loop) {
                Source.Stop();
                Played = false;
            }
        }
    }

    private void Update() {
        if (Played && !Source.loop && !Source.isPlaying) {
            Destroy(this.gameObject);
        }
    }

}
