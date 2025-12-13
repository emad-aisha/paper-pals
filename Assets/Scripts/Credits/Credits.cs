using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Credits : MonoBehaviour
{
    public float scrollSpeed;

    private RectTransform rectTransform;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    void Update()
    {
        rectTransform.anchoredPosition += new Vector2(0, scrollSpeed * Time.deltaTime);
        ReturnToMenu();
    }


    void ReturnToMenu()
    {
        if (Input.GetKeyUp(KeyCode.M) || Input.GetKeyUp(KeyCode.Escape))
        {
            SceneManager.LoadScene("Main Menu");
        }
    }

}
