using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager instance;

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
    void Start()
    {
        if (LoadSave.instance.GetMusicVolume() > 0)
        {
            GetComponent<AudioSource>().volume = LoadSave.instance.GetMusicVolume();
        }
        else
        {
            GetComponent<AudioSource>().volume = 0;
        }
    }

    void Update()
    {
        if (LoadSave.instance.GetMusicVolume() > 0)
        {
            GetComponent<AudioSource>().volume = LoadSave.instance.GetMusicVolume();
        }
        else
        {
            GetComponent<AudioSource>().volume = 0;
        }
    }

    public void StopMusic()
    {
        GetComponent<AudioSource>().Stop();
        Destroy(gameObject);
    }
}
