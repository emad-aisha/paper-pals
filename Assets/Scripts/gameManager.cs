using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEditor;

public class gameManager : MonoBehaviour
{
    public static gameManager instance;

    [SerializeField] GameObject menuActive;
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuWin;
    [SerializeField] GameObject menuLose;
    [SerializeField] GameObject menuDialouge;

    [SerializeField] GameObject Interactable;
    public bool isInteractOn;

    public TMP_Text gameGoalCountText;

    public GameObject player;
    public PlayerController controller;

    public bool isPaused;
    float originalTimeScale;

    int gameGoalCount = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        instance = this;
        originalTimeScale = Time.timeScale;
        
        player = GameObject.FindWithTag("Player");
        controller = player.GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            if (menuActive == null)
            {
                PauseGame();
                menuActive = menuPause;
                menuActive.SetActive(true);
            }
            else
            {
                UnpauseGame();
            }
        }
    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void UnpauseGame()
    {
        isPaused = false;
        Time.timeScale = originalTimeScale;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        menuActive.SetActive(false);
        menuActive = null;
    }

    public void updateGameGoal(int amount)
    {
        gameGoalCount += amount;

        if (gameGoalCount >= 1)
        {
            PauseGame();
            menuActive = menuWin;
            menuActive.SetActive(true);
        }
    }

    public void Defeat()
    {
        PauseGame();
        menuActive = menuLose;
        menuActive.SetActive(true);
    }

    public void Dialouge() {
        if (menuActive == null) {
            Time.timeScale = 0;
            menuActive = menuDialouge;
            menuActive.SetActive(true);
        }
        else {
            Time.timeScale = originalTimeScale;
            menuActive.SetActive(false);
            menuActive = null;
        }  
    }

    public void InteractOn() {
        if (menuActive == null) {
            isInteractOn = true;
            menuActive = Interactable;
            menuActive.SetActive(true);
        }
    }

    public void InteractOff() {
        if (menuActive != null) {
            isInteractOn = false;
            menuActive.SetActive(false);
            menuActive = null;
        } 
    }
}
