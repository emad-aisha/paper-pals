using UnityEngine;
using UnityEngine.SceneManagement;
public class ButtonFunctions : MonoBehaviour
{
    // TODO: add a button for, next level in Menu Win (it doesnt get shown in level 3)
    // loads the next level and sets the appropriate variables for each level
    // lvl1 - all = false
    // lvl2 - flashlight = true
    // lvl3 all = true

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
        SceneManager.LoadScene("Level 1");
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
