using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour {
    public static UIManager instance;

    enum Type { shop, options };
    [SerializeField] Type type;

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
    [SerializeField] Slider mMusicSliderObj;
    [SerializeField] TMP_Text mMusicNumberDisplay;

    [SerializeField] Slider mSFXSliderObj;
    [SerializeField] TMP_Text mSFXNumberDisplay;

    [SerializeField] Slider mMouseSensSliderObj;
    [SerializeField] TMP_Text mMouseSensNumberDisplay;

    [SerializeField] TMP_Text mDisplay_X_Button;


    void Awake() {
        if (instance == null) instance = this;

        if (Cursor.lockState == CursorLockMode.Locked) Cursor.lockState = CursorLockMode.None;
        if (Cursor.visible == false) Cursor.visible = true;

        if (type == Type.shop) {
            CoinAmount.text = LoadSave.instance.GetPlayerCoins().ToString();
            AmmoAmount.text = LoadSave.instance.GetPlayerAmmo().ToString();

            haveMap = LoadSave.instance.GetPlayerMap();
            haveTape = LoadSave.instance.GetPlayerTape();

            // Shop UI
            SetCostColors();
            UpdateHearts();

            SetInventory();
            SetEquipSave();

            SetTapeMap();

            OnPurchasable();
            StartCoroutine(StopTalking(5f));
        }


        if (type == Type.options) {
            // set the button
            if (LoadSave.instance.GetInvertYSettings()) {
                mDisplay_X_Button.text = "(X)";
            }
            else {
                mDisplay_X_Button.text = "( )";
            }

            SetSliders();

            // set the sliders
            mMusicSliderObj.onValueChanged.AddListener(DisplayTextSlider);
            mSFXSliderObj.onValueChanged.AddListener(DisplayTextSlider);
            mMouseSensSliderObj.onValueChanged.AddListener(DisplayTextSlider);   
        }
    }

    void Update() {

        if (type == Type.shop) {
            if (!isTalking && !stopTalking) {
                StartCoroutine(Talking());
            }
            if (Input.GetKey(KeyCode.P) && false) {
                int money = LoadSave.instance.GetPlayerCoins();
                money += 50;
                LoadSave.instance.SetPlayerCoins(money);
                CoinAmount.text = LoadSave.instance.GetPlayerCoins().ToString();
                SetCostColors();
            }
        }

    }


    // SHOP FUNCTIONS

    void SetTapeMap() {
        if (LoadSave.instance.GetPlayerTape()) {
            Tape.SetActive(true);
            haveTape = true;
        }

        if (LoadSave.instance.GetPlayerMap()) {
            Map.SetActive(true);
            haveMap = true;

            PurchaseOptions[3].SetActive(false);
        }
    }

    void SetEquipSave() {
        if (LoadSave.instance.GetBoghtStapler()) {
            EquipOptions[1].SetActive(true);
            PurchaseOptions[4].SetActive(false);
        }

        if (LoadSave.instance.GetBoughtC4()) {
            EquipOptions[2].SetActive(true);
            PurchaseOptions[5].SetActive(false);
        }
    }

    // call these functions on Start() and OnButton()
    // call SetPositon OnBuy()

    void SetCostColors() {
        for (int i = 0; i < CostTexts.Count; i++) {
            int cost = int.Parse(CostTexts[i].text);


            // if cost is too high, make the text gray
            if (cost > LoadSave.instance.GetPlayerCoins())
                CostTexts[i].color = Color.gray;
            else
                CostTexts[i].color = Color.black;

            if (haveTape) CostTexts[0].color = Color.gray;
        }
    }


    void UpdateHearts() {
        if (LoadSave.instance.GetHeartsBought() == 0) {
            Hearts[0].SetActive(false);
            Hearts[1].SetActive(false);
        }
        else if (LoadSave.instance.GetHeartsBought() == 1) {
            Hearts[0].SetActive(true);
            Hearts[1].SetActive(false);
        }
        else if (LoadSave.instance.GetHeartsBought() == 2) {
            Hearts[0].SetActive(true);
            Hearts[1].SetActive(true);
        }

    }

    void UpdateCoins() {
        CoinAmount.text = LoadSave.instance.GetPlayerCoins().ToString();
    }

    void UpdateAmmo() {
        AmmoAmount.text = LoadSave.instance.GetPlayerAmmo().ToString();
    }

    void SetInventory() {
        int currentItem = -1;
        if (LoadSave.instance.GetPlayerWeapons().Count >= 1) {
            if (LoadSave.instance.GetPlayerWeapons()[0].type == WeaponType.Gun) {
                currentItem = 1;
            }
            else if (LoadSave.instance.GetPlayerWeapons()[0].type == WeaponType.Melee) {
                currentItem = 0;
            }
            else if (LoadSave.instance.GetPlayerWeapons()[0].type == WeaponType.Explosive) {
                currentItem = 2;
            }
        }

        int nextItem = -1;
        if (LoadSave.instance.GetPlayerWeapons().Count == 2) {
            if (LoadSave.instance.GetPlayerWeapons()[1].type == WeaponType.Gun) {
                nextItem = 1;
            }
            else if (LoadSave.instance.GetPlayerWeapons()[1].type == WeaponType.Melee) {
                nextItem = 0;
            }
            else if (LoadSave.instance.GetPlayerWeapons()[1].type == WeaponType.Explosive) {
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
        int levelToLoad = LoadSave.instance.GetLevelLoad();


        if (levelToLoad == 1) SceneManager.LoadScene("Level 1");
        else if (levelToLoad == 2) SceneManager.LoadScene("Level 2");
        else if (levelToLoad == 3) SceneManager.LoadScene("Level 3");
        else if (levelToLoad == 4) SceneManager.LoadScene("Level 4");
        
    }

    public void OnPurchasable() {
        // set menus
        PurchaseMenu.SetActive(true);
        EquipMenu.SetActive(false);

        Color gray = new Color(0.4f, 0.4f, 0.4f);

        // set colors
        PurchaseBkgrd.color = Color.gray;
        PurchaseText.color = Color.white;


        EquipBkgrd.color = gray;
        EquipText.color = Color.black;
    }

    public void OnEquipable() {
        // set menus
        PurchaseMenu.SetActive(false);
        EquipMenu.SetActive(true);

        Color gray = new Color(0.4f, 0.4f, 0.4f);

        // set colors
        PurchaseBkgrd.color = gray;
        PurchaseText.color = Color.black;

        EquipBkgrd.color = Color.gray;
        EquipText.color = Color.white;
    }

    // equiping UNFINISHED
    public void OnPencilEquip() {
        // make sure this doesnt put multiple
        for (int i = 1; i < 3; i++) {
            if (LoadSave.instance.GetPlayerWeapons().Count == 0) {
                LoadSave.instance.AddPlayerWeapon(Melee);
                break;
            }
            else if (LoadSave.instance.GetPlayerWeapons().Count == 1) {
                if (LoadSave.instance.GetPlayerWeapons()[0].type == WeaponType.Melee) break;
                LoadSave.instance.AddPlayerWeapon(Melee);
                break;
            }
        }

        SetInventory();
    }

    public void OnStaplerEquip() {
        for (int i = 1; i < 3; i++) {
            if (LoadSave.instance.GetPlayerWeapons().Count == 0) {
                LoadSave.instance.AddPlayerWeapon(Gun);
                break;
            }
            else if (LoadSave.instance.GetPlayerWeapons().Count == 1) {
                if (LoadSave.instance.GetPlayerWeapons()[0].type == WeaponType.Gun) break;
                LoadSave.instance.AddPlayerWeapon(Gun);
                break;
            }
        }

        SetInventory();
    }

    public void OnC4Equip() {
        // set player inventory (if empty)
        for (int i = 1; i < 3; i++) {
            if (LoadSave.instance.GetPlayerWeapons().Count == 0) {
                LoadSave.instance.AddPlayerWeapon(C4);
                break;
            }
            else if (LoadSave.instance.GetPlayerWeapons().Count == 1) {
                if (LoadSave.instance.GetPlayerWeapons()[0].type == WeaponType.Explosive) break;
                LoadSave.instance.AddPlayerWeapon(C4);
                break;
            }
        }

        SetInventory();
    }

    // unequiping UNFINISHED
    public void OnInventory1() {
        if (LoadSave.instance.GetPlayerWeapons().Count > 0) {
            LoadSave.instance.GetPlayerWeapons().RemoveAt(0);
        }

        SetInventory();
    }

    public void OnInventory2() {
        if (LoadSave.instance.GetPlayerWeapons().Count > 1) {
            LoadSave.instance.GetPlayerWeapons().RemoveAt(1);
        }

        SetInventory();
    }

    // buying Items
    public void OnBuyMap() {
        if (LoadSave.instance.GetPlayerCoins() >= int.Parse(CostTexts[3].text)) {
            int oldCoins = LoadSave.instance.GetPlayerCoins();
            int newCoins = oldCoins -= int.Parse(CostTexts[3].text);
            LoadSave.instance.SetPlayerCoins(newCoins);

            UpdateCoins();

            haveMap = true;
            LoadSave.instance.SetPlayerMap(haveMap);
            if (haveMap) Map.SetActive(true);

            PurchaseOptions[3].SetActive(false);
            SetCostColors();
        }

    }

    public void OnBuyTape() {
        if (LoadSave.instance.GetPlayerCoins() >= int.Parse(CostTexts[0].text) && haveTape == false) {
            int oldCoins = LoadSave.instance.GetPlayerCoins();
            int newCoins = oldCoins -= int.Parse(CostTexts[0].text);
            LoadSave.instance.SetPlayerCoins(newCoins);


            UpdateCoins();

            haveTape = true;
            LoadSave.instance.SetPlayerTape(haveTape);
            Tape.SetActive(haveTape);

            SetCostColors();
        }
    }

    public void OnBuyHeart() {
        if (LoadSave.instance.GetPlayerCoins() >= int.Parse(CostTexts[2].text)) {
            int oldCoins = LoadSave.instance.GetPlayerCoins();
            int newCoins = oldCoins -= int.Parse(CostTexts[2].text);
            LoadSave.instance.SetPlayerCoins(newCoins);
            LoadSave.instance.IncrementHeartsBought();
            UpdateCoins();


            if (LoadSave.instance.GetHeartsBought() == 2) {
                PurchaseOptions[2].SetActive(false);
            }

            UpdateHearts();
            SetCostColors();
        }
    }

    public void OnBuyAmmo() {
        if (LoadSave.instance.GetPlayerCoins() >= int.Parse(CostTexts[1].text)) {
            int oldCoins = LoadSave.instance.GetPlayerCoins();
            int newCoins = oldCoins -= int.Parse(CostTexts[1].text);
            LoadSave.instance.SetPlayerCoins(newCoins);

            UpdateCoins();
            LoadSave.instance.AddAmmo(5);
            UpdateAmmo();

            SetCostColors();
        }
    }

    public void OnBuyStapler() {
        if (LoadSave.instance.GetPlayerCoins() >= int.Parse(CostTexts[4].text)) {
            int oldCoins = LoadSave.instance.GetPlayerCoins();
            int newCoins = oldCoins -= int.Parse(CostTexts[4].text);
            LoadSave.instance.SetPlayerCoins(newCoins);

            LoadSave.instance.SetBoughtStapler(true);

            if (LoadSave.instance.GetPlayerWeapons().Count < 2)
                LoadSave.instance.AddPlayerWeapon(Gun);
            SetInventory();

            UpdateCoins();

            EquipOptions[1].SetActive(true);
            PurchaseOptions[4].SetActive(false);
            SetCostColors();
        }
    }

    public void OnBuyCalculator() {
        if (LoadSave.instance.GetPlayerCoins() >= int.Parse(CostTexts[5].text)) {
            int oldCoins = LoadSave.instance.GetPlayerCoins();
            int newCoins = oldCoins -= int.Parse(CostTexts[5].text);
            LoadSave.instance.SetPlayerCoins(newCoins);

            LoadSave.instance.SetBoughtC4(true);

            if (LoadSave.instance.GetPlayerWeapons().Count < 2)
                LoadSave.instance.AddPlayerWeapon(C4);
            SetInventory();

            UpdateCoins();

            EquipOptions[2].SetActive(true);
            PurchaseOptions[5].SetActive(false);
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
    public void DisplayTextSlider(float _Value) {
        //takes the value from the slider and displays it on top to the slider
        mMusicNumberDisplay.text = (mMusicSliderObj.value * 100).ToString("F0");
        LoadSave.instance.SetMusicSettings(mMusicSliderObj);


        //takes the value from the slider and displays it on top to the slider
        mSFXNumberDisplay.text = (mSFXSliderObj.value * 100).ToString("F0");
        LoadSave.instance.SetSFXSettings(mSFXSliderObj);


        //takes the value from the slider and displays it on top to the slider
        mMouseSensNumberDisplay.text = mMouseSensSliderObj.value.ToString("F0");
        LoadSave.instance.SetMouseSens(mMouseSensSliderObj);
    }

    public void SetSliders() {
        mMusicSliderObj.value = LoadSave.instance.GetMusicVolume();
        mMusicNumberDisplay.text = (mMusicSliderObj.value * 100).ToString("F0");


        mSFXSliderObj.value = LoadSave.instance.GetSFXVolume();
        mSFXNumberDisplay.text = (mSFXSliderObj.value * 100).ToString("F0");


        mMouseSensSliderObj.value = LoadSave.instance.GetMouseSens();
        mMouseSensNumberDisplay.text = mMouseSensSliderObj.value.ToString("F0");
    }
}
