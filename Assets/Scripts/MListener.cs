using UnityEngine;

public class MListener : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameObject.GetComponent<AudioSource>().volume = LoadSave.instance.GetMusicVolume();
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.GetComponent<AudioSource>().volume = LoadSave.instance.GetMusicVolume();
    }
}
