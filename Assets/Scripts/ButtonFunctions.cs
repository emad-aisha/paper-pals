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

    //option button function
    public void MMOption() {
        SceneManager.LoadScene("MMOption");
    }

    public void Option()
    {
        GameManager.instance.OptionMenu();
    }
    public void OptionExit()
    {
        GameManager.instance.ExitOptionMenu();
    }
    public void OptionVOL()
    {
        GameManager.instance.VolOptionMenu();
    }
    public void OptionMouse()
    {
        GameManager.instance.MouseOptionMenu();
    }
    public static void OptionMouseInvert()
    {
        GameManager.instance.UpdateInvert();
    }


    public void Play()
    {
        MusicManager music = Object.FindFirstObjectByType<MusicManager>();

        if (music != null)
        {
            music.StopMusic();
        }

        Time.timeScale = 1.0f;
        SceneManager.LoadScene("The Map");
    }

    public void Exit()
    {
        SceneManager.LoadScene("Main Menu");
    }

    public void Respawn()
    {
        GameManager.instance.controller.RespawnPlayer();
        GameManager.instance.UnpauseGame();
    }

    public void Quit()
    {
#if UNITY_EDITOR
        Destroy(LoadSave.instance);
        UnityEditor.EditorApplication.isPlaying = false;
#else
    Destroy(LoadSave.instance);
        Application.Quit();
#endif
    }


}
