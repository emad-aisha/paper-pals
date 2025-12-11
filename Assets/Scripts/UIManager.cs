using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour {
    public static UIManager instance;

    [Header("Shop Dependencies")]
    // folders
    [SerializeField] Image PurchaseBkgrd;
    [SerializeField] TMP_Text PurchaseText;

    [SerializeField] Image EquipBkgrd;
    [SerializeField] TMP_Text EquipText;

    [SerializeField] GameObject PurchaseMenu;
    [SerializeField] GameObject EquipMenu;

    [Header("")]
    [SerializeField] List<TMP_Text> CostTexts;
    [SerializeField] List<GameObject> PurchaseOptions;

    [Header("")]
    [SerializeField] List<GameObject> EquipOptions;
    [SerializeField] List<GameObject> Inventory1;
    [SerializeField] List<GameObject> Inventory2;

    [Header("ShopKeep")]
    [SerializeField] GameObject OpenMouth;

    [Header("Player Inventory")]
    [SerializeField] List<GameObject> Hearts;
    [SerializeField] TMP_Text CoinAmount;
    [SerializeField] TMP_Text AmmoAmount;

    [Header("Buyables")]
    [SerializeField] GameObject Tape;
    [SerializeField] GameObject Map;

    [SerializeField] WeaponStats Melee;
    [SerializeField] WeaponStats Gun;
    [SerializeField] WeaponStats C4;

    // private
    // saves
    bool haveMap = false;
    bool haveTape;


    bool isTalking = false;
    bool stopTalking = false;


    [Header("\n\nOption Dependencies")]
    [SerializeField] GameObject MinMenuMouse;
    [SerializeField] GameObject MinMenuVol;

    public Slider mMusicSliderObj;
    public TMP_Text mMusicNumberDisplay;

    public Slider mSFXSliderObj;
    public TMP_Text mSFXNumberDisplay;

    public Slider mMouseSensSliderObj;
    public TMP_Text mMouseSensNumberDisplay;

    public TMP_Text mDisplay_X_Button;


    void Awake() {

        if (instance == null) instance = this;

        CoinAmount.text = LoadSave.instance.playerCoins.ToString();
        AmmoAmount.text = LoadSave.instance.playerAmmo.ToString();


        haveMap = LoadSave.instance.playerMap;
        haveTape = LoadSave.instance.playerTape;

        // Shop UI
        SetCostColors();
        UpdateHearts();

        SetPurchasePositions();
        SetEquipPositions();
        SetInventory();

        OnPurchasable();
        StartCoroutine(StopTalking(5f));

    }

    void Update() {
        if (!isTalking && !stopTalking) {
            StartCoroutine(Talking());
        }
        
        if (Input.GetKey(KeyCode.P) && Input.GetKey(KeyCode.L) && Input.GetKey(KeyCode.A)) {
            SceneManager.LoadScene("Mat's Scene");
        }

        //used to set the default values of the sliders for the options menu
        //mMusicSliderObj = GameManager.instance.MusicSliderObj;
        mMusicSliderObj.onValueChanged.AddListener(DisplayTextSlider);

        //mSFXSliderObj = GameManager.instance.SFXSliderObj;
        mSFXSliderObj.onValueChanged.AddListener(DisplayTextSlider);

        //mMouseSensSliderObj = GameManager.instance.MouseSensSliderObj;
        mMouseSensSliderObj.onValueChanged.AddListener(DisplayTextSlider);
    }


    // SHOP FUNCTIONS

    // call these functions on Start() and OnButton()
    // call SetPositon OnBuy()

    void SetCostColors() {
        for (int i = 0; i < CostTexts.Count; i++) {
            // TODO: set this once save is properly made
            int cost = int.Parse(CostTexts[i].text);


            // if cost is too high, make the text gray
            if (cost > LoadSave.instance.playerCoins)
                CostTexts[i].color = Color.gray;
            else
                CostTexts[i].color = Color.black;

            if (haveTape) CostTexts[0].color = Color.gray;
        }
    }

    void SetPurchasePositions() {
        int initial = 1205;

        for (int i = 0; i < PurchaseOptions.Count; i++) {
            float xValue = PurchaseOptions[i].transform.position.x;
            int height = 62;

            float yValue = i * height;

            PurchaseOptions[i].transform.position = new Vector3(xValue, initial - yValue, 0);
        }
    }

    /*
    void MoveItemsUp() {
        int index = 1;
        bool startMoving = false;

        for (int i = 0; i < PurchaseOptions.Count; i++) {
            if (!PurchaseOptions[i].activeSelf)
                startMoving = true;
            if (!startMoving) continue;


            float xValue = PurchaseOptions[i].transform.position.x;
            float yValue = PurchaseOptions[i].transform.position.y;
            int height = 300 * index;

            PurchaseOptions[i].transform.position = new Vector3(xValue, yValue + height, 0);

        }
    }
    */

    void SetEquipPositions() {
        float initial = 1175;
        for (int i = 0; i < EquipOptions.Count; i++) {
            float xValue = EquipOptions[i].transform.position.x;
            int height = 125;

            float yValue = -i * height;

            EquipOptions[i].transform.position = new Vector3(xValue, initial + yValue, 0);
        }
    }

    void UpdateHearts() {
        if (LoadSave.instance.heartsBought == 0) {
            Hearts[0].SetActive(false);
            Hearts[1].SetActive(false);
        }
        else if (LoadSave.instance.heartsBought == 1) {
            Hearts[0].SetActive(true);
            Hearts[1].SetActive(false);
        }
        else if (LoadSave.instance.heartsBought == 2) {
            Hearts[0].SetActive(true);
            Hearts[1].SetActive(true);
        }

    }

    void UpdateCoins() {
        CoinAmount.text = LoadSave.instance.playerCoins.ToString();
    }

    void UpdateAmmo() {
        AmmoAmount.text = LoadSave.instance.playerAmmo.ToString();
    }

    void SetInventory() {
        // TODO: set item
        //       get item from save

        // if item.name = player.inven.1
        int currentItem = -1;
        if (LoadSave.instance.playerWeapons.Count >= 1) {
            if (LoadSave.instance.playerWeapons[0].type == WeaponType.Gun) {
                currentItem = 1;
            }
            else if (LoadSave.instance.playerWeapons[0].type == WeaponType.Melee) {
                currentItem = 0;
            }
            else if (LoadSave.instance.playerWeapons[0].type == WeaponType.Explosive) {
                currentItem = 2;
            }
        }

        int nextItem = -1;
        if (LoadSave.instance.playerWeapons.Count == 2) {
            if (LoadSave.instance.playerWeapons[1].type == WeaponType.Gun) {
                nextItem = 1;
            }
            else if (LoadSave.instance.playerWeapons[1].type == WeaponType.Melee) {
                nextItem = 0;
            }
            else if (LoadSave.instance.playerWeapons[1].type == WeaponType.Explosive) {
                nextItem = 2;
            }
        }

        // set slot 1
        for (int i = 0; i < Inventory1.Count; i++) {
            bool setItem = i == currentItem;

            if (setItem) Inventory1[i].SetActive(true);
            else Inventory1[i].SetActive(false);
        }

        // set slot 2
        for (int i = 0; i < Inventory2.Count; i++) {
            bool setItem = i == nextItem;

            if (setItem) Inventory2[i].SetActive(true);
            else Inventory2[i].SetActive(false);
        }
    }


    // SHOP BUTTON FUNCTIONS

    public void OnNextLevel() {
        LoadSave.instance.LevelLoad++;
        GameManager.instance.LoadNextLevel(LoadSave.instance.LevelLoad);

        //string level = "Level " + LoadSave.instance.LevelLoad.ToString();

        //Debug.Log(level);
        //SceneManager.LoadScene(level);
    }
    public void OnPurchasable() {
        // set menus
        PurchaseMenu.SetActive(true);
        EquipMenu.SetActive(false);

        // set colors
        PurchaseBkgrd.color = Color.gray;
        PurchaseText.color = Color.white;

        EquipBkgrd.color = Color.black;
        EquipText.color = Color.gray;
    }

    public void OnEquipable() {
        // set menus
        PurchaseMenu.SetActive(false);
        EquipMenu.SetActive(true);

        // set colors
        PurchaseBkgrd.color = Color.black;
        PurchaseText.color = Color.gray;

        EquipBkgrd.color = Color.gray;
        EquipText.color = Color.white;
    }

    // equiping UNFINISHED
    // TODO: save system to call inventory
    public void OnPencilEquip() {
        // make sure this doesnt put multiple
        for (int i = 1; i < 3; i++) {
            if (LoadSave.instance.playerWeapons.Count == 0) {
                LoadSave.instance.playerWeapons.Add(Melee);
                break;
            }
            else if (LoadSave.instance.playerWeapons.Count == 1) {
                if (LoadSave.instance.playerWeapons[0].type == WeaponType.Melee) break;
                LoadSave.instance.playerWeapons.Add(Melee);
                break;
            }
        }

        SetInventory();
    }

    public void OnStaplerEquip() {
        for (int i = 1; i < 3; i++) {
            if (LoadSave.instance.playerWeapons.Count == 0) {
                LoadSave.instance.playerWeapons.Add(Gun);
                break;
            }
            else if (LoadSave.instance.playerWeapons.Count == 1) {
                if (LoadSave.instance.playerWeapons[0].type == WeaponType.Gun) break;
                LoadSave.instance.playerWeapons.Add(Gun);
                break;
            }
        }

        SetInventory();
    }

    public void OnC4Equip() {
        // set player inventory (if empty)
        for (int i = 1; i < 3; i++) {
            if (LoadSave.instance.playerWeapons.Count == 0) {
                LoadSave.instance.playerWeapons.Add(C4);
                break;
            }
            else if (LoadSave.instance.playerWeapons.Count == 1) {
                if (LoadSave.instance.playerWeapons[0].type == WeaponType.Explosive) break;
                LoadSave.instance.playerWeapons.Add(C4);
                break;
            }
        }

        SetInventory();
    }

    // unequiping UNFINISHED
    public void OnInventory1() {
        if (LoadSave.instance.playerWeapons.Count > 0) {
            LoadSave.instance.playerWeapons.RemoveAt(0);
        }

        SetInventory();
    }

    public void OnInventory2() {
        if (LoadSave.instance.playerWeapons.Count > 1) {
            LoadSave.instance.playerWeapons.RemoveAt(1);
        }

        SetInventory();
    }

    // buying Items UNFINISHED
    // check if you have the item
    // add it to save
    // TODO: save these to LoadSave
    public void OnBuyMap() {
        if (LoadSave.instance.playerCoins >= int.Parse(CostTexts[3].text)) {
            LoadSave.instance.playerCoins -= int.Parse(CostTexts[3].text);
            UpdateCoins();

            haveMap = true;
            LoadSave.instance.playerMap = haveMap;
            if (haveMap) Map.SetActive(true);

            PurchaseOptions[3].SetActive(false);
            //MoveItemsUp();
            SetCostColors();
        }

    }

    public void OnBuyTape() {
        if (LoadSave.instance.playerCoins >= int.Parse(CostTexts[0].text)) {
            LoadSave.instance.playerCoins -= int.Parse(CostTexts[0].text);
            UpdateCoins();

            haveTape = true;
            Tape.SetActive(haveTape);

            SetCostColors();
        }
    }

    public void OnBuyHeart() {
        if (LoadSave.instance.playerCoins >= int.Parse(CostTexts[2].text)) {
            LoadSave.instance.playerCoins -= int.Parse(CostTexts[2].text);
            UpdateCoins();


            // this is before the increment so minus 1
            if (LoadSave.instance.heartsBought == 1) {
                PurchaseOptions[2].SetActive(false); 
                //MoveItemsUp();
            }
            // increment hearts bought
            else LoadSave.instance.heartsBought++;

            UpdateHearts();
            SetCostColors();
        }
    }

    public void OnBuyAmmo() {
        if (LoadSave.instance.playerCoins >= int.Parse(CostTexts[1].text)) {
            LoadSave.instance.playerCoins -= int.Parse(CostTexts[1].text); 
            UpdateCoins();
            LoadSave.instance.playerAmmo += 5;
            UpdateAmmo();

            SetCostColors();
        }
    }

    public void OnBuyStapler() {
        if (LoadSave.instance.playerCoins >= int.Parse(CostTexts[4].text)) {
            LoadSave.instance.playerCoins -= int.Parse(CostTexts[4].text);
            UpdateCoins();

            PurchaseOptions[4].SetActive(false);
            //MoveItemsUp();
            SetCostColors();
        }
    }

    public void OnBuyCalculator() {
        if (LoadSave.instance.playerCoins >= int.Parse(CostTexts[5].text)) {
            LoadSave.instance.playerCoins -= int.Parse(CostTexts[5].text);
            UpdateCoins();

            PurchaseOptions[5].SetActive(false);
            //MoveItemsUp();
            SetCostColors();
        }
    }
    
    public IEnumerator Talking() {
        isTalking = true;
        OpenMouth.SetActive(true);
        yield return new WaitForSeconds(0.3f);
        OpenMouth.SetActive(false);
        yield return new WaitForSeconds(0.3f);
        isTalking = false;
    }

    public IEnumerator StopTalking(float time) {
        yield return new WaitForSeconds(time);
        stopTalking = true;
    }


    // OPTION BUTTON FUNCTIONS
    public void DisplayTextSlider(float _Value)
    {
        //controller.GetComponent<AudioSource>().volume = 100;
        //takes the value from the slider and displays it on top to the slider
        mMusicNumberDisplay.text = mMusicSliderObj.value.ToString("F2");
        LoadSave.instance.SetMusicSettings(mMusicSliderObj);
        Debug.Log("Music Value: " + mMusicSliderObj.value.ToString("F2"));

        //takes the value from the slider and displays it on top to the slider
        mSFXNumberDisplay.text = mSFXSliderObj.value.ToString("F2");
        LoadSave.instance.SetMusicSettings(mMusicSliderObj);
        Debug.Log("SFX Value: " + mSFXSliderObj.value.ToString("F2"));  

        //takes the value from the slider and displays it on top to the slider
        mMouseSensNumberDisplay.text = mMouseSensSliderObj.value.ToString("F2");
        LoadSave.instance.SetMusicSettings(mMusicSliderObj);
        Debug.Log("Mouse Sensitivity Value: " + mMouseSensSliderObj.value.ToString("F2"));
    }
}
