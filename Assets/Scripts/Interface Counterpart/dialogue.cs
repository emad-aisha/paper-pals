using UnityEngine;
using System.Collections.Generic;

public class dialogue : MonoBehaviour, IDialogue
{
    [Header("Menus")]
    [SerializeField] string name;
    [SerializeField] List<string> text = new List<string>();

    int textIndexMax;
    int textIndex;

    public bool isExhausted;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        textIndexMax = text.Count;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool IsExhausted() {
        return isExhausted;
    }

    public void SetDialogue() {        
        if (textIndex == textIndexMax) isExhausted = true;
        if (isExhausted) { 
            GameManager.instance.EndDialogue();
            return;
        }

        GameManager.instance.characterName.SetText(name);
        GameManager.instance.characterText.SetText(text[textIndex]);
        GameManager.instance.Dialouge();

        if (textIndex < textIndexMax) textIndex++;
    }
}
