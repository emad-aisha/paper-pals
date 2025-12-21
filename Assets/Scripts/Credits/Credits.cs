using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Credits : MonoBehaviour
{
    public float scrollSpeed;
    public RectTransform viewport;
    private RectTransform rectTransform;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, -100f);
    }

    void Update()
    {
        rectTransform.anchoredPosition += new Vector2(0, scrollSpeed * Time.deltaTime);
        if (CreditsFinished())
        {
            SceneManager.LoadScene("Main Menu");
        }
        ReturnToMenu();
    }

  bool CreditsFinished()
    {
        float TopY = rectTransform.anchoredPosition.y;
        float Height = rectTransform.rect.height;
        float VPHeigiht = viewport.rect.height;

        return TopY >= Height + VPHeigiht; 
    }

    void ReturnToMenu()
    {
        if (Input.GetKeyUp(KeyCode.M) || Input.GetKeyUp(KeyCode.Escape))
        {
            SceneManager.LoadScene("Main Menu");
        }
    }

}
