using UnityEngine;

public class AtmosphereSound : MonoBehaviour
{
    public Collider Area;
    public GameObject Player;
    private AudioSource Source;

    // Start is called before the first frame update
    void Start()
    {
        Source = GetComponent<AudioSource>();

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == Player)
        {
            Vector3 closestPoint = Area.ClosestPoint(Player.transform.position);
            transform.position = closestPoint;
            if (!Source.isPlaying)
                Source.Play();
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.gameObject == Player)
        {
            if (Source.isPlaying) 
            Source.Stop();
        }
    }



}
