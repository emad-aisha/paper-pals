using UnityEngine;

public class MusicManager : MonoBehaviour
{
    private static MusicManager instance;

    private void Awake()
    {
       
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void StopMusic()
    {
        GetComponent<AudioSource>().Stop();
        Destroy(gameObject); // Remove it permanently once real game starts
    }
}
