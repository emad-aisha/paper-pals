using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class LoadSave : MonoBehaviour
{
    public static LoadSave instance;

    AudioSource audListen;

    float MusicValue;
    float SFXValue;
    float MouseValue;
    bool isInvertedY;

    // player stuff
    int playerCoins = 0;
    int playerAmmo = 0;
    List<WeaponStats> playerWeapons = new List<WeaponStats>();
    bool playerTape = false;
    bool playerMap = false;

    int heartsBought = 0;
    int levelLoad = 0;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;

            //Does not get destroyed on another scene
            DontDestroyOnLoad(this.gameObject);
        }

        if (GameManager.instance != null)
        {
            audListen = GameManager.instance.controller.GetComponent<AudioSource>();
        }

        // TODO: MAT - Set Default Values

        MusicValue = 100;
        SFXValue = 100;
        MouseValue = 0;
        isInvertedY = false;
    }

    //setters
    public void SetMusicSettings(Slider _sMusicValue)
    {
        //MusicValue when it's created
        MusicValue = _sMusicValue.value;
        //MusicValue = float.Parse(_sMusicValue.value.ToString());
    }
    public void SetVolumeSettings()
    {
        //GameManager.instance.controller.GetComponent<AudioSource>().volume = GetSFXSettings().volume;
    }

    public void SetSFXSettings(Slider _sSFXValue)
    {
        SFXValue = _sSFXValue.value;
    }
    public void SetMouseSettings(Slider _sMouseValue)
    {
        MouseValue = _sMouseValue.value;
    }
    public void SetInvertYSettings(bool _isInvertedY)
    {
        isInvertedY = _isInvertedY;
    }

    // player shit
    public void SetPlayerCoins(int _playerCoins) {
        playerCoins = _playerCoins;
    }

    public void SetPlayerAmmo(int _playerAmmo) {
        playerAmmo = _playerAmmo;
    }

    public void SetPlayerWeapons(List<WeaponStats> _playerWeapons) {
        playerWeapons = _playerWeapons;
    }

    public void SetPlayerTape(bool _playerTape) {
        playerTape = _playerTape;
    }

    public void SetPlayerMap(bool _playerMap) {
        playerMap = _playerMap;
    }

    public void SetHeartsBought(int _heartsBought) {
        heartsBought = _heartsBought;
    }

    public void SetLevelLoad(int _levelLoad) {
        levelLoad = _levelLoad;
    }


    //getters
    public void GetMusicSettings()
    {
        //music is needed for this.....

        //MusicValue
        //MusicValue = int.Parse(sMusicValue.value.ToString());
        //GameManager.instance.aud
    }
    //public AudioSource GetSFXSettings()
    //{
    //    //SFXValue
    //    audListen.volume = SFXValue;
    //    return audListen;
    //}
    public float GetMouseSettings()
    {
        //Mouse Sensitivity
        return MouseValue;
    }
    public bool GetInvertYSettings()
    {
        return isInvertedY;
    }

    public float GetSFXSettings()
    {
        //SFX Sensitivity
        return SFXValue;
    }

    //player shit
    public int GetPlayerCoins() {
        return playerCoins;
    }

    public int GetPlayerAmmo() {
        return playerAmmo;
    }

    public List<WeaponStats> GetPlayerWeapons() {
        return playerWeapons;
    }

    public bool GetPlayerTape() {
        return playerTape;
    }

    public bool GetPlayerMap() {
        return playerMap;
    }

    public int GetHeartsBought() {
        return heartsBought;
    }

    public int GetLevelLoad() {
        return levelLoad;
    }


    public void IncrementLevelLoad() {
        levelLoad++;
    }

    public void AddPlayerWeapon(WeaponStats weapon) {
        playerWeapons.Add(weapon);
    }

    public void IncrementHeartsBought() {
        heartsBought++;
    }

    public void AddAmmo(int amount) {
        playerAmmo += amount;
    }

    //miscellaneous settings

}

