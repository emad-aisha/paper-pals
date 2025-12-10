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
    public bool isInvertedY = false;

    // player stuff
    public int playerCoins = 100;
    public int playerAmmo = 0;
    public List<WeaponStats> playerWeapons;
    public bool playerTape;
    public bool playerMap;

    public int heartsBought;

    public int LevelLoad = 0;


    void Awake()
    {
        if(instance == null)
        {
            instance = this;
            
            //Does not get destroyed on another scene
            DontDestroyOnLoad(this.gameObject);
        }

        if (GameManager.instance != null)
        {
            audListen = GameManager.instance.controller.GetComponent<AudioSource>();
        } 
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

    //getters
    public void GetMusicSettings()
    {
        //nmusic is needed for this.....

    //MusicValue
    //MusicValue = int.Parse(sMusicValue.value.ToString());
    //GameManager.instance.aud
    }
    public AudioSource GetSFXSettings()
    {
        //SFXValue
        audListen.volume = SFXValue;
        return audListen;
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

    //miscellaneous settings

}

