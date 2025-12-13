using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadSave : MonoBehaviour
{
    public static LoadSave instance;

    AudioSource audListen;

    double MusicValue;
    double SFXValue;
    float MouseValue;
    bool isInvertedY;

    // player stuff
    int playerCoins;
    int playerAmmo;
    List<WeaponStats> playerWeapons;
    bool playerTape;
    bool playerMap;

    int heartsBought;
    int levelLoad;

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
        else
        {
            audListen = new AudioSource();
        }

            // TODO: MAT - Set Default Values

            MusicValue = 0.0001;
        SFXValue = 0.0001;
        MouseValue = 0;
        isInvertedY = false;

        playerCoins = 0;
        playerAmmo = 0;
        playerWeapons = new List<WeaponStats>();
        playerTape = false;
        playerMap = false;

        heartsBought = 0;
        levelLoad = 0;

    }

    //setters
    public void SetMusicSettings(Slider _sMusicValue)
    {
       MusicValue = _sMusicValue.value;
    }
    public void SetVolumeSettings()
    {
        //GameManager.instance.controller.GetComponent<AudioSource>().volume = GetSFXSettings().volume;
    }

    public void SetSFXSettings(Slider _sSFXValue)
    {
       //audListen.volume
      SFXValue = _sSFXValue.value;
    }

    public void SetAudio(AudioSource Audio)
    {
        audListen = Audio;
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
        if (_playerCoins > 999) _playerCoins = 999;
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
    public float GetMusicSettings()
    {
        return (float)MusicValue;
    }

    public float GetSFXSettings()
    {
        //SFXValue
        return (float)SFXValue;
    }

    public AudioSource GetAudio()
    {
        return audListen;
    }
    public void AlterAudio()
    {
       audListen.volume = GetMusicSettings();
    }

    public float GetMouseSettings()
    {
        //Mouse Sensitivity
        return MouseValue;
    }
    public bool GetInvertYSettings()
    {
        return isInvertedY;
    }

    //public float GetSFXSettings()
    //{
    //    //SFX Sensitivity
    //    return SFXValue;
    //}

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

