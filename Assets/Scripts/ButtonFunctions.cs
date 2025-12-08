using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
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
    public void OptionMouseInvert()
    {
        //if it's off turn it on
        if (!GameManager.instance.invertY)
        {
            GameManager.instance.invertY = true;
            GameManager.instance.Display_X_Button.text = "(X)";
            Debug.Log("Invert Y is ON: " + GameManager.instance.Display_X_Button.text);
        }
        else
        {
            //if it's on turn it off
            GameManager.instance.invertY = false;
            GameManager.instance.Display_X_Button.text = "( )";
            Debug.Log("Invert Y is off: " + GameManager.instance.Display_X_Button.text);
        }
        GameManager.instance.Display_X_Button.text = "(X)";
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
