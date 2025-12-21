using System.Collections;
using UnityEngine;

public class FadeMusic : MonoBehaviour {
    private AudioSource source;

    void Awake() {
        source = GetComponent<AudioSource>();

        source.volume = LoadSave.instance.GetMusicVolume();
    }

    private void Update() {
        source.volume = LoadSave.instance.GetMusicVolume();
    }


    private IEnumerator FadeOutRoutine(float duration) {
        float startVolume = source.volume;
        float timer = 0;

        while (timer < duration) {
            source.volume = Mathf.Lerp(startVolume, 0, timer / duration);
            timer += Time.deltaTime;
            yield return null;
        }
        source.volume = 0;
        source.Stop();
    }
    public void FadeOut(float duration) {
        StartCoroutine(FadeOutRoutine(duration));
    }
}
