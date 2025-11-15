using UnityEngine;
using TMPro;
using Unity.VisualScripting;


public class GameManager : MonoBehaviour
{
    public static GameManager instance;


    [Header("Directional Lighting")]
    [SerializeField] bool isTurnOffLighting;
    [SerializeField] GameObject Lighting;

    [Header("Menus")]
    [SerializeField] GameObject menuActive;
    //[SerializeField] GameObject mainMenu;
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuWin;
    [SerializeField] GameObject menuLose;

    [Header("Dialogue")]
    [SerializeField] GameObject menuDialogue;
    public TMP_Text characterName;
    public TMP_Text characterText;
    public bool isDialogueActive;
    
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
    public bool isPaused;

    [Header("Sticky Notes")]
    [SerializeField] GameObject stickyNotePrefab;
    [SerializeField] Transform stickyNoteParent;
    [SerializeField] public string defaultNoteTitle = "Note Title";
    [SerializeField] public string defaultNoteBody = "This is the sticky note body text.";

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

        if (isTurnOffLighting) Destroy(Lighting);
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

    public void WinTrophy(int amount)
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
            menuActive = menuDialogue;
            menuActive.SetActive(true);
            isDialogueActive = true;
        }
    }

    public void EndDialogue() {
        if (menuActive == menuDialogue) {
            isDialogueActive = false;
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

    public void CreateStickyNote(string title, string body)
    {
        GameObject note = Instantiate(stickyNotePrefab, stickyNoteParent);
        StickyNotesInfo notesInfo = note.GetComponent<StickyNotesInfo>();

        if(notesInfo != null)
        {
            notesInfo.SetNoteText(title, body);
        }


    }
}
