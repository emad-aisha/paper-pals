using System.Collections;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Debug = UnityEngine.Debug;

public class Intro : MonoBehaviour
{
    [SerializeField] RawImage StudioIcon;
    [SerializeField] RawImage GameIcon;
    [SerializeField] float AnimationTime = 1.5f;
    void Start()
    {
        Debug.Log("hello");
        StartCoroutine(PlayIntro());   
    }

    IEnumerator PlayIntro()
    {
        SetAlpha(StudioIcon, 0);
        Debug.Log("yep");
        SetAlpha(GameIcon, 0);
        Debug.Log("yep");
        yield return Fade(StudioIcon, 0, 1, AnimationTime);
        Debug.Log("fade 1");
        yield return new WaitForSeconds(1);

        yield return Fade(StudioIcon, 1, 0, AnimationTime);
        Debug.Log("fade 0");
        yield return new WaitForSeconds(0.5f);

        yield return Fade(GameIcon, 0, 1, AnimationTime);
        Debug.Log("fade 1");
        yield return new WaitForSeconds(1);

        yield return Fade(GameIcon, 1, 0, AnimationTime);
        Debug.Log("fade 0");

        SceneManager.LoadScene("Main Menu");
    }

    IEnumerator Fade(RawImage Icon, float Start, float Finish, float Duration)
    {
        float Timer = 0;

        while (Timer < Duration)
        {
            float Alpha = Mathf.Lerp(Start, Finish, Timer / Duration);
            SetAlpha(Icon, Alpha);
            Timer += Time.deltaTime;
            yield return null;
        }
        SetAlpha(Icon, Finish);
    }

    void SetAlpha(RawImage Icon, float alpha)
    {
        Color color = Icon.color;
        color.a = alpha;
        Icon.color = color;
    }
}
