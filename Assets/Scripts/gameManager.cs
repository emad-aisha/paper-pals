using UnityEngine;
using TMPro;

public class gameManager : MonoBehaviour
{
    public static gameManager instance;

    [Header("Menus")]
    [SerializeField] GameObject menuActive;
    [SerializeField] GameObject mainMenu;
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuWin;
    [SerializeField] GameObject menuLose;
    [SerializeField] GameObject menuDialouge;
    
    [Header("\nPlayer UI")]
    [SerializeField] GameObject Interactable;
    public GameObject HealthBar;
    public GameObject flashRed;
    

    [Header("\nInventory")]
    public GameObject TapeImage;

    [SerializeField] TMP_Text CoinCountText;
    [SerializeField] TMP_Text AmmoCountText;


    [Header("\nInteraction")]
    public GameObject interactActive;
    public bool isInteractOn;


    [Header("\n\nPublic variables")]

    [Header("Player")]
    public GameObject player;
    public PlayerController controller;

    [Header("Camera")]
    public Camera mainCamera;

    [Header("Misc")]
    public TMP_Text gameGoalCountText;
    public bool mainMenuActive = true;
    public bool isPaused;


    // private variables
    float originalTimeScale = 1f;
    int gameGoalCount = 0;

    int coinCount;
    int ammoCount;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if (instance == null) instance = this;

        originalTimeScale = Time.timeScale;
        
        player = GameObject.FindWithTag("Player");
        controller = player.GetComponent<PlayerController>();

        mainCamera = player.GetComponent<Camera>();

        StartMainMenu();
    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            if (menuActive == null) {
                PauseGame();
                menuActive = menuPause;
                menuActive.SetActive(true);
            }
            else if (menuActive == menuPause)
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

    public void winTrophy(int amount)
    {
        gameGoalCount += amount;

        if (gameGoalCount >= 1)
        {
            PauseGame();
            menuActive = menuWin;
            menuActive.SetActive(true);
        }
    }

    public void StartMainMenu() {
        isPaused = true;
        mainMenu.SetActive(true);
        PauseGame();
        mainMenuActive = true;
    }

    public void EndMainMenu() {
        mainMenuActive = false;
        mainMenu.SetActive(false);
        UnpauseGame();
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

    public void UpdateCoinCount(int ammount) {
        if (coinCount < 999) coinCount += ammount;
        CoinCountText.text = coinCount.ToString("F0");
    }

    public void UpdateAmmoCount(int ammount) {
        if (ammoCount < 999) ammoCount += ammount;
        AmmoCountText.text = ammoCount.ToString("F0");
    }
}
