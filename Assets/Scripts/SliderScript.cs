using Unity.VisualScripting;
using UnityEngine;

public class SliderScript : MonoBehaviour
{
    float MusicValueAlter;
    float SFXValueAlter;
    float MouseValueAlter;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //music volume slider and value display
        GameManager.instance.MusicSliderObj.onValueChanged.AddListener((value) => MusicValueAlter = value);
        GameManager.instance.MusicNumberDisplay.text = MusicValueAlter.ToString("0.00");

        //sfx volume slider and value display
        GameManager.instance.SFXSliderObj.onValueChanged.AddListener((value) => SFXValueAlter = value);
        GameManager.instance.SFXNumberDisplay.text = SFXValueAlter.ToString("0.00");

        //Mouse sensitivity slider and value display
        GameManager.instance.MouseSensSliderObj.onValueChanged.AddListener((value) => MouseValueAlter = value);
        GameManager.instance.MouseSensNumberDisplay.text = MouseValueAlter.ToString("0.00");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
