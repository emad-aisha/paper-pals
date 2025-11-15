using UnityEngine;
using UnityEngine.SceneManagement;
public class ButtonFunctions : MonoBehaviour
{

    public void Resume()
    {
      GameManager.instance.UnpauseGame();
    }

    public void Restart()
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void Play()
    {
        Time.timeScale = 1.0f;
        SceneManager.LoadScene("The Map");
    }

    public void Exit()
    {
        SceneManager.LoadScene("Main Menu");
    }


    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }


}
