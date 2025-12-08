using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class LoadSave : MonoBehaviour
{
    public static LoadSave instance;

    AudioSource audListen;

    float MusicValue;
    float SFXValue;
    float MouseValue;
    bool isInvertedY = false;

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
        }

        if (GameManager.instance != null) audListen = GameManager.instance.controller.GetComponent<AudioSource>();
    }

    //setters
    public void SetMusicSettings(Slider _sMusicValue)
    {
        //MusicValue when it's created
        MusicValue = _sMusicValue.value;
        //MusicValue = float.Parse(_sMusicValue.value.ToString());
    }
    public void SetSFXSettings(Slider _sSFXValue)
    {
        SFXValue = _sSFXValue.value;
        Debug.Log("SFX Value set to: " + SFXValue);
        //SFXValue = float.Parse(_sSFXValue.value.ToString());
    }
    public void SetMouseSettings(Slider _sMouseValue)
    {
        MouseValue = _sMouseValue.value;
        //MouseValue = float.Parse(_sMouseValue.value.ToString());
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

    void SaveSettings() {
        //string path = "LoadSave.bin";
        //byte[] bytes;

        //FileStream file = new FileStream(path, FileMode.Create);

        //file.Write(bytes, 0, bytes.Length);

        //file.open("settings.bin", FileMode.OpenOrCreate | FileAccess.Write);
        //std::ofstream file("settings.bin", std::ios::out | std::ios::binary);

        //if (file.is_open()) {

        //	file.write(reinterpret_cast<const char*>(this), sizeof(Settings));
        //	file.close();
        //}
    }

    void LoadSettings()
    {

        //ifstream file("settings.bin", std::ios::binary | std::ios::in);

        //if (file.is_open())
        //{

        //    file.read(reinterpret_cast<char*>(this), sizeof(Settings));
        //    file.close();
        //}
    }


}

//struct Save 
//{

//    int MusicValue;
//    Slider sMusicValue = GameManager.instance.MusicSliderObj;

//    int SFXValue;
//    Slider sSFXValue = GameManager.instance.SFXSliderObj;

//    int MouseValue;
//    Slider sMouseValue = GameManager.instance.MouseSensSliderObj;

//    //music settings
//    //MusicValue
//    MusicValue = (int) sMusicValue.;

//    //SFXValue
//    //MouseValue

//    //SFX settings

//    //Mouse settings
//    //Mouse Sensitivity
//    //invert Y

//    //miscellaneous settings

//}
//struct Load
//{
//    Save SaveSettings;


//    //music settings method

//    //SFX settings method

//    //Mouse settings method
//        //Mouse Sensitivity
//        //invert Y

//    //miscellaneous settings method

//}