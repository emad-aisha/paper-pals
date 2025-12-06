using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    public static GameManager instance;


    [Header("Directional Lighting")]
    [SerializeField] bool isTurnOffLighting;
    [SerializeField] GameObject Lighting;

    [Header("Menus")]
    [SerializeField] GameObject menuActive;
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuWin;
    [SerializeField] GameObject menuLose;
    public GameObject mapMenu;

    [Header("Dialogue")]
    [SerializeField] GameObject menuDialogue;
    public TMP_Text characterName;
    public TMP_Text characterText;
    public bool isDialogueActive;

    [Header("\nPlayer UI")]
    [SerializeField] GameObject Interactable;
    public List<GameObject> Hearts;
    public List<GameObject> Weapons;
    public List<GameObject> EyedropPhases;
    public Image SprintBar;
    public GameObject flashRed; 
    public GameObject FlashFrames;
    public GameObject FlashlightMessage;
    public GameObject crosshair;

    [Header("\nInventory")]
    public GameObject TapeImage;
    public GameObject MapImage;
    [SerializeField] TMP_Text CoinCountText;
    [SerializeField] TMP_Text AmmoCountText;

    [Header("Weapons")]
    public GameObject AmmoMenu;
    public TMP_Text CurrAmmo;
    public TMP_Text TotalAmmo;

    [Header("\nInteraction")]
    public GameObject interactActive;
    public bool isInteractOn;

    [Header("Trophy Stuff")]
    public GameObject exit;
    public GameObject exitCover;
    public GameObject reminderMenu;
    public TMP_Text reminderText;

    [Header("Level specific")]
    public GunStats gun;
    public MeleeStats pencil;

    public GameObject gunObject;
    public GameObject pencilObject;

    public GameObject stickyNoteFinal;

    [Header("\n\nPublic variables")]

    [Header("Collectables")]
    public int TotalAmmoOwned;
    public int TotalCoinsOwned;
    int CoinsMax = 15;
    public int CoinsCounter;

    [Header("Player")]
    public GameObject player;
    public PlayerController controller;
    public bool hasFlashlight;
    public bool hasDoubleJump;
    public int totalKeys = 3;
    public int ownedKeys = 0;
    

    [Header("Camera")]
    public Camera mainCamera;

    [Header("Checkpoints")]
    public GameObject playerSpawnPos;
    public GameObject checkpointPopup;

    [Header("\nMisc")]
    //music volume slider and value display
    public Slider MusicSliderObj;
    public TMP_Text MusicNumberDisplay;

    //sfx volume slider and value display
    public Slider SFXSliderObj;
    public TMP_Text SFXNumberDisplay;

    //Mouse sensitivity slider and value display
    public Slider MouseSensSliderObj;
    public TMP_Text MouseSensNumberDisplay;
    
    public TMP_Text gameGoalCountText;
    public bool isPaused;


    // private variables
    float originalTimeScale = 1f;
    public int gameGoalCount = 0;
    public int gameGoalCounter;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if (instance == null) instance = this;

        originalTimeScale = Time.timeScale;

        player = GameObject.FindWithTag("Player");
        controller = player.GetComponent<PlayerController>();

        playerSpawnPos = GameObject.FindWithTag("Player Spawn Pos");

        mainCamera = Camera.main;

        exit = GameObject.FindWithTag("Exit");
        exitCover = GameObject.FindWithTag("Exit Cover");

        if (gunObject != null) gunObject.layer = 7;
        if (pencilObject != null) pencilObject.layer = 7;

        SetAbilities();
        UpdateKeysLeft();


        SetHearts();
        SetEydrops();
        SetWepons();

        SetEyedrop();

        if (isTurnOffLighting) Destroy(Lighting);
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
            else if (menuActive == menuPause)
            {
                UnpauseGame();
            }
        }

        if (Input.GetButtonDown("Flashlight") && hasFlashlight)
        {
            controller.FlashlightToggle();
        }

        if (Input.GetButtonDown("Map"))
        {
            controller.MapToggle();
        }
    }

    void SetHearts() {
        List<GameObject> temp = new List<GameObject>();
        temp.AddRange(GameObject.FindGameObjectsWithTag("Alive"));

        for (int i = 0; i < temp.Count; i++) {
            for (int j = 0; j < temp.Count; j++) {
                string name = temp[j].name;
                int heartOrder = int.Parse(name.Substring(5, 1));

                if (heartOrder == i + 1) {
                    Hearts.Add(temp[j]);
                    break;
                }
            }
        }
    }
    void SetEydrops() {
        List<GameObject> temp = new List<GameObject>();
        temp.AddRange(GameObject.FindGameObjectsWithTag("Eyedrop"));

        for (int i = 0; i < temp.Count; i++) {
            for (int j = 0; j < temp.Count; j++) {
                string name = temp[j].name;
                int eyedropOrder = int.Parse(name.Substring(7, 1));

                if (eyedropOrder == i + 1) {
                    EyedropPhases.Add(temp[j]);
                    break;
                }
            }
        }

    }
    void SetWepons() {
        Weapons.AddRange(GameObject.FindGameObjectsWithTag("Weapon"));
        for (int i = 0; i < Weapons.Count; i++) {
            Weapons[i].SetActive(false);
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

    public void SetAbilities()
    {
        string currLevelName = SceneManager.GetActiveScene().name;
        string levelOne = "Level 1";
        string levelTwo = "Level 2";
        string levelThree = "Level 3";  

        if (currLevelName == levelOne) {
            hasFlashlight = false;
            hasDoubleJump = false;
        }
        else if (currLevelName == levelTwo) {
            hasFlashlight = true;
            hasDoubleJump = false;
            controller.GetWeaponStats(pencil);
        }
        else if (currLevelName == levelThree) {
            hasFlashlight = true;
            hasDoubleJump = true;
            controller.GetWeaponStats(pencil);
            controller.GetWeaponStats(gun);
        }
        else {
            hasFlashlight = true;
            hasDoubleJump = true;
          //  controller.GetWeaponStats(pencil);
            // controller.GetWeaponStats(gun);
        }
    }

    public void UpdateKeysLeft()
    {
        int keysLeft = totalKeys - ownedKeys;

        if (keysLeft == 1 && SceneManager.GetActiveScene().name == "Level 3" && gameGoalCounter == gameGoalCount)
            stickyNoteFinal.SetActive(true);
        else if (keysLeft == 1 && SceneManager.GetActiveScene().name == "Level 3")
            reminderText.text = "Defeat the enemies!";
        else if (keysLeft != 0)
            reminderText.text = "You still need to get " + keysLeft.ToString() + " more keys...";
        else
            reminderText.text = "You can escape now!";
    }

    public void KeyCheck()
    {
        UpdateKeysLeft();
        StartCoroutine(ReminderText());

        if (ownedKeys == totalKeys)
        {
            Destroy(exitCover);
        }

    }

    public IEnumerator ReminderText()
    {
        reminderMenu.SetActive(true);
        yield return new WaitForSeconds(5);
        reminderMenu.SetActive(false);
    }

    public void LoadNextLevel()
    {
        string currLevelName = SceneManager.GetActiveScene().name;

        string levelOne = "Level 1";
        string levelTwo = "Level 2";
        string levelThree = "Level 3";

        if (currLevelName == levelOne)
        {
            SceneManager.LoadScene(levelTwo);
        }
        else if (currLevelName == levelTwo)
        {
            SceneManager.LoadScene(levelThree);
        }
        else if (currLevelName == levelThree) {
            Win();
        }
        else {
            SceneManager.LoadScene(levelOne);
        }
    }

    public void Win() {
        PauseGame();
        menuActive = menuWin;
        menuActive.SetActive(true);
    }

    public void WinTrophy(int amount)
    {
        gameGoalCount += amount;

        if (gameGoalCount == 1)
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

    public void Dialogue()
    {
        if (menuActive == null)
        {
            Time.timeScale = 0;
            menuActive = menuDialogue;
            menuActive.SetActive(true);
            isDialogueActive = true;
        }
    }

    public void EndDialogue()
    {
        if (menuActive == menuDialogue)
        {
            isDialogueActive = false;
            Time.timeScale = originalTimeScale;
            menuActive.SetActive(false);
            menuActive = null;
        }
    }

    public void InteractOn()
    {
        if (interactActive == null)
        {
            isInteractOn = true;
            interactActive = Interactable;
            interactActive.SetActive(true);
        }
    }

    public void InteractOff()
    {
        if (interactActive != null)
        {
            isInteractOn = false;
            interactActive.SetActive(false);
            interactActive = null;
        }
    }

    void SetEyedrop() {
        float percent = ((float)CoinsCounter / CoinsMax);
        int phase = 0;

        if (percent < 0.25) {
            phase = 1;
        }
        else if (percent < 0.5) {
            phase = 2;
        }
        else if (percent < 0.75) {
            phase = 3;
        }
        else if (percent < 1) {
            phase = 4;
        }

        for (int i = 0; i < EyedropPhases.Count; i++) {

            if (phase == i + 1) {
                EyedropPhases[i].SetActive(true);
            }
            else {
                EyedropPhases[i].SetActive(false);
            }
        }
    }

    public void UpdateCoinCount(int ammount)
    {
        if (CoinsCounter < CoinsMax) {
            CoinsCounter += ammount;
        }

        if (CoinsCounter >= CoinsMax) {
            CoinsCounter -= CoinsMax;

            if (TotalCoinsOwned < 999) {
                TotalCoinsOwned += 1;
            }
        }


        SetEyedrop();
        CoinCountText.text = TotalCoinsOwned.ToString("F0");
    }

    public void UpdateAmmoCount(int ammount)
    {
        if (TotalAmmoOwned < 999) { 
            TotalAmmoOwned += ammount;
        }
        AmmoCountText.text = TotalAmmoOwned.ToString("F0");
    }

    public void ShowFlashlightHint()
    {
        FlashlightMessage.SetActive(true);
        StartCoroutine(HideFlashlightHintAfterDelay());
    }

    IEnumerator HideFlashlightHintAfterDelay()
    {
        yield return new WaitForSeconds(2f);
        FlashlightMessage.SetActive(false);
    }
}
