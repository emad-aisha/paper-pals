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
    [SerializeField] GameObject HealthBar;
    [SerializeField] GameObject TapeImage;

    // interaction
    public GameObject interactActive;
    public bool isInteractOn;

    public GameObject publicTapeImage;

    // health bar + paused
    public GameObject publicHealthBar;
    public bool isPaused;

    // game goal
    public TMP_Text gameGoalCountText;

    // player
    public GameObject player;
    public PlayerController controller;

    

    // private variables
    float originalTimeScale;
    int gameGoalCount = 0;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        instance = this;
        originalTimeScale = Time.timeScale;
        
        player = GameObject.FindWithTag("Player");
        controller = player.GetComponent<PlayerController>();
        publicHealthBar = HealthBar;

        publicTapeImage = TapeImage;
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
        if (interactActive == null) {
            isInteractOn = true;
            interactActive = Interactable;
            interactActive.SetActive(true);
        }
    }

    public void InteractOff() {
        if (interactActive != null) {
            isInteractOn = false;
            interactActive.SetActive(false);
            interactActive = null;
        } 
    }
}
