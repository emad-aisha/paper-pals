using System.Collections;
using System.Diagnostics;
using TMPro;
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
    [SerializeField] TextMeshProUGUI TYMessage;
    void Start()
    {

        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("Intro"))
        {
            StartCoroutine(PlayIntro());
        }
        else if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("Outro"))
        {
            StartCoroutine(PlayOutro());
        }
    }

    IEnumerator PlayIntro()
    {
        SetAlpha(StudioIcon, 0);
        SetAlpha(GameIcon, 0);
        yield return Fade(StudioIcon, 0, 1, AnimationTime);
        yield return new WaitForSeconds(1);

        yield return Fade(StudioIcon, 1, 0, AnimationTime);
        yield return new WaitForSeconds(0.5f);

        yield return Fade(GameIcon, 0, 1, AnimationTime);
        yield return new WaitForSeconds(1);

        yield return Fade(GameIcon, 1, 0, AnimationTime);

        SceneManager.LoadScene("Main Menu");
    }

    IEnumerator PlayOutro()
    {
        SetAlpha(TYMessage, 0);
        TYMessage.text = "Thanks for Playing";
        yield return Fade(TYMessage, 0, 1, AnimationTime);
        yield return new WaitForSeconds(1);
        yield return Fade(TYMessage, 1, 0, AnimationTime);
        yield return new WaitForSeconds(0.5f);
        TYMessage.text = "From the Paper Pals Team";
        yield return Fade(TYMessage, 0, 1, AnimationTime);
        yield return new WaitForSeconds(1);
        StartCoroutine(Fade(StudioIcon, 1, 0, AnimationTime));
        StartCoroutine(Fade(GameIcon, 1, 0, AnimationTime));
        StartCoroutine(Fade(TYMessage, 1, 0, AnimationTime));
        yield return new WaitForSeconds(AnimationTime);
        SceneManager.LoadScene("Credits");
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

    IEnumerator Fade(TextMeshProUGUI Msg, float Start, float Finish, float Duration)
    {
        float Timer = 0;

        while (Timer < Duration)
        {
            float Alpha = Mathf.Lerp(Start, Finish, Timer / Duration);
            SetAlpha(Msg, Alpha);
            Timer += Time.deltaTime;
            yield return null;
        }
        SetAlpha(Msg, Finish);
    }

    void SetAlpha(RawImage Icon, float alpha)
    {
        Color color = Icon.color;
        color.a = alpha;
        Icon.color = color;
    }

    void SetAlpha(TextMeshProUGUI Message, float alpha)
    {
        Color color = Message.color;
        color.a = alpha;
        Message.color = color;
    }

}
