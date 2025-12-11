using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class ButtonFunctions : MonoBehaviour
{
    LoadSave loadSave;

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

    //option button function
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
        //if it's off turn it on
        if (!GameManager.instance.invertY)
        {
            LoadSave.instance.SetInvertYSettings(true);
            GameManager.instance.Display_X_Button.text = "(X)";
        }
        else
        {
            //if it's on turn it off
            LoadSave.instance.SetInvertYSettings(false);
            GameManager.instance.Display_X_Button.text = "( )";
        }
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
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }


}
