using UnityEngine;
using UnityEngine.SceneManagement;
public class ButtonFunctions : MonoBehaviour
{

    public void Resume()
    {
        if (gameManager.instance.mainMenuActive == true)
        {
            gameManager.instance.mainMenuActive = false;
            Debug.Log(gameManager.instance.mainMenuActive);
        }
        gameManager.instance.UnpauseGame();
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        gameManager.instance.UnpauseGame();
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
