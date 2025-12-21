using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class ButtonFunctions : MonoBehaviour
{
    [Header("Main Menu UI Stuff")]
    [SerializeField] GameObject VolMenu;
    [SerializeField] GameObject MouseMenu;
    [SerializeField] GameObject InfoMenu;
    [SerializeField] TMP_Text InvertDisplay;

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
        VolMenu.SetActive(true);
        InfoMenu.SetActive(false);
        MouseMenu.SetActive(false);
    }

    public void MMOptionVol() {
        VolMenu.SetActive(true);
        MouseMenu.SetActive(false);
        InfoMenu.SetActive(false);
    }

    public void MMOptionMouse() {
        VolMenu.SetActive(false);
        InfoMenu.SetActive(false);
        MouseMenu.SetActive(true);
    }
    public void MMOptionInfo() {
        VolMenu.SetActive(false);
        MouseMenu.SetActive(false);
        InfoMenu.SetActive(true);
    }
    

    public void MMOptionExit() {
        SceneManager.LoadScene("Main Menu");
    }

    public void MMOptionInvert() {
        if (!LoadSave.instance.GetInvertYSettings()) {
            LoadSave.instance.SetInvertYSettings(true);
            InvertDisplay.text = "(X)";
        }
        else {
            LoadSave.instance.SetInvertYSettings(false);
            InvertDisplay.text = "( )";
        }
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
    public void OptionInfo()
    {
        GameManager.instance.InfoOptionMenu();
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
        LoadSave.instance.ResetSettings();
        SceneManager.LoadScene("Main Menu");
    }

    public void Respawn()
    {
        GameManager.instance.controller.RespawnPlayer();
        GameManager.instance.UnpauseGame();
    }

    public void LoadCredits()
    {
        // 1. Reset time (in case the Win screen paused it)
        Time.timeScale = 1.0f;

        // 2. Unlock Mouse
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        // 3. Load Scene
        SceneManager.LoadScene("Outro");
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
