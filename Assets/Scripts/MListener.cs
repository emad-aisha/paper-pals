using UnityEngine;

public class MListener : MonoBehaviour
{
    [SerializeField] bool isOffset = false;

    float offset;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        offset = 0.15f;

        if (isOffset) {
            if (LoadSave.instance.GetMusicVolume() - offset >= 0)
                gameObject.GetComponent<AudioSource>().volume = LoadSave.instance.GetMusicVolume() - offset;
            else
                gameObject.GetComponent<AudioSource>().volume = 0;
        }
        else
            gameObject.GetComponent<AudioSource>().volume = LoadSave.instance.GetMusicVolume();
    }

    // Update is called once per frame
    void Update()
    {
        if (isOffset) {
            if (LoadSave.instance.GetMusicVolume() - offset >= 0)
                gameObject.GetComponent<AudioSource>().volume = LoadSave.instance.GetMusicVolume() - offset;
            else
                gameObject.GetComponent<AudioSource>().volume = 0;
        }
        else
            gameObject.GetComponent<AudioSource>().volume = LoadSave.instance.GetMusicVolume();
    }
}
